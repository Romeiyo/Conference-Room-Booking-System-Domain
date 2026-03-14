import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosError } from 'axios';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;
console.log('API Client initialized with URL:', API_BASE_URL);
console.log('Environment:', process.env.NODE_ENV);

const apiClient: AxiosInstance = axios.create({
    baseURL: API_BASE_URL,
    timeout: 5000,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true,
});

// Request interceptor with full traceability
apiClient.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
        // Generate request ID for traceability
        const requestId = Math.random().toString(36).substring(7);
        config.headers['X-Request-ID'] = requestId;
        
        console.log(`[${requestId}] ${config.method?.toUpperCase()} to ${config.url}`);
        
        if (config.data) {
            console.log(`[${requestId}] Request data:`, config.data);
        }
        
        if (typeof window !== 'undefined') {
            const token = localStorage.getItem('token');
            if (token) {
                config.headers.Authorization = `Bearer ${token}`;
                console.log(`[${requestId}] Authorization header added`);
            }
        }

        return config;
    },
    (error: AxiosError) => {
        console.error('Request interceptor error:', error);
        return Promise.reject(error);
    }
);

// Response interceptor with full traceability
apiClient.interceptors.response.use(
    (response) => {
        const requestId = response.config.headers['X-Request-ID'];
        console.log(`[${requestId}] Response received:`, {
            status: response.status,
            statusText: response.statusText,
            dataSize: JSON.stringify(response.data).length
        });
        
        // Log the full request chain for audit purposes
        console.log(`Full request chain:`, {
            url: response.config.url,
            method: response.config.method,
            baseURL: response.config.baseURL,
            timeout: response.config.timeout,
            withCredentials: response.config.withCredentials
        });
        
        return response.data;
    },
    (error: AxiosError) => {
        const requestId = error.config?.headers?.['X-Request-ID'] || 'unknown';
        
        if (axios.isCancel(error)) {
            console.log(`[${requestId}] Request cancelled:`, error.message);
            return Promise.reject(error);
        }

        // Detailed error logging for audit
        console.error(`[${requestId}] Request failed:`, {
            message: error.message,
            code: error.code,
            status: error.response?.status,
            statusText: error.response?.statusText,
            data: error.response?.data,
            config: {
                url: error.config?.url,
                method: error.config?.method,
                baseURL: error.config?.baseURL,
                timeout: error.config?.timeout
            }
        });

        if (error.response && error.response.status === 401) {
            console.error(`[${requestId}] Unauthorized access`);
            
            if (typeof window !== 'undefined') {
                localStorage.removeItem('token');
                localStorage.removeItem('user');
                window.dispatchEvent(new Event('unauthorized'));
                console.log('Session expired. User logged out.');
            }
        }

        if (error.code === 'ECONNABORTED') {
            console.error(`[${requestId}] Request timeout`);
        } else if (error.response) {
            console.error(`[${requestId}] Server error ${error.response.status}:`, error.response.data);
        } else if (error.request) {
            console.error(`[${requestId}] Network error - no response received`);
        } else {
            console.error(`[${requestId}] Error:`, error.message);
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