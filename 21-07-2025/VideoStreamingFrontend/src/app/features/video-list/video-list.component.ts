import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { LucideAngularModule, Search, Plus, Filter, Grid, List } from 'lucide-angular';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';

import { VideoService } from '../../core/services/video.service';
import { NotificationService } from '../../core/services/notification.service';
import { VideoResponseDto, PaginationRequestDto } from '../../core/models/video.models';
import { VideoCardComponent } from '../../shared/components/video-card/video-card.component';

@Component({
  selector: 'app-video-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatPaginatorModule,
    MatCardModule,
    MatDialogModule,
    LucideAngularModule,
    VideoCardComponent
  ],
  template: `
    <div class="video-list-container">
      <!-- Header Section -->
      <div class="header-section">
        <div class="title-section">
          <h1 class="page-title">Training Videos</h1>
          <p class="page-subtitle">Discover and stream professional training content</p>
        </div>
        
        <div class="action-section">
          <button 
            mat-fab 
            extended 
            color="primary" 
            routerLink="/upload"
            class="upload-button"
          >
            <lucide-icon [img]="plusIcon" size="20"></lucide-icon>
            Upload Video
          </button>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="filters-section">
        <div class="search-container">
          <mat-form-field class="search-field" appearance="outline">
            <mat-label>Search videos...</mat-label>
            <input 
              matInput 
              [formControl]="searchControl" 
              placeholder="Enter video title or description"
            >
            <mat-icon matPrefix>
              <lucide-icon [img]="searchIcon" size="20"></lucide-icon>
            </mat-icon>
          </mat-form-field>
        </div>

        <div class="filter-controls">
          <mat-form-field appearance="outline" class="sort-field">
            <mat-label>Sort by</mat-label>
            <mat-select [value]="currentSort" (selectionChange)="onSortChange($event.value)">
              <mat-option value="uploadDate">Upload Date</mat-option>
              <mat-option value="title">Title</mat-option>
              <mat-option value="fileSize">File Size</mat-option>
            </mat-select>
          </mat-form-field>

          <div class="view-toggle">
            <button 
              mat-icon-button 
              [class.active]="viewMode === 'grid'"
              (click)="viewMode = 'grid'"
              title="Grid View"
            >
              <lucide-icon [img]="gridIcon" size="20"></lucide-icon>
            </button>
            <button 
              mat-icon-button 
              [class.active]="viewMode === 'list'"
              (click)="viewMode = 'list'"
              title="List View"
            >
              <lucide-icon [img]="listIcon" size="20"></lucide-icon>
            </button>
          </div>
        </div>
      </div>

      <!-- Videos Grid/List -->
      <div class="videos-section">
        <div 
          class="videos-container" 
          [class.grid-view]="viewMode === 'grid'"
          [class.list-view]="viewMode === 'list'"
          *ngIf="videos.length > 0; else noVideos"
        >
          <app-video-card
            *ngFor="let video of videos; let i = index"
            [video]="video"
            [featured]="i === 0"
            (delete)="onDeleteVideo($event)"
            (edit)="onEditVideo($event)"
          ></app-video-card>
        </div>

        <ng-template #noVideos>
          <div class="no-videos">
            <div class="no-videos-content">
              <lucide-icon [img]="searchIcon" size="64" class="no-videos-icon"></lucide-icon>
              <h2>No videos found</h2>
              <p>{{ searchControl.value ? 'Try adjusting your search terms' : 'Upload your first video to get started' }}</p>
              <button 
                mat-fab 
                extended 
                color="primary" 
                routerLink="/upload"
                class="upload-button"
              >
                <lucide-icon [img]="plusIcon" size="20"></lucide-icon>
                Upload Video
              </button>
            </div>
          </div>
        </ng-template>
      </div>

      <!-- Pagination -->
      <div class="pagination-section" *ngIf="totalCount > pageSize">
        <mat-paginator
          [length]="totalCount"
          [pageSize]="pageSize"
          [pageSizeOptions]="[6, 12, 24, 48]"
          [pageIndex]="currentPage - 1"
          (page)="onPageChange($event)"
          showFirstLastButtons
        ></mat-paginator>
      </div>
    </div>
  `,
  styles: [`
    .video-list-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
      min-height: calc(100vh - 140px);
    }

    .header-section {
      display: flex;
      justify-content: space-between;
      align-items: flex-end;
      margin-bottom: 3rem;
      gap: 2rem;
    }

    .title-section {
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

    .upload-button {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
      color: white !important;
      box-shadow: 0 4px 20px rgba(102, 126, 234, 0.3);
      transition: all 0.3s ease;
    }

    .upload-button:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 25px rgba(102, 126, 234, 0.4);
    }

    .filters-section {
      background: rgba(255, 255, 255, 0.8);
      backdrop-filter: blur(10px);
      border: 1px solid rgba(255, 255, 255, 0.2);
      border-radius: 16px;
      padding: 1.5rem;
      margin-bottom: 2rem;
      display: flex;
      gap: 2rem;
      align-items: flex-end;
    }

    .search-container {
      flex: 1;
    }

    .search-field {
      width: 100%;
    }

    .filter-controls {
      display: flex;
      gap: 1rem;
      align-items: center;
    }

    .sort-field {
      min-width: 140px;
    }

    .view-toggle {
      display: flex;
      border: 1px solid rgba(0, 0, 0, 0.2);
      border-radius: 8px;
      overflow: hidden;
    }

    .view-toggle button {
      border-radius: 0;
      border: none;
      color: #666;
      transition: all 0.3s ease;
    }

    .view-toggle button.active {
      background: #667eea;
      color: white;
    }

    .videos-section {
      margin-bottom: 2rem;
    }

    .videos-container.grid-view {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
      gap: 2rem;
    }

    .videos-container.list-view {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .no-videos {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 400px;
    }

    .no-videos-content {
      text-align: center;
      max-width: 400px;
    }

    .no-videos-icon {
      color: #ccc;
      margin-bottom: 1rem;
    }

    .no-videos-content h2 {
      color: #666;
      margin-bottom: 0.5rem;
    }

    .no-videos-content p {
      color: #888;
      margin-bottom: 2rem;
    }

    .pagination-section {
      display: flex;
      justify-content: center;
      margin-top: 3rem;
    }

    mat-paginator {
      background: transparent;
    }

    @media (max-width: 768px) {
      .video-list-container {
        padding: 1rem;
      }

      .header-section {
        flex-direction: column;
        align-items: flex-start;
        gap: 1rem;
      }

      .page-title {
        font-size: 2rem;
      }

      .filters-section {
        flex-direction: column;
        align-items: stretch;
        gap: 1rem;
      }

      .filter-controls {
        justify-content: space-between;
      }

      .videos-container.grid-view {
        grid-template-columns: 1fr;
        gap: 1rem;
      }
    }
  `]
})
export class VideoListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  videos: VideoResponseDto[] = [];
  totalCount = 0;
  currentPage = 1;
  pageSize = 12;
  currentSort = 'uploadDate';
  sortDescending = true;
  viewMode: 'grid' | 'list' = 'grid';

  searchControl = new FormControl('');

  // Icons
  searchIcon = Search;
  plusIcon = Plus;
  filterIcon = Filter;
  gridIcon = Grid;
  listIcon = List;

  constructor(
    private videoService: VideoService,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.setupSearch();
    this.loadVideos();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSearch(): void {
    this.searchControl.valueChanges.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.currentPage = 1;
      this.loadVideos();
    });
  }

  loadVideos(): void {
    const pagination: PaginationRequestDto = {
      page: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchControl.value || undefined,
      sortBy: this.currentSort,
      sortDescending: this.sortDescending
    };

    this.videoService.getVideos(pagination).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.videos = response.data.videos;
          this.totalCount = response.data.totalCount;
          this.videoService.updateVideosCache(this.videos);
        }
      },
      error: (error) => {
        console.error('Error loading videos:', error);
        this.notificationService.showError('Failed to load videos');
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadVideos();
  }

  onSortChange(sortBy: string): void {
    this.currentSort = sortBy;
    this.currentPage = 1;
    this.loadVideos();
  }

  onDeleteVideo(videoId: number): void {
    if (confirm('Are you sure you want to delete this video?')) {
      this.videoService.deleteVideo(videoId).pipe(
        takeUntil(this.destroy$)
      ).subscribe({
        next: (response) => {
          if (response.success) {
            this.notificationService.showSuccess('Video deleted successfully');
            this.loadVideos();
          }
        },
        error: (error) => {
          console.error('Error deleting video:', error);
          this.notificationService.showError('Failed to delete video');
        }
      });
    }
  }

  onEditVideo(video: VideoResponseDto): void {
    // TODO: Implement edit functionality
    this.notificationService.showInfo('Edit functionality coming soon');
  }
}
