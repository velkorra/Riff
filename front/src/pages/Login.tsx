import { useState } from 'react';
import { api } from '../lib/api';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';

export default function Login() {
  const [isRegister, setIsRegister] = useState(false);
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (isRegister) {
        await api.post('/auth/register', { email, password });
        alert('Registered! Now login.');
        setIsRegister(false);
      } else {
        const res = await api.post('/auth/login', { email, password });
        await login(res.accessToken);
        navigate('/');
      }
    } catch (err: any) {
      alert(err.message);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-black">
      <div className="w-full max-w-md p-8 bg-neutral-800 rounded-xl border border-neutral-700">
        <h1 className="text-2xl font-bold mb-6 text-center text-purple-400">RIFF {isRegister ? 'Register' : 'Login'}</h1>
        <form onSubmit={handleSubmit} className="space-y-4">
          <input
            type="email"
            placeholder="Email"
            className="w-full p-3 bg-neutral-900 rounded border border-neutral-700 focus:border-purple-500 outline-none"
            value={email}
            onChange={e => setEmail(e.target.value)}
          />
          <input
            type="password"
            placeholder="Password"
            className="w-full p-3 bg-neutral-900 rounded border border-neutral-700 focus:border-purple-500 outline-none"
            value={password}
            onChange={e => setPassword(e.target.value)}
          />
          <button className="w-full p-3 bg-purple-600 hover:bg-purple-700 rounded font-bold transition">
            {isRegister ? 'Create Account' : 'Sign In'}
          </button>
        </form>
        <button 
          onClick={() => setIsRegister(!isRegister)} 
          className="w-full mt-4 text-sm text-neutral-400 hover:text-white"
        >
          {isRegister ? 'Already have an account? Login' : 'Need an account? Register'}
        </button>
      </div>
    </div>
  );
}