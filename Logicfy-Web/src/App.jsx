import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import AdminLogin from './components/AdminLogin/AdminLogin';
import AdminDashboard from './components/AdminDashboard/AdminDashboard';
import ProgrammingLanguages from './components/ProgrammingLanguages/ProgrammingLanguages';
import Sections from './components/Sections/Sections';
import Units from './components/Units/Units';
import Lessons from './components/Lessons/Lessons';
import Questions from './components/Questions/Questions';
import UserStats from './components/UserStats/UserStats';
import { authService } from './services/authService';
import './App.css';
import SectionContents from './components/SectionContents/SectionContents';

function App() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [authChecked, setAuthChecked] = useState(false);

  useEffect(() => {
    checkAuth();
  }, []);

  const checkAuth = async () => {
    try {
      const userData = await authService.getMe();
      setUser(userData);
    } catch (error) {
      console.log('Kullanıcı oturumu bulunamadı:', error.message);
      setUser(null);
    } finally {
      setLoading(false);
      setAuthChecked(true);
    }
  };

  const handleLoginSuccess = (userData) => {
    setUser(userData);
  };

  const handleLogout = async () => {
    try {
      await authService.logout();
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      setUser(null);
    }
  };

  // Loading state
  if (loading) {
    return (
      <div className="loading-container">
        <div className="loading-spinner"></div>
        <p>Yükleniyor...</p>
      </div>
    );
  }

  return (
    <Router>
      <div className="App">
        {!authChecked ? (
          <div className="loading-container">
            <div className="loading-spinner"></div>
            <p>Kimlik doğrulama kontrol ediliyor...</p>
          </div>
        ) : user ? (
          <Routes>
            <Route path="/" element={<AdminDashboard user={user} onLogout={handleLogout} />} />
            <Route path="/dashboard" element={<AdminDashboard user={user} onLogout={handleLogout} />} />
            <Route path="/languages" element={<ProgrammingLanguages user={user} onLogout={handleLogout} />} />
            <Route path="/sections" element={<Sections user={user} onLogout={handleLogout} />} />
            <Route path="/units" element={<Units user={user} onLogout={handleLogout} />} />
            <Route path="/lessons" element={<Lessons user={user} onLogout={handleLogout} />} />
            <Route path="/sectioncontents" element={<SectionContents user={user} onLogout={handleLogout} />} />
            <Route path="/questions" element={<Questions user={user} onLogout={handleLogout} />} />
            <Route path="/users" element={<UserStats user={user} onLogout={handleLogout} />} />
            <Route path="/stats/:userId" element={<UserStats user={user} onLogout={handleLogout} />} />
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
          </Routes>
        ) : (
          <AdminLogin onLoginSuccess={handleLoginSuccess} />
        )}
      </div>
    </Router>
  );
}

export default App;