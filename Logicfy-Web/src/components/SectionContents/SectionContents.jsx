import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { sectionContentService } from '../../services/sectionContentService';
import { programmingLanguageService } from '../../services/programmingLanguageService';
import { unitService } from '../../services/unitService';
import { sectionService } from '../../services/sectionService';
import DockMenu from '../DockMenu/DockMenu';
import './SectionContents.css';

const SectionContents = ({ user, onLogout }) => {
    const [contents, setContents] = useState([]);
    const [languages, setLanguages] = useState([]);
    const [units, setUnits] = useState([]);
    const [sections, setSections] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [editingContent, setEditingContent] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedLanguage, setSelectedLanguage] = useState('');
    const [selectedUnit, setSelectedUnit] = useState('');
    const [selectedSection, setSelectedSection] = useState('');
    const [filteredUnits, setFilteredUnits] = useState([]);
    const [filteredSections, setFilteredSections] = useState([]);
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        baslik: '',
        programlamaDiliId: '',
        uniteId: '',
        kisimId: '',
        aciklamaHtml: '',
        ornekKod: '',
        ekstraJson: ''
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
            const [contentsData, languagesData, unitsData, sectionsData] = await Promise.all([
                sectionContentService.getAll(),
                programmingLanguageService.getAll(),
                unitService.getAll(),
                sectionService.getAll()
            ]);
            setContents(contentsData);
            setLanguages(languagesData);
            setUnits(unitsData);
            setSections(sectionsData);
            setFilteredUnits(unitsData);
            setFilteredSections(sectionsData);
        } catch (err) {
            setError(err.message || 'Veriler yüklenirken bir hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = () => {
        if (!searchTerm.trim()) {
            loadData();
            return;
        }

        const filtered = contents.filter(content =>
            content.baslik.toLowerCase().includes(searchTerm.toLowerCase()) ||
            content.programlamaDili.toLowerCase().includes(searchTerm.toLowerCase()) ||
            content.unite.toLowerCase().includes(searchTerm.toLowerCase()) ||
            content.kisim.toLowerCase().includes(searchTerm.toLowerCase())
        );
        setContents(filtered);
    };

    const handleLanguageFilter = (dilId) => {
        setSelectedLanguage(dilId);
        setSelectedUnit('');
        setSelectedSection('');

        if (!dilId) {
            loadData();
            return;
        }

        const filtered = contents.filter(content =>
            content.programlamaDiliId.toString() === dilId
        );
        setContents(filtered);
    };

    const handleUnitFilter = (unitId) => {
        setSelectedUnit(unitId);
        setSelectedSection('');

        if (!unitId) {
            handleLanguageFilter(selectedLanguage);
            return;
        }

        const filtered = contents.filter(content =>
            content.uniteId.toString() === unitId
        );
        setContents(filtered);
    };

    const handleSectionFilter = (sectionId) => {
        setSelectedSection(sectionId);

        if (!sectionId) {
            handleUnitFilter(selectedUnit);
            return;
        }

        const filtered = contents.filter(content =>
            content.kisimId.toString() === sectionId
        );
        setContents(filtered);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            if (editingContent) {
                await sectionContentService.update(editingContent.id, formData);
            } else {
                await sectionContentService.create(formData);
            }

            setShowModal(false);
            resetForm();
            loadData();
        } catch (err) {
            setError(err.message || 'İşlem sırasında hata oluştu');
        }
    };

    const handleEdit = (content) => {
        setEditingContent(content);
        setFormData({
            baslik: content.baslik,
            programlamaDiliId: content.programlamaDiliId.toString(),
            uniteId: content.uniteId.toString(),
            kisimId: content.kisimId.toString(),
            aciklamaHtml: content.aciklamaHtml || '',
            ornekKod: content.ornekKod || '',
            ekstraJson: content.ekstraJson || ''
        });

        setSelectedLanguage(content.programlamaDiliId.toString());
        setSelectedUnit(content.uniteId.toString());
        setSelectedSection(content.kisimId.toString());

        setShowModal(true);
    };

    const handleDelete = async (id, baslik) => {
        if (!window.confirm(`"${baslik}" içeriğini silmek istediğinizden emin misiniz?`)) {
            return;
        }

        try {
            await sectionContentService.delete(id);
            loadData();
        } catch (err) {
            setError(err.message || 'Silme işlemi sırasında hata oluştu');
        }
    };

    const handleViewDetails = async (id) => {
        try {
            const detail = await sectionContentService.getById(id);
            console.log('İçerik Detayları:', detail);

            let message = `${detail.baslik}\n\n`;
            message += `Dil: ${detail.programlamaDili}\n`;
            message += `Ünite: ${detail.unite}\n`;
            message += `Kısım: ${detail.kisim}\n`;

            if (detail.aciklamaHtml) {
                message += `\nAçıklama: ${detail.aciklamaHtml.substring(0, 100)}...\n`;
            }

            alert(message);
        } catch (err) {
            setError(err.message || 'Detaylar yüklenirken hata oluştu');
        }
    };

    const resetForm = () => {
        setFormData({
            baslik: '',
            programlamaDiliId: '',
            uniteId: '',
            kisimId: '',
            aciklamaHtml: '',
            ornekKod: '',
            ekstraJson: ''
        });
        setEditingContent(null);
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
            users: '/users',
            sectioncontents: '/sectioncontents'
        };

        if (routes[menuId]) {
            navigate(routes[menuId]);
        }
    };

    const truncateText = (text, maxLength = 100) => {
        if (!text) return '';
        return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
    };

    const stripHtml = (html) => {
        if (!html) return '';
        return html.replace(/<[^>]*>/g, '');
    };

    const filteredContents = contents.filter(content =>
        content.baslik.toLowerCase().includes(searchTerm.toLowerCase()) ||
        content.programlamaDili.toLowerCase().includes(searchTerm.toLowerCase()) ||
        content.unite.toLowerCase().includes(searchTerm.toLowerCase()) ||
        content.kisim.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (loading && contents.length === 0) {
        return (
            <div className="section-contents-page">
                <header className="admin-header">
                    <nav className="admin-nav">
                        <div className="admin-brand">
                            <div className="admin-logo">Logicfy Admin</div>
                            <div className="active-section">
                                📝 Kısım İçerikleri
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
                        <p>İçerikler yükleniyor...</p>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="section-contents-page">
            {/* Header */}
            <header className="admin-header">
                <nav className="admin-nav">
                    <div className="admin-brand">
                        <div className="admin-logo">Logicfy Admin</div>
                        <div className="active-section">
                            📝 Kısım İçerikleri
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
                        <h1>📝 Kısım İçerikleri</h1>
                        <p>Kısımlara ait açıklama ve örnek kod içeriklerini yönetin</p>
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
                                    placeholder="İçerik ara..."
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
                        </div>

                        <button className="btn-primary" onClick={openCreateModal}>
                            + Yeni İçerik Ekle
                        </button>
                    </div>

                    {/* Contents Grid */}
                    {!loading && (
                        <div className="contents-grid">
                            {filteredContents.map((content) => (
                                <div key={content.id} className="content-card">
                                    <div className="content-header">
                                        <div className="content-icon">
                                            <span>📝</span>
                                        </div>
                                        <div className="content-meta">
                                            <span className="content-language">
                                                {content.programlamaDili}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="content-info">
                                        <h3>{content.baslik}</h3>
                                        <div className="content-path">
                                            <span>{content.unite} → {content.kisim}</span>
                                        </div>
                                        {content.aciklamaHtml && (
                                            <p className="content-description">
                                                {truncateText(stripHtml(content.aciklamaHtml), 120)}
                                            </p>
                                        )}
                                        {content.ornekKod && (
                                            <div className="content-code">
                                                <small>Örnek kod içeriyor</small>
                                            </div>
                                        )}
                                    </div>

                                    <div className="content-actions">
                                        <button
                                            className="btn-details"
                                            onClick={() => handleViewDetails(content.id)}
                                        >
                                            👁️ Detay
                                        </button>
                                        <button
                                            className="btn-edit"
                                            onClick={() => handleEdit(content)}
                                        >
                                            ✏️ Düzenle
                                        </button>
                                        <button
                                            className="btn-delete"
                                            onClick={() => handleDelete(content.id, content.baslik)}
                                        >
                                            🗑️ Sil
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

                    {/* Empty State */}
                    {filteredContents.length === 0 && !loading && (
                        <div className="empty-state">
                            <div className="empty-icon">📝</div>
                            <h3>Henüz içerik eklenmemiş</h3>
                            <p>İlk içeriği eklemek için "Yeni İçerik Ekle" butonuna tıklayın.</p>
                            <button className="btn-primary" onClick={openCreateModal}>
                                + İlk İçeriği Ekle
                            </button>
                        </div>
                    )}

                    {/* Loading State for Grid */}
                    {loading && contents.length > 0 && (
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
                    <div className="modal large-modal" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <h2>{editingContent ? 'İçeriği Düzenle' : 'Yeni İçerik Ekle'}</h2>
                            <button className="close-btn" onClick={closeModal}>×</button>
                        </div>

                        <form onSubmit={handleSubmit} className="modal-form">
                            <div className="form-group">
                                <label>Başlık *</label>
                                <input
                                    type="text"
                                    value={formData.baslik}
                                    onChange={(e) => setFormData({ ...formData, baslik: e.target.value })}
                                    required
                                    placeholder="İçerik başlığını yazın..."
                                />
                            </div>

                            <div className="form-row">
                                <div className="form-group">
                                    <label>Programlama Dili *</label>
                                    <select
                                        value={formData.programlamaDiliId}
                                        onChange={(e) => {
                                            setFormData({ ...formData, programlamaDiliId: e.target.value });
                                            setSelectedLanguage(e.target.value);
                                        }}
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

                                <div className="form-group">
                                    <label>Ünite *</label>
                                    <select
                                        value={formData.uniteId}
                                        onChange={(e) => {
                                            setFormData({ ...formData, uniteId: e.target.value });
                                            setSelectedUnit(e.target.value);
                                        }}
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
                                            Önce bir dil seçin
                                        </small>
                                    )}
                                </div>
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

                            <div className="form-group">
                                <label>Açıklama (HTML)</label>
                                <textarea
                                    value={formData.aciklamaHtml}
                                    onChange={(e) => setFormData({ ...formData, aciklamaHtml: e.target.value })}
                                    placeholder="HTML formatında açıklama yazın..."
                                    rows="6"
                                />
                            </div>

                            <div className="form-group">
                                <label>Örnek Kod</label>
                                <textarea
                                    value={formData.ornekKod}
                                    onChange={(e) => setFormData({ ...formData, ornekKod: e.target.value })}
                                    placeholder="Örnek kod yazın..."
                                    rows="4"
                                    className="code-textarea"
                                />
                            </div>

                            <div className="form-group">
                                <label>Ekstra JSON Verisi</label>
                                <textarea
                                    value={formData.ekstraJson}
                                    onChange={(e) => setFormData({ ...formData, ekstraJson: e.target.value })}
                                    placeholder='{"key": "value"} formatında JSON verisi...'
                                    rows="3"
                                />
                            </div>

                            <div className="modal-actions">
                                <button type="button" className="btn-secondary" onClick={closeModal}>
                                    İptal
                                </button>
                                <button
                                    type="submit"
                                    className="btn-primary"
                                    disabled={!formData.baslik || !formData.programlamaDiliId || !formData.uniteId || !formData.kisimId}
                                >
                                    {editingContent ? 'Güncelle' : 'Oluştur'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Dock Menu */}
            <DockMenu onMenuSelect={handleMenuSelect} activeMenu="sectioncontents" />
        </div>
    );
};

export default SectionContents;