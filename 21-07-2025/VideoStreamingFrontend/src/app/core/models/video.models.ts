export interface VideoResponseDto {
  id: number;
  title: string;
  description?: string;
  uploadDate: string;
  blobUrl: string;
  fileName?: string;
  fileSize: number;
  contentType?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface VideoListResponseDto {
  videos: VideoResponseDto[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface VideoUploadRequestDto {
  title: string;
  description?: string;
  videoFile: File;
}

export interface ApiResponseDto<T> {
  success: boolean;
  message: string;
  data?: T;
  errors: string[];
}

export interface PaginationRequestDto {
  page: number;
  pageSize: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending: boolean;
}

export interface VideoStreamResponseDto {
  videoStream: Blob;
  contentType: string;
  contentLength: number;
  fileName: string;
}
