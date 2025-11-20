import axios from 'axios';

const API_BASE_URL = 'https://localhost:7140/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
});

export const adminService = {
    getDashboard: async () => {
        try {
            const response = await api.get('/admin/dashboard');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Dashboard verileri alınamadı' };
        }
    },

    getHardestQuestions: async () => {
        try {
            const response = await api.get('/admin/dashboard/sorular/en-zor');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Zor sorular alınamadı' };
        }
    },

    getMostFollowedLessons: async () => {
        try {
            const response = await api.get('/admin/dashboard/dersler/en-populer');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Popüler dersler alınamadı' };
        }
    },

    getLanguageDetails: async (id) => {
        try {
            const response = await api.get(`/admin/dashboard/dil/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Dil detayları alınamadı' };
        }
    },

    getWeeklyQuestionActivity: async () => {
        try {
            const response = await api.get('/admin/dashboard/grafik/haftalik-soru');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Haftalık veriler alınamadı' };
        }
    }
};

export default adminService;