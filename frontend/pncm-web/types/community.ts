export interface Post {
  id: string;
  content: string;
  authorId: string;
  authorName: string;
  authorEmail: string;
  mediaIds: string[];
  primaryPhotoUrl?: string | null;
  mediaUrls?: string[] | null;
  likesCount: number;
  commentsCount: number;
  isLiked: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface Contest {
  id: string;
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  isActive: boolean;
  entriesCount: number;
  winnerId: string | null;
}

export interface LeaderboardEntry {
  userId: string;
  userName: string;
  score: number;
  rank: number;
}

export interface CreatePostRequest {
  content: string;
  mediaIds?: string[];
}
