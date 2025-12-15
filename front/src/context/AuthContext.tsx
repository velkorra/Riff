import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { AuthState } from '../types';
import { api } from '../lib/api';

interface AuthContextType extends AuthState {
  login: (token: string) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>({ token: null, user: null });
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      const token = localStorage.getItem('riff_token');
      if (token) {
        try {
          const user = await api.get('/api/users/me');
          setState({ token, user });
        } catch {
          localStorage.removeItem('riff_token');
        }
      }
      setIsLoading(false);
    };
    initAuth();
  }, []);

  const login = async (token: string) => {
    localStorage.setItem('riff_token', token);
    const user = await api.get('/api/users/me');
    setState({ token, user });
  };

  const logout = () => {
    localStorage.removeItem('riff_token');
    setState({ token: null, user: null });
  };

  return (
    <AuthContext.Provider value={{ ...state, login, logout, isLoading }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
};