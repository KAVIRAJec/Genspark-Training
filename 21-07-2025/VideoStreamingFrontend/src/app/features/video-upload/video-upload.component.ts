import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { LucideAngularModule, Upload, Video, ArrowLeft, X, CheckCircle } from 'lucide-angular';
import { Subject, takeUntil } from 'rxjs';

import { VideoService } from '../../core/services/video.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-video-upload',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatProgressBarModule,
    LucideAngularModule
  ],
  template: `
    <div class="upload-container">
      <div class="upload-header">
        <button mat-icon-button routerLink="/videos" class="back-button">
          <lucide-icon [img]="arrowLeftIcon" size="24"></lucide-icon>
        </button>
        <div class="header-content">
          <h1 class="page-title">Upload Training Video</h1>
          <p class="page-subtitle">Share your knowledge with the world</p>
        </div>
      </div>

      <div class="upload-content">
        <form [formGroup]="uploadForm" (ngSubmit)="onSubmit()" class="upload-form">
          <!-- File Upload Area -->
          <div class="file-upload-section">
            <div 
              class="file-drop-zone"
              [class.dragover]="isDragOver"
              [class.has-file]="selectedFile"
              (dragover)="onDragOver($event)"
              (dragleave)="onDragLeave($event)"
              (drop)="onFileDropped($event)"
              (click)="fileInput.click()"
            >
              <input 
                #fileInput
                type="file" 
                accept="video/*" 
                (change)="onFileSelected($event)"
                hidden
              >
              
              <div class="drop-zone-content" *ngIf="!selectedFile">
                <lucide-icon [img]="uploadIcon" size="64" class="upload-icon"></lucide-icon>
                <h3>Drop your video here or click to browse</h3>
                <p>Supports MP4, AVI, MOV, WMV files up to 500MB</p>
                <button type="button" mat-raised-button color="primary" class="browse-button">
                  Choose File
                </button>
              </div>

              <div class="file-info" *ngIf="selectedFile">
                <div class="file-preview">
                  <lucide-icon [img]="videoIcon" size="48" class="file-icon"></lucide-icon>
                  <div class="file-details">
                    <h4>{{ selectedFile.name }}</h4>
                    <p>{{ formatFileSize(selectedFile.size) }}</p>
                    <p>{{ selectedFile.type }}</p>
                  </div>
                  <button 
                    type="button" 
                    mat-icon-button 
                    class="remove-file"
                    (click)="removeFile($event)"
                  >
                    <lucide-icon [img]="xIcon" size="20"></lucide-icon>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Form Fields -->
          <div class="form-fields">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Video Title *</mat-label>
              <input 
                matInput 
                formControlName="title" 
                placeholder="Enter a descriptive title for your video"
                maxlength="200"
              >
              <mat-hint align="end">{{ uploadForm.get('title')?.value?.length || 0 }}/200</mat-hint>
              <mat-error *ngIf="uploadForm.get('title')?.invalid && uploadForm.get('title')?.touched">
                Title is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Description</mat-label>
              <textarea 
                matInput 
                formControlName="description" 
                placeholder="Describe what this video covers..."
                rows="4"
                maxlength="1000"
              ></textarea>
              <mat-hint align="end">{{ uploadForm.get('description')?.value?.length || 0 }}/1000</mat-hint>
            </mat-form-field>
          </div>

          <!-- Upload Progress -->
          <div class="upload-progress" *ngIf="isUploading">
            <div class="progress-info">
              <span>Uploading your video...</span>
              <span>{{ uploadProgress }}%</span>
            </div>
            <mat-progress-bar 
              mode="determinate" 
              [value]="uploadProgress"
              class="progress-bar"
            ></mat-progress-bar>
          </div>

          <!-- Success Message -->
          <div class="success-message" *ngIf="uploadSuccess">
            <lucide-icon [img]="checkIcon" size="48" class="success-icon"></lucide-icon>
            <h3>Upload Successful!</h3>
            <p>Your video has been uploaded and is ready to view.</p>
          </div>

          <!-- Action Buttons -->
          <div class="form-actions">
            <button 
              type="button" 
              mat-stroked-button 
              routerLink="/videos"
              [disabled]="isUploading"
            >
              Cancel
            </button>
            
            <button 
              type="submit" 
              mat-raised-button 
              color="primary"
              [disabled]="uploadForm.invalid || !selectedFile || isUploading"
              class="upload-submit-button"
            >
              <lucide-icon [img]="uploadIcon" size="20" *ngIf="!isUploading"></lucide-icon>
              {{ isUploading ? 'Uploading...' : 'Upload Video' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .upload-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 2rem;
      min-height: calc(100vh - 140px);
    }

    .upload-header {
      display: flex;
      align-items: center;
      gap: 1rem;
      margin-bottom: 3rem;
    }

    .back-button {
      color: #667eea;
      background: rgba(102, 126, 234, 0.1);
      border: 1px solid rgba(102, 126, 234, 0.2);
    }

    .header-content {
      flex: 1;
    }

    .page-title {
      font-size: 2.5rem;
      font-weight: 700;
      margin: 0 0 0.5rem 0;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .page-subtitle {
      font-size: 1.1rem;
      color: #666;
      margin: 0;
    }

    .upload-content {
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(10px);
      border: 1px solid rgba(255, 255, 255, 0.2);
      border-radius: 20px;
      padding: 2rem;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
    }

    .upload-form {
      display: flex;
      flex-direction: column;
      gap: 2rem;
    }

    .file-upload-section {
      margin-bottom: 1rem;
    }

    .file-drop-zone {
      border: 2px dashed #ddd;
      border-radius: 16px;
      padding: 3rem 2rem;
      text-align: center;
      cursor: pointer;
      transition: all 0.3s ease;
      background: rgba(248, 250, 252, 0.8);
    }

    .file-drop-zone:hover,
    .file-drop-zone.dragover {
      border-color: #667eea;
      background: rgba(102, 126, 234, 0.05);
      transform: scale(1.02);
    }

    .file-drop-zone.has-file {
      border-color: #4caf50;
      background: rgba(76, 175, 80, 0.05);
    }

    .drop-zone-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1rem;
    }

    .upload-icon {
      color: #999;
      transition: color 0.3s ease;
    }

    .file-drop-zone:hover .upload-icon,
    .file-drop-zone.dragover .upload-icon {
      color: #667eea;
    }

    .drop-zone-content h3 {
      margin: 0;
      color: #333;
      font-weight: 600;
    }

    .drop-zone-content p {
      margin: 0;
      color: #666;
      font-size: 0.9rem;
    }

    .browse-button {
      margin-top: 1rem;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
      color: white !important;
    }

    .file-info {
      display: flex;
      justify-content: center;
    }

    .file-preview {
      display: flex;
      align-items: center;
      gap: 1rem;
      background: white;
      padding: 1.5rem;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
      max-width: 400px;
      width: 100%;
    }

    .file-icon {
      color: #4caf50;
      flex-shrink: 0;
    }

    .file-details {
      flex: 1;
    }

    .file-details h4 {
      margin: 0 0 0.25rem 0;
      color: #333;
      font-weight: 600;
      word-break: break-word;
    }

    .file-details p {
      margin: 0;
      color: #666;
      font-size: 0.9rem;
    }

    .remove-file {
      color: #f44336;
      flex-shrink: 0;
    }

    .form-fields {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .full-width {
      width: 100%;
    }

    .upload-progress {
      background: rgba(102, 126, 234, 0.05);
      border: 1px solid rgba(102, 126, 234, 0.1);
      border-radius: 12px;
      padding: 1.5rem;
    }

    .progress-info {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
      font-weight: 500;
      color: #667eea;
    }

    .progress-bar {
      height: 8px;
      border-radius: 4px;
    }

    .success-message {
      text-align: center;
      background: rgba(76, 175, 80, 0.05);
      border: 1px solid rgba(76, 175, 80, 0.1);
      border-radius: 12px;
      padding: 2rem;
    }

    .success-icon {
      color: #4caf50;
      margin-bottom: 1rem;
    }

    .success-message h3 {
      margin: 0 0 0.5rem 0;
      color: #4caf50;
      font-weight: 600;
    }

    .success-message p {
      margin: 0;
      color: #666;
    }

    .form-actions {
      display: flex;
      gap: 1rem;
      justify-content: flex-end;
    }

    .upload-submit-button {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
      color: white !important;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    @media (max-width: 768px) {
      .upload-container {
        padding: 1rem;
      }

      .page-title {
        font-size: 2rem;
      }

      .upload-content {
        padding: 1.5rem;
      }

      .file-drop-zone {
        padding: 2rem 1rem;
      }

      .form-actions {
        flex-direction: column;
      }

      .form-actions button {
        width: 100%;
      }
    }
  `]
})
export class VideoUploadComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  uploadForm!: FormGroup;
  selectedFile: File | null = null;
  isDragOver = false;
  isUploading = false;
  uploadProgress = 0;
  uploadSuccess = false;

  // Icons
  uploadIcon = Upload;
  videoIcon = Video;
  arrowLeftIcon = ArrowLeft;
  xIcon = X;
  checkIcon = CheckCircle;

  constructor(
    private fb: FormBuilder,
    private videoService: VideoService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): void {
    this.uploadForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(1000)]]
    });
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = false;
  }

  onFileDropped(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = false;
    
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.handleFileSelection(files[0]);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.handleFileSelection(input.files[0]);
    }
  }

  private handleFileSelection(file: File): void {
    // Validate file type
    if (!file.type.startsWith('video/')) {
      this.notificationService.showError('Please select a valid video file');
      return;
    }

    // Validate file size (500MB limit)
    const maxSize = 500 * 1024 * 1024; // 500MB in bytes
    if (file.size > maxSize) {
      this.notificationService.showError('File size must be less than 500MB');
      return;
    }

    this.selectedFile = file;
    
    // Auto-fill title if empty
    if (!this.uploadForm.get('title')?.value) {
      const fileName = file.name.split('.')[0];
      this.uploadForm.patchValue({ title: fileName });
    }
  }

  removeFile(event: Event): void {
    event.stopPropagation();
    this.selectedFile = null;
  }

  onSubmit(): void {
    if (this.uploadForm.invalid || !this.selectedFile || this.isUploading) {
      return;
    }

    const formData = {
      title: this.uploadForm.value.title,
      description: this.uploadForm.value.description || '',
      videoFile: this.selectedFile
    };

    this.isUploading = true;
    this.uploadProgress = 0;

    // Simulate upload progress (in real implementation, this would come from the HTTP request)
    const progressInterval = setInterval(() => {
      if (this.uploadProgress < 90) {
        this.uploadProgress += Math.random() * 10;
      }
    }, 500);

    this.videoService.uploadVideo(formData).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (response) => {
        clearInterval(progressInterval);
        this.uploadProgress = 100;
        
        if (response.success) {
          this.uploadSuccess = true;
          this.notificationService.showSuccess('Video uploaded successfully!');
          
          // Redirect to videos list after 2 seconds
          setTimeout(() => {
            this.router.navigate(['/videos']);
          }, 2000);
        }
      },
      error: (error) => {
        clearInterval(progressInterval);
        this.isUploading = false;
        this.uploadProgress = 0;
        console.error('Upload error:', error);
        this.notificationService.showError('Failed to upload video. Please try again.');
      }
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
