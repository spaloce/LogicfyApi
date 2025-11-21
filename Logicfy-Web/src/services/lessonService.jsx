import axios from 'axios';

const API_BASE_URL = 'https://localhost:7140/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
});

export const lessonService = {
    getAll: async () => {
        try {
            const response = await api.get('/ders');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Dersler alınamadı' };
        }
    },

    getById: async (id) => {
        try {
            const response = await api.get(`/ders/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ders bulunamadı' };
        }
    },

    getBySection: async (kisimId) => {
        try {
            const response = await api.get(`/ders/kisim/${kisimId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Kısıma ait dersler alınamadı' };
        }
    },

    getStatistics: async (id) => {
        try {
            const response = await api.get(`/ders/${id}/istatistik`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ders istatistikleri alınamadı' };
        }
    },

    getByDifficulty: async (zorlukSeviyesi) => {
        try {
            const response = await api.get(`/ders/zorluk/${zorlukSeviyesi}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Zorluk seviyesine göre dersler alınamadı' };
        }
    },

    getByDuration: async (maxSure) => {
        try {
            const response = await api.get(`/ders/sureli/${maxSure}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Süreye göre dersler alınamadı' };
        }
    },

    create: async (lessonData) => {
        try {
            const response = await api.post('/ders', lessonData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ders oluşturulamadı' };
        }
    },

    update: async (id, lessonData) => {
        try {
            const response = await api.put(`/ders/${id}`, lessonData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ders güncellenemedi' };
        }
    },

    delete: async (id) => {
        try {
            const response = await api.delete(`/ders/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ders silinemedi' };
        }
    },

    search: async (query) => {
        try {
            const response = await api.get(`/ders/ara/${query}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Arama yapılamadı' };
        }
    },

    updateQuestionCache: async (id) => {
        try {
            const response = await api.put(`/ders/${id}/update-soru-cache`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Soru cache güncellenemedi' };
        }
    }
};

export default lessonService;