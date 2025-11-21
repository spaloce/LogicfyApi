import axios from 'axios';

const API_BASE_URL = 'https://localhost:7140/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
});

export const programmingLanguageService = {
    getAll: async () => {
        try {
            const response = await api.get('/programlamadili');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Diller alınamadı' };
        }
    },

    getById: async (id) => {
        try {
            const response = await api.get(`/programlamadili/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Dil bulunamadı' };
        }
    },

    create: async (languageData) => {
        try {
            const response = await api.post('/programlamadili', languageData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Dil oluşturulamadı' };
        }
    },

    update: async (id, languageData) => {
        try {
            const response = await api.put(`/programlamadili/${id}`, languageData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Dil güncellenemedi' };
        }
    },

    updateStatus: async (id, aktif) => {
        try {
            const response = await api.put(`/programlamadili/${id}/durum/${aktif}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Durum güncellenemedi' };
        }
    },

    delete: async (id) => {
        try {
            const response = await api.delete(`/programlamadili/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Dil silinemedi' };
        }
    },

    search: async (query) => {
        try {
            const response = await api.get(`/programlamadili/ara/${query}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Arama yapılamadı' };
        }
    }
};

export default programmingLanguageService;