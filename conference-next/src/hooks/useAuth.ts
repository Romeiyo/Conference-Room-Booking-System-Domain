"use client";

import { useState, useEffect, useCallback, useRef } from 'react';
import authService from '@/services/authService';
import { useRouter } from 'next/navigation';

interface User {
    username: string;
    userId: string;
    expires: string;
}

interface Credentials {
    username: string;
    password: string;
}

export function useAuth() {
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const router = useRouter();
    const initialCheckDone = useRef(false);

    useEffect(() => {
        if (initialCheckDone.current) return;
        
        console.log('🔍 Checking for existing token...');
        const token = localStorage.getItem('token');
        const userDataString = localStorage.getItem('user');
        
        if (token && userDataString) {
            try {
                const parsedUser = JSON.parse(userDataString) as User;
                console.log('✅ Parsed user data:', parsedUser);
                
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

    const login = async (credentials: Credentials) => {
        try {
            setIsLoading(true);
            setError(null);
            
            console.log('📤 Sending login request:', credentials);
            const response = await authService.login(credentials);
            console.log('📥 Login response:', response);
            
            if (response && response.token) {
                console.log('✅ Token received, storing...');
                
                localStorage.setItem('token', response.token);
                
                const userData: User = {
                    username: response.username,
                    userId: response.userId,
                    expires: response.expires
                };
                
                localStorage.setItem('user', JSON.stringify(userData));
                console.log('✅ User data stored:', userData);

                setUser(userData);
                setIsAuthenticated(true);
                
                console.log('🚀 Navigation will happen in component');
                
                return response;
            } else {
                console.error('❌ No token in response:', response);
                throw new Error('Invalid response from server');
            }
        } catch (err: any) {
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
        router.push('/login');
    }, [router]);

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