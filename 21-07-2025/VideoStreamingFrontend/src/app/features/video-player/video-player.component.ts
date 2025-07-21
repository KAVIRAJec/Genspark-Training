import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { LucideAngularModule, ArrowLeft, Download, Share, MoreVertical, Play, Pause, Volume2, VolumeX, Maximize, Minimize } from 'lucide-angular';
import { Subject, takeUntil } from 'rxjs';

import { VideoService } from '../../core/services/video.service';
import { NotificationService } from '../../core/services/notification.service';
import { VideoResponseDto } from '../../core/models/video.models';

@Component({
  selector: 'app-video-player',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    LucideAngularModule
  ],
  template: `
    <div class="player-container">
      <!-- Header -->
      <div class="player-header">
        <button mat-icon-button routerLink="/videos" class="back-button">
          <lucide-icon [img]="arrowLeftIcon" size="24"></lucide-icon>
        </button>
        <div class="header-content" *ngIf="video">
          <h1 class="video-title">{{ video.title }}</h1>
          <div class="video-meta">
            <span class="upload-date">Uploaded {{ formatDate(video.uploadDate) }}</span>
            <span class="file-size">{{ formatFileSize(video.fileSize) }}</span>
          </div>
        </div>
        <div class="header-actions" *ngIf="video">
          <button mat-icon-button [matMenuTriggerFor]="menu" class="menu-button">
            <lucide-icon [img]="moreIcon" size="24"></lucide-icon>
          </button>
        </div>
      </div>

      <!-- Video Player -->
      <div class="player-section" *ngIf="video">
        <div class="video-container" [class.fullscreen]="isFullscreen">
          <video
            #videoPlayer
            class="video-element"
            [src]="getVideoUrl()"
            (loadedmetadata)="onVideoLoaded()"
            (timeupdate)="onTimeUpdate()"
            (ended)="onVideoEnded()"
            (play)="onPlay()"
            (pause)="onPause()"
            preload="metadata"
            controls
          ></video>

          <!-- Custom Controls Overlay -->
          <div class="video-overlay" [class.show-controls]="showControls">
            <div class="overlay-content">
              <button 
                mat-fab 
                class="play-pause-button"
                (click)="togglePlayPause()"
              >
                <lucide-icon [img]="isPlaying ? pauseIcon : playIcon" size="32"></lucide-icon>
              </button>
            </div>

            <div class="bottom-controls">
              <div class="progress-container">
                <div class="progress-bar" (click)="seek($event)">
                  <div class="progress-filled" [style.width.%]="progressPercentage"></div>
                  <div class="progress-handle" [style.left.%]="progressPercentage"></div>
                </div>
              </div>

              <div class="control-buttons">
                <div class="left-controls">
                  <span class="time-display">
                    {{ formatTime(currentTime) }} / {{ formatTime(duration) }}
                  </span>
                </div>

                <div class="right-controls">
                  <button mat-icon-button (click)="toggleMute()" class="volume-button">
                    <lucide-icon [img]="isMuted ? volumeOffIcon : volumeIcon" size="20"></lucide-icon>
                  </button>
                  <button mat-icon-button (click)="toggleFullscreen()" class="fullscreen-button">
                    <lucide-icon [img]="isFullscreen ? minimizeIcon : maximizeIcon" size="20"></lucide-icon>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Video Information -->
      <div class="video-info" *ngIf="video">
        <mat-card class="info-card">
          <mat-card-content>
            <h2>Description</h2>
            <p *ngIf="video.description; else noDescription" class="description">
              {{ video.description }}
            </p>
            <ng-template #noDescription>
              <p class="no-description">No description available.</p>
            </ng-template>

            <mat-divider class="divider"></mat-divider>

            <div class="video-details">
              <h3>Video Details</h3>
              <div class="details-grid">
                <div class="detail-item">
                  <span class="label">File Name:</span>
                  <span class="value">{{ video.fileName || 'N/A' }}</span>
                </div>
                <div class="detail-item">
                  <span class="label">File Size:</span>
                  <span class="value">{{ formatFileSize(video.fileSize) }}</span>
                </div>
                <div class="detail-item">
                  <span class="label">Format:</span>
                  <span class="value">{{ video.contentType || 'Unknown' }}</span>
                </div>
                <div class="detail-item">
                  <span class="label">Upload Date:</span>
                  <span class="value">{{ formatDate(video.uploadDate) }}</span>
                </div>
              </div>
            </div>
          </mat-card-content>
        </mat-card>
      </div>

      <!-- Loading State -->
      <div class="loading-state" *ngIf="!video && !error">
        <div class="loading-content">
          <div class="loading-spinner"></div>
          <p>Loading video...</p>
        </div>
      </div>

      <!-- Error State -->
      <div class="error-state" *ngIf="error">
        <div class="error-content">
          <h2>Video Not Found</h2>
          <p>The requested video could not be loaded.</p>
          <button mat-raised-button color="primary" routerLink="/videos">
            Back to Videos
          </button>
        </div>
      </div>

      <!-- Action Menu -->
      <mat-menu #menu="matMenu" class="video-menu">
        <button mat-menu-item (click)="shareVideo()">
          <lucide-icon [img]="shareIcon" size="16"></lucide-icon>
          <span>Share</span>
        </button>
        <button mat-menu-item (click)="downloadVideo()">
          <lucide-icon [img]="downloadIcon" size="16"></lucide-icon>
          <span>Download</span>
        </button>
      </mat-menu>
    </div>
  `,
  styles: [`
    .player-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
      min-height: calc(100vh - 140px);
    }

    .player-header {
      display: flex;
      align-items: flex-start;
      gap: 1rem;
      margin-bottom: 2rem;
    }

    .back-button {
      color: #667eea;
      background: rgba(102, 126, 234, 0.1);
      border: 1px solid rgba(102, 126, 234, 0.2);
      margin-top: 0.5rem;
    }

    .header-content {
      flex: 1;
    }

    .video-title {
      font-size: 2rem;
      font-weight: 700;
      margin: 0 0 0.5rem 0;
      color: #333;
      line-height: 1.2;
    }

    .video-meta {
      display: flex;
      gap: 1rem;
      color: #666;
      font-size: 0.9rem;
    }

    .header-actions {
      margin-top: 0.5rem;
    }

    .menu-button {
      color: #666;
    }

    .player-section {
      margin-bottom: 2rem;
    }

    .video-container {
      position: relative;
      background: #000;
      border-radius: 16px;
      overflow: hidden;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
    }

    .video-container.fullscreen {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      z-index: 2000;
      border-radius: 0;
    }

    .video-element {
      width: 100%;
      height: auto;
      min-height: 400px;
      display: block;
    }

    .video-container.fullscreen .video-element {
      height: 100vh;
      object-fit: contain;
    }

    .video-overlay {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: linear-gradient(
        to bottom,
        transparent 0%,
        transparent 70%,
        rgba(0, 0, 0, 0.7) 100%
      );
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      pointer-events: none;
      opacity: 0;
      transition: opacity 0.3s ease;
    }

    .video-overlay.show-controls {
      opacity: 1;
    }

    .overlay-content {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      pointer-events: auto;
    }

    .play-pause-button {
      background: rgba(255, 255, 255, 0.9) !important;
      color: #333 !important;
      transform: scale(0.8);
      transition: transform 0.3s ease;
    }

    .play-pause-button:hover {
      transform: scale(1);
    }

    .bottom-controls {
      padding: 1rem;
      pointer-events: auto;
    }

    .progress-container {
      margin-bottom: 1rem;
    }

    .progress-bar {
      width: 100%;
      height: 6px;
      background: rgba(255, 255, 255, 0.3);
      border-radius: 3px;
      cursor: pointer;
      position: relative;
    }

    .progress-filled {
      height: 100%;
      background: #667eea;
      border-radius: 3px;
      transition: width 0.1s ease;
    }

    .progress-handle {
      position: absolute;
      top: 50%;
      transform: translateY(-50%) translateX(-50%);
      width: 12px;
      height: 12px;
      background: #667eea;
      border-radius: 50%;
      border: 2px solid white;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
    }

    .control-buttons {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .left-controls,
    .right-controls {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .time-display {
      color: white;
      font-size: 0.9rem;
      font-weight: 500;
    }

    .volume-button,
    .fullscreen-button {
      color: white;
    }

    .info-card {
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(10px);
      border: 1px solid rgba(255, 255, 255, 0.2);
      border-radius: 16px;
    }

    .description {
      color: #666;
      line-height: 1.6;
      margin: 0;
    }

    .no-description {
      color: #999;
      font-style: italic;
      margin: 0;
    }

    .divider {
      margin: 2rem 0;
    }

    .video-details h3 {
      margin: 0 0 1rem 0;
      color: #333;
      font-weight: 600;
    }

    .details-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1rem;
    }

    .detail-item {
      display: flex;
      justify-content: space-between;
      padding: 0.75rem;
      background: rgba(102, 126, 234, 0.05);
      border-radius: 8px;
      border: 1px solid rgba(102, 126, 234, 0.1);
    }

    .label {
      font-weight: 500;
      color: #666;
    }

    .value {
      color: #333;
    }

    .loading-state,
    .error-state {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 400px;
    }

    .loading-content,
    .error-content {
      text-align: center;
    }

    .loading-spinner {
      width: 40px;
      height: 40px;
      border: 4px solid #f3f3f3;
      border-top: 4px solid #667eea;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin: 0 auto 1rem;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .error-content h2 {
      color: #666;
      margin-bottom: 0.5rem;
    }

    .error-content p {
      color: #888;
      margin-bottom: 2rem;
    }

    .video-menu button {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    @media (max-width: 768px) {
      .player-container {
        padding: 1rem;
      }

      .video-title {
        font-size: 1.5rem;
      }

      .video-meta {
        flex-direction: column;
        gap: 0.25rem;
      }

      .details-grid {
        grid-template-columns: 1fr;
      }

      .control-buttons {
        flex-direction: column;
        gap: 0.5rem;
      }
    }
  `]
})
export class VideoPlayerComponent implements OnInit, OnDestroy {
  @ViewChild('videoPlayer') videoPlayerRef!: ElementRef<HTMLVideoElement>;

