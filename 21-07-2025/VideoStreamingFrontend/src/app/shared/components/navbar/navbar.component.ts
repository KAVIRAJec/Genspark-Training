import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { LucideAngularModule, Upload, Video, Home } from 'lucide-angular';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    LucideAngularModule
  ],
  template: `
    <mat-toolbar class="navbar-container">
      <div class="navbar-content">
        <div class="brand-section">
          <lucide-icon 
            [img]="videoIcon" 
            class="brand-icon"
            size="32"
          ></lucide-icon>
          <span class="brand-text">VideoHub</span>
          <span class="brand-tag">Portal</span>
        </div>
        
        <nav class="nav-links">
          <a mat-button routerLink="/videos" routerLinkActive="active" class="nav-link">
            <lucide-icon [img]="homeIcon" size="20"></lucide-icon>
            <span>Videos</span>
          </a>
          <a mat-button routerLink="/upload" routerLinkActive="active" class="nav-link">
            <lucide-icon [img]="uploadIcon" size="20"></lucide-icon>
            <span>Upload</span>
          </a>
        </nav>
      </div>
    </mat-toolbar>
  `,
  styles: [`
    .navbar-container {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
      backdrop-filter: blur(10px);
      border-bottom: 1px solid rgba(255, 255, 255, 0.1);
      position: sticky;
      top: 0;
      z-index: 1000;
      min-height: 70px;
    }

    .navbar-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      width: 100%;
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
    }

    .brand-section {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      font-weight: 600;
    }

    .brand-icon {
      color: #fff;
      filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.2));
    }

    .brand-text {
      font-size: 1.5rem;
      font-weight: 700;
      background: linear-gradient(45deg, #fff, #e0e7ff);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .brand-tag {
      font-size: 0.75rem;
      background: rgba(255, 255, 255, 0.2);
      padding: 0.25rem 0.5rem;
      border-radius: 12px;
      backdrop-filter: blur(10px);
      border: 1px solid rgba(255, 255, 255, 0.1);
    }

    .nav-links {
      display: flex;
      gap: 0.5rem;
    }

    .nav-link {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      color: rgba(255, 255, 255, 0.9) !important;
      border: 1px solid transparent;
      border-radius: 12px;
      padding: 0.5rem 1rem;
      transition: all 0.3s ease;
      font-weight: 500;
      backdrop-filter: blur(10px);
    }

    .nav-link:hover {
      background: rgba(255, 255, 255, 0.1);
      border-color: rgba(255, 255, 255, 0.2);
      transform: translateY(-1px);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    }

    .nav-link.active {
      background: rgba(255, 255, 255, 0.2);
      border-color: rgba(255, 255, 255, 0.3);
      color: white !important;
    }

    @media (max-width: 768px) {
      .brand-text {
        font-size: 1.25rem;
      }
      
      .nav-link span {
        display: none;
      }
      
      .nav-link {
        padding: 0.5rem;
      }
      
      .navbar-content {
        padding: 0 0.5rem;
      }
    }
  `]
})
export class NavbarComponent {
  videoIcon = Video;
  uploadIcon = Upload;
  homeIcon = Home;
}
