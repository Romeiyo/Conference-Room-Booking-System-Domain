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
            <input
                type = "text"
                placeholder="Room Name"
                value={roomName}
                onChange={(e) => setName(e.target.value)}
                required
            />

            <input 
                type="date" 
                value={formatDateForInput(date)}
                onChange={(e) => setDate(new Date(e.target.value))}
                required
            />

            <label>Start Time:</label>
            <input 
                type="time" 
                value={startTime}
                onChange={handleStart}
                required
            />

            <label>End Time:</label>
            <input 
                type="time" 
                value={endTime}
                onChange={handleEnd}
                required
            />

            <select value={location} onChange={(e) => setLocation(e.target.value)}>
                <option value="Bloemfontein">Bloemfontein</option>
                <option value="Cape Town">Cape Town</option>
            </select>

            <input
                type="text"
                value={bookedBy}
                onChange={(e) => setBookedBy(e.target.value)}
                placeholder="Booked By"
                required
            />

            <Button label="Book Room"/>
        </form>      
    );
}

export default BookingForm;