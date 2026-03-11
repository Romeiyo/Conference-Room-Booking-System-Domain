"use client";

import { createContext, useState, useEffect, ReactNode, useContext } from 'react';
import { useRouter } from 'next/navigation';
import authService from '@/services/authService';

// Define the User type
interface User {
    username: string;
    userId: string;
    expires: string;
}

// Define the credentials type
interface Credentials {
    username: string;
    password: string;
}

// Define the shape of our Auth Context
interface AuthContextType {
    user: User | null;
    isLoading: boolean;
    error: string | null;
    isAuthenticated: boolean;
    login: (credentials: Credentials) => Promise<any>;
    logout: () => void;
}

// Create the Context with a default value
const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Provider component
export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const router = useRouter();

    // Check for existing token on mount (hydration)
    useEffect(() => {
        console.log('AuthProvider: Checking for existing token...');
        const token = localStorage.getItem('token');
        const userDataString = localStorage.getItem('user');
        
        if (token && userDataString) {
            try {
                const parsedUser = JSON.parse(userDataString) as User;
                console.log('AuthProvider: User restored from localStorage:', parsedUser);
                setUser(parsedUser);
            } catch (e) {
                console.error('AuthProvider: Failed to parse user data:', e);
                // Clear invalid data
                localStorage.removeItem('token');
                localStorage.removeItem('user');
            }
        } else {
            console.log('AuthProvider: No existing token found');
        }
        
        setIsLoading(false);
    }, []);

    // Login function
    const login = async (credentials: Credentials) => {
        try {
            setIsLoading(true);
            setError(null);
            
            console.log('AuthProvider: Sending login request');
            const response = await authService.login(credentials);
            console.log('AuthProvider: Login response:', response);
            
            if (response && response.token) {
                console.log('AuthProvider: Token received, storing...');
                
                // Store in localStorage
                localStorage.setItem('token', response.token);
                
                const userData: User = {
                    username: response.username,
                    userId: response.userId,
                    expires: response.expires
                };
                
                localStorage.setItem('user', JSON.stringify(userData));
                console.log('AuthProvider: User data stored:', userData);

                // Update state
                setUser(userData);
                
                return response;
            } else {
                console.error('AuthProvider: No token in response');
                throw new Error('Invalid response from server');
            }
        } catch (err: any) {
            console.error('AuthProvider: Login error:', err);
            const errorMessage = err.response?.data?.message || 
                                err.message || 
                                'Login failed. Please check your credentials.';
            setError(errorMessage);
            throw err;
        } finally {
            setIsLoading(false);
        }
    };

    // Logout function
    const logout = () => {
        console.log('AuthProvider: Logging out...');
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        setUser(null);
        router.push('/login');
    };

    // Handle 401 unauthorized events (for extra credit)
    useEffect(() => {
        const handleUnauthorized = () => {
            console.log('AuthProvider: Unauthorized event received');
            if (user) {
                logout();
            }
        };

        window.addEventListener('unauthorized', handleUnauthorized);
        
        return () => {
            window.removeEventListener('unauthorized', handleUnauthorized);
        };
    }, [user]);

    // Value object that will be provided to consumers
    const value = {
        user,
        isLoading,
        error,
        isAuthenticated: !!user,
        login,
        logout
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}

// Custom hook to use the Auth Context
export function useAuth() {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
}