import { useState } from "react";
import Button from "./Button";
import '../App.css';

function BookingForm({ onAddBooking }) {

    const [roomName, setName] = useState("");
    const [date, setDate] = useState(new Date());
    const [startTime, setStartTime] = useState("");
    const [endTime, setEndTime] = useState("");
    const [location, setLocation] = useState("Bloemfontein");
    const [bookedBy, setBookedBy] = useState("");

    const formatDateForInput = (dateObject) => {
        const year = dateObject.getFullYear();
        const month = (dateObject.getMonth() + 1).toString().padStart(2, '0');
        const day = dateObject.getDate().toString().padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    const handleStart = (event) => {
        setStartTime(event.target.value);
    } 

    const handleEnd = (event) => {
        setEndTime(event.target.value);
    }

    const handleSubmit = (e) => {
        e.preventDefault();

        const newBooking = {
            id: Date.now(),
            roomName,
            date: formatDateForInput(date),
            startTime,
            endTime,
            location,
            bookedBy
        }

        onAddBooking(newBooking);

        // Clear form
        setName("");
        setDate(new Date());
        setStartTime("");
        setEndTime("");
        setLocation("");
        setBookedBy("");
    };

    return (
        <form className="booking-form" onSubmit={handleSubmit}>
            <h3 style={{ marginBottom: '1.5rem', color: '#444', borderBottom: '2px solid #667eea', paddingBottom: '0.5rem' }}>
                Book a New Room
            </h3>

            <div className="form-row">
                <label>Room Name:</label>
                <select 
                    id="roomName"
                    value={roomName} 
                    onChange={(e) => setName(e.target.value)}
                >
                    <option value="Room A">Room A</option>
                    <option value="Room B">Room B</option>
                    <option value="Room C">Room C</option>
                    <option value="Room D">Room D</option>
                    <option value="Room E">Room E</option>
                    <option value="Room F">Room F</option>
                    <option value="Room G">Room G</option>
                    
                </select>
            </div>

            <div className="form-row">
                <label>Date:</label>
                <input 
                    id="date"
                    type="date" 
                    value={formatDateForInput(date)}
                    onChange={(e) => setDate(new Date(e.target.value))}
                    required
                />
            </div>

            <div className="form-row">
                <label>Start Time:</label>
                <input 
                    id="startTime"
                    type="time" 
                    value={startTime}
                    onChange={(e) => setStartTime(e.target.value)}
                    required
                />
            </div>

            <div className="form-row">
                <label>End Time:</label>
                <input 
                    id="endTime"
                    type="time" 
                    value={endTime}
                    onChange={(e) => setEndTime(e.target.value)}
                    required
                />
            </div>

            <div className="form-row">
                <label>Location:</label>
                <select 
                    id="location"
                    value={location} 
                    onChange={(e) => setLocation(e.target.value)}
                >
                    <option value="Bloemfontein">Bloemfontein</option>
                    <option value="Cape Town">Cape Town</option>
                </select>
            </div>

            <div className="form-row">
                <label>Booked By:</label>
                <input
                    id="bookedBy"
                    type="text"
                    value={bookedBy}
                    onChange={(e) => setBookedBy(e.target.value)}
                    placeholder="Enter your name"
                    required
                />
            </div>

            <div className="form-row">
                <Button label="Book Room"/>
            </div>
        </form>       
    );
}

export default BookingForm;