import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import DockMenu from '../DockMenu/DockMenu';
import '../Lessons/Lessons.css';
import axios from 'axios';

import { Line } from 'react-chartjs-2';
import {
    Chart as ChartJS,
    LineElement,
    CategoryScale,
    LinearScale,
    PointElement,
    Tooltip,
    Legend
} from 'chart.js';

ChartJS.register(LineElement, CategoryScale, LinearScale, PointElement, Tooltip, Legend);

const api = axios.create({
    baseURL: 'https://localhost:7140/api',
    withCredentials: true
});

const UserStats = ({ user, onLogout }) => {
    const navigate = useNavigate();
    const { userId: routeUserId } = useParams();

    const [userList, setUserList] = useState([]);
    const [selectedUserId, setSelectedUserId] = useState(routeUserId || '');

    const [lessonProgress, setLessonProgress] = useState([]);
    const [unitProgress, setUnitProgress] = useState([]);
    const [sectionProgress, setSectionProgress] = useState([]);
    const [answers, setAnswers] = useState([]);
    const [xpLogs, setXpLogs] = useState([]);
    const [xpStats, setXpStats] = useState({ toplamXp: 0, gunlukXp: 0 });

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const [showModal, setShowModal] = useState(false);
    const [modalTitle, setModalTitle] = useState('');
    const [modalContent, setModalContent] = useState(null);

    useEffect(() => {
        setSelectedUserId(routeUserId || '');
    }, [routeUserId]);

    useEffect(() => {
        loadUsers();
    }, []);

    useEffect(() => {
        loadAll();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [selectedUserId]);

    const loadUsers = async () => {
        try {
            // Burada kullanıcı listesi endpoint'inle eşleştir:
            // Örn: GET /api/Kullanici
            const res = await api.get('/UserProgress');
            setUserList(res.data || []);
        } catch (err) {
            console.error('Kullanıcı listesi alınamadı', err);
        }
    };

    const buildUrl = (basePath) => {
        if (selectedUserId) {
            // URL'ye userId query parametresi ekle
            const separator = basePath.includes('?') ? '&' : '?';
            return `${basePath}${separator}userId=${selectedUserId}`;
        }
        return basePath;
    };

    const loadAll = async () => {
        try {
            setLoading(true);
            setError('');

            const [
                dersIlerlemeRes,
                uniteIlerlemeRes,
                kisimIlerlemeRes,
                soruCevapRes,
                xpLogsRes,
                xpStatsRes
            ] = await Promise.all([
                api.get(buildUrl('/UserProgress/ders-ilerleme')),
                api.get(buildUrl('/UserProgress/unite-ilerleme')),
                api.get(buildUrl('/UserProgress/kisim-ilerleme')),
                api.get(buildUrl('/UserProgress/soru-cevap')),
                api.get(buildUrl('/UserProgress/xp-log')),
                api.get(buildUrl('/UserProgress/xp-istatistik'))
            ]);

            setLessonProgress(dersIlerlemeRes.data || []);
            setUnitProgress(uniteIlerlemeRes.data || []);
            setSectionProgress(kisimIlerlemeRes.data || []);
            setAnswers(soruCevapRes.data || []);
            setXpLogs(xpLogsRes.data || []);
            setXpStats(xpStatsRes.data || { toplamXp: 0, gunlukXp: 0 });
        } catch (err) {
            console.error(err);
            setError(err.message || 'İstatistikler alınırken bir hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleUserChange = (e) => {
        const newUserId = e.target.value;
        setSelectedUserId(newUserId);

        if (newUserId) {
            navigate(`/stats/${newUserId}`);
        } else {
            navigate('/stats');
        }
    };

    const openModal = (title, content) => {
        setModalTitle(title);
        setModalContent(content);
        setShowModal(true);
    };

    const closeModal = () => {
        setShowModal(false);
        setModalTitle('');
        setModalContent(null);
    };

    const handleMenuSelect = (menuId) => {
        const routes = {
            dashboard: '/dashboard',
            languages: '/languages',
            sections: '/sections',
            units: '/units',
            lessons: '/lessons',
            questions: '/questions',
            users: '/users',
            stats: '/stats'
        };
        if (routes[menuId]) {
            navigate(routes[menuId]);
        }
    };

    const getDateValue = (obj) => obj?.createdAt || obj?.CreatedAt;
    const getXpValue = (log) => log?.xp ?? log?.Xp ?? 0;
    const getXpSource = (log) => log?.kaynak || log?.Kaynak || '';

    const xpChartData = {
        labels: xpLogs.slice(-10).map(log =>
            getDateValue(log) ? new Date(getDateValue(log)).toLocaleDateString() : ''
        ),
        datasets: [
            {
                data: xpLogs.slice(-10).map(log => getXpValue(log)),
                borderColor: '#4e1260',
                backgroundColor: 'rgba(78,18,96,0.1)',
                tension: 0.3,
                borderWidth: 3,
                pointRadius: 4
            }
        ]
    };

    const xpChartOptions = {
        responsive: true,
        plugins: {
            legend: {
                display: false
            },
            tooltip: {
                enabled: true
            }
        },
        scales: {
            x: {
                ticks: { display: false },
                grid: { display: false }
            },
            y: {
                ticks: { display: false },
                grid: { display: false }
            }
        }
    };

    const activeUser = selectedUserId
        ? userList.find(u => u.id === selectedUserId || u.Id === selectedUserId)
        : null;

    if (loading && !lessonProgress.length && !xpLogs.length) {
        return (
            <div className="lessons-page">
                <header className="admin-header">
                    <nav className="admin-nav">
                        <div className="admin-brand">
                            <div className="admin-logo">Logicfy Admin</div>
                            <div className="active-section">
                                📊 Kullanıcı İstatistikleri
                            </div>
                        </div>
                        <div className="admin-user">
                            <span>Hoş geldiniz, {user?.adSoyad || 'Admin'}</span>
                            <button className="logout-btn" onClick={onLogout}>
                                Çıkış Yap
                            </button>
                        </div>
                    </nav>
                </header>
                <main className="page-main-content">
                    <div className="loading-container">
                        <div className="loading-spinner"></div>
                        <p>İstatistikler yükleniyor...</p>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="lessons-page">
            {/* HEADER */}
            <header className="admin-header">
                <nav className="admin-nav">
                    <div className="admin-brand">
                        <div className="admin-logo">Logicfy Admin</div>
                        <div className="active-section">
                            📊 Kullanıcı İstatistikleri
                        </div>
                    </div>
                    <div className="admin-user">
                        <span>Hoş geldiniz, {user?.adSoyad || 'Admin'}</span>
                        <button className="logout-btn" onClick={onLogout}>
                            Çıkış Yap
                        </button>
                    </div>
                </nav>
            </header>

            {/* MAIN */}
            <main className="page-main-content">
                <div className="page-content">
                    {/* PAGE HEADER */}
                    <div className="page-header">
                        <h1>📊 Kullanıcı İstatistikleri</h1>
                        <p>
                            {activeUser
                                ? `${activeUser.adSoyad || `${activeUser.ad} ${activeUser.soyad || ''}`} kullanıcısının öğrenme yolculuğu`
                                : 'Bir kullanıcı seçerek detaylı istatistikleri görüntüleyin.'}
                        </p>
                    </div>

                    {/* HATA MESAJI */}
                    {error && (
                        <div className="error-message">
                            {error}
                            <button className="close-error" onClick={() => setError('')}>×</button>
                        </div>
                    )}

                    {/* KULLANICI SEÇİCİ */}
                    <div className="filters-actions-bar">
                        <div className="filters">
                            <select
                                className="filter-select"
                                value={selectedUserId}
                                onChange={handleUserChange}
                            >
                                <option value="">Aktif kullanıcı (ben)</option>
                                {userList.map(u => {
                                    const id = u.id || u.Id;
                                    const ad = u.ad || u.Ad || '';
                                    const soyad = u.soyad || u.Soyad || '';
                                    const email = u.email || u.Email || '';
                                    return (
                                        <option key={id} value={id}>
                                            {ad} {soyad} ({email})
                                        </option>
                                    );
                                })}
                            </select>
                        </div>
                    </div>

                    {/* İSTATİSTİK KARTLARI */}
                    <div className="lessons-grid">
                        {/* XP GENEL PANEL */}
                        <div
                            className="lesson-card"
                            onClick={() =>
                                openModal(
                                    'XP Logları',
                                    <div>
                                        {xpLogs.length === 0 && (
                                            <div className="lesson-section">
                                                XP kaydı bulunamadı.
                                            </div>
                                        )}
                                        {xpLogs.map(log => (
                                            <div key={log.id || log.Id} className="lesson-section">
                                                {getXpSource(log)} → +{getXpValue(log)} XP
                                                <small style={{ float: 'right', opacity: 0.8 }}>
                                                    {getDateValue(log)
                                                        ? new Date(getDateValue(log)).toLocaleString()
                                                        : ''}
                                                </small>
                                            </div>
                                        ))}
                                    </div>
                                )
                            }
                        >
                            <div className="lesson-header">
                                <div className="lesson-icon">⭐</div>
                                <div className="lesson-meta">
                                    <span className="lesson-sira">
                                        {xpStats.toplamXp || 0} XP
                                    </span>
                                </div>
                            </div>
                            <h3>Toplam XP</h3>
                            <p className="lesson-section">
                                Bugün: {xpStats.gunlukXp || 0} XP
                            </p>
                            <div style={{ height: '150px', marginTop: '1rem' }}>
                                {xpLogs.length > 0 ? (
                                    <Line data={xpChartData} options={xpChartOptions} />
                                ) : (
                                    <small style={{ color: '#666' }}>
                                        XP grafiği için yeterli veri yok.
                                    </small>
                                )}
                            </div>
                        </div>

                        {/* DERS İLERLEMELERİ */}
                        <div
                            className="lesson-card"
                            onClick={() =>
                                openModal(
                                    'Ders İlerlemeleri',
                                    <div>
                                        {lessonProgress.length === 0 && (
                                            <div className="lesson-section">
                                                Herhangi bir ders ilerlemesi bulunamadı.
                                            </div>
                                        )}
                                        {lessonProgress.map(p => {
                                            const dersId = p.dersId || p.DersId;
                                            const tamamlanan = p.tamamlananSoruSayisi || p.TamamlananSoruSayisi || 0;
                                            const toplam = p.toplamSoruSayisi || p.ToplamSoruSayisi || 0;
                                            const oran = p.ilerlemeOrani || p.IlerlemeOrani || 0;
                                            return (
                                                <div key={p.id || p.Id} className="lesson-section">
                                                    Ders #{dersId}
                                                    <span style={{ float: 'right' }}>
                                                        {oran}% ({tamamlanan}/{toplam})
                                                    </span>
                                                </div>
                                            );
                                        })}
                                    </div>
                                )
                            }
                        >
                            <div className="lesson-header">
                                <div className="lesson-icon">📘</div>
                                <div className="lesson-meta">
                                    <span className="lesson-sira">
                                        {lessonProgress.length}
                                    </span>
                                </div>
                            </div>
                            <h3>Ders İlerlemeleri</h3>
                            <p className="lesson-section">
                                {lessonProgress.length} derste ilerleme kaydedildi
                            </p>
                        </div>

                        {/* KISIM İLERLEMELERİ */}
                        <div
                            className="lesson-card"
                            onClick={() =>
                                openModal(
                                    'Kısım İlerlemeleri',
                                    <div>
                                        {sectionProgress.length === 0 && (
                                            <div className="lesson-section">
                                                Herhangi bir kısım ilerlemesi bulunamadı.
                                            </div>
                                        )}
                                        {sectionProgress.map(p => {
                                            const kisimId = p.kisimId || p.KisimId;
                                            const tamamlanan = p.tamamlananDersSayisi || p.TamamlananDersSayisi || 0;
                                            const toplam = p.toplamDersSayisi || p.ToplamDersSayisi || 0;
                                            const oran = p.ilerlemeOrani || p.IlerlemeOrani || 0;
                                            return (
                                                <div key={p.id || p.Id} className="lesson-section">
                                                    Kısım #{kisimId}
                                                    <span style={{ float: 'right' }}>
                                                        {oran}% ({tamamlanan}/{toplam})
                                                    </span>
                                                </div>
                                            );
                                        })}
                                    </div>
                                )
                            }
                        >
                            <div className="lesson-header">
                                <div className="lesson-icon">📂</div>
                                <div className="lesson-meta">
                                    <span className="lesson-sira">
                                        {sectionProgress.length}
                                    </span>
                                </div>
                            </div>
                            <h3>Kısım İlerlemeleri</h3>
                            <p className="lesson-section">
                                {sectionProgress.length} kısımda ilerleme var
                            </p>
                        </div>

                        {/* ÜNİTE İLERLEMELERİ */}
                        <div
                            className="lesson-card"
                            onClick={() =>
                                openModal(
                                    'Ünite İlerlemeleri',
                                    <div>
                                        {unitProgress.length === 0 && (
                                            <div className="lesson-section">
                                                Herhangi bir ünite ilerlemesi bulunamadı.
                                            </div>
                                        )}
                                        {unitProgress.map(p => {
                                            const uniteId = p.uniteId || p.UniteId;
                                            const tamamlanan = p.tamamlananDersSayisi || p.TamamlananDersSayisi || 0;
                                            const toplam = p.toplamDersSayisi || p.ToplamDersSayisi || 0;
                                            const oran = p.ilerlemeOrani || p.IlerlemeOrani || 0;
                                            return (
                                                <div key={p.id || p.Id} className="lesson-section">
                                                    Ünite #{uniteId}
                                                    <span style={{ float: 'right' }}>
                                                        {oran}% ({tamamlanan}/{toplam})
                                                    </span>
                                                </div>
                                            );
                                        })}
                                    </div>
                                )
                            }
                        >
                            <div className="lesson-header">
                                <div className="lesson-icon">📦</div>
                                <div className="lesson-meta">
                                    <span className="lesson-sira">
                                        {unitProgress.length}
                                    </span>
                                </div>
                            </div>
                            <h3>Ünite İlerlemeleri</h3>
                            <p className="lesson-section">
                                {unitProgress.length} ünitede ilerleme kaydedildi
                            </p>
                        </div>

                        {/* SORU ÇÖZÜM GEÇMİŞİ */}
                        <div
                            className="lesson-card"
                            onClick={() =>
                                openModal(
                                    'Soru Çözüm Geçmişi',
                                    <div style={{ overflowX: 'auto' }}>
                                        {answers.length === 0 && (
                                            <div className="lesson-section">
                                                Henüz çözülen soru bulunamadı.
                                            </div>
                                        )}
                                        {answers.length > 0 && (
                                            <table
                                                style={{
                                                    width: '100%',
                                                    borderCollapse: 'collapse',
                                                    fontSize: '0.9rem'
                                                }}
                                            >
                                                <thead>
                                                    <tr>
                                                        <th style={{ textAlign: 'left', padding: '6px' }}>Soru ID</th>
                                                        <th style={{ textAlign: 'left', padding: '6px' }}>Durum</th>
                                                        <th style={{ textAlign: 'left', padding: '6px' }}>Süre (ms)</th>
                                                        <th style={{ textAlign: 'left', padding: '6px' }}>Tarih</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    {answers.map(a => {
                                                        const soruId = a.soruId || a.SoruId;
                                                        const dogruMu = a.dogruMu ?? a.DogruMu;
                                                        const sureMs = a.sureMs || a.SureMs || 0;
                                                        const created = getDateValue(a);
                                                        return (
                                                            <tr key={a.id || a.Id}>
                                                                <td style={{ padding: '6px' }}>{soruId}</td>
                                                                <td
                                                                    style={{
                                                                        padding: '6px',
                                                                        color: dogruMu ? 'green' : 'red',
                                                                        fontWeight: 600
                                                                    }}
                                                                >
                                                                    {dogruMu ? 'Doğru' : 'Yanlış'}
                                                                </td>
                                                                <td style={{ padding: '6px' }}>{sureMs}</td>
                                                                <td style={{ padding: '6px' }}>
                                                                    {created
                                                                        ? new Date(created).toLocaleString()
                                                                        : ''}
                                                                </td>
                                                            </tr>
                                                        );
                                                    })}
                                                </tbody>
                                            </table>
                                        )}
                                    </div>
                                )
                            }
                        >
                            <div className="lesson-header">
                                <div className="lesson-icon">❓</div>
                                <div className="lesson-meta">
                                    <span className="lesson-sira">
                                        {answers.length}
                                    </span>
                                </div>
                            </div>
                            <h3>Çözülen Sorular</h3>
                            <p className="lesson-section">
                                {answers.length} soru çözülmüş
                            </p>
                        </div>
                    </div>

                    {/* Güncelleme sırasında overlay */}
                    {loading && (
                        <div className="loading-overlay">
                            <div className="loading-spinner"></div>
                            <p>Güncelleniyor...</p>
                        </div>
                    )}
                </div>
            </main>

            {/* DETAY MODAL */}
            {showModal && (
                <div className="modal-overlay" onClick={closeModal}>
                    <div className="modal" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <h2>{modalTitle}</h2>
                            <button className="close-btn" onClick={closeModal}>×</button>
                        </div>
                        <div className="modal-form">
                            {modalContent}
                        </div>
                    </div>
                </div>
            )}

            {/* Dock Menu */}
            <DockMenu onMenuSelect={handleMenuSelect} activeMenu="stats" />
        </div>
    );
};

export default UserStats;
