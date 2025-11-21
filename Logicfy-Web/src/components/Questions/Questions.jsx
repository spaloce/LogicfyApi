import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { questionService } from '../../services/questionService';
import { lessonService } from '../../services/lessonService';
import { sectionService } from '../../services/sectionService';
import { unitService } from '../../services/unitService';
import { programmingLanguageService } from '../../services/programmingLanguageService';
import DockMenu from '../DockMenu/DockMenu';
import QuestionDetailModal from './QuestionDetailModal';
import './Questions.css';

const Questions = ({ user, onLogout }) => {
    const [questions, setQuestions] = useState([]);
    const [lessons, setLessons] = useState([]);
    const [sections, setSections] = useState([]);
    const [units, setUnits] = useState([]);
    const [languages, setLanguages] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [showDetailModal, setShowDetailModal] = useState(false);
    const [editingQuestion, setEditingQuestion] = useState(null);
    const [selectedQuestion, setSelectedQuestion] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedLesson, setSelectedLesson] = useState('');
    const [selectedSection, setSelectedSection] = useState('');
    const [selectedUnit, setSelectedUnit] = useState('');
    const [selectedLanguage, setSelectedLanguage] = useState('');
    const [selectedType, setSelectedType] = useState('');
    const [selectedDifficulty, setSelectedDifficulty] = useState('');
    const [filteredSections, setFilteredSections] = useState([]);
    const [filteredUnits, setFilteredUnits] = useState([]);
    const [filteredLessons, setFilteredLessons] = useState([]);
    const navigate = useNavigate();

    const questionTypes = [
        { value: 1, label: 'Çoklu Seçenek', icon: '🔘', color: '#007bff', description: 'Birden fazla seçenek arasından doğru cevabı seçme' },
        { value: 2, label: 'Kod Tamamlama', icon: '🧩', color: '#28a745', description: 'Eksik kod parçalarını tamamlama' },
        { value: 3, label: 'Fonksiyon Çözüm', icon: '⚙️', color: '#ffc107', description: 'Fonksiyon veya algoritma yazma' },
        { value: 4, label: 'Canlı Preview', icon: '🎨', color: '#e83e8c', description: 'HTML/CSS canlı önizleme oluşturma' }
    ];

    const difficultyLevels = [
        { value: 1, label: 'Çok Kolay', color: '#28a745' },
        { value: 2, label: 'Kolay', color: '#20c997' },
        { value: 3, label: 'Orta', color: '#ffc107' },
        { value: 4, label: 'Zor', color: '#fd7e14' },
        { value: 5, label: 'Çok Zor', color: '#dc3545' }
    ];

    const [formData, setFormData] = useState({
        soruMetni: '',
        dersId: '',
        soruTipi: 1,
        seviye: 2,
        kodMetni: '',
        ekVeriJson: ''
    });

    // Çoklu seçenek formu
    const [optionsForm, setOptionsForm] = useState({
        options: ['', '', '', ''],
        correctOption: 0
    });

    // Fonksiyon çözüm formu
    const [functionForm, setFunctionForm] = useState({
        cozumKod: ''
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

    useEffect(() => {
        if (selectedSection && lessons.length > 0) {
            const filtered = lessons.filter(lesson =>
                lesson.kisimId.toString() === selectedSection
            );
            setFilteredLessons(filtered);
        } else {
            setFilteredLessons(lessons);
        }
    }, [selectedSection, lessons]);

    const loadData = async () => {
        try {
            setLoading(true);
            setError('');
            const [questionsData, lessonsData, sectionsData, unitsData, languagesData] = await Promise.all([
                questionService.getAll(),
                lessonService.getAll(),
                sectionService.getAll(),
                unitService.getAll(),
                programmingLanguageService.getAll()
            ]);
            setQuestions(questionsData);
            setLessons(lessonsData);
            setSections(sectionsData);
            setUnits(unitsData);
            setLanguages(languagesData);
            setFilteredLessons(lessonsData);
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
            const data = await questionService.getAll();
            const filtered = data.filter(question =>
                question.soruMetni.toLowerCase().includes(searchTerm.toLowerCase()) ||
                (question.ders && question.ders.baslik.toLowerCase().includes(searchTerm.toLowerCase()))
            );
            setQuestions(filtered);
        } catch (err) {
            setError(err.message || 'Arama sırasında hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleLessonFilter = async (lessonId) => {
        setSelectedLesson(lessonId);

        if (!lessonId) {
            loadData();
            return;
        }

        try {
            setLoading(true);
            const data = await questionService.getByLesson(lessonId);
            setQuestions(data);
        } catch (err) {
            setError(err.message || 'Filtreleme sırasında hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleTypeFilter = (type) => {
        setSelectedType(type);
        if (!type) {
            loadData();
            return;
        }

        const filtered = questions.filter(q => q.soruTipi.toString() === type);
        setQuestions(filtered);
    };

    const handleDifficultyFilter = (difficulty) => {
        setSelectedDifficulty(difficulty);
        if (!difficulty) {
            loadData();
            return;
        }

        const filtered = questions.filter(q => q.seviye.toString() === difficulty);
        setQuestions(filtered);
    };

    const handleLanguageFilter = (dilId) => {
        setSelectedLanguage(dilId);
        setSelectedUnit('');
        setSelectedSection('');
        setSelectedLesson('');
    };

    const handleUnitFilter = (unitId) => {
        setSelectedUnit(unitId);
        setSelectedSection('');
        setSelectedLesson('');
    };

    const handleSectionFilter = (sectionId) => {
        setSelectedSection(sectionId);
        setSelectedLesson('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            let result;
            if (editingQuestion) {
                result = await questionService.update(editingQuestion.id, formData);
            } else {
                result = await questionService.create(formData);
            }

            // Çoklu seçenek soruları için seçenekleri oluştur
            if (formData.soruTipi === 1 && optionsForm.options.some(opt => opt.trim() !== '')) {
                for (let i = 0; i < optionsForm.options.length; i++) {
                    if (optionsForm.options[i].trim()) {
                        await questionService.createOption({
                            soruId: result.id || editingQuestion.id,
                            secenekMetni: optionsForm.options[i],
                            isDogruCevap: i === optionsForm.correctOption
                        });
                    }
                }
            }

            // Fonksiyon çözüm soruları için çözüm oluştur
            if (formData.soruTipi === 3 && functionForm.cozumKod.trim()) {
                await questionService.createFunctionSolution({
                    soruId: result.id || editingQuestion.id,
                    cozumKod: functionForm.cozumKod
                });
            }

            setShowModal(false);
            resetForm();
            loadData();
        } catch (err) {
            setError(err.message || 'İşlem sırasında hata oluştu');
        }
    };

    const handleEdit = (question) => {
        setEditingQuestion(question);
        setFormData({
            soruMetni: question.soruMetni,
            dersId: question.dersId.toString(),
            soruTipi: question.soruTipi,
            seviye: question.seviye,
            kodMetni: question.kodMetni || '',
            ekVeriJson: question.ekVeriJson || ''
        });

        // İlişkili verileri otomatik ayarla
        const lesson = lessons.find(l => l.id === question.dersId);
        if (lesson) {
            setSelectedLesson(lesson.id.toString());
            const section = sections.find(s => s.id === lesson.kisimId);
            if (section) {
                setSelectedSection(section.id.toString());
                const unit = units.find(u => u.id === section.uniteId);
                if (unit) {
                    setSelectedUnit(unit.id.toString());
                    setSelectedLanguage(unit.programlamaDiliId.toString());
                }
            }
        }

        setShowModal(true);
    };

    const handleDelete = async (id, soruMetni) => {
        const shortText = soruMetni.length > 50 ? soruMetni.substring(0, 50) + '...' : soruMetni;
        if (!window.confirm(`"${shortText}" sorusunu silmek istediğinizden emin misiniz?`)) {
            return;
        }

        try {
            await questionService.delete(id);
            loadData();
        } catch (err) {
            setError(err.message || 'Silme işlemi sırasında hata oluştu');
        }
    };

    const handleViewDetails = async (question) => {
        try {
            const detail = await questionService.getById(question.id);
            setSelectedQuestion(detail);
            setShowDetailModal(true);
        } catch (err) {
            setError(err.message || 'Detaylar yüklenirken hata oluştu');
        }
    };

    const handleOptionChange = (index, value) => {
        const newOptions = [...optionsForm.options];
        newOptions[index] = value;
        setOptionsForm({ ...optionsForm, options: newOptions });
    };

    const addOption = () => {
        setOptionsForm({
            ...optionsForm,
            options: [...optionsForm.options, '']
        });
    };

    const removeOption = (index) => {
        const newOptions = optionsForm.options.filter((_, i) => i !== index);
        setOptionsForm({
            ...optionsForm,
            options: newOptions,
            correctOption: optionsForm.correctOption >= index ? Math.max(0, optionsForm.correctOption - 1) : optionsForm.correctOption
        });
    };

    const resetForm = () => {
        setFormData({
            soruMetni: '',
            dersId: '',
            soruTipi: 1,
            seviye: 2,
            kodMetni: '',
            ekVeriJson: ''
        });
        setOptionsForm({
            options: ['', '', '', ''],
            correctOption: 0
        });
        setFunctionForm({
            cozumKod: ''
        });
        setEditingQuestion(null);
        setSelectedLanguage('');
        setSelectedUnit('');
        setSelectedSection('');
        setSelectedLesson('');
    };

    const openCreateModal = () => {
        resetForm();
        setShowModal(true);
    };

    const closeModal = () => {
        setShowModal(false);
        resetForm();
    };

    const closeDetailModal = () => {
        setShowDetailModal(false);
        setSelectedQuestion(null);
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

    const getLessonName = (dersId) => {
        const lesson = lessons.find(l => l.id === dersId);
        return lesson ? lesson.baslik : 'Bilinmeyen Ders';
    };

    const getTypeLabel = (type) => {
        const questionType = questionTypes.find(t => t.value === type);
        return questionType ? questionType.label : 'Bilinmeyen';
    };

    const getTypeIcon = (type) => {
        const questionType = questionTypes.find(t => t.value === type);
        return questionType ? questionType.icon : '❓';
    };

    const getTypeColor = (type) => {
        const questionType = questionTypes.find(t => t.value === type);
        return questionType ? questionType.color : '#6c757d';
    };

    const getTypeDescription = (type) => {
        const questionType = questionTypes.find(t => t.value === type);
        return questionType ? questionType.description : '';
    };

    const getDifficultyLabel = (level) => {
        const difficulty = difficultyLevels.find(d => d.value === level);
        return difficulty ? difficulty.label : 'Bilinmeyen';
    };

    const getDifficultyColor = (level) => {
        const difficulty = difficultyLevels.find(d => d.value === level);
        return difficulty ? difficulty.color : '#6c757d';
    };

    const truncateText = (text, maxLength = 100) => {
        if (!text) return '';
        return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
    };

    const filteredQuestions = questions.filter(question =>
        question.soruMetni.toLowerCase().includes(searchTerm.toLowerCase()) ||
        getLessonName(question.dersId).toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (loading && questions.length === 0) {
        return (
            <div className="questions-page">
                <header className="admin-header">
                    <nav className="admin-nav">
                        <div className="admin-brand">
                            <div className="admin-logo">Logicfy Admin</div>
                            <div className="active-section">
                                ❓ Sorular
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
                        <p>Sorular yükleniyor...</p>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="questions-page">
            {/* Header */}
            <header className="admin-header">
                <nav className="admin-nav">
                    <div className="admin-brand">
                        <div className="admin-logo">Logicfy Admin</div>
                        <div className="active-section">
                            ❓ Sorular
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
                        <h1>❓ Sorular</h1>
                        <p>Platformdaki tüm soruları yönetin</p>
                    </div>

                    {/* Error Display */}
                    {error && (
                        <div className="error-message">
                            {error}
                            <button onClick={() => setError('')} className="close-error">×</button>
                        </div>
                    )}

                    {/* Stats Overview */}
                    <div className="stats-overview">
                        <div className="stat-card">
                            <div className="stat-number">{questions.length}</div>
                            <div className="stat-label">Toplam Soru</div>
                        </div>
                        {questionTypes.map(type => (
                            <div key={type.value} className="stat-card" style={{ borderTopColor: type.color }}>
                                <div className="stat-number">
                                    {questions.filter(q => q.soruTipi === type.value).length}
                                </div>
                                <div className="stat-label">{type.label}</div>
                            </div>
                        ))}
                    </div>

                    {/* Filters and Actions Bar */}
                    <div className="filters-actions-bar">
                        <div className="filters">
                            <div className="search-box">
                                <input
                                    type="text"
                                    placeholder="Soru ara..."
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
                                value={selectedLesson}
                                onChange={(e) => handleLessonFilter(e.target.value)}
                                className="filter-select"
                            >
                                <option value="">Tüm Dersler</option>
                                {filteredLessons.map(lesson => (
                                    <option key={lesson.id} value={lesson.id}>
                                        {lesson.baslik}
                                    </option>
                                ))}
                            </select>

                            <select
                                value={selectedType}
                                onChange={(e) => handleTypeFilter(e.target.value)}
                                className="filter-select"
                            >
                                <option value="">Tüm Tipler</option>
                                {questionTypes.map(type => (
                                    <option key={type.value} value={type.value}>
                                        {type.label}
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
                            + Yeni Soru Ekle
                        </button>
                    </div>

                    {/* Questions Grid */}
                    {!loading && (
                        <div className="questions-grid">
                            {filteredQuestions.map((question) => (
                                <div key={question.id} className="question-card">
                                    <div className="question-header">
                                        <div
                                            className="question-type"
                                            style={{ backgroundColor: getTypeColor(question.soruTipi) }}
                                            title={getTypeDescription(question.soruTipi)}
                                        >
                                            {getTypeIcon(question.soruTipi)}
                                        </div>
                                        <div className="question-meta">
                                            <span
                                                className="question-difficulty"
                                                style={{ backgroundColor: getDifficultyColor(question.seviye) }}
                                            >
                                                {getDifficultyLabel(question.seviye)}
                                            </span>
                                            <span className="question-date">
                                                {new Date(question.createdAt).toLocaleDateString('tr-TR')}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="question-content">
                                        <h3>{truncateText(question.soruMetni, 120)}</h3>
                                        <p className="question-lesson">
                                            {getLessonName(question.dersId)}
                                        </p>
                                        {question.secenekler && question.secenekler.length > 0 && (
                                            <div className="question-options">
                                                <small>Seçenekler: {question.secenekler.length}</small>
                                            </div>
                                        )}
                                        {question.kodMetni && (
                                            <div className="question-code">
                                                <small>Kod içeriyor</small>
                                            </div>
                                        )}
                                        {question.dogruCevap && (
                                            <div className="question-correct">
                                                <small>Doğru Cevap: {truncateText(question.dogruCevap.secenekMetni, 30)}</small>
                                            </div>
                                        )}
                                    </div>

                                    <div className="question-actions">
                                        <button
                                            className="btn-details"
                                            onClick={() => handleViewDetails(question)}
                                        >
                                            👁️ Detay
                                        </button>
                                        <button
                                            className="btn-edit"
                                            onClick={() => handleEdit(question)}
                                        >
                                            ✏️ Düzenle
                                        </button>
                                        <button
                                            className="btn-delete"
                                            onClick={() => handleDelete(question.id, question.soruMetni)}
                                        >
                                            🗑️ Sil
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

                    {/* Empty State */}
                    {filteredQuestions.length === 0 && !loading && (
                        <div className="empty-state">
                            <div className="empty-icon">❓</div>
                            <h3>Henüz soru eklenmemiş</h3>
                            <p>İlk soruyu eklemek için "Yeni Soru Ekle" butonuna tıklayın.</p>
                            <button className="btn-primary" onClick={openCreateModal}>
                                + İlk Soruyu Ekle
                            </button>
                        </div>
                    )}

                    {/* Loading State for Grid */}
                    {loading && questions.length > 0 && (
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
                            <h2>{editingQuestion ? 'Soruyu Düzenle' : 'Yeni Soru Ekle'}</h2>
                            <button className="close-btn" onClick={closeModal}>×</button>
                        </div>

                        <form onSubmit={handleSubmit} className="modal-form">
                            <div className="form-group">
                                <label>Soru Metni *</label>
                                <textarea
                                    value={formData.soruMetni}
                                    onChange={(e) => setFormData({ ...formData, soruMetni: e.target.value })}
                                    required
                                    placeholder="Sorunuzu buraya yazın..."
                                    rows="4"
                                />
                            </div>

                            <div className="form-row">
                                <div className="form-group">
                                    <label>Soru Tipi *</label>
                                    <select
                                        value={formData.soruTipi}
                                        onChange={(e) => setFormData({ ...formData, soruTipi: parseInt(e.target.value) })}
                                        required
                                    >
                                        {questionTypes.map(type => (
                                            <option key={type.value} value={type.value}>
                                                {type.label}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div className="form-group">
                                    <label>Zorluk Seviyesi *</label>
                                    <select
                                        value={formData.seviye}
                                        onChange={(e) => setFormData({ ...formData, seviye: parseInt(e.target.value) })}
                                        required
                                    >
                                        {difficultyLevels.map(level => (
                                            <option key={level.value} value={level.value}>
                                                {level.label}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            </div>

                            {/* Çoklu Seçenek Formu */}
                            {formData.soruTipi === 1 && (
                                <div className="question-type-form">
                                    <h4>Çoklu Seçenekler</h4>
                                    {optionsForm.options.map((option, index) => (
                                        <div key={index} className="option-row">
                                            <input
                                                type="radio"
                                                name="correctOption"
                                                checked={optionsForm.correctOption === index}
                                                onChange={() => setOptionsForm({ ...optionsForm, correctOption: index })}
                                                className="correct-radio"
                                            />
                                            <input
                                                type="text"
                                                value={option}
                                                onChange={(e) => handleOptionChange(index, e.target.value)}
                                                placeholder={`Seçenek ${index + 1}`}
                                                className="option-input"
                                            />
                                            {optionsForm.options.length > 2 && (
                                                <button
                                                    type="button"
                                                    onClick={() => removeOption(index)}
                                                    className="remove-option"
                                                >
                                                    ×
                                                </button>
                                            )}
                                        </div>
                                    ))}
                                    <button
                                        type="button"
                                        onClick={addOption}
                                        className="add-option-btn"
                                    >
                                        + Seçenek Ekle
                                    </button>
                                </div>
                            )}

                            {/* Fonksiyon Çözüm Formu */}
                            {formData.soruTipi === 3 && (
                                <div className="question-type-form">
                                    <h4>Fonksiyon Çözümü</h4>
                                    <textarea
                                        value={functionForm.cozumKod}
                                        onChange={(e) => setFunctionForm({ cozumKod: e.target.value })}
                                        placeholder="Fonksiyon çözüm kodunu buraya yazın..."
                                        rows="4"
                                        className="code-textarea"
                                    />
                                </div>
                            )}

                            <div className="form-row">
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
                            </div>

                            <div className="form-row">
                                <div className="form-group">
                                    <label>Kısım</label>
                                    <select
                                        value={selectedSection}
                                        onChange={(e) => setSelectedSection(e.target.value)}
                                        disabled={filteredSections.length === 0}
                                    >
                                        <option value="">Kısım Seçin</option>
                                        {filteredSections.map(section => (
                                            <option key={section.id} value={section.id}>
                                                {section.baslik}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div className="form-group">
                                    <label>Ders *</label>
                                    <select
                                        value={formData.dersId}
                                        onChange={(e) => setFormData({ ...formData, dersId: e.target.value })}
                                        required
                                        disabled={filteredLessons.length === 0}
                                    >
                                        <option value="">Ders Seçin</option>
                                        {filteredLessons.map(lesson => (
                                            <option key={lesson.id} value={lesson.id}>
                                                {lesson.baslik}
                                            </option>
                                        ))}
                                    </select>
                                    {filteredLessons.length === 0 && (
                                        <small className="form-hint">
                                            Önce bir dil, ünite ve kısım seçin
                                        </small>
                                    )}
                                </div>
                            </div>

                            <div className="form-group">
                                <label>Kod Metni (Opsiyonel)</label>
                                <textarea
                                    value={formData.kodMetni}
                                    onChange={(e) => setFormData({ ...formData, kodMetni: e.target.value })}
                                    placeholder="Kod içeriyorsa buraya yazın..."
                                    rows="3"
                                />
                            </div>

                            <div className="form-group">
                                <label>Ek Veri JSON (Opsiyonel)</label>
                                <textarea
                                    value={formData.ekVeriJson}
                                    onChange={(e) => setFormData({ ...formData, ekVeriJson: e.target.value })}
                                    placeholder='{"key": "value"} formatında JSON verisi...'
                                    rows="2"
                                />
                            </div>

                            <div className="modal-actions">
                                <button type="button" className="btn-secondary" onClick={closeModal}>
                                    İptal
                                </button>
                                <button
                                    type="submit"
                                    className="btn-primary"
                                    disabled={!formData.soruMetni || !formData.dersId}
                                >
                                    {editingQuestion ? 'Güncelle' : 'Oluştur'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Detay Modal */}
            {showDetailModal && selectedQuestion && (
                <QuestionDetailModal
                    question={selectedQuestion}
                    onClose={closeDetailModal}
                    questionTypes={questionTypes}
                    difficultyLevels={difficultyLevels}
                    getTypeLabel={getTypeLabel}
                    getDifficultyLabel={getDifficultyLabel}
                    getLessonName={getLessonName}
                />
            )}

            {/* Dock Menu */}
            <DockMenu onMenuSelect={handleMenuSelect} activeMenu="questions" />
        </div>
    );
};

export default Questions;