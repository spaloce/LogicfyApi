import axios from 'axios';

const API_BASE_URL = 'https://localhost:7140/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
});

export const fileUploadService = {
    uploadImage: async (file) => {
        try {
            const formData = new FormData();
            formData.append('file', file);

            const response = await api.post('/file/upload', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });

            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Resim yüklenirken bir hata oluştu' };
        }
    },

    deleteImage: async (fileUrl) => {
        try {
            const response = await api.delete('/file/delete', {
                data: { fileUrl }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || { message: 'Resim silinirken bir hata oluştu' };
        }
    }
};

export default fileUploadService;