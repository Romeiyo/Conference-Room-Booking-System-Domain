import apiClient from "../api/apiClient";

const authService = {
  // Only need this one function for auto-login!
  async login(credentials) {
    try {
      const response = await apiClient.post('/auth/login', credentials);
      return response.data;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  }
};

export default authService;