export interface NewsDto {
  newsId: number;
  title: string;
  content: string;
  summary?: string;
  image?: string;
  createdDate: string;
  updatedDate?: string;
  isPublished: boolean;
  isActive: boolean;
  authorId: number;
  authorName: string;
}

export interface CreateNewsDto {
  title: string;
  content: string;
  summary?: string;
  image?: string;
  isPublished?: boolean;
  authorId: number;
}

export interface UpdateNewsDto {
  newsId: number;
  title: string;
  content: string;
  summary?: string;
  image?: string;
  isPublished?: boolean;
  authorId: number;
}
