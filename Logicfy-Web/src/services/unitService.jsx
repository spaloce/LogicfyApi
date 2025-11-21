import axios from 'axios';

const API_BASE_URL = 'https://localhost:7140/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
});

export const unitService = {
    getAll: async () => {
        try {
            const response = await api.get('/unite');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Üniteler alınamadı' };
        }
    },

    getById: async (id) => {
        try {
            const response = await api.get(`/unite/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ünite bulunamadı' };
        }
    },

    getDetail: async (id) => {
        try {
            const response = await api.get(`/unite/${id}/detay`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ünite detayları alınamadı' };
        }
    },

    getByLanguage: async (dilId) => {
        try {
            const response = await api.get(`/unite/dil/${dilId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Dile ait üniteler alınamadı' };
        }
    },

    create: async (unitData) => {
        try {
            const response = await api.post('/unite', unitData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ünite oluşturulamadı' };
        }
    },

    update: async (id, unitData) => {
        try {
            const response = await api.put(`/unite/${id}`, unitData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ünite güncellenemedi' };
        }
    },

    delete: async (id) => {
        try {
            const response = await api.delete(`/unite/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Ünite silinemedi' };
        }
    },

    search: async (query) => {
        try {
            const response = await api.get(`/unite/ara/${query}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Arama yapılamadı' };
        }
    }
};

export default unitService;