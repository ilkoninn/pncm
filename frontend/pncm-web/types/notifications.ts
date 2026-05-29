export interface NotificationDto {
  id: string;
  userId: string;
  title: string;
  body: string;
  type: number;
  isRead: boolean;
  createdAt: string;
}
