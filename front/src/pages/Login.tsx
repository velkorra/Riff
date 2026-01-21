import { useAuth } from "react-oidc-context";
import { Navigate } from "react-router-dom";

export default function Login() {
  const auth = useAuth();

  if (auth.isAuthenticated) {
    return <Navigate to="/" />;
  }

  if (auth.isLoading) return <div className="p-10 text-gray-600">Загрузка...</div>;
  if (auth.error) return <div className="text-red-600 p-10 bg-red-50">Ошибка авторизации: {auth.error.message}</div>;

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="w-full max-w-md p-10 bg-white rounded-2xl border border-gray-200 shadow-xl text-center">
        <div className="mb-8">
            <h1 className="text-4xl font-black text-gray-900 mb-2">RIFF</h1>
            <p className="text-gray-500">Музыкальная платформа для совместного прослушивания</p>
        </div>
        
        <button 
          onClick={() => auth.signinRedirect()}
          className="w-full py-4 px-6 bg-purple-600 hover:bg-purple-700 rounded-xl font-bold text-white transition transform active:scale-95 shadow-lg shadow-purple-200"
        >
          Войти через Riff ID
        </button>
        
        <p className="mt-6 text-xs text-gray-400">
            Используя сервис, вы соглашаетесь с правилами сообщества.
        </p>
      </div>
    </div>
  );
}