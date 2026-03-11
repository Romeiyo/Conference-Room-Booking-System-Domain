"use client";

import { useState, useEffect } from "react";
import Button from "./Button";
import '@/app/globals.css';

interface BookingFormProps {
    onAddBooking: (booking: any) => Promise<any>;
    formErrors?: Record<string, string>;
    clearFormErrors?: () => void;
}

export default function BookingForm({ onAddBooking, formErrors = {}, clearFormErrors }: BookingFormProps) {
    const [roomName, setName] = useState("");
    const [date, setDate] = useState(new Date());
    const [startTime, setStartTime] = useState("");
    const [endTime, setEndTime] = useState("");
    const [location, setLocation] = useState("Bloemfontein");
    const [bookedBy, setBookedBy] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [submitError, setSubmitError] = useState("");

    useEffect(() => {
        if (clearFormErrors && Object.keys(formErrors).length > 0) {
            const timer = setTimeout(() => {
                clearFormErrors();
            }, 300);
            return () => clearTimeout(timer);
        }
    }, [roomName, date, startTime, endTime, location, bookedBy, formErrors, clearFormErrors]);

    const formatDateForInput = (dateObject: Date) => {
        const year = dateObject.getFullYear();
        const month = (dateObject.getMonth() + 1).toString().padStart(2, '0');
        const day = dateObject.getDate().toString().padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    const handleStart = (event: React.ChangeEvent<HTMLInputElement>) => {
        setStartTime(event.target.value);
    } 

    const handleEnd = (event: React.ChangeEvent<HTMLInputElement>) => {
        setEndTime(event.target.value);
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSubmitError("");

        console.log('📝 Form values:', {
            roomName,
            date: formatDateForInput(date),
            startTime,
            endTime,
            location,
            bookedBy
        });

        if (!roomName || !date || !startTime || !endTime || !location ) {
            setSubmitError("Please fill in all fields");
            return;
        }

        if (startTime >= endTime) {
            setSubmitError("End time must be after start time");
            return;
        }

        console.log('✅ Validation passed, submitting...');
        setIsSubmitting(true);

        const newBooking = {
            roomName,
            date: formatDateForInput(date),
            startTime,
            endTime,
            location,
            bookedBy
        };

        try {
            const result = await onAddBooking(newBooking);
            
            if (result && result.success) {
                setName("");
                setDate(new Date());
                setStartTime("");
                setEndTime("");
                setLocation("Bloemfontein");
                setBookedBy("");
                setSubmitError("");
                alert("Booking created successfully!");
            } else if (result && result.error) {
                if (result.error.errors) {
                    setSubmitError("Please fix the errors below");
                } else {
                    setSubmitError(result.error.message || "Failed to create booking");
                }
            }
        } catch (error) {
            setSubmitError(error instanceof Error ? error.message : "An unexpected error occurred");
        } finally {
            setIsSubmitting(false);
        }
    };

    const getFieldError = (fieldName: string) => {
        return formErrors[fieldName] ? (
            <span className="field-error">{formErrors[fieldName]}</span>
        ) : null;
    };

    return (
        <form className="booking-form" onSubmit={handleSubmit}>
            <h3 style={{ marginBottom: '1.5rem', color: '#444', borderBottom: '2px solid #667eea', paddingBottom: '0.5rem' }}>
                Book a New Room
            </h3>

            {submitError && (<div className="form-error-message">
                {submitError}
            </div>)}

            <div className="form-row">
                <label>Room Name:</label>
                <div className="field-container">
                    <select 
                        id="roomName"
                        value={roomName} 
                        onChange={(e) => setName(e.target.value)}
                        disabled={isSubmitting}
                        className={formErrors.roomName ? 'error' : ''}
                    >   
                        <option value="">Select a room</option>
                        <option value="Room A">Room A</option>
                        <option value="Room B">Room B</option>
                        <option value="Room C">Room C</option>
                        <option value="Room D">Room D</option>
                        <option value="Room E">Room E</option>
                        <option value="Room F">Room F</option>
                        <option value="Room G">Room G</option>
                        <option value="Room H">Room H</option>
                        <option value="Room I">Room I</option>
                        <option value="Room J">Room J</option>
                        <option value="Room K">Room K</option>
                        <option value="Room L">Room L</option>
                        <option value="Room M">Room M</option>
                        <option value="Room N">Room N</option>
                        <option value="Room O">Room O</option>
                        <option value="Room P">Room P</option>
                    </select>
                    {getFieldError('roomName')}
                    {getFieldError('Room')}
                </div>
            </div>

            <div className="form-row">
                <label>Date:</label>
                <div className="field-container">
                    <input 
                        id="date"
                        type="date" 
                        value={formatDateForInput(date)}
                        onChange={(e) => setDate(new Date(e.target.value))}
                        required
                        disabled={isSubmitting}
                        className={formErrors.date || formErrors.BookingDate ? 'error' : ''}
                    />
                    {getFieldError('date')}
                    {getFieldError('BookingDate')}
                </div>
            </div>

            <div className="form-row">
                <label>Start Time:</label>
                <div className="field-container">
                    <input 
                        id="startTime"
                        type="time" 
                        value={startTime}
                        onChange={(e) => setStartTime(e.target.value)}
                        required
                        disabled={isSubmitting}
                        className={formErrors.startTime || formErrors.StartTime ? 'error' : ''}
                    />
                    {getFieldError('startTime')}
                    {getFieldError('StartTime')}
                </div>
            </div>

            <div className="form-row">
                <label>End Time:</label>
                <div className="field-container">
                    <input 
                        id="endTime"
                        type="time" 
                        value={endTime}
                        onChange={(e) => setEndTime(e.target.value)}
                        required
                        disabled={isSubmitting}
                        className={formErrors.endTime || formErrors.EndTime ? 'error' : ''}
                    />
                    {getFieldError('endTime')}
                    {getFieldError('EndTime')}
                </div>
            </div>

            <div className="form-row">
                <label>Location:</label>
                <div className="field-container">
                    <select 
                        id="location"
                        value={location} 
                        onChange={(e) => setLocation(e.target.value)}
                        disabled={isSubmitting}
                        className={formErrors.location ? 'error' : ''}
                    >
                        <option value="Bloemfontein">Bloemfontein</option>
                        <option value="Cape Town">Cape Town</option>
                    </select>
                    {getFieldError('location')}
                </div>
            </div>

            {formErrors.time && (
                <div className="form-row">
                    <div className="field-container">
                        <span className="field-error">{formErrors.time}</span>
                    </div>
                </div>
            )}

            <div className="form-row">
                <Button label={isSubmitting ? "Booking..." : "Book Room"} />
            </div>
        </form>       
    );
}