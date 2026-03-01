import axios from 'axios';

// Read the base URL from environment variable
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;
console.log('API URL:', API_BASE_URL);

// Create axios instance with default config
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 5000,  //its timing out after 5 seconds
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // Important for CORS with credentials
});

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  (config) => {
    //outgoing request logged
    console.log(`Sending ${config.method.toUpperCase()} to ${config.url}`);
    
    //adding my auth token
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => {
    return response.data;
  },
  (error) => {
    if (axios.isCancel(error)) {
        // Log cancelled requests but don't treat as error
        console.log('Request cancelled:', error.message);
        
        return Promise.reject(error);
    }

     // Handle 401 Unauthorized errors globally
    if (error.response && error.response.status === 401) {
        console.error('Unauthorized access - redirecting to login');
        
        // Clear token
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        
        // Dispatch custom event for components to listen to
        window.dispatchEvent(new Event('unauthorized'));
        
        // Don't redirect immediately - let the event handler do it
        // This prevents redirect loops
    }

     // Log error message
    if (error.code === 'ECONNABORTED') {
      console.error('Request timeout:', error.message);
    } else if (error.response) {
      // Server responded with error status
      console.error(`Server error ${error.response.status}:`, error.response.data);
    } else if (error.request) {
      // Request made but no response received
      console.error('Network error - no response received:', error.message);
    } else {
      // Something else happened
      console.error('Error:', error.message);
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;