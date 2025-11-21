import React, { useState } from 'react';
import './DockMenu.css';

const DockMenu = ({ onMenuSelect, activeMenu }) => {
    const [hoveredMenu, setHoveredMenu] = useState(null);

    const menuItems = [
        {
            id: 'dashboard',
            icon: '📊',
            label: 'Dashboard',
            description: 'Genel bakış'
        },
        {
            id: 'languages',
            icon: '💻',
            label: 'Programlama Dilleri',
            description: 'Dil yönetimi'
        },
        {
            id: 'sections',
            icon: '📚',
            label: 'Kısımlar',
            description: 'Bölüm yönetimi'
        },
        {
            id: 'units',
            icon: '📂',
            label: 'Üniteler',
            description: 'Ünite yönetimi'
        },
        {
            id: 'lessons',
            icon: '📖',
            label: 'Dersler',
            description: 'Ders yönetimi'
        },
        {
            id: 'sectioncontents',
            icon: '📖',
            label: 'Ders İçerikleri',
            description: 'Ders Öğrenim İçerikleri'
        },
        {
            id: 'questions',
            icon: '❓',
            label: 'Sorular',
            description: 'Soru bankası'
        },
        {
            id: 'users',
            icon: '👥',
            label: 'Kullanıcı İstatistikleri',
            description: 'Kullanıcı analizleri'
        }

    ];

    const handleMouseEnter = (menuId) => {
        setHoveredMenu(menuId);
    };

    const handleMouseLeave = () => {
        setHoveredMenu(null);
    };

    const handleMenuClick = (menuId) => {
        onMenuSelect(menuId);
    };

    return (
        <div className="dock-container">
            <nav className="dock-menu">
                {menuItems.map((item) => (
                    <div
                        key={item.id}
                        className={`dock-item ${activeMenu === item.id ? 'active' : ''} ${hoveredMenu === item.id ? 'hovered' : ''
                            } ${hoveredMenu && hoveredMenu !== item.id ? 'inactive' : ''}`}
                        onMouseEnter={() => handleMouseEnter(item.id)}
                        onMouseLeave={handleMouseLeave}
                        onClick={() => handleMenuClick(item.id)}
                    >
                        <div className="dock-icon">{item.icon}</div>
                        <div className="dock-label">
                            <span className="label-text">{item.label}</span>
                            <span className="label-description">{item.description}</span>
                        </div>
                    </div>
                ))}
            </nav>
        </div>
    );
};

export default DockMenu;