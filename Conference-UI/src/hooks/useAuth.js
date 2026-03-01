import { useState, useEffect, useCallback, useRef } from 'react';
import authService from '../Services/authService';
import { useNavigate } from 'react-router-dom';

export function useAuth() {
    const [user, setUser] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const navigate = useNavigate();
    const initialCheckDone = useRef(false);

    // Check for existing token on mount - only once
    useEffect(() => {
        if (initialCheckDone.current) return;
        
        console.log('🔍 Checking for existing token...');
        const token = localStorage.getItem('token');
        const userDataString = localStorage.getItem('user');
        
        if (token && userDataString) {
            try {
                const parsedUser = JSON.parse(userDataString);
                console.log('✅ Parsed user data:', parsedUser);
                
                // Validate that parsedUser has the expected structure
                if (parsedUser && parsedUser.username) {
                    setUser(parsedUser);
                    setIsAuthenticated(true);
                    console.log('✅ User authenticated from localStorage:', parsedUser);
                } else {
                    console.error('❌ Invalid user data structure:', parsedUser);
                    localStorage.removeItem('token');
                    localStorage.removeItem('user');
                }
            } catch (e) {
                console.error('❌ Failed to parse user data:', e);
                localStorage.removeItem('token');
                localStorage.removeItem('user');
            }
        } else {
            console.log('ℹ️ No existing token found');
        }
        
        setIsLoading(false);
        initialCheckDone.current = true;
    }, []);

    const login = async (credentials) => {
        try {
            setIsLoading(true);
            setError(null);
            
            console.log('📤 Sending login request:', credentials);
            const response = await authService.login(credentials);
            console.log('📥 Login response:', response);
            
            if (response && response.token) {
                console.log('✅ Token received, storing...');
                
                // Store token
                localStorage.setItem('token', response.token);
                
                // Create user data object
                const userData = {
                    username: response.username,
                    userId: response.userId,
                    expires: response.expires
                };
                
                // Store user data as JSON string
                localStorage.setItem('user', JSON.stringify(userData));
                console.log('✅ User data stored:', userData);

                // Update state
                setUser(userData);
                setIsAuthenticated(true);
                
                console.log('🚀 Navigating to home page...');
                
                // Navigate after state updates
                setTimeout(() => {
                    navigate('/', { replace: true });
                }, 100);
                
                return response;
            } else {
                console.error('❌ No token in response:', response);
                throw new Error('Invalid response from server');
            }
        } catch (err) {
            console.error('❌ Login error:', err);
            const errorMessage = err.response?.data?.message || 
                                err.message || 
                                'Login failed. Please check your credentials.';
            setError(errorMessage);
            throw err;
        } finally {
            setIsLoading(false);
        }
    };

    const logout = useCallback(() => {
        console.log('🚪 Logging out...');
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        setUser(null);
        setIsAuthenticated(false);
        navigate('/login', { replace: true });
    }, [navigate]);

    // Auto-logout on 401 responses
    useEffect(() => {
        const handleUnauthorized = () => {
            console.log('🔒 Unauthorized event received');
            if (isAuthenticated) {
                logout();
            }
        };

        window.addEventListener('unauthorized', handleUnauthorized);
        
        return () => {
            window.removeEventListener('unauthorized', handleUnauthorized);
        };
    }, [isAuthenticated, logout]);

    return {
        user,
        isLoading,
        error,
        isAuthenticated,
        login,
        logout
    };
}

export default useAuth;