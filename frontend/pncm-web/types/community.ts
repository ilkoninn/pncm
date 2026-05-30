export interface Post {
  id: string;
  userId: string;
  content: string;
  authorName: string;
  authorAvatarUrl?: string | null;
  mediaIds: string[];
  primaryPhotoUrl?: string | null;
  mediaUrls?: string[] | null;
  likesCount: number;
  commentsCount: number;
  isLiked: boolean;
  createdAt: string;
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
  authorAvatarUrl?: string | null;
}
