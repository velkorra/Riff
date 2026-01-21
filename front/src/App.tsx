import { Routes, Route, BrowserRouter, Navigate } from 'react-router-dom';
import { hasAuthParams, useAuth } from 'react-oidc-context';
import RoomList from './pages/RoomList';
import Login from './pages/Login';
import RoomView from './pages/RoomView';
import { Toaster } from 'react-hot-toast';
import { JSX } from 'react';

function ProtectedRoute({ children }: { children: JSX.Element }) {
    const auth = useAuth();
    if (auth.isLoading) return <div className="flex h-screen items-center justify-center text-gray-500">Загрузка...</div>;
    if (auth.isAuthenticated) return children;
    return <Navigate to="/login" />;
}

function App() {
    const auth = useAuth();

    if (auth.isLoading || (hasAuthParams() && !auth.isAuthenticated && !auth.activeNavigator)) {
        return <div className="bg-white h-screen text-black flex items-center justify-center">Авторизация...</div>;
    }

    return (
        <BrowserRouter>
            <Toaster position="top-right" toastOptions={{ duration: 4000 }} />

            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/" element={
                    <ProtectedRoute>
                        <RoomList />
                    </ProtectedRoute>
                } />

                <Route path="/room/:id" element={
                    <ProtectedRoute>
                        <RoomView />
                    </ProtectedRoute>
                } />
            </Routes>
        </BrowserRouter>
    );
}

export default App;