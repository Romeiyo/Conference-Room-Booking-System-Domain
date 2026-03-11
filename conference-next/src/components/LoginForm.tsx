"use client";

import { useState } from 'react';
import { useAuth } from '@/hooks/useAuth';
import { useRouter } from 'next/navigation';
import '@/app/globals.css';

export default function LoginForm() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [formError, setFormError] = useState('');
    const { login, isLoading, error } = useAuth();
    const router = useRouter();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setFormError('');
        
        if (!username || !password) {
            setFormError('Please fill in all fields');
            return;
        }

        try {
            await login({ username, password });
            router.push('/dashboard'); // Navigate after login
        } catch (err) {
            console.error('Login failed:', err);
        }
    };

    return (
        <div className="login-container">
            <form className="login-form" onSubmit={handleSubmit}>
                <h2>Login to Conference Room Booking</h2>
                
                {(formError || error) && (
                    <div className="error-message">
                        {formError || error}
                    </div>
                )}

                <div className="form-group">
                    <label htmlFor="username">Username:</label>
                    <input
                        type="text"
                        id="username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        disabled={isLoading}
                        placeholder="Enter username"
                        required
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="password">Password:</label>
                    <input
                        type="password"
                        id="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        disabled={isLoading}
                        placeholder="Enter password"
                        required
                    />
                </div>

                <button 
                    type="submit" 
                    className="btn btn-primary"
                    disabled={isLoading}
                >
                    {isLoading ? 'Logging in...' : 'Login'}
                </button>

                <div className="login-info">
                    <p>Demo credentials:</p>
                    <p>Username: Admin / Employee1 / Employee2</p>
                    <p>Password: Admin123! / Password1! / Password2!</p>
                </div>
            </form>
        </div>
    );
}