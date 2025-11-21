import axios from 'axios';

const API_BASE_URL = 'https://localhost:7140/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
});

export const questionService = {
    getAll: async () => {
        try {
            const response = await api.get('/soru');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Sorular alınamadı' };
        }
    },

    getById: async (id) => {
        try {
            const response = await api.get(`/soru/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Soru bulunamadı' };
        }
    },

    getByLesson: async (dersId) => {
        try {
            const response = await api.get(`/soru/ders/${dersId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Derse ait sorular alınamadı' };
        }
    },

    create: async (questionData) => {
        try {
            const response = await api.post('/soru', questionData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Soru oluşturulamadı' };
        }
    },

    update: async (id, questionData) => {
        try {
            const response = await api.put(`/soru/${id}`, questionData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Soru güncellenemedi' };
        }
    },

    delete: async (id) => {
        try {
            const response = await api.delete(`/soru/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Soru silinemedi' };
        }
    },

    // Soru seçenekleri
    getOptions: async (soruId) => {
        try {
            const response = await api.get(`/sorucecenek/soru/${soruId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Seçenekler alınamadı' };
        }
    },

    createOption: async (optionData) => {
        try {
            const response = await api.post('/sorucecenek', optionData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Seçenek oluşturulamadı' };
        }
    },

    updateOption: async (id, optionData) => {
        try {
            const response = await api.put(`/sorucecenek/${id}`, optionData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Seçenek güncellenemedi' };
        }
    },

    deleteOption: async (id) => {
        try {
            const response = await api.delete(`/sorucecenek/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Seçenek silinemedi' };
        }
    },

    // Fonksiyon çözümleri
    getFunctionSolutions: async (soruId) => {
        try {
            const response = await api.get(`/sorufonksiyoncozum/soru/${soruId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Fonksiyon çözümleri alınamadı' };
        }
    },

    createFunctionSolution: async (solutionData) => {
        try {
            const response = await api.post('/sorufonksiyoncozum', solutionData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Fonksiyon çözümü oluşturulamadı' };
        }
    },

    updateFunctionSolution: async (id, solutionData) => {
        try {
            const response = await api.put(`/sorufonksiyoncozum/${id}`, solutionData);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Fonksiyon çözümü güncellenemedi' };
        }
    },

    deleteFunctionSolution: async (id) => {
        try {
            const response = await api.delete(`/sorufonksiyoncozum/${id}`);
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Fonksiyon çözümü silinemedi' };
        }
    }
};

export default questionService;