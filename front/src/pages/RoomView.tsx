import { useEffect, useState, useRef } from 'react';
import { useParams } from 'react-router-dom';
import { api } from '../lib/api';
import { Room, Track } from '../types';
import { useSignalR } from '../hooks/useSignalR';
import { Play, Pause, SkipForward, Trash2, ThumbsUp, PlusCircle, Music2 } from 'lucide-react';
import clsx from 'clsx';
import ReactPlayer from 'react-player'; // Используем облегченную версию для YT
import { useAuth } from 'react-oidc-context';
import toast from 'react-hot-toast';

// Получаем картинку
const getYoutubeThumbnail = (url: string) => {
    try {
        const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/;
        const match = url.match(regExp);
        return (match && match[2].length === 11) 
            ? `https://img.youtube.com/vi/${match[2]}/maxresdefault.jpg` 
            : null;
    } catch { return null; }
};

export default function RoomView() {
    const { id } = useParams<{ id: string }>();
    const [room, setRoom] = useState<Room | null>(null);
    const [playlist, setPlaylist] = useState<Track[]>([]);
    
    const [currentTrack, setCurrentTrack] = useState<Track | null>(null);
    const [isPlaying, setIsPlaying] = useState(false);
    
    const [playedSeconds, setPlayedSeconds] = useState(0);
    const [, setDuration] = useState(0);
    const [seekOnReady, setSeekOnReady] = useState<number | null>(null);

    const auth = useAuth();
    const userId = auth.user?.profile.sub
    const [addUrl, setAddUrl] = useState('');
    const [addTitle, setAddTitle] = useState('');

    const playerRef = useRef<ReactPlayer>(null);

    const syncPlayer = (track: Track) => {
        setCurrentTrack(track);
        
        if (track.status === 'Playing' && track.startedAt) {
            setIsPlaying(true);
            const startedAt = new Date(track.startedAt).getTime();
            const now = new Date().getTime();
            
            const diffSeconds = (now - startedAt) / 1000;
            
            if (diffSeconds > 0) {
                setSeekOnReady(diffSeconds);
                if (playerRef.current) {
                    playerRef.current.seekTo(diffSeconds, 'seconds');
                }
            }
        } else {
            setIsPlaying(false);
        }
    };

    const refresh = async () => {
        if (!id) return;
        try {
            const r = await api.get(`/api/rooms/${id}`);
            setRoom(r);
            const p = await api.get(`/api/rooms/${id}/playlist`);
            setPlaylist(p);

            const active = p.find((t: Track) => t.status === 'Playing' || t.status === 'Paused');
            if (active) {
                if (currentTrack?.id !== active.id || !currentTrack) {
                    syncPlayer(active);
                } else if (active.status !== currentTrack.status) {
                    setIsPlaying(active.status === 'Playing');
                }
            } else {
                setCurrentTrack(null);
                setIsPlaying(false);
            }
        } catch (e) {
            console.error(e);
        }
    };

    useEffect(() => { refresh(); }, [id]);

    useSignalR(id!, {
        onTrackAdded: (event: any) => {
            toast.success(`Трек добавлен: ${event.title}`);
            refresh();
        },
        onPlaybackChanged: (event: any) => {
            console.log('Event Playback:', event);
            if (event.status === 'Playing') {
                setIsPlaying(true);
                toast('▶ Воспроизведение');
                if (event.positionInSeconds >= 0 && playerRef.current) {
                    playerRef.current.seekTo(event.positionInSeconds);
                }
            } else if (event.status === 'Paused') {
                setIsPlaying(false);
                toast('⏸ Пауза');
            } else if (event.status === 'Played' || event.status === 'Stopped') {
                setIsPlaying(false);
            }
            refresh();
        },
        onScoreUpdated: () => refresh(),
        onTrackRemoved: () => refresh()
    });

    const addTrack = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!addUrl || !addTitle) return;
        try {
            await api.post(`/api/rooms/${id}/playlist`, {
                title: addTitle,
                artist: 'User Request',
                url: addUrl,
                durationInSeconds: 300 
            });
            setAddUrl('');
            setAddTitle('');
        } catch (error) {
            toast.error("Ошибка добавления трека");
        }
    };

    const handlePlay = async () => await api.post(`/api/rooms/${id}/play`, {});
    const handlePause = async () => await api.post(`/api/rooms/${id}/pause`, {});
    const handleSkip = async () => {
        await api.post(`/api/rooms/${id}/skip`, {});
        toast('⏭ Пропуск');
    };
    const vote = async (trackId: string, val: number) => {
        await api.post(`/api/tracks/${trackId}/vote`, { value: val });
    };
    const handleDelete = async (trackId: string) => await api.delete(`/api/tracks/${trackId}`);

    if (!room) return <div className="p-10 text-center text-gray-500">Загрузка комнаты...</div>;

    const thumbnailUrl = currentTrack ? getYoutubeThumbnail(currentTrack.url) : null;

    return (
        <div className="min-h-screen bg-gray-50 text-gray-900 p-6 grid grid-cols-1 lg:grid-cols-3 gap-6 font-sans">

            <div className="lg:col-span-1 space-y-6">
                <div className="bg-white rounded-2xl overflow-hidden border border-gray-200 shadow-lg">
                    
                    {/* VIDEO CONTAINER */}
                    <div className="aspect-video bg-black relative group">
                        {currentTrack ? (
                            <>
                                <ReactPlayer
                                    ref={playerRef}
                                    url={currentTrack.url}
                                    playing={isPlaying}
                                    width="100%"
                                    height="100%"
                                    controls={false}
                                    onProgress={(state) => {
                                        setPlayedSeconds(state.playedSeconds);
                                        setDuration(state.loadedSeconds || currentTrack.durationInSeconds);
                                    }}
                                    onReady={(player) => {
                                        if (seekOnReady !== null) {
                                            player.seekTo(seekOnReady);
                                            setSeekOnReady(null);
                                        }
                                    }}
                                    onEnded={handleSkip}
                                    config={{
                                        youtube: {
                                            playerVars: { showinfo: 0, controls: 0, modestbranding: 1 }
                                        }
                                    }}
                                />
                                {!isPlaying && thumbnailUrl && (
                                    <div className="absolute inset-0 z-10 pointer-events-none">
                                        <img src={thumbnailUrl} className="w-full h-full object-cover opacity-60" />
                                        <div className="absolute inset-0 flex items-center justify-center">
                                            <Play size={64} className="text-white drop-shadow-lg opacity-80" fill="currentColor"/>
                                        </div>
                                    </div>
                                )}
                            </>
                        ) : (
                            <div className="flex flex-col items-center justify-center h-full text-gray-500">
                                <Music2 size={48} className="mb-2 opacity-50" />
                                <span>Нет активного трека</span>
                            </div>
                        )}
                    </div>

                    {/* PROGRESS BAR & INFO */}
                    <div className="p-6">
                        <div className="mb-4">
                            <h2 className="text-xl font-bold truncate text-gray-900">
                                {currentTrack ? currentTrack.title : 'Очередь пуста'}
                            </h2>
                            <p className="text-gray-500 text-sm">
                                {currentTrack ? currentTrack.artist : 'Добавьте музыку'}
                            </p>
                        </div>

                        {/* Progress Bar (Visual Only) */}
                        {currentTrack && (
                            <div className="w-full bg-gray-200 rounded-full h-1.5 mb-6 overflow-hidden">
                                <div 
                                    className="bg-purple-600 h-1.5 rounded-full transition-all duration-500" 
                                    style={{ width: `${(playedSeconds / (currentTrack.durationInSeconds || 1)) * 100}%` }}
                                ></div>
                            </div>
                        )}

                        <div className="flex items-center justify-center gap-6">
                            {userId ? (
                                <>
                                    {isPlaying ? (
                                        <button onClick={handlePause} className="p-4 bg-gray-100 rounded-full hover:bg-gray-200 text-gray-900 transition border border-gray-300 shadow-sm">
                                            <Pause size={24} fill="currentColor" />
                                        </button>
                                    ) : (
                                        <button onClick={handlePlay} className="p-4 bg-purple-600 text-white rounded-full hover:bg-purple-700 transition shadow-lg shadow-purple-200">
                                            <Play size={24} fill="currentColor" className="ml-1" />
                                        </button>
                                    )}
                                    <button onClick={handleSkip} className="p-4 bg-gray-100 rounded-full hover:bg-gray-200 text-gray-900 transition border border-gray-300 shadow-sm">
                                        <SkipForward size={24} />
                                    </button>
                                </>
                            ) : (
                                <p className="text-xs text-gray-400">Войдите для управления</p>
                            )}
                        </div>
                    </div>
                </div>

                {/* ADD TRACK */}
                {userId && (
                    <form onSubmit={addTrack} className="bg-white p-6 rounded-2xl border border-gray-200 shadow-sm space-y-4">
                        <h3 className="font-bold text-gray-400 text-xs uppercase tracking-wider">Добавить трек</h3>
                        <div className="space-y-2">
                            <input
                                className="w-full bg-gray-50 border border-gray-200 p-3 rounded-lg text-sm focus:border-purple-500 outline-none transition text-black"
                                placeholder="Ссылка на YouTube..."
                                value={addUrl}
                                onChange={e => setAddUrl(e.target.value)}
                            />
                            <div className="flex gap-2">
                                <input
                                    className="w-full bg-gray-50 border border-gray-200 p-3 rounded-lg text-sm focus:border-purple-500 outline-none transition text-black"
                                    placeholder="Название..."
                                    value={addTitle}
                                    onChange={e => setAddTitle(e.target.value)}
                                />
                                <button className="bg-purple-600 text-white px-4 rounded-lg hover:bg-purple-700 transition shadow-md">
                                    <PlusCircle />
                                </button>
                            </div>
                        </div>
                    </form>
                )}
            </div>

            <div className="lg:col-span-2">
                <div className="flex justify-between items-center mb-4">
                    <h3 className="text-2xl font-bold text-gray-800">Очередь воспроизведения</h3>
                    <span className="text-sm bg-gray-200 px-3 py-1 rounded-full text-gray-600">{playlist.length} треков</span>
                </div>
                
                <div className="space-y-3">
                    {playlist.length === 0 && (
                        <div className="p-10 border-2 border-dashed border-gray-300 rounded-xl text-center text-gray-400">
                            Очередь пуста.
                        </div>
                    )}

                    {playlist.map((track, idx) => {
                        const thumbnail = getYoutubeThumbnail(track.url);
                        const isCurrent = currentTrack?.id === track.id;
                        
                        return (
                            <div key={track.id}
                                className={clsx(
                                    "flex items-center gap-4 p-3 rounded-xl border transition shadow-sm",
                                    isCurrent
                                        ? "bg-purple-50 border-purple-200 ring-1 ring-purple-200"
                                        : "bg-white border-gray-200 hover:border-purple-300"
                                )}>

                                <div className="font-mono text-gray-400 w-6 text-center text-sm">
                                    {isCurrent && isPlaying ? <div className="animate-bounce">♫</div> : idx + 1}
                                </div>

                                {/* MINI THUMBNAIL */}
                                <div className="w-16 h-12 bg-gray-200 rounded-md overflow-hidden flex-shrink-0 relative">
                                    {thumbnail ? (
                                        <img src={thumbnail} alt="cover" className="w-full h-full object-cover" />
                                    ) : (
                                        <div className="w-full h-full flex items-center justify-center text-gray-400"><Music2 size={16}/></div>
                                    )}
                                </div>

                                <div className="flex-1 min-w-0">
                                    <div className={clsx("font-bold truncate text-base", isCurrent ? "text-purple-700" : "text-gray-800")}>
                                        {track.title}
                                    </div>
                                    <div className="text-xs text-gray-500 truncate">{track.artist}</div>
                                </div>

                                <div className="flex items-center gap-1 bg-gray-100 rounded-lg p-1 border border-gray-200">
                                    <button
                                        onClick={() => vote(track.id, 1)}
                                        className="p-1.5 hover:bg-white hover:text-green-600 rounded-md transition text-gray-500"
                                        disabled={!userId}
                                    >
                                        <ThumbsUp size={16} />
                                    </button>
                                    <span className="font-mono font-bold text-sm w-6 text-center text-gray-700">{track.score}</span>
                                </div>

                                {userId && (userId === track.addedById || userId === room.ownerId) && (
                                    <button onClick={() => handleDelete(track.id)} className="text-gray-400 hover:text-red-500 hover:bg-red-50 p-2 rounded-full transition">
                                        <Trash2 size={18} />
                                    </button>
                                )}
                            </div>
                        );
                    })}
                </div>
            </div>
        </div>
    );
}