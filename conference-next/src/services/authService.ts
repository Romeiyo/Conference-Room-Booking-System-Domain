import apiClient from "@/api/apiClient";

interface Credentials {
    username: string;
    password: string;
}

interface LoginResponse {
    token: string;
    username: string;
    userId: string;
    expires: string;
}

const authService = {
    async login(credentials: Credentials): Promise<LoginResponse> {
        try {
            const response = await apiClient.post<LoginResponse>('/auth/login', credentials);
            
            return response;
        } catch (error) {
            console.error('Login error:', error);
            throw error;
        }
    }
};

export default authService;