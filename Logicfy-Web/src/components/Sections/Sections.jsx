import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { sectionService } from '../../services/sectionService';
import { unitService } from '../../services/unitService';
import { programmingLanguageService } from '../../services/programmingLanguageService';
import DockMenu from '../DockMenu/DockMenu';
import './Sections.css';

const Sections = ({ user, onLogout }) => {
    const [sections, setSections] = useState([]);
    const [units, setUnits] = useState([]);
    const [languages, setLanguages] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [editingSection, setEditingSection] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedUnit, setSelectedUnit] = useState('');
    const [selectedLanguage, setSelectedLanguage] = useState('');
    const [filteredUnits, setFilteredUnits] = useState([]);
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        baslik: '',
        uniteId: '',
        sira: 1
    });

    useEffect(() => {
        loadData();
    }, []);

    useEffect(() => {
        if (selectedLanguage && units.length > 0) {
            const filtered = units.filter(unit =>
                unit.programlamaDiliId.toString() === selectedLanguage
            );
            setFilteredUnits(filtered);
            if (filtered.length > 0 && !filtered.find(u => u.id.toString() === formData.uniteId)) {
                setFormData(prev => ({ ...prev, uniteId: filtered[0].id.toString() }));
            }
        } else {
            setFilteredUnits(units);
        }
    }, [selectedLanguage, units, formData.uniteId]);

    const loadData = async () => {
        try {
            setLoading(true);
            setError('');
            const [sectionsData, unitsData, languagesData] = await Promise.all([
                sectionService.getAll(),
                unitService.getAll(),
                programmingLanguageService.getAll()
            ]);
            setSections(sectionsData);
            setUnits(unitsData);
            setLanguages(languagesData);
            setFilteredUnits(unitsData);
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
            const data = await sectionService.search(searchTerm);
            setSections(data);
        } catch (err) {
            setError(err.message || 'Arama sırasında hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleUnitFilter = async (unitId) => {
        setSelectedUnit(unitId);

        if (!unitId) {
            loadData();
            return;
        }

        try {
            setLoading(true);
            const data = await sectionService.getByUnit(unitId);
            setSections(data.kisimlar || []);
        } catch (err) {
            setError(err.message || 'Filtreleme sırasında hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleLanguageFilter = (dilId) => {
        setSelectedLanguage(dilId);
        setSelectedUnit('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            if (editingSection) {
                await sectionService.update(editingSection.id, formData);
            } else {
                await sectionService.create(formData);
            }

            setShowModal(false);
            resetForm();
            loadData();
        } catch (err) {
            setError(err.message || 'İşlem sırasında hata oluştu');
        }
    };

    const handleEdit = (section) => {
        setEditingSection(section);
        setFormData({
            baslik: section.baslik,
            uniteId: section.uniteId.toString(),
            sira: section.sira
        });

        // Üniteye göre dil seçimini otomatik yap
        const unit = units.find(u => u.id === section.uniteId);
        if (unit) {
            setSelectedLanguage(unit.programlamaDiliId.toString());
        }

        setShowModal(true);
    };

    const handleDelete = async (id, baslik) => {
        if (!window.confirm(`"${baslik}" kısmını silmek istediğinizden emin misiniz?`)) {
            return;
        }

        try {
            await sectionService.delete(id);
            loadData();
        } catch (err) {
            setError(err.message || 'Silme işlemi sırasında hata oluştu');
        }
    };

    const handleViewDetails = async (id) => {
        try {
            const detail = await sectionService.getDetail(id);
            console.log('Kısım Detayları:', detail);
            // Burada detayları modalda gösterebilir veya yeni sayfaya yönlendirebilirsiniz
            alert(`${detail.kisim.baslik} - ${detail.ozet.toplamDers} ders, ${detail.ozet.toplamSoru} soru`);
        } catch (err) {
            setError(err.message || 'Detaylar yüklenirken hata oluştu');
        }
    };

    const handleViewLessons = async (id, baslik) => {
        try {
            const lessons = await sectionService.getLessons(id);
            console.log(`${baslik} Dersleri:`, lessons);
            // Dersler sayfasına yönlendirme yapılabilir
            alert(`${baslik} - ${lessons.toplamDers} ders bulunuyor`);
        } catch (err) {
            setError(err.message || 'Dersler yüklenirken hata oluştu');
        }
    };

    const resetForm = () => {
        setFormData({
            baslik: '',
            uniteId: '',
            sira: 1
        });
        setEditingSection(null);
        setSelectedLanguage('');
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

    const getUnitName = (uniteId) => {
        const unit = units.find(u => u.id === uniteId);
        return unit ? unit.baslik : 'Bilinmeyen Ünite';
    };

    const getLanguageName = (uniteId) => {
        const unit = units.find(u => u.id === uniteId);
        if (!unit) return 'Bilinmeyen';

        const language = languages.find(l => l.id === unit.programlamaDiliId);
        return language ? language.ad : 'Bilinmeyen Dil';
    };

    const filteredSections = sections.filter(section =>
        section.baslik.toLowerCase().includes(searchTerm.toLowerCase()) ||
        getUnitName(section.uniteId).toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (loading && sections.length === 0) {
        return (
            <div className="sections-page">
                <header className="admin-header">
                    <nav className="admin-nav">
                        <div className="admin-brand">
                            <div className="admin-logo">Logicfy Admin</div>
                            <div className="active-section">
                                📚 Kısımlar
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
                        <p>Kısımlar yükleniyor...</p>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="sections-page">
            {/* Header */}
            <header className="admin-header">
                <nav className="admin-nav">
                    <div className="admin-brand">
                        <div className="admin-logo">Logicfy Admin</div>
                        <div className="active-section">
                            📚 Kısımlar
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
                        <h1>📚 Kısımlar</h1>
                        <p>Platformdaki tüm kısımları yönetin</p>
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
                                    placeholder="Kısım ara..."
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                    onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                                />
                                <button onClick={handleSearch}>🔍</button>
                            </div>

                            <select
                                value={selectedLanguage}
                                onChange={(e) => handleLanguageFilter(e.target.value)}
                                className="filter-select"
                            >
                                <option value="">Tüm Diller</option>
                                {languages.map(language => (
                                    <option key={language.id} value={language.id}>
                                        {language.ad}
                                    </option>
                                ))}
                            </select>

                            <select
                                value={selectedUnit}
                                onChange={(e) => handleUnitFilter(e.target.value)}
                                className="filter-select"
                            >
                                <option value="">Tüm Üniteler</option>
                                {filteredUnits.map(unit => (
                                    <option key={unit.id} value={unit.id}>
                                        {unit.baslik}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <button className="btn-primary" onClick={openCreateModal}>
                            + Yeni Kısım Ekle
                        </button>
                    </div>

                    {/* Sections Grid */}
                    {!loading && (
                        <div className="sections-grid">
                            {filteredSections.map((section) => (
                                <div key={section.id} className="section-card">
                                    <div className="section-header">
                                        <div className="section-icon">
                                            <span>📚</span>
                                        </div>
                                        <div className="section-meta">
                                            <span className="section-sira">#{section.sira}</span>
                                            <span className="section-language">
                                                {getLanguageName(section.uniteId)}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="section-info">
                                        <h3>{section.baslik}</h3>
                                        <p className="section-unit">
                                            {getUnitName(section.uniteId)}
                                        </p>
                                        <div className="section-stats">
                                            <span>{section.dersSayisi || section.dersSayisiCache || 0} Ders</span>
                                            <span className="section-date">
                                                {new Date(section.createdAt).toLocaleDateString('tr-TR')}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="section-actions">
                                        <button
                                            className="btn-view"
                                            onClick={() => handleViewLessons(section.id, section.baslik)}
                                        >
                                            📖 Dersler
                                        </button>
                                        <button
                                            className="btn-details"
                                            onClick={() => handleViewDetails(section.id)}
                                        >
                                            👁️ Detay
                                        </button>
                                        <button
                                            className="btn-edit"
                                            onClick={() => handleEdit(section)}
                                        >
                                            ✏️ Düzenle
                                        </button>
                                        <button
                                            className="btn-delete"
                                            onClick={() => handleDelete(section.id, section.baslik)}
                                        >
                                            🗑️ Sil
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

                    {/* Empty State */}
                    {filteredSections.length === 0 && !loading && (
                        <div className="empty-state">
                            <div className="empty-icon">📚</div>
                            <h3>Henüz kısım eklenmemiş</h3>
                            <p>İlk kısmı eklemek için "Yeni Kısım Ekle" butonuna tıklayın.</p>
                            <button className="btn-primary" onClick={openCreateModal}>
                                + İlk Kısmı Ekle
                            </button>
                        </div>
                    )}

                    {/* Loading State for Grid */}
                    {loading && sections.length > 0 && (
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
                            <h2>{editingSection ? 'Kısmı Düzenle' : 'Yeni Kısım Ekle'}</h2>
                            <button className="close-btn" onClick={closeModal}>×</button>
                        </div>

                        <form onSubmit={handleSubmit} className="modal-form">
                            <div className="form-group">
                                <label>Kısım Başlığı *</label>
                                <input
                                    type="text"
                                    value={formData.baslik}
                                    onChange={(e) => setFormData({ ...formData, baslik: e.target.value })}
                                    required
                                    placeholder="Örn: Değişkenler ve Veri Tipleri"
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
                                    <label>Programlama Dili</label>
                                    <select
                                        value={selectedLanguage}
                                        onChange={(e) => setSelectedLanguage(e.target.value)}
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
                                <label>Ünite *</label>
                                <select
                                    value={formData.uniteId}
                                    onChange={(e) => setFormData({ ...formData, uniteId: e.target.value })}
                                    required
                                    disabled={filteredUnits.length === 0}
                                >
                                    <option value="">Ünite Seçin</option>
                                    {filteredUnits.map(unit => (
                                        <option key={unit.id} value={unit.id}>
                                            {unit.baslik}
                                        </option>
                                    ))}
                                </select>
                                {filteredUnits.length === 0 && (
                                    <small className="form-hint">
                                        Önce bir dil seçin veya ünite ekleyin
                                    </small>
                                )}
                            </div>

                            <div className="modal-actions">
                                <button type="button" className="btn-secondary" onClick={closeModal}>
                                    İptal
                                </button>
                                <button
                                    type="submit"
                                    className="btn-primary"
                                    disabled={!formData.baslik || !formData.uniteId}
                                >
                                    {editingSection ? 'Güncelle' : 'Oluştur'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Dock Menu */}
            <DockMenu onMenuSelect={handleMenuSelect} activeMenu="sections" />
        </div>
    );
};

export default Sections;