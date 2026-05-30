import client from "./client";
import type { Post, Contest, LeaderboardEntry, CreatePostRequest, ToggleLikeResponse } from "@/types/community";

export async function getPosts(): Promise<Post[]> {
  const { data } = await client.get<Post[]>("/posts");
  return data;
}

export async function createPost(req: CreatePostRequest): Promise<Post> {
  const { data } = await client.post<Post>("/posts", req);
  return data;
}

export async function toggleLike(postId: string): Promise<ToggleLikeResponse> {
  const { data } = await client.post<ToggleLikeResponse>(`/posts/${postId}/like`);
  return data;
}

export async function getContests(): Promise<Contest[]> {
  const { data } = await client.get<Contest[]>("/contests");
  return data;
}

export async function getContestLeaderboard(contestId: string): Promise<LeaderboardEntry[]> {
  const { data } = await client.get<LeaderboardEntry[]>(`/contests/${contestId}/leaderboard`);
  return data;
}
