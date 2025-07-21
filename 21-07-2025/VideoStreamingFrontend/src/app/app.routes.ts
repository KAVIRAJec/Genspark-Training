import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/videos',
    pathMatch: 'full'
  },
  {
    path: 'videos',
    loadComponent: () => import('./features/video-list/video-list.component').then(m => m.VideoListComponent)
  },
  {
    path: 'upload',
    loadComponent: () => import('./features/video-upload/video-upload.component').then(m => m.VideoUploadComponent)
  },
  {
    path: 'video/:id',
    loadComponent: () => import('./features/video-player/video-player.component').then(m => m.VideoPlayerComponent)
  },
  {
    path: '**',
    redirectTo: '/videos'
  }
];
