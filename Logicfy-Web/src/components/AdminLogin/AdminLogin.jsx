import React, { useState } from 'react';
import { authService } from '../../services/authService';
import './AdminLogin.css';

const AdminLogin = ({ onLoginSuccess }) => {
    const [formData, setFormData] = useState({
        email: '',
        password: ''
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
        // Hata mesajını temizle
        if (error) setError('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        setSuccess('');

        // Basit validasyon
        if (!formData.email || !formData.password) {
            setError('Lütfen email ve şifre giriniz');
            setLoading(false);
            return;
        }

        try {
            const result = await authService.login(formData.email, formData.password);

            if (result.success) {
                setSuccess('Giriş başarılı! Yönlendiriliyorsunuz...');

                // Kullanıcı bilgilerini al
                setTimeout(async () => {
                    try {
                        const userData = await authService.getMe();
                        onLoginSuccess(userData);
                    } catch (userError) {
                        console.error('Kullanıcı bilgileri alınamadı:', userError);
                        onLoginSuccess(result);
                    }
                }, 1500);
            } else {
                setError(result.message || 'Giriş başarısız');
            }
        } catch (err) {
            setError(err.message || 'Bir hata oluştu. Lütfen tekrar deneyin.');
            console.error('Login error:', err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="admin-login-container">
            <div className="admin-login-card">
                <div className="admin-login-header">
                    <div className="admin-login-logo">Logicfy</div>
                    <p className="admin-login-subtitle">Admin Paneli Girişi</p>
                </div>

                <form className="admin-login-form" onSubmit={handleSubmit}>
                    {error && (
                        <div className="error-message">
                            {error}
                        </div>
                    )}

                    {success && (
                        <div className="success-message">
                            {success}
                        </div>
                    )}

                    <div className="form-group">
                        <label htmlFor="email">E-posta</label>
                        <input
                            type="email"
                            id="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            placeholder="admin@logicfy.com"
                            disabled={loading}
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">Şifre</label>
                        <input
                            type="password"
                            id="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            placeholder="••••••••"
                            disabled={loading}
                            required
                        />
                    </div>

                    <button
                        type="submit"
                        className={`login-button ${loading ? 'loading' : ''}`}
                        disabled={loading}
                    >
                        {loading ? '' : 'Giriş Yap'}
                    </button>
                </form>

                <div className="admin-login-footer">
                    <p>Sadece yetkili personel için</p>
                </div>
            </div>
        </div>
    );
};

export default AdminLogin;