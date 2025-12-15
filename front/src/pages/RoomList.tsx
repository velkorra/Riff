import { useEffect, useState } from 'react';
import { api } from '../lib/api';
import { Room } from '../types';
import { Link } from 'react-router-dom';
import { Plus, Music } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export default function RoomList() {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [newRoomName, setNewRoomName] = useState('');
  const { token } = useAuth();

  const loadRooms = async () => {
    const data = await api.get('/api/rooms?limit=50');
    setRooms(data);
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
    <div className="max-w-5xl mx-auto p-6">
      <header className="flex justify-between items-center mb-10">
        <h1 className="text-4xl font-black text-transparent bg-clip-text bg-gradient-to-r from-purple-400 to-pink-600">
          RIFF ROOMS
        </h1>
        {token && (
          <form onSubmit={createRoom} className="flex gap-2">
            <input 
              className="bg-neutral-800 border border-neutral-700 p-2 rounded text-sm w-64 focus:border-purple-500 outline-none"
              placeholder="New Room Name..."
              value={newRoomName}
              onChange={e => setNewRoomName(e.target.value)}
            />
            <button className="bg-purple-600 p-2 rounded hover:bg-purple-700"><Plus /></button>
          </form>
        )}
      </header>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {rooms.map(room => (
          <Link to={`/room/${room.id}`} key={room.id} className="group">
            <div className="bg-neutral-800 p-6 rounded-xl border border-neutral-700 hover:border-purple-500 transition h-40 flex flex-col justify-between relative overflow-hidden">
              <div className="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition">
                <Music size={64} />
              </div>
              <h2 className="text-xl font-bold truncate pr-4">{room.name}</h2>
              <div className="text-sm text-neutral-400">
                Created {new Date(room.createdAt).toLocaleDateString()}
              </div>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}