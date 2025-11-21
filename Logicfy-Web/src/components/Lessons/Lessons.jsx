import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { lessonService } from '../../services/lessonService';
import { sectionService } from '../../services/sectionService';
import { unitService } from '../../services/unitService';
import { programmingLanguageService } from '../../services/programmingLanguageService';
import DockMenu from '../DockMenu/DockMenu';
import './Lessons.css';

const Lessons = ({ user, onLogout }) => {
    const [lessons, setLessons] = useState([]);
    const [sections, setSections] = useState([]);
    const [units, setUnits] = useState([]);
    const [languages, setLanguages] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [editingLesson, setEditingLesson] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedSection, setSelectedSection] = useState('');
    const [selectedUnit, setSelectedUnit] = useState('');
    const [selectedLanguage, setSelectedLanguage] = useState('');
    const [selectedDifficulty, setSelectedDifficulty] = useState('');
    const [filteredSections, setFilteredSections] = useState([]);
    const [filteredUnits, setFilteredUnits] = useState([]);
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        baslik: '',
        kisimId: '',
        sira: 1,
        tahminiSure: 30,
        zorlukSeviyesi: 2
    });

    const difficultyLevels = [
        { value: 1, label: 'Çok Kolay', color: '#28a745' },
        { value: 2, label: 'Kolay', color: '#20c997' },
        { value: 3, label: 'Orta', color: '#ffc107' },
        { value: 4, label: 'Zor', color: '#fd7e14' },
        { value: 5, label: 'Çok Zor', color: '#dc3545' }
    ];

    useEffect(() => {
        loadData();
    }, []);

    useEffect(() => {
        if (selectedLanguage && units.length > 0) {
            const filtered = units.filter(unit =>
                unit.programlamaDiliId.toString() === selectedLanguage
            );
            setFilteredUnits(filtered);
        } else {
            setFilteredUnits(units);
        }
    }, [selectedLanguage, units]);

    useEffect(() => {
        if (selectedUnit && sections.length > 0) {
            const filtered = sections.filter(section =>
                section.uniteId.toString() === selectedUnit
            );
            setFilteredSections(filtered);
        } else {
            setFilteredSections(sections);
        }
    }, [selectedUnit, sections]);

    const loadData = async () => {
        try {
            setLoading(true);
            setError('');
            const [lessonsData, sectionsData, unitsData, languagesData] = await Promise.all([
                lessonService.getAll(),
                sectionService.getAll(),
                unitService.getAll(),
                programmingLanguageService.getAll()
            ]);
            setLessons(lessonsData);
            setSections(sectionsData);
            setUnits(unitsData);
            setLanguages(languagesData);
            setFilteredSections(sectionsData);
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
            const data = await lessonService.search(searchTerm);
            setLessons(data);
        } catch (err) {
            setError(err.message || 'Arama sırasında hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleSectionFilter = async (sectionId) => {
        setSelectedSection(sectionId);

        if (!sectionId) {
            loadData();
            return;
        }

        try {
            setLoading(true);
            const data = await lessonService.getBySection(sectionId);
            setLessons(data);
        } catch (err) {
            setError(err.message || 'Filtreleme sırasında hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleDifficultyFilter = async (difficulty) => {
        setSelectedDifficulty(difficulty);

        if (!difficulty) {
            loadData();
            return;
        }

        try {
            setLoading(true);
            const data = await lessonService.getByDifficulty(difficulty);
            setLessons(data);
        } catch (err) {
            setError(err.message || 'Filtreleme sırasında hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleLanguageFilter = (dilId) => {
        setSelectedLanguage(dilId);
        setSelectedUnit('');
        setSelectedSection('');
    };

    const handleUnitFilter = (unitId) => {
        setSelectedUnit(unitId);
        setSelectedSection('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            if (editingLesson) {
                await lessonService.update(editingLesson.id, formData);
            } else {
                await lessonService.create(formData);
            }

            setShowModal(false);
            resetForm();
            loadData();
        } catch (err) {
            setError(err.message || 'İşlem sırasında hata oluştu');
        }
    };

    const handleEdit = (lesson) => {
        setEditingLesson(lesson);
        setFormData({
            baslik: lesson.baslik,
            kisimId: lesson.kisimId.toString(),
            sira: lesson.sira,
            tahminiSure: lesson.tahminiSure || 30,
            zorlukSeviyesi: lesson.zorlukSeviyesi || 2
        });

        // Kısım ve ünite bilgilerini otomatik ayarla
        const section = sections.find(s => s.id === lesson.kisimId);
        if (section) {
            setSelectedUnit(section.uniteId.toString());
            const unit = units.find(u => u.id === section.uniteId);
            if (unit) {
                setSelectedLanguage(unit.programlamaDiliId.toString());
            }
        }

        setShowModal(true);
    };

    const handleDelete = async (id, baslik) => {
        if (!window.confirm(`"${baslik}" dersini silmek istediğinizden emin misiniz?`)) {
            return;
        }

        try {
            await lessonService.delete(id);
            loadData();
        } catch (err) {
            setError(err.message || 'Silme işlemi sırasında hata oluştu');
        }
    };

    const handleViewDetails = async (id) => {
        try {
            const detail = await lessonService.getStatistics(id);
            console.log('Ders Detayları:', detail);
            alert(`${detail.dersBaslik}\n\nSüre: ${detail.tahminiSure} dakika\nZorluk: ${getDifficultyLabel(detail.zorlukSeviyesi)}\nToplam Soru: ${detail.toplamSoru}\nKayıtlı Kullanıcı: ${detail.kayitliKullaniciSayisi}`);
        } catch (err) {
            setError(err.message || 'Detaylar yüklenirken hata oluştu');
        }
    };

    const handleViewQuestions = (lessonId, baslik) => {
        // Sorular sayfasına yönlendirme yapılabilir
        alert(`${baslik} dersinin soruları görüntülenecek`);
    };

    const resetForm = () => {
        setFormData({
            baslik: '',
            kisimId: '',
            sira: 1,
            tahminiSure: 30,
            zorlukSeviyesi: 2
        });
        setEditingLesson(null);
        setSelectedLanguage('');
        setSelectedUnit('');
        setSelectedSection('');
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

    const getSectionName = (kisimId) => {
        const section = sections.find(s => s.id === kisimId);
        return section ? section.baslik : 'Bilinmeyen Kısım';
    };

    const getUnitName = (kisimId) => {
        const section = sections.find(s => s.id === kisimId);
        if (!section) return 'Bilinmeyen';

        const unit = units.find(u => u.id === section.uniteId);
        return unit ? unit.baslik : 'Bilinmeyen Ünite';
    };

    const getLanguageName = (kisimId) => {
        const section = sections.find(s => s.id === kisimId);
        if (!section) return 'Bilinmeyen';

        const unit = units.find(u => u.id === section.uniteId);
        if (!unit) return 'Bilinmeyen';

        const language = languages.find(l => l.id === unit.programlamaDiliId);
        return language ? language.ad : 'Bilinmeyen Dil';
    };

    const getDifficultyLabel = (level) => {
        const difficulty = difficultyLevels.find(d => d.value === level);
        return difficulty ? difficulty.label : 'Bilinmeyen';
    };

    const getDifficultyColor = (level) => {
        const difficulty = difficultyLevels.find(d => d.value === level);
        return difficulty ? difficulty.color : '#6c757d';
    };

    const filteredLessons = lessons.filter(lesson =>
        lesson.baslik.toLowerCase().includes(searchTerm.toLowerCase()) ||
        getSectionName(lesson.kisimId).toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (loading && lessons.length === 0) {
        return (
            <div className="lessons-page">
                <header className="admin-header">
                    <nav className="admin-nav">
                        <div className="admin-brand">
                            <div className="admin-logo">Logicfy Admin</div>
                            <div className="active-section">
                                📖 Dersler
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
                        <p>Dersler yükleniyor...</p>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="lessons-page">
            {/* Header */}
            <header className="admin-header">
                <nav className="admin-nav">
                    <div className="admin-brand">
                        <div className="admin-logo">Logicfy Admin</div>
                        <div className="active-section">
                            📖 Dersler
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
                        <h1>📖 Dersler</h1>
                        <p>Platformdaki tüm dersleri yönetin</p>
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
                                    placeholder="Ders ara..."
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

                            <select
                                value={selectedSection}
                                onChange={(e) => handleSectionFilter(e.target.value)}
                                className="filter-select"
                            >
                                <option value="">Tüm Kısımlar</option>
                                {filteredSections.map(section => (
                                    <option key={section.id} value={section.id}>
                                        {section.baslik}
                                    </option>
                                ))}
                            </select>

                            <select
                                value={selectedDifficulty}
                                onChange={(e) => handleDifficultyFilter(e.target.value)}
                                className="filter-select"
                            >
                                <option value="">Tüm Zorluklar</option>
                                {difficultyLevels.map(level => (
                                    <option key={level.value} value={level.value}>
                                        {level.label}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <button className="btn-primary" onClick={openCreateModal}>
                            + Yeni Ders Ekle
                        </button>
                    </div>

                    {/* Lessons Grid */}
                    {!loading && (
                        <div className="lessons-grid">
                            {filteredLessons.map((lesson) => (
                                <div key={lesson.id} className="lesson-card">
                                    <div className="lesson-header">
                                        <div className="lesson-icon">
                                            <span>📖</span>
                                        </div>
                                        <div className="lesson-meta">
                                            <span className="lesson-sira">#{lesson.sira}</span>
                                            <span
                                                className="lesson-difficulty"
                                                style={{ backgroundColor: getDifficultyColor(lesson.zorlukSeviyesi) }}
                                            >
                                                {getDifficultyLabel(lesson.zorlukSeviyesi)}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="lesson-info">
                                        <h3>{lesson.baslik}</h3>
                                        <div className="lesson-details">
                                            <span className="lesson-duration">
                                                ⏱️ {lesson.tahminiSure || 30} dakika
                                            </span>
                                            <span className="lesson-questions">
                                                ❓ {lesson.soruSayisi || lesson.soruSayisiCache || 0} soru
                                            </span>
                                        </div>
                                        <p className="lesson-section">
                                            {getSectionName(lesson.kisimId)}
                                        </p>
                                        <div className="lesson-path">
                                            <small>
                                                {getLanguageName(lesson.kisimId)} → {getUnitName(lesson.kisimId)}
                                            </small>
                                        </div>
                                    </div>

                                    <div className="lesson-actions">
                                        <button
                                            className="btn-questions"
                                            onClick={() => handleViewQuestions(lesson.id, lesson.baslik)}
                                        >
                                            ❓ Sorular
                                        </button>
                                        <button
                                            className="btn-details"
                                            onClick={() => handleViewDetails(lesson.id)}
                                        >
                                            👁️ Detay
                                        </button>
                                        <button
                                            className="btn-edit"
                                            onClick={() => handleEdit(lesson)}
                                        >
                                            ✏️ Düzenle
                                        </button>
                                        <button
                                            className="btn-delete"
                                            onClick={() => handleDelete(lesson.id, lesson.baslik)}
                                        >
                                            🗑️ Sil
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

                    {/* Empty State */}
                    {filteredLessons.length === 0 && !loading && (
                        <div className="empty-state">
                            <div className="empty-icon">📖</div>
                            <h3>Henüz ders eklenmemiş</h3>
                            <p>İlk dersi eklemek için "Yeni Ders Ekle" butonuna tıklayın.</p>
                            <button className="btn-primary" onClick={openCreateModal}>
                                + İlk Dersi Ekle
                            </button>
                        </div>
                    )}

                    {/* Loading State for Grid */}
                    {loading && lessons.length > 0 && (
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
                            <h2>{editingLesson ? 'Dersi Düzenle' : 'Yeni Ders Ekle'}</h2>
                            <button className="close-btn" onClick={closeModal}>×</button>
                        </div>

                        <form onSubmit={handleSubmit} className="modal-form">
                            <div className="form-group">
                                <label>Ders Başlığı *</label>
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
                                    <label>Tahmini Süre (dakika) *</label>
                                    <input
                                        type="number"
                                        value={formData.tahminiSure}
                                        onChange={(e) => setFormData({ ...formData, tahminiSure: parseInt(e.target.value) || 30 })}
                                        required
                                        min="1"
                                        placeholder="30"
                                    />
                                </div>
                            </div>

                            <div className="form-row">
                                <div className="form-group">
                                    <label>Zorluk Seviyesi *</label>
                                    <select
                                        value={formData.zorlukSeviyesi}
                                        onChange={(e) => setFormData({ ...formData, zorlukSeviyesi: parseInt(e.target.value) })}
                                        required
                                    >
                                        {difficultyLevels.map(level => (
                                            <option key={level.value} value={level.value}>
                                                {level.label}
                                            </option>
                                        ))}
                                    </select>
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

                            <div className="form-row">
                                <div className="form-group">
                                    <label>Ünite</label>
                                    <select
                                        value={selectedUnit}
                                        onChange={(e) => setSelectedUnit(e.target.value)}
                                        disabled={filteredUnits.length === 0}
                                    >
                                        <option value="">Ünite Seçin</option>
                                        {filteredUnits.map(unit => (
                                            <option key={unit.id} value={unit.id}>
                                                {unit.baslik}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div className="form-group">
                                    <label>Kısım *</label>
                                    <select
                                        value={formData.kisimId}
                                        onChange={(e) => setFormData({ ...formData, kisimId: e.target.value })}
                                        required
                                        disabled={filteredSections.length === 0}
                                    >
                                        <option value="">Kısım Seçin</option>
                                        {filteredSections.map(section => (
                                            <option key={section.id} value={section.id}>
                                                {section.baslik}
                                            </option>
                                        ))}
                                    </select>
                                    {filteredSections.length === 0 && (
                                        <small className="form-hint">
                                            Önce bir dil ve ünite seçin
                                        </small>
                                    )}
                                </div>
                            </div>

                            <div className="modal-actions">
                                <button type="button" className="btn-secondary" onClick={closeModal}>
                                    İptal
                                </button>
                                <button
                                    type="submit"
                                    className="btn-primary"
                                    disabled={!formData.baslik || !formData.kisimId}
                                >
                                    {editingLesson ? 'Güncelle' : 'Oluştur'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Dock Menu */}
            <DockMenu onMenuSelect={handleMenuSelect} activeMenu="lessons" />
        </div>
    );
};

export default Lessons;