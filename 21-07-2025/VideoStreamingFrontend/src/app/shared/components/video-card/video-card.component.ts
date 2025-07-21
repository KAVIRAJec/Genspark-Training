import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { LucideAngularModule, Play, MoreVertical, Trash2, Edit } from 'lucide-angular';
import { VideoResponseDto } from '../../../core/models/video.models';

@Component({
  selector: 'app-video-card',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    LucideAngularModule
  ],
  template: `
    <mat-card class="video-card" [class.featured]="featured">
      <div class="video-thumbnail">
        <div class="thumbnail-placeholder">
          <lucide-icon [img]="playIcon" size="48" class="play-icon"></lucide-icon>
        </div>
        <div class="video-overlay">
          <button 
            mat-fab 
            color="primary" 
            class="play-button"
            [routerLink]="['/video', video.id]"
          >
            <lucide-icon [img]="playIcon" size="24"></lucide-icon>
          </button>
        </div>
        <div class="video-duration">
          {{ formatFileSize(video.fileSize) }}
        </div>
      </div>
      
      <mat-card-content class="video-content">
        <div class="video-header">
          <h3 class="video-title" [title]="video.title">{{ video.title }}</h3>
          <button 
            mat-icon-button 
            class="menu-button"
            [matMenuTriggerFor]="menu"
          >
            <lucide-icon [img]="moreIcon" size="20"></lucide-icon>
          </button>
        </div>
        
        <p class="video-description" *ngIf="video.description">
          {{ video.description | slice:0:100 }}{{ video.description.length > 100 ? '...' : '' }}
        </p>
        
        <div class="video-meta">
          <span class="upload-date">{{ formatDate(video.uploadDate) }}</span>
          <span class="file-info">{{ video.contentType }}</span>
        </div>
      </mat-card-content>

      <mat-menu #menu="matMenu" class="video-menu">
        <button mat-menu-item (click)="onEdit()">
          <lucide-icon [img]="editIcon" size="16"></lucide-icon>
          <span>Edit</span>
        </button>
        <button mat-menu-item (click)="onDelete()" class="delete-item">
          <lucide-icon [img]="deleteIcon" size="16"></lucide-icon>
          <span>Delete</span>
        </button>
      </mat-menu>
    </mat-card>
  `,
  styles: [`
    .video-card {
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(10px);
      border: 1px solid rgba(255, 255, 255, 0.2);
      border-radius: 16px;
      overflow: hidden;
      transition: all 0.3s ease;
      cursor: pointer;
      height: 100%;
      display: flex;
      flex-direction: column;
    }

    .video-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 12px 40px rgba(0, 0, 0, 0.15);
      border-color: rgba(102, 126, 234, 0.3);
    }

    .video-card.featured {
      border: 2px solid #667eea;
      box-shadow: 0 8px 32px rgba(102, 126, 234, 0.2);
    }

    .video-thumbnail {
      position: relative;
      width: 100%;
      height: 200px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      display: flex;
      align-items: center;
      justify-content: center;
      overflow: hidden;
    }

    .thumbnail-placeholder {
      color: rgba(255, 255, 255, 0.7);
    }

    .video-overlay {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: rgba(0, 0, 0, 0.4);
      display: flex;
      align-items: center;
      justify-content: center;
      opacity: 0;
      transition: opacity 0.3s ease;
    }

    .video-card:hover .video-overlay {
      opacity: 1;
    }

    .play-button {
      background: rgba(255, 255, 255, 0.9) !important;
      color: #667eea !important;
      transform: scale(0.8);
      transition: transform 0.3s ease;
    }

    .video-card:hover .play-button {
      transform: scale(1);
    }

    .video-duration {
      position: absolute;
      bottom: 8px;
      right: 8px;
      background: rgba(0, 0, 0, 0.8);
      color: white;
      padding: 4px 8px;
      border-radius: 6px;
      font-size: 0.75rem;
      font-weight: 500;
    }

    .video-content {
      padding: 1rem;
      flex: 1;
      display: flex;
      flex-direction: column;
    }

    .video-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 0.5rem;
    }

    .video-title {
      font-size: 1.1rem;
      font-weight: 600;
      color: #333;
      margin: 0;
      line-height: 1.3;
      overflow: hidden;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      flex: 1;
      margin-right: 0.5rem;
    }

    .menu-button {
      color: #666;
      width: 32px;
      height: 32px;
      flex-shrink: 0;
    }

    .video-description {
      color: #666;
      font-size: 0.9rem;
      line-height: 1.4;
      margin: 0 0 1rem 0;
      flex: 1;
    }

    .video-meta {
      display: flex;
      justify-content: space-between;
      align-items: center;
      font-size: 0.8rem;
      color: #888;
      border-top: 1px solid rgba(0, 0, 0, 0.1);
      padding-top: 0.75rem;
    }

    .upload-date {
      font-weight: 500;
    }

    .file-info {
      background: rgba(102, 126, 234, 0.1);
      color: #667eea;
      padding: 2px 6px;
      border-radius: 6px;
      font-weight: 500;
    }

    .video-menu {
      backdrop-filter: blur(10px);
    }

    .video-menu button {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .delete-item {
      color: #f44336;
    }

    @media (max-width: 768px) {
      .video-thumbnail {
        height: 160px;
      }
      
      .video-title {
        font-size: 1rem;
      }
    }
  `]
})
export class VideoCardComponent {
  @Input() video!: VideoResponseDto;
  @Input() featured = false;
  @Output() delete = new EventEmitter<number>();
  @Output() edit = new EventEmitter<VideoResponseDto>();

  playIcon = Play;
  moreIcon = MoreVertical;
  editIcon = Edit;
  deleteIcon = Trash2;

  onDelete(): void {
    this.delete.emit(this.video.id);
  }

  onEdit(): void {
    this.edit.emit(this.video);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }
}
