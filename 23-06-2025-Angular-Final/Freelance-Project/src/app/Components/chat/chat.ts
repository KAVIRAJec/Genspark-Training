import { Component, signal, computed, OnDestroy, AfterViewInit, ViewChild, ElementRef, effect } from '@angular/core';
import { ChatRoomModel } from '../../Models/ChatRoom.model';
import { ChatMessageModel } from '../../Models/ChatMessage.model';
import { ChatService } from '../../Services/chat.service';
import { ToastrService } from 'ngx-toastr';
import { ClientModel } from '../../Models/Client.model';
import { FreelancerModel } from '../../Models/Freelancer.model';
import { AuthenticationService } from '../../Services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import * as ChatActions from '../../NgRx/Chat/chat.actions';
import { selectChatMessages, selectChatLoading, selectChatError } from '../../NgRx/Chat/chat.selector';
import { Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { SignalRService } from '../../Services/signalR.service';
import { ChatNotificationService } from './chat-notification.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.html',
  styleUrl: './chat.css',
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class Chat implements OnDestroy, AfterViewInit {
  user = signal<ClientModel | FreelancerModel | null>(null);
  chatRooms = signal<ChatRoomModel[]>([]);
  selectedRoom = signal<ChatRoomModel | null>(null);
  messages = signal<ChatMessageModel[]>([]);
  loadingRooms = signal(false);
  loadingMessages = signal(false);
  hasMoreMessages = signal(true);
  messagePage = signal(1);
  pageSize = 20;
  newMessage = signal('');
  editingMessageId = signal<string | null>(null);
  editContent = signal('');

  @ViewChild('messagesContainer') messagesContainer!: ElementRef<HTMLDivElement>;

  private subscriptions = new Subscription();
  private lastScrollHeight = 0;

  constructor(
    private chatService: ChatService,
    private authService: AuthenticationService,
    private toastr: ToastrService,
    private store: Store,
    private signalRService: SignalRService,
    private chatNotificationService: ChatNotificationService
  ) { }

  ngOnInit() {
    this.subscriptions.add(
      this.authService.user$.subscribe(user => {
        this.user.set(user);
        if (user) {
          this.fetchChatRooms(user.id);
        }
      })
    );
    this.subscriptions.add(
      this.store.select(selectChatMessages)
        .pipe(debounceTime(100))
        .subscribe(messages => {
          this.messages.set(messages);
          this.markLatestOtherUnreadAsRead();
          // Scroll logic here
          if (this.lastScrollHeight && this.messagesContainer && this.messagesContainer.nativeElement) {
            const el = this.messagesContainer.nativeElement;
            el.scrollTop = el.scrollHeight - this.lastScrollHeight;
            this.lastScrollHeight = 0;
          } else {
            setTimeout(() => this.scrollToBottom(), 50);
          }
        })
    );
    this.subscriptions.add(
      this.store.select(selectChatLoading).subscribe(loading => {
        this.loadingMessages.set(loading);
      })
    );
    this.subscriptions.add(
      this.store.select(selectChatError).subscribe(error => {
        if (error) {
          this.hasMoreMessages.set(false); // Stop further loading
          this.loadingMessages.set(false);
          console.error('Failed to load messages:', error);
          if(error.includes('does not exist')) {
            this.toastr.info("Reached end of messages", 'Info');
          } else {
            this.toastr.error('Failed to load messages', 'Error');
          }
        }
      })
    );
    this.subscriptions.add(
      this.signalRService.chatNotification$.subscribe(message => {
        if (!message) return;
        // If message is for the open chat, dispatch to store
        if (this.selectedRoom() && message.chatRoomId === this.selectedRoom()!.id) {
          this.store.dispatch(ChatActions.receiveMessage({ message }));
        } else {
          // Otherwise, store as notification and show a toast
          this.chatNotificationService.addNotification(message);
          this.toastr.info('New message received in chat!', 'Chat Notification');
        }
      })
    );
  }

  onScrollTop(event: any) {
    const target = event.target;
    if (target.scrollTop === 0 && this.hasMoreMessages() && !this.loadingMessages()) {
      this.lastScrollHeight = target.scrollHeight;
      this.loadingMessages.set(true);
      this.messagePage.set(this.messagePage() + 1);
      this.store.dispatch(ChatActions.loadMessages({
        chatRoomId: this.selectedRoom()!.id,
        page: this.messagePage(),
        pageSize: this.pageSize
      }));
    }
  }

  ngAfterViewInit() {
    this.scrollToBottom();
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  fetchChatRooms(userId: string) {
    this.loadingRooms.set(true);
    this.chatService.getChatRoomsByUserId(userId).subscribe({
      next: response => {
        this.chatRooms.set(response.data.data);
        this.loadingRooms.set(false);
      },
      error: () => {
        this.toastr.error('Failed to load chat rooms');
        this.loadingRooms.set(false);
      }
    });
  }

  selectRoom(room: ChatRoomModel) {
    this.selectedRoom.set(room);
    this.messagePage.set(1);
    this.hasMoreMessages.set(true);
    // Clear notifications for this room when opened
    this.chatNotificationService.clearNotificationsForRoom(room.id);
    this.store.dispatch(ChatActions.loadMessages({
      chatRoomId: room.id,
      page: this.messagePage(),
      pageSize: this.pageSize
    }));
  }

  selectRoomById(roomId: string) {
    const room = this.chatRooms().find(r => r.id === roomId);
    if (room) this.selectRoom(room);
  }

  markLatestOtherUnreadAsRead() {
    const user = this.user();
    const messages = this.messages();
    if (!user || !messages || messages.length === 0) return;
    // Find the latest message from the other person that is unread
    const latestOtherUnread = messages.slice().reverse().find(m => !m.isRead && m.senderId !== user.id);
    // console.log('Marking latest other unread as read:', latestOtherUnread, user?.username);
    if (!latestOtherUnread) return;
    this.store.dispatch(ChatActions.markMessageAsRead({
      messageId: latestOtherUnread.id,
      chatRoomId: latestOtherUnread.chatRoomId,
      senderId: user.id
    }));
    // console.log('Marked as read:', latestOtherUnread.id, user?.username);
  }

  sendMessage() {
    const content = this.newMessage().trim();
    const room = this.selectedRoom();
    if (!content || !room) return;
    this.store.dispatch(ChatActions.sendMessage({
      content,
      chatRoomId: room.id,
      senderId: this.user()!.id
    }));
    this.newMessage.set('');
  }

  startEdit(msg: ChatMessageModel) {
    this.editingMessageId.set(msg.id);
    this.editContent.set(msg.content);
  }

  saveEdit(msg: ChatMessageModel) {
    const content = this.editContent().trim();
    if (!content) return;
    this.store.dispatch(ChatActions.updateMessage({
      messageId: msg.id,
      content
    }));
    this.editingMessageId.set(null);
    this.editContent.set('');
  }

  deleteMessage(msg: ChatMessageModel) {
    this.store.dispatch(ChatActions.deleteMessage({
      messageId: msg.id,
      chatRoomId: this.selectedRoom()!.id
    }));
  }

  isUnread(msg: ChatMessageModel) {
    return !msg.isRead && msg.senderId !== this.user()!.id;
  }
  public getUnreadCount(room: ChatRoomModel): number {
    return this.chatNotificationService.getUnreadCountForRoom(room.id);
  }
  public getLastUnreadMessage(room: ChatRoomModel): string {
    const unread = this.chatNotificationService.getNotifications().find(n => n.chatRoomId === room.id);
    return unread ? unread.content : (room.messages && room.messages.length > 0 ? room.messages[room.messages.length - 1].content : 'No messages yet');
  }
  public getLastMessageTime(room: ChatRoomModel): string {
    if (!room.messages || room.messages.length === 0) return '';
    const msg = room.messages[room.messages.length - 1];
    return msg.sentAt ? (new Date(msg.sentAt)).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', hour12: true }) : '';
  }
  public getMessageSenderInitial(msg: ChatMessageModel): string {
    const room = this.selectedRoom();
    if (!room) return '?';
    if (msg.senderId === room.clientId) return room.clientName ? room.clientName[0] : '?';
    if (msg.senderId === room.freelancerId) return room.freelancerName ? room.freelancerName[0] : '?';
    return '?';
  }

  scrollToBottom() {
    if (this.messagesContainer && this.messagesContainer.nativeElement) {
      const el = this.messagesContainer.nativeElement;
      el.scrollTop = el.scrollHeight;
    }
  }
}
