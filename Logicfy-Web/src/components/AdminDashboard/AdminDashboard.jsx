import React, { useState, useEffect } from 'react';
import { adminService } from '../../services/adminService';
import './AdminDashboard.css';

const AdminDashboard = ({ user, onLogout }) => {
    const [dashboardData, setDashboardData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        loadDashboardData();
    }, []);

    const loadDashboardData = async () => {
        try {
            setLoading(true);
            setError('');
            const data = await adminService.getDashboard();
            setDashboardData(data);
        } catch (err) {
            setError(err.message || 'Veriler yüklenirken bir hata oluştu');
            console.error('Dashboard error:', err);
        } finally {
            setLoading(false);
        }
    };

    const formatNumber = (num) => {
        if (num === undefined || num === null) return '0';
        return new Intl.NumberFormat('tr-TR').format(num);
    };

    const formatPercentage = (num) => {
        if (num === undefined || num === null) return '0%';
        return `${num.toFixed(1)}%`;
    };

    const formatTime = (seconds) => {
        if (!seconds) return '0s';
        const mins = Math.floor(seconds / 60);
        const secs = Math.floor(seconds % 60);
        return mins > 0 ? `${mins}d ${secs}s` : `${secs}s`;
    };

    if (loading) {
        return (
            <div className="admin-dashboard">
                <div className="loading-container">
                    <div className="loading-spinner"></div>
                    <p>Dashboard yükleniyor...</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="admin-dashboard">
                <div className="error-container">
                    <h3>Hata!</h3>
                    <p>{error}</p>
                    <button className="retry-btn" onClick={loadDashboardData}>
                        Tekrar Dene
                    </button>
                </div>
            </div>
        );
    }

    if (!dashboardData) {
        return (
            <div className="admin-dashboard">
                <div className="error-container">
                    <p>Veriler yüklenemedi</p>
                    <button className="retry-btn" onClick={loadDashboardData}>
                        Tekrar Dene
                    </button>
                </div>
            </div>
        );
    }

    const { Genel, Takip, SoruAnaliz, DersAnaliz, ZorlukDagilim, DilIstatistikleri, Son7Gun } = dashboardData;

    return (
        <div className="admin-dashboard">
            {/* Header */}
            <header className="admin-header">
                <nav className="admin-nav">
                    <div className="admin-brand">
                        <div className="admin-logo">Logicfy Admin</div>
                    </div>
                    <div className="admin-user">
                        <span>Hoş geldiniz, {user?.adSoyad || 'Admin'}</span>
                        <button className="logout-btn" onClick={onLogout}>
                            Çıkış Yap
                        </button>
                    </div>
                </nav>
            </header>

            {/* Main Content */}
            <main className="admin-main">
                {/* Welcome Section */}
                <section className="welcome-section">
                    <h1>Dashboard Genel Bakış</h1>
                    <p>Logicfy platformunun genel istatistikleri ve performans metrikleri</p>
                </section>

                {/* Stats Grid */}
                <div className="stats-grid">
                    <div className="stat-card">
                        <div className="stat-number">{formatNumber(Genel?.ToplamDil)}</div>
                        <div className="stat-label">Programlama Dili</div>
                    </div>
                    <div className="stat-card">
                        <div className="stat-number">{formatNumber(Genel?.ToplamUnite)}</div>
                        <div className="stat-label">Toplam Ünite</div>
                    </div>
                    <div className="stat-card">
                        <div className="stat-number">{formatNumber(Genel?.ToplamDers)}</div>
                        <div className="stat-label">Toplam Ders</div>
                    </div>
                    <div className="stat-card">
                        <div className="stat-number">{formatNumber(Genel?.ToplamSoru)}</div>
                        <div className="stat-label">Toplam Soru</div>
                    </div>
                    <div className="stat-card">
                        <div className="stat-number">{formatNumber(Takip?.EnCokTakipEdilenDers?.TakipEdenKullaniciSayisi)}</div>
                        <div className="stat-label">En Popüler Ders Takipçi</div>
                    </div>
                </div>

                {/* Analytics Grid */}
                <div className="analytics-grid">
                    {/* Left Column - Charts and Main Analytics */}
                    <div className="chart-column">
                        {/* Performance Metrics */}
                        <div className="chart-section">
                            <h3 className="section-title">Performans Metrikleri</h3>
                            <div className="metrics-grid">
                                <div className="metric-card">
                                    <div className="metric-value">{formatPercentage(SoruAnaliz?.OrtalamaDogruOrani)}</div>
                                    <div className="metric-label">Ortalama Doğru Cevap</div>
                                </div>
                                <div className="metric-card">
                                    <div className="metric-value">{formatTime(SoruAnaliz?.OrtalamaCevaplanmaSuresi)}</div>
                                    <div className="metric-label">Ort. Cevaplama Süresi</div>
                                </div>
                                <div className="metric-card">
                                    <div className="metric-value">{formatNumber(SoruAnaliz?.ToplamCevaplanma)}</div>
                                    <div className="metric-label">Toplam Cevaplama</div>
                                </div>
                                <div className="metric-card">
                                    <div className="metric-value">{formatTime(DersAnaliz?.OrtalamaTamamlama)}</div>
                                    <div className="metric-label">Ort. Ders Tamamlama</div>
                                </div>
                            </div>
                        </div>

                        {/* Language Statistics */}
                        <div className="chart-section">
                            <h3 className="section-title">Programlama Dilleri</h3>
                            <div className="language-stats">
                                {DilIstatistikleri?.map((dil) => (
                                    <div key={dil.Id} className="language-item">
                                        <div className="language-name">{dil.Ad}</div>
                                        <div className="language-numbers">
                                            <div className="language-count">
                                                {dil.DersSayisi} ders, {dil.SoruSayisi} soru
                                            </div>
                                            <div className="language-label">
                                                {dil.UniteSayisi} ünite, {dil.KisimSayisi} kısım
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>

                    {/* Right Column - Sidebar Sections */}
                    <div className="sidebar-column">
                        {/* Most Followed Content */}
                        <div className="sidebar-section">
                            <h3 className="section-title">En Popüler İçerikler</h3>
                            <div className="list-container">
                                <div className="list-item">
                                    <div className="list-item-info">
                                        <div className="list-item-title">
                                            {Takip?.EnCokTakipEdilenDers?.Baslik || 'Veri yok'}
                                        </div>
                                        <div className="list-item-stats">
                                            {Takip?.EnCokTakipEdilenDers?.TakipEdenKullaniciSayisi || 0} takipçi
                                        </div>
                                    </div>
                                    <div className="list-item-badge">Ders</div>
                                </div>
                                <div className="list-item">
                                    <div className="list-item-info">
                                        <div className="list-item-title">
                                            {Takip?.EnCokTakipEdilenUnite?.Baslik || 'Veri yok'}
                                        </div>
                                        <div className="list-item-stats">
                                            {Takip?.EnCokTakipEdilenUnite?.TakipEdenKullaniciSayisi || 0} takipçi
                                        </div>
                                    </div>
                                    <div className="list-item-badge">Ünite</div>
                                </div>
                            </div>
                        </div>

                        {/* Recent Activity */}
                        <div className="sidebar-section">
                            <h3 className="section-title">Son 7 Gün</h3>
                            <div className="list-container">
                                <div className="list-item">
                                    <div className="list-item-info">
                                        <div className="list-item-title">Yeni Dersler</div>
                                        <div className="list-item-stats">{Son7Gun?.YeniDersler || 0} eklenen</div>
                                    </div>
                                </div>
                                <div className="list-item">
                                    <div className="list-item-info">
                                        <div className="list-item-title">Yeni Sorular</div>
                                        <div className="list-item-stats">{Son7Gun?.YeniSorular || 0} eklenen</div>
                                    </div>
                                </div>
                                <div className="list-item">
                                    <div className="list-item-info">
                                        <div className="list-item-title">Yeni Üniteler</div>
                                        <div className="list-item-stats">{Son7Gun?.YeniUniteler || 0} eklenen</div>
                                    </div>
                                </div>
                                <div className="list-item">
                                    <div className="list-item-info">
                                        <div className="list-item-title">Yeni Diller</div>
                                        <div className="list-item-stats">{Son7Gun?.YeniDiller || 0} eklenen</div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Difficulty Distribution */}
                        <div className="sidebar-section">
                            <h3 className="section-title">Zorluk Dağılımı</h3>
                            <div className="list-container">
                                {ZorlukDagilim?.map((item) => (
                                    <div key={item.Zorluk} className="list-item">
                                        <div className="list-item-info">
                                            <div className="list-item-title">
                                                Seviye {item.Zorluk}
                                            </div>
                                            <div className="list-item-stats">
                                                {item.DersSayisi} ders
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default AdminDashboard;