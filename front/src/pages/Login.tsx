import { useAuth } from "react-oidc-context";
import { Navigate } from "react-router-dom";

export default function Login() {
  const auth = useAuth();

  if (auth.isAuthenticated) {
    return <Navigate to="/" />;
  }

  if (auth.isLoading) return <div className="text-white p-10">Loading auth...</div>;
  if (auth.error) return <div className="text-red-500 p-10">Auth Error: {auth.error.message}</div>;

  return (
    <div className="min-h-screen flex items-center justify-center bg-black">
      <div className="w-full max-w-md p-8 bg-neutral-800 rounded-xl border border-neutral-700 text-center">
        <h1 className="text-2xl font-bold mb-6 text-purple-400">Welcome to RIFF</h1>
        <p className="text-neutral-400 mb-8">Please login via our secure Identity Server to continue.</p>
        
        <button 
          onClick={() => auth.signinRedirect()}
          className="w-full p-4 bg-purple-600 hover:bg-purple-700 rounded font-bold text-white transition"
        >
          Sign In via Riff Identity
        </button>
      </div>
    </div>
  );
}