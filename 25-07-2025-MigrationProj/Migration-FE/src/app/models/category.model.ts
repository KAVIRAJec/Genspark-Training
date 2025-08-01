export interface CategoryDto {
  categoryId: number;
  name: string;
  isActive: boolean;
  productCount: number;
  createdDate: string;
}
export interface CreateCategoryDto {
  name: string;
}
export interface UpdateCategoryDto {
  categoryId: number;
  name: string;
}
