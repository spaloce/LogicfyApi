import axios from 'axios';

const API_BASE_URL = 'https://localhost:7140/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
});

export const sectionContentService = {
    getAll: async () => {
        try {
            const response = await api.get('/admin/kisimicerik');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'İçerikler alınamadı' };
        }
    },

    getById: async (id) => {
        try {
            const response = await api.get(`/admin/kisimicerik/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'İçerik bulunamadı' };
        }
    },

    create: async (contentData) => {
        try {
            const response = await api.post('/admin/kisimicerik', contentData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'İçerik oluşturulamadı' };
        }
    },

    update: async (id, contentData) => {
        try {
            const response = await api.put(`/admin/kisimicerik/${id}`, contentData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'İçerik güncellenemedi' };
        }
    },

    delete: async (id) => {
        try {
            const response = await api.delete(`/admin/kisimicerik/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'İçerik silinemedi' };
        }
    }
};

export default sectionContentService;