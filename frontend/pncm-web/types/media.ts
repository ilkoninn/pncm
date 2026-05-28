export enum EOwnerType {
  User = 0,
  Store = 1,
  Pet = 2,
  Community = 3,
}

export interface MediaFileDto {
  id: string;
  fileName: string;
  url: string;
  contentType: string;
  size: number;
  ownerType: EOwnerType;
  ownerId: string;
  createdAt: string;
}
