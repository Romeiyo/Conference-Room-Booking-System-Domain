import apiClient from '../api/apiClient';

export const authService = {
  async login(username: string, password: string) {
    return await apiClient.post('/auth/login', { username, password });
  }
};