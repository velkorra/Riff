import { useEffect, useState, useRef } from 'react';
import { useParams } from 'react-router-dom';
import { api } from '../lib/api';
import { Room, Track } from '../types';
import { useSignalR } from '../hooks/useSignalR';
import { useAuth } from '../context/AuthContext';
import { Play, Pause, SkipForward, Trash2, ThumbsUp, PlusCircle } from 'lucide-react';
import clsx from 'clsx';
import ReactPlayer from 'react-player';

export default function RoomView() {
  const { id } = useParams<{ id: string }>();
  const [room, setRoom] = useState<Room | null>(null);
  const [playlist, setPlaylist] = useState<Track[]>([]);
  const [currentTrack, setCurrentTrack] = useState<Track | null>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const { user } = useAuth();
  
  const [addUrl, setAddUrl] = useState('');
  const [addTitle, setAddTitle] = useState('');
  
  const playerRef = useRef<ReactPlayer>(null);

  const refresh = async () => {
    if (!id) return;
    const r = await api.get(`/api/rooms/${id}`);
    setRoom(r);
    const p = await api.get(`/api/rooms/${id}/playlist`);
    setPlaylist(p);
    
    // Ищем играющий трек
    const playing = p.find((t: Track) => t.status === 'Playing');
    if (playing) {
        setCurrentTrack(playing);
        setIsPlaying(true);
    } else {
        const paused = p.find((t: Track) => t.status === 'Paused');
        if (paused) {
            setCurrentTrack(paused);
            setIsPlaying(false);
        }
    }
  };

  useEffect(() => { refresh(); }, [id]);

  // SignalR Handlers
  useSignalR(id!, {
    onTrackAdded: (_event: any) => {
        refresh();
    },
    onPlaybackChanged: (event: any) => {
        console.log('Playback changed:', event);
        if (event.status === 'Playing') {
            setIsPlaying(true);
            const track = playlist.find(t => t.id === event.currentTrackId);
            if (track) setCurrentTrack(track);
            if (event.positionInSeconds > 0 && playerRef.current) {
                 playerRef.current.seekTo(event.positionInSeconds);
            }
        } else if (event.status === 'Paused') {
            setIsPlaying(false);
        } else if (event.status === 'Stopped') {
            setIsPlaying(false);
            setCurrentTrack(null);
        }
        refresh();
    },
    onScoreUpdated: () => refresh(),
    onTrackRemoved: () => refresh()
  });

  const addTrack = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!addUrl || !addTitle) return;
    await api.post(`/api/rooms/${id}/playlist`, { 
        title: addTitle, 
        artist: 'Unknown', 
        url: addUrl, 
        durationInSeconds: 300 
    });
    setAddUrl('');
    setAddTitle('');
  };

  const vote = async (trackId: string, val: number) => {
    await api.post(`/api/tracks/${trackId}/vote`, { value: val });
  };

  const handlePlay = async () => await api.post(`/api/rooms/${id}/play`, {});
  const handlePause = async () => await api.post(`/api/rooms/${id}/pause`, {});
  const handleSkip = async () => await api.post(`/api/rooms/${id}/skip`, {});
  const handleDelete = async (trackId: string) => await api.delete(`/api/tracks/${trackId}`);

  if (!room) return <div className="p-10 text-center">Loading...</div>;

  return (
    <div className="min-h-screen bg-neutral-950 text-white p-6 grid grid-cols-1 lg:grid-cols-3 gap-6">
      
      {/* Плеер и Инфо */}
      <div className="lg:col-span-1 space-y-6">
        <div className="bg-neutral-900 rounded-2xl overflow-hidden border border-neutral-800 shadow-2xl">
          {/* VIDEO PLAYER CONTAINER */}
          <div className="aspect-video bg-black relative">
            {currentTrack ? (
                <ReactPlayer
                    ref={playerRef}
                    url={currentTrack.url}
                    playing={isPlaying}
                    width="100%"
                    height="100%"
                    controls={true}
                />
            ) : (
                <div className="flex items-center justify-center h-full text-neutral-600">
                    No Track Playing
                </div>
            )}
          </div>
          
          {/* CONTROLS */}
          <div className="p-6">
            <h2 className="text-xl font-bold truncate mb-1">
                {currentTrack ? currentTrack.title : 'Waiting for music...'}
            </h2>
            <p className="text-neutral-400 text-sm mb-6">
                {currentTrack ? currentTrack.artist : 'Add a track to start'}
            </p>

            <div className="flex items-center justify-center gap-6">
               {user ? (
                   <>
                    {isPlaying ? (
                        <button onClick={handlePause} className="p-4 bg-neutral-800 rounded-full hover:bg-neutral-700 transition"><Pause /></button>
                    ) : (
                        <button onClick={handlePlay} className="p-4 bg-white text-black rounded-full hover:bg-neutral-200 transition"><Play fill="currentColor" /></button>
                    )}
                    <button onClick={handleSkip} className="p-4 bg-neutral-800 rounded-full hover:bg-neutral-700 transition"><SkipForward /></button>
                   </>
               ) : (
                   <p className="text-xs text-neutral-500">Login to control playback</p>
               )}
            </div>
          </div>
        </div>

        {/* ADD TRACK FORM */}
        {user && (
            <form onSubmit={addTrack} className="bg-neutral-900 p-6 rounded-2xl border border-neutral-800 space-y-3">
                <h3 className="font-bold text-neutral-400 text-sm uppercase tracking-wider">Add Track</h3>
                <input 
                    className="w-full bg-neutral-950 border border-neutral-800 p-3 rounded text-sm focus:border-purple-600 outline-none"
                    placeholder="YouTube URL"
                    value={addUrl}
                    onChange={e => setAddUrl(e.target.value)}
                />
                <div className="flex gap-2">
                    <input 
                        className="w-full bg-neutral-950 border border-neutral-800 p-3 rounded text-sm focus:border-purple-600 outline-none"
                        placeholder="Song Title"
                        value={addTitle}
                        onChange={e => setAddTitle(e.target.value)}
                    />
                    <button className="bg-purple-600 px-4 rounded hover:bg-purple-500 transition"><PlusCircle /></button>
                </div>
            </form>
        )}
      </div>

      {/* ПРАВАЯ КОЛОНКА: Плейлист */}
      <div className="lg:col-span-2">
         <h3 className="text-2xl font-bold mb-4">Queue</h3>
         <div className="space-y-2">
             {playlist.length === 0 && <div className="text-neutral-500">Queue is empty.</div>}
             
             {playlist.map((track, idx) => (
                 <div key={track.id} 
                      className={clsx(
                          "flex items-center gap-4 p-4 rounded-xl border transition",
                          track.status === 'Playing' 
                            ? "bg-purple-900/20 border-purple-500/50" 
                            : "bg-neutral-900 border-neutral-800 hover:border-neutral-600"
                      )}>
                     
                     <div className="font-mono text-neutral-500 w-6 text-center">{idx + 1}</div>
                     
                     <div className="flex-1 min-w-0">
                         <div className={clsx("font-bold truncate", track.status === 'Playing' && "text-purple-400")}>
                             {track.title}
                         </div>
                         <div className="text-xs text-neutral-400 truncate">{track.artist}</div>
                     </div>

                     {/* Score & Vote */}
                     <div className="flex items-center gap-2 bg-neutral-950 rounded-lg p-1">
                         <button 
                            onClick={() => vote(track.id, 1)}
                            className="p-1 hover:text-green-400 transition"
                            disabled={!user}
                         >
                            <ThumbsUp size={16} />
                         </button>
                         <span className="font-mono font-bold text-sm w-6 text-center">{track.score}</span>
                     </div>

                     {/* Delete (only owner or addedBy) */}
                     {user && (user.id === track.addedById || user.id === room.ownerId) && (
                         <button onClick={() => handleDelete(track.id)} className="text-neutral-600 hover:text-red-500 transition">
                             <Trash2 size={18} />
                         </button>
                     )}
                 </div>
             ))}
         </div>
      </div>
    </div>
  );
}