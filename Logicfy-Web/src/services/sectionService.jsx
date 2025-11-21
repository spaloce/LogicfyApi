import axios from 'axios';

const API_BASE_URL = 'https://localhost:7140/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
});

export const sectionService = {
    getAll: async () => {
        try {
            const response = await api.get('/kisim');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Kısımlar alınamadı' };
        }
    },

    getById: async (id) => {
        try {
            const response = await api.get(`/kisim/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Kısım bulunamadı' };
        }
    },

    getByUnit: async (uniteId) => {
        try {
            const response = await api.get(`/kisim/unite/${uniteId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Üniteye ait kısımlar alınamadı' };
        }
    },

    getDetail: async (id) => {
        try {
            const response = await api.get(`/kisim/${id}/detay`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Kısım detayları alınamadı' };
        }
    },

    getStatistics: async (id) => {
        try {
            const response = await api.get(`/kisim/${id}/istatistik`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Kısım istatistikleri alınamadı' };
        }
    },

    getLessons: async (id) => {
        try {
            const response = await api.get(`/kisim/${id}/dersler`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Kısım dersleri alınamadı' };
        }
    },

    create: async (sectionData) => {
        try {
            const response = await api.post('/kisim', sectionData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Kısım oluşturulamadı' };
        }
    },

    update: async (id, sectionData) => {
        try {
            const response = await api.put(`/kisim/${id}`, sectionData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Kısım güncellenemedi' };
        }
    },

    delete: async (id) => {
        try {
            const response = await api.delete(`/kisim/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Kısım silinemedi' };
        }
    },

    search: async (query) => {
        try {
            const response = await api.get(`/kisim/ara/${query}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Arama yapılamadı' };
        }
    },

    updateLessonCache: async (id) => {
        try {
            const response = await api.put(`/kisim/${id}/update-ders-cache`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ders cache güncellenemedi' };
        }
    },

    advancedSearch: async (baslik, uniteId) => {
        try {
            const params = new URLSearchParams();
            if (baslik) params.append('baslik', baslik);
            if (uniteId) params.append('uniteId', uniteId);

            const response = await api.get(`/kisim/advanced-arama?${params}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Gelişmiş arama yapılamadı' };
        }
    }
};

export default sectionService;