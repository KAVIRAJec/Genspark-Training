export interface ProductDto {
  productId: number;
  productName: string;
  image?: string;
  price?: number;
  userId?: number;
  userName?: string;
  categoryId?: number;
  categoryName?: string;
  colorId?: number;
  colorName?: string;
  colorHexCode?: string;
  modelId?: number;
  modelName?: string;
  storageId?: number;
  sellStartDate?: string;
  sellEndDate?: string;
  isNew?: number;
  isActive: boolean;
  createdDate?: string;
}
export interface CreateProductDto {
  productName: string;
  image?: string;
  price?: number;
  userId?: number;
  categoryId?: number;
  colorId?: number;
  modelId?: number;
  storageId?: number;
  sellStartDate?: string;
  sellEndDate?: string;
  isNew?: number;
  isActive?: boolean;
  createdDate?: string;
}
export interface UpdateProductDto {
  productId: number;
  productName: string;
  image?: string;
  price?: number;
  userId?: number;
  categoryId?: number;
  colorId?: number;
  modelId?: number;
  storageId?: number;
  sellStartDate?: string;
  sellEndDate?: string;
  isNew?: number;
  isActive?: boolean;
  createdDate?: string;
}
