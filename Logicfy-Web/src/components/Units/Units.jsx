import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { unitService } from '../../services/unitService';
import { programmingLanguageService } from '../../services/programmingLanguageService';
import DockMenu from '../DockMenu/DockMenu';
import './Units.css';

const Units = ({ user, onLogout }) => {
    const [units, setUnits] = useState([]);
    const [languages, setLanguages] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [editingUnit, setEditingUnit] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedLanguage, setSelectedLanguage] = useState('');
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        baslik: '',
        sira: 1,
        aciklama: '',
        programlamaDiliId: ''
    });

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {
        try {
            setLoading(true);
            setError('');
            const [unitsData, languagesData] = await Promise.all([
                unitService.getAll(),
                programmingLanguageService.getAll()
            ]);
            setUnits(unitsData);
            setLanguages(languagesData);
        } catch (err) {
            setError(err.message || 'Veriler yüklenirken bir hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = async () => {
        if (!searchTerm.trim()) {
            loadData();
            return;
        }

        try {
            setLoading(true);
            const data = await unitService.search(searchTerm);
            setUnits(data);
        } catch (err) {
            setError(err.message || 'Arama sırasında hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleLanguageFilter = async (dilId) => {
        setSelectedLanguage(dilId);

        if (!dilId) {
            loadData();
            return;
        }

        try {
            setLoading(true);
            const data = await unitService.getByLanguage(dilId);
            setUnits(data);
        } catch (err) {
            setError(err.message || 'Filtreleme sırasında hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            if (editingUnit) {
                await unitService.update(editingUnit.id, formData);
            } else {
                await unitService.create(formData);
            }

            setShowModal(false);
            resetForm();
            loadData();
        } catch (err) {
            setError(err.message || 'İşlem sırasında hata oluştu');
        }
    };

    const handleEdit = (unit) => {
        setEditingUnit(unit);
        setFormData({
            baslik: unit.baslik,
            sira: unit.sira,
            aciklama: unit.aciklama || '',
            programlamaDiliId: unit.programlamaDiliId.toString()
        });
        setShowModal(true);
    };

    const handleDelete = async (id, baslik) => {
        if (!window.confirm(`"${baslik}" ünitesini silmek istediğinizden emin misiniz?`)) {
            return;
        }

        try {
            await unitService.delete(id);
            loadData();
        } catch (err) {
            setError(err.message || 'Silme işlemi sırasında hata oluştu');
        }
    };

    const resetForm = () => {
        setFormData({
            baslik: '',
            sira: 1,
            aciklama: '',
            programlamaDiliId: ''
        });
        setEditingUnit(null);
    };

    const openCreateModal = () => {
        resetForm();
        setShowModal(true);
    };

    const closeModal = () => {
        setShowModal(false);
        resetForm();
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
            sectioncontents: '/sectioncontents'
        };

        if (routes[menuId]) {
            navigate(routes[menuId]);
        }
    };

    const getLanguageName = (dilId) => {
        const language = languages.find(l => l.id === dilId);
        return language ? language.ad : 'Bilinmeyen';
    };

    const filteredUnits = units.filter(unit =>
        unit.baslik.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (unit.aciklama && unit.aciklama.toLowerCase().includes(searchTerm.toLowerCase()))
    );

    if (loading && units.length === 0) {
        return (
            <div className="units-page">
                <header className="admin-header">
                    <nav className="admin-nav">
                        <div className="admin-brand">
                            <div className="admin-logo">Logicfy Admin</div>
                            <div className="active-section">
                                📂 Üniteler
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
                        <p>Üniteler yükleniyor...</p>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="units-page">
            {/* Header */}
            <header className="admin-header">
                <nav className="admin-nav">
                    <div className="admin-brand">
                        <div className="admin-logo">Logicfy Admin</div>
                        <div className="active-section">
                            📂 Üniteler
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

            {/* Main Content */}
            <main className="page-main-content">
                <div className="page-content">
                    {/* Page Header */}
                    <div className="page-header">
                        <h1>📂 Üniteler</h1>
                        <p>Platformdaki tüm üniteleri yönetin</p>
                    </div>

                    {/* Error Display */}
                    {error && (
                        <div className="error-message">
                            {error}
                            <button onClick={() => setError('')} className="close-error">×</button>
                        </div>
                    )}

                    {/* Filters and Actions Bar */}
                    <div className="filters-actions-bar">
                        <div className="filters">
                            <div className="search-box">
                                <input
                                    type="text"
                                    placeholder="Ünite ara..."
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                    onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                                />
                                <button onClick={handleSearch}>🔍</button>
                            </div>

                            <select
                                value={selectedLanguage}
                                onChange={(e) => handleLanguageFilter(e.target.value)}
                                className="language-filter"
                            >
                                <option value="">Tüm Diller</option>
                                {languages.map(language => (
                                    <option key={language.id} value={language.id}>
                                        {language.ad}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <button className="btn-primary" onClick={openCreateModal}>
                            + Yeni Ünite Ekle
                        </button>
                    </div>

                    {/* Units Grid */}
                    {!loading && (
                        <div className="units-grid">
                            {filteredUnits.map((unit) => (
                                <div key={unit.id} className="unit-card">
                                    <div className="unit-header">
                                        <div className="unit-icon">
                                            <span>📂</span>
                                        </div>
                                        <div className="unit-meta">
                                            <span className="unit-sira">#{unit.sira}</span>
                                            <span className="unit-language">
                                                {getLanguageName(unit.programlamaDiliId)}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="unit-info">
                                        <h3>{unit.baslik}</h3>
                                        {unit.aciklama && (
                                            <p className="unit-description">{unit.aciklama}</p>
                                        )}
                                        <div className="unit-stats">
                                            <span>{unit.kisimSayisi || 0} Kısım</span>
                                            <span className="unit-date">
                                                {new Date(unit.createdAt).toLocaleDateString('tr-TR')}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="unit-actions">
                                        <button
                                            className="btn-edit"
                                            onClick={() => handleEdit(unit)}
                                        >
                                            ✏️ Düzenle
                                        </button>
                                        <button
                                            className="btn-view"
                                            onClick={() => navigate(`/units/${unit.id}`)}
                                        >
                                            👁️ Detay
                                        </button>
                                        <button
                                            className="btn-delete"
                                            onClick={() => handleDelete(unit.id, unit.baslik)}
                                        >
                                            🗑️ Sil
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

                    {/* Empty State */}
                    {filteredUnits.length === 0 && !loading && (
                        <div className="empty-state">
                            <div className="empty-icon">📂</div>
                            <h3>Henüz ünite eklenmemiş</h3>
                            <p>İlk üniteyi eklemek için "Yeni Ünite Ekle" butonuna tıklayın.</p>
                            <button className="btn-primary" onClick={openCreateModal}>
                                + İlk Üniteyi Ekle
                            </button>
                        </div>
                    )}

                    {/* Loading State for Grid */}
                    {loading && units.length > 0 && (
                        <div className="loading-overlay">
                            <div className="loading-spinner"></div>
                            <p>Güncelleniyor...</p>
                        </div>
                    )}
                </div>
            </main>

            {/* Create/Edit Modal */}
            {showModal && (
                <div className="modal-overlay" onClick={closeModal}>
                    <div className="modal" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <h2>{editingUnit ? 'Üniteyi Düzenle' : 'Yeni Ünite Ekle'}</h2>
                            <button className="close-btn" onClick={closeModal}>×</button>
                        </div>

                        <form onSubmit={handleSubmit} className="modal-form">
                            <div className="form-group">
                                <label>Ünite Başlığı *</label>
                                <input
                                    type="text"
                                    value={formData.baslik}
                                    onChange={(e) => setFormData({ ...formData, baslik: e.target.value })}
                                    required
                                    placeholder="Örn: Temel Kavramlar"
                                />
                            </div>

                            <div className="form-row">
                                <div className="form-group">
                                    <label>Sıra No *</label>
                                    <input
                                        type="number"
                                        value={formData.sira}
                                        onChange={(e) => setFormData({ ...formData, sira: parseInt(e.target.value) || 1 })}
                                        required
                                        min="1"
                                        placeholder="1"
                                    />
                                </div>

                                <div className="form-group">
                                    <label>Programlama Dili *</label>
                                    <select
                                        value={formData.programlamaDiliId}
                                        onChange={(e) => setFormData({ ...formData, programlamaDiliId: e.target.value })}
                                        required
                                    >
                                        <option value="">Dil Seçin</option>
                                        {languages.map(language => (
                                            <option key={language.id} value={language.id}>
                                                {language.ad}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            </div>

                            <div className="form-group">
                                <label>Açıklama</label>
                                <textarea
                                    value={formData.aciklama}
                                    onChange={(e) => setFormData({ ...formData, aciklama: e.target.value })}
                                    placeholder="Ünite hakkında kısa bir açıklama..."
                                    rows="3"
                                />
                            </div>

                            <div className="modal-actions">
                                <button type="button" className="btn-secondary" onClick={closeModal}>
                                    İptal
                                </button>
                                <button type="submit" className="btn-primary">
                                    {editingUnit ? 'Güncelle' : 'Oluştur'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Dock Menu */}
            <DockMenu onMenuSelect={handleMenuSelect} activeMenu="units" />
        </div>
    );
};

export default Units;