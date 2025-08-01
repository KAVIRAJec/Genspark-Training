export interface ColorDto {
  colorId: number;
  name: string;
  hexCode: string;
  isActive: boolean;
  productCount: number;
  createdDate: string;
}
export interface CreateColorDto {
  name: string;
  hexCode: string;
}
export interface UpdateColorDto {
  colorId: number;
  name: string;
  hexCode: string;
}