  private destroy$ = new Subject<void>();
  
  video: VideoResponseDto | null = null;
  error = false;
  
  // Video player state
  isPlaying = false;
  isMuted = false;
  isFullscreen = false;
  showControls = true;
  currentTime = 0;
  duration = 0;
  progressPercentage = 0;

  private controlsTimeout: any;

  // Icons
  arrowLeftIcon = ArrowLeft;
  downloadIcon = Download;
  shareIcon = Share;
  moreIcon = MoreVertical;
  playIcon = Play;
  pauseIcon = Pause;
  volumeIcon = Volume2;
  volumeOffIcon = VolumeX;
  maximizeIcon = Maximize;
  minimizeIcon = Minimize;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private videoService: VideoService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.route.params.pipe(
      takeUntil(this.destroy$)
    ).subscribe(params => {
      const videoId = +params['id'];
      if (videoId) {
        this.loadVideo(videoId);
      }
    });

    // Listen for fullscreen changes
    document.addEventListener('fullscreenchange', this.handleFullscreenChange.bind(this));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    document.removeEventListener('fullscreenchange', this.handleFullscreenChange.bind(this));
    if (this.controlsTimeout) {
      clearTimeout(this.controlsTimeout);
    }
  }

  private loadVideo(id: number): void {
    this.videoService.getVideoById(id).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.video = response.data;
        } else {
          this.error = true;
        }
      },
      error: (error) => {
        console.error('Error loading video:', error);
        this.error = true;
      }
    });
  }

  getVideoUrl(): string {
    if (!this.video) return '';
    return this.videoService.getVideoStreamUrl(this.video.id);
  }

  onVideoLoaded(): void {
    if (this.videoPlayerRef?.nativeElement) {
      this.duration = this.videoPlayerRef.nativeElement.duration;
    }
  }

  onTimeUpdate(): void {
    if (this.videoPlayerRef?.nativeElement) {
      this.currentTime = this.videoPlayerRef.nativeElement.currentTime;
      this.progressPercentage = (this.currentTime / this.duration) * 100;
    }
  }

  onVideoEnded(): void {
    this.isPlaying = false;
  }

  onPlay(): void {
    this.isPlaying = true;
  }

  onPause(): void {
    this.isPlaying = false;
  }

  togglePlayPause(): void {
    if (this.videoPlayerRef?.nativeElement) {
      if (this.isPlaying) {
        this.videoPlayerRef.nativeElement.pause();
      } else {
        this.videoPlayerRef.nativeElement.play();
      }
    }
  }

  toggleMute(): void {
    if (this.videoPlayerRef?.nativeElement) {
      this.videoPlayerRef.nativeElement.muted = !this.videoPlayerRef.nativeElement.muted;
      this.isMuted = this.videoPlayerRef.nativeElement.muted;
    }
  }

  toggleFullscreen(): void {
    if (!document.fullscreenElement) {
      this.videoPlayerRef?.nativeElement.parentElement?.requestFullscreen();
    } else {
      document.exitFullscreen();
    }
  }

  private handleFullscreenChange(): void {
    this.isFullscreen = !!document.fullscreenElement;
  }

  seek(event: MouseEvent): void {
    if (this.videoPlayerRef?.nativeElement) {
      const progressBar = event.currentTarget as HTMLElement;
      const rect = progressBar.getBoundingClientRect();
      const percentage = (event.clientX - rect.left) / rect.width;
      const newTime = percentage * this.duration;
      this.videoPlayerRef.nativeElement.currentTime = newTime;
    }
  }

  shareVideo(): void {
    if (navigator.share && this.video) {
      navigator.share({
        title: this.video.title,
        text: this.video.description || '',
        url: window.location.href
      }).catch(err => {
        console.log('Error sharing:', err);
        this.copyToClipboard();
      });
    } else {
      this.copyToClipboard();
    }
  }

  private copyToClipboard(): void {
    navigator.clipboard.writeText(window.location.href).then(() => {
      this.notificationService.showSuccess('Video link copied to clipboard');
    }).catch(() => {
      this.notificationService.showError('Failed to copy link');
    });
  }

  downloadVideo(): void {
    if (this.video) {
      const link = document.createElement('a');
      link.href = this.getVideoUrl();
      link.download = this.video.fileName || `${this.video.title}.mp4`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  }

  formatTime(seconds: number): string {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = Math.floor(seconds % 60);

    if (hours > 0) {
      return `${hours}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    }
    return `${minutes}:${secs.toString().padStart(2, '0')}`;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
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
