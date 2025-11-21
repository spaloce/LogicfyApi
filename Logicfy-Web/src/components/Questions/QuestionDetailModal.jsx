import React from 'react';
import './Questions.css';

const QuestionDetailModal = ({
    question,
    onClose,
    questionTypes,
    difficultyLevels,
    getTypeLabel,
    getDifficultyLabel,
    getLessonName
}) => {
    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal large-modal" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h2>Soru Detayları</h2>
                    <button className="close-btn" onClick={onClose}>×</button>
                </div>

                <div className="modal-content">
                    <div className="detail-section">
                        <h3>Soru Bilgileri</h3>
                        <div className="detail-grid">
                            <div className="detail-item">
                                <label>Soru Tipi:</label>
                                <span>{getTypeLabel(question.soruTipi)}</span>
                            </div>
                            <div className="detail-item">
                                <label>Zorluk Seviyesi:</label>
                                <span>{getDifficultyLabel(question.seviye)}</span>
                            </div>
                            <div className="detail-item">
                                <label>Ders:</label>
                                <span>{getLessonName(question.dersId)}</span>
                            </div>
                            <div className="detail-item">
                                <label>Oluşturulma Tarihi:</label>
                                <span>{new Date(question.createdAt).toLocaleDateString('tr-TR')}</span>
                            </div>
                        </div>
                    </div>

                    <div className="detail-section">
                        <h3>Soru Metni</h3>
                        <div className="question-text">
                            {question.soruMetni}
                        </div>
                    </div>

                    {question.kodMetni && (
                        <div className="detail-section">
                            <h3>Kod Metni</h3>
                            <pre className="code-block">
                                {question.kodMetni}
                            </pre>
                        </div>
                    )}

                    {question.secenekler && question.secenekler.length > 0 && (
                        <div className="detail-section">
                            <h3>Seçenekler</h3>
                            <div className="options-list">
                                {question.secenekler.map((secenek, index) => (
                                    <div
                                        key={secenek.id}
                                        className={`option-item ${question.dogruCevap && secenek.id === question.dogruCevap.id ? 'correct' : ''
                                            }`}
                                    >
                                        <span className="option-number">{index + 1}.</span>
                                        <span className="option-text">{secenek.secenekMetni}</span>
                                        {question.dogruCevap && secenek.id === question.dogruCevap.id && (
                                            <span className="correct-badge">✓ Doğru Cevap</span>
                                        )}
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}

                    {question.fonksiyonCozumler && question.fonksiyonCozumler.length > 0 && (
                        <div className="detail-section">
                            <h3>Fonksiyon Çözümleri</h3>
                            {question.fonksiyonCozumler.map((cozum, index) => (
                                <div key={cozum.id} className="solution-item">
                                    <h4>Çözüm {index + 1}</h4>
                                    <pre className="code-block">
                                        {cozum.cozumKod}
                                    </pre>
                                </div>
                            ))}
                        </div>
                    )}

                    {question.kelimeBloklar && question.kelimeBloklar.length > 0 && (
                        <div className="detail-section">
                            <h3>Kelime Blokları</h3>
                            {question.kelimeBloklar.map((blok, index) => (
                                <div key={blok.id} className="word-block-item">
                                    <h4>Blok {index + 1}</h4>
                                    <pre className="code-block">
                                        {blok.dogruKod}
                                    </pre>
                                </div>
                            ))}
                        </div>
                    )}

                    {question.ekVeriJson && (
                        <div className="detail-section">
                            <h3>Ek Veri</h3>
                            <pre className="code-block">
                                {JSON.stringify(JSON.parse(question.ekVeriJson), null, 2)}
                            </pre>
                        </div>
                    )}
                </div>

                <div className="modal-actions">
                    <button type="button" className="btn-secondary" onClick={onClose}>
                        Kapat
                    </button>
                </div>
            </div>
        </div>
    );
};

export default QuestionDetailModal;