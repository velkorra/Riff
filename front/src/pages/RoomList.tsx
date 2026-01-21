import { useEffect, useState } from 'react';
import { api } from '../lib/api';
import { Room } from '../types';
import { Link } from 'react-router-dom';
import { Music } from 'lucide-react';
import { useAuth } from 'react-oidc-context';

export default function RoomList() {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [newRoomName, setNewRoomName] = useState('');
  const auth = useAuth();
  const token = auth.user?.access_token;

  const loadRooms = async () => {
    try {
        const data = await api.get('/api/rooms?limit=50');
        setRooms(data);
    } catch(e) { console.error(e); }
  };

  useEffect(() => { loadRooms(); }, []);

  const createRoom = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newRoomName) return;
    await api.post('/api/rooms', { name: newRoomName });
    setNewRoomName('');
    loadRooms();
  };

  return (
    <div className="min-h-screen bg-gray-50 text-gray-900">
        <div className="max-w-6xl mx-auto p-8">
        <header className="flex justify-between items-center mb-12 border-b border-gray-200 pb-6">
            <div>
                <h1 className="text-4xl font-black text-gray-900 tracking-tight">
                RIFF <span className="text-purple-600">ROOMS</span>
                </h1>
                <p className="text-gray-500 mt-2">Выберите комнату и слушайте музыку вместе</p>
            </div>
            
            {token && (
            <form onSubmit={createRoom} className="flex gap-2 shadow-sm">
                <input
                className="bg-white border border-gray-300 p-3 rounded-l-lg text-sm w-64 focus:border-purple-500 focus:ring-1 focus:ring-purple-500 outline-none transition"
                placeholder="Название новой комнаты..."
                value={newRoomName}
                onChange={e => setNewRoomName(e.target.value)}
                />
                <button className="bg-purple-600 px-4 rounded-r-lg hover:bg-purple-700 text-white transition font-medium">
                    Создать
                </button>
            </form>
            )}
        </header>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {rooms.map(room => (
            <Link to={`/room/${room.id}`} key={room.id} className="group">
                <div className="bg-white p-6 rounded-xl border border-gray-200 hover:border-purple-400 hover:shadow-lg transition-all duration-300 h-48 flex flex-col justify-between relative overflow-hidden group-hover:-translate-y-1">
                <div className="absolute -bottom-4 -right-4 p-4 text-gray-100 group-hover:text-purple-50 transition transform rotate-12">
                    <Music size={100} />
                </div>
                
                <div>
                    <div className="text-xs font-bold text-purple-600 uppercase tracking-wide mb-2">Room</div>
                    <h2 className="text-xl font-bold truncate pr-4 text-gray-800 leading-tight">{room.name}</h2>
                </div>
                
                <div className="text-sm text-gray-400 flex justify-between items-end relative z-10">
                    <span>Создано: {new Date(room.createdAt).toLocaleDateString()}</span>
                </div>
                </div>
            </Link>
            ))}
        </div>
        </div>
    </div>
  );
}