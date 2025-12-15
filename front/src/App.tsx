import { Routes, Route, BrowserRouter } from 'react-router-dom';
import RoomList from './pages/RoomList';
import Login from './pages/Login';
import RoomView from './pages/RoomView';
import { AuthProvider } from './context/AuthContext';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<RoomList />} />
          <Route path="/login" element={<Login />} />
          <Route path="/room/:id" element={<RoomView />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;