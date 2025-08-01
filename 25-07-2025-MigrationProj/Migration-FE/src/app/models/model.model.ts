export interface ModelDto {
  modelId: number;
  name: string;
  createdDate: string;
  isActive: boolean;
  productCount: number;
}

export interface CreateModelDto {
  name: string;
}

export interface UpdateModelDto {
  modelId: number;
  name: string;
}
