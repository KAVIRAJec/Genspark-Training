export interface Address {
  street: string;
  city: string;
  state: string;
  zip: string;
}

export interface CustomerDetailsModel {
  id: number;
  name: string;
  email: string;
  phone: string;
  address: Address;
  registeredDate: string;
}