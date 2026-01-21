import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { AuthProvider } from 'react-oidc-context';
import './index.css'
import App from './App.tsx'

const oidcConfig = {
  authority: "https://auth.local.oshideck.app",
  client_id: "riff_frontend",
  redirect_uri: "https://riff.local.oshideck.app/callback",
  scope: "openid profile riff_api",
  
  onSigninCallback: () => {
      window.history.replaceState({}, document.title, window.location.pathname);
      window.location.href = '/'; 
  }
};

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AuthProvider {...oidcConfig}>
      <App />
    </AuthProvider>
  </StrictMode>,
)