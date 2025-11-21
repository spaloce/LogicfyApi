import axios from 'axios';

const API_BASE_URL = 'https://localhost:7140/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true, // Cookie için önemli
});

export const authService = {
    login: async (email, password) => {
        try {
            const response = await api.post('/auth/login', {
                email,
                password
            });
            return response.data;
        } catch (error) {
            // Hata yönetimini iyileştirelim
            if (error.response?.status === 401) {
                throw { message: 'Geçersiz email veya şifre' };
            }
            throw error.response?.data || { message: 'Login işlemi başarısız' };
        }
    },

    getMe: async () => {
        try {
            const response = await api.get('/auth/me');
            return response.data;
        } catch (error) {
            // 401 hatasını normal bir durum olarak kabul edelim
            if (error.response?.status === 401) {
                throw { message: 'Oturum bulunamadı', status: 401 };
            }
            throw error.response?.data || { message: 'Kullanıcı bilgileri alınamadı' };
        }
    },

    logout: async () => {
        try {
            const response = await api.post('/auth/logout');
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Logout işlemi başarısız' };
        }
    }
};

export default authService;