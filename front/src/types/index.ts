export interface User {
  id: string;
  username: string;
}

export interface Room {
  id: string;
  name: string;
  ownerId: string;
  createdAt: string;
}

export interface Track {
  id: string;
  title: string;
  artist: string;
  url: string;
  durationInSeconds: number;
  score: number;
  addedById: string;
  roomId: string;
  startedAt?: string; 
  status?: 'Pending' | 'Playing' | 'Paused' | 'Played'; 
}

export interface AuthState {
  token: string | null;
  user: User | null;
}