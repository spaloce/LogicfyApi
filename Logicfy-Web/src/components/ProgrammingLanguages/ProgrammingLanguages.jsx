import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { programmingLanguageService } from '../../services/programmingLanguageService';
import { fileUploadService } from '../../services/fileUploadService';
import DockMenu from '../DockMenu/DockMenu';
import './ProgrammingLanguages.css';

const ProgrammingLanguages = ({ user, onLogout }) => {
    const [languages, setLanguages] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [editingLanguage, setEditingLanguage] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [uploading, setUploading] = useState(false);
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        ad: '',
        kod: '',
        ikonUrl: '',
        aktifMi: true
    });

    useEffect(() => {
        loadLanguages();
    }, []);

    const loadLanguages = async () => {
        try {
            setLoading(true);
            setError('');
            const data = await programmingLanguageService.getAll();
            setLanguages(data);
        } catch (err) {
            setError(err.message || 'Diller yüklenirken bir hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = async () => {
        if (!searchTerm.trim()) {
            loadLanguages();
            return;
        }

        try {
            setLoading(true);
            const data = await programmingLanguageService.search(searchTerm);
            setLanguages(data);
        } catch (err) {
            setError(err.message || 'Arama sırasında hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleFileUpload = async (file) => {
        try {
            setUploading(true);
            const result = await fileUploadService.uploadImage(file);
            setFormData(prev => ({ ...prev, ikonUrl: result.fileUrl }));
            return result.fileUrl;
        } catch (err) {
            setError(err.message || 'Resim yüklenirken hata oluştu');
            return null;
        } finally {
            setUploading(false);
        }
    };

    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            handleFileUpload(file);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            if (editingLanguage) {
                await programmingLanguageService.update(editingLanguage.id, formData);
            } else {
                await programmingLanguageService.create(formData);
            }

            setShowModal(false);
            resetForm();
            loadLanguages();
        } catch (err) {
            setError(err.message || 'İşlem sırasında hata oluştu');
        }
    };

    const handleEdit = (language) => {
        setEditingLanguage(language);
        setFormData({
            ad: language.ad,
            kod: language.kod,
            ikonUrl: language.ikonUrl || '',
            aktifMi: language.aktifMi
        });
        setShowModal(true);
    };

    const handleDelete = async (id, ad) => {
        if (!window.confirm(`"${ad}" dilini silmek istediğinizden emin misiniz?`)) {
            return;
        }

        try {
            await programmingLanguageService.delete(id);
            loadLanguages();
        } catch (err) {
            setError(err.message || 'Silme işlemi sırasında hata oluştu');
        }
    };

    const handleStatusChange = async (id, aktif) => {
        try {
            await programmingLanguageService.updateStatus(id, !aktif);
            loadLanguages();
        } catch (err) {
            setError(err.message || 'Durum güncellenirken hata oluştu');
        }
    };

    const resetForm = () => {
        setFormData({
            ad: '',
            kod: '',
            ikonUrl: '',
            aktifMi: true
        });
        setEditingLanguage(null);
        setUploading(false);
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
            users: '/users'
        };

        if (routes[menuId]) {
            navigate(routes[menuId]);
        }
    };

    const filteredLanguages = languages.filter(language =>
        language.ad.toLowerCase().includes(searchTerm.toLowerCase()) ||
        language.kod.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (loading && languages.length === 0) {
        return (
            <div className="programming-languages-page">
                <header className="admin-header">
                    <nav className="admin-nav">
                        <div className="admin-brand">
                            <div className="admin-logo">Logicfy Admin</div>
                            <div className="active-section">
                                💻 Programlama Dilleri
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
                        <p>Diller yükleniyor...</p>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="programming-languages-page">
            {/* Header */}
            <header className="admin-header">
                <nav className="admin-nav">
                    <div className="admin-brand">
                        <div className="admin-logo">Logicfy Admin</div>
                        <div className="active-section">
                            💻 Programlama Dilleri
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
                        <h1>💻 Programlama Dilleri</h1>
                        <p>Platformdaki programlama dillerini yönetin</p>
                    </div>

                    {/* Error Display */}
                    {error && (
                        <div className="error-message">
                            {error}
                            <button onClick={() => setError('')} className="close-error">×</button>
                        </div>
                    )}

                    {/* Actions Bar */}
                    <div className="actions-bar">
                        <div className="search-box">
                            <input
                                type="text"
                                placeholder="Dil ara..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                            />
                            <button onClick={handleSearch}>🔍</button>
                        </div>
                        <button className="btn-primary" onClick={openCreateModal}>
                            + Yeni Dil Ekle
                        </button>
                    </div>

                    {/* Languages Grid */}
                    {!loading && (
                        <div className="languages-grid">
                            {filteredLanguages.map((language) => (
                                <div key={language.id} className="language-card">
                                    <div className="language-header">
                                        <div className="language-icon">
                                            {language.ikonUrl ? (
                                                <img
                                                    src={`https://localhost:7140${language.ikonUrl}`}
                                                    alt={language.ad}
                                                    onError={(e) => {
                                                        e.target.style.display = 'none';
                                                        e.target.nextSibling.style.display = 'flex';
                                                    }}
                                                />
                                            ) : (
                                                <span>💻</span>
                                            )}
                                            {language.ikonUrl && <span style={{ display: 'none' }}>💻</span>}
                                        </div>
                                        <div className="language-status">
                                            <span className={`status-badge ${language.aktifMi ? 'active' : 'inactive'}`}>
                                                {language.aktifMi ? 'Aktif' : 'Pasif'}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="language-info">
                                        <h3>{language.ad}</h3>
                                        <p className="language-code">{language.kod}</p>
                                        <div className="language-stats">
                                            <span>{language.uniteSayisi || 0} Ünite</span>
                                            <span>{language.aktifUniteSayisi || 0} Aktif Ünite</span>
                                        </div>
                                    </div>

                                    <div className="language-actions">
                                        <button
                                            className={`btn-status ${language.aktifMi ? 'btn-inactive' : 'btn-active'}`}
                                            onClick={() => handleStatusChange(language.id, language.aktifMi, language.ad)}
                                        >
                                            {language.aktifMi ? '❌ Pasif Yap' : '✅ Aktif Yap'}
                                        </button>
                                        <button
                                            className="btn-edit"
                                            onClick={() => handleEdit(language)}
                                        >
                                            ✏️ Düzenle
                                        </button>
                                        <button
                                            className="btn-delete"
                                            onClick={() => handleDelete(language.id, language.ad)}
                                        >
                                            🗑️ Sil
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

                    {/* Empty State */}
                    {filteredLanguages.length === 0 && !loading && (
                        <div className="empty-state">
                            <div className="empty-icon">💻</div>
                            <h3>Henüz programlama dili eklenmemiş</h3>
                            <p>İlk programlama dilini eklemek için "Yeni Dil Ekle" butonuna tıklayın.</p>
                            <button className="btn-primary" onClick={openCreateModal}>
                                + İlk Dili Ekle
                            </button>
                        </div>
                    )}

                    {/* Loading State for Grid */}
                    {loading && languages.length > 0 && (
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
                            <h2>{editingLanguage ? 'Dili Düzenle' : 'Yeni Dil Ekle'}</h2>
                            <button className="close-btn" onClick={closeModal}>×</button>
                        </div>

                        <form onSubmit={handleSubmit} className="modal-form">
                            <div className="form-group">
                                <label>Dil Adı *</label>
                                <input
                                    type="text"
                                    value={formData.ad}
                                    onChange={(e) => setFormData({ ...formData, ad: e.target.value })}
                                    required
                                    placeholder="Örn: JavaScript"
                                    disabled={uploading}
                                />
                            </div>

                            <div className="form-group">
                                <label>Dil Kodu *</label>
                                <input
                                    type="text"
                                    value={formData.kod}
                                    onChange={(e) => setFormData({ ...formData, kod: e.target.value })}
                                    required
                                    placeholder="Örn: javascript"
                                    disabled={uploading || editingLanguage}
                                />
                                {editingLanguage && (
                                    <small className="form-hint">Dil kodu değiştirilemez</small>
                                )}
                            </div>

                            <div className="form-group">
                                <label>İkon</label>
                                <div className="file-upload-section">
                                    {formData.ikonUrl ? (
                                        <div className="image-preview">
                                            <img
                                                src={`https://localhost:7140${formData.ikonUrl}`}
                                                alt="Preview"
                                                onError={(e) => {
                                                    e.target.style.display = 'none';
                                                    e.target.nextSibling.style.display = 'block';
                                                }}
                                            />
                                            <span style={{ display: 'none' }}>📁 İkon yüklenemedi</span>
                                            <button
                                                type="button"
                                                onClick={() => setFormData({ ...formData, ikonUrl: '' })}
                                                className="remove-image"
                                                disabled={uploading}
                                            >
                                                ×
                                            </button>
                                        </div>
                                    ) : (
                                        <div className="file-upload-area">
                                            <input
                                                type="file"
                                                accept=".jpg,.jpeg,.png,.gif,.svg,.webp"
                                                onChange={handleFileChange}
                                                disabled={uploading}
                                            />
                                            <div className="upload-placeholder">
                                                {uploading ? (
                                                    <>
                                                        <div className="upload-spinner"></div>
                                                        <p>Yükleniyor...</p>
                                                    </>
                                                ) : (
                                                    <>
                                                        <span>📁</span>
                                                        <p>İkon yüklemek için tıklayın</p>
                                                        <small>JPG, PNG, GIF, SVG, WebP (max 5MB)</small>
                                                    </>
                                                )}
                                            </div>
                                        </div>
                                    )}
                                </div>
                            </div>

                            <div className="form-group checkbox-group">
                                <label>
                                    <input
                                        type="checkbox"
                                        checked={formData.aktifMi}
                                        onChange={(e) => setFormData({ ...formData, aktifMi: e.target.checked })}
                                        disabled={uploading}
                                    />
                                    Aktif
                                </label>
                            </div>

                            <div className="modal-actions">
                                <button
                                    type="button"
                                    className="btn-secondary"
                                    onClick={closeModal}
                                    disabled={uploading}
                                >
                                    İptal
                                </button>
                                <button
                                    type="submit"
                                    className="btn-primary"
                                    disabled={uploading || !formData.ad || !formData.kod}
                                >
                                    {uploading ? 'Yükleniyor...' : (editingLanguage ? 'Güncelle' : 'Oluştur')}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Dock Menu */}
            <DockMenu onMenuSelect={handleMenuSelect} activeMenu="languages" />
        </div>
    );
};

export default ProgrammingLanguages;