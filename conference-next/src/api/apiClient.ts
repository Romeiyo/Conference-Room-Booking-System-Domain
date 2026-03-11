import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosError } from 'axios';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;
console.log('API URL:', API_BASE_URL);

const apiClient: AxiosInstance = axios.create({
    baseURL: API_BASE_URL,
    timeout: 5000,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true,
});

apiClient.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
        console.log(`Sending ${config.method?.toUpperCase()} to ${config.url}`);
        
        if (typeof window !== 'undefined') {
            const token = localStorage.getItem('token');
            if (token) {
                config.headers.Authorization = `Bearer ${token}`;
            }
        }

        return config;
    },
    (error: AxiosError) => Promise.reject(error)
);

apiClient.interceptors.response.use(
    (response) => {
        return response.data;
    },
    (error: AxiosError) => {
        if (axios.isCancel(error)) {
            console.log('Request cancelled:', error.message);
            return Promise.reject(error);
        }

        if (error.response && error.response.status === 401) {
            console.error('Unauthorized access - redirecting to login');
            
            if (typeof window !== 'undefined') {
                localStorage.removeItem('token');
                localStorage.removeItem('user');
                
                window.dispatchEvent(new Event('unauthorized'));
            }
        }

        if (error.code === 'ECONNABORTED') {
            console.error('Request timeout:', error.message);
        } else if (error.response) {
            console.error(`Server error ${error.response.status}:`, error.response.data);
        } else if (error.request) {
            console.error('Network error - no response received:', error.message);
        } else {
            console.error('Error:', error.message);
        }
        
        return Promise.reject(error);
    }
);

const apiClientWrapper = {
    get: <T>(url: string, config?: any): Promise<T> => 
        apiClient.get(url, config) as Promise<T>,
    
    post: <T>(url: string, data?: any, config?: any): Promise<T> => 
        apiClient.post(url, data, config) as Promise<T>,
    
    put: <T>(url: string, data?: any, config?: any): Promise<T> => 
        apiClient.put(url, data, config) as Promise<T>,
    
    delete: <T>(url: string, config?: any): Promise<T> => 
        apiClient.delete(url, config) as Promise<T>,
};

export default apiClientWrapper;