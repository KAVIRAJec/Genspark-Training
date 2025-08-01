export interface ContactUsDto {
  contactId: number;
  name: string;
  email: string;
  subject?: string;
  message: string;
  createdDate: string;
  isActive: boolean;
  isRead: boolean;
  response?: string;
  responseDate?: string;
}
export interface CreateContactUsDto {
  name: string;
  email: string;
  subject?: string;
  message: string;
}
export interface UpdateContactUsDto {
  contactId: number;
  name: string;
  email: string;
  subject?: string;
  message: string;
  response?: string;
  responseDate?: string;
  isRead: boolean;
}