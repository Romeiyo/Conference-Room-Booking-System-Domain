import initialBookings from "../mockData";

export const fetchAllBookings = () => {
    return new Promise((resolve, reject) => {
    
    const delay = Math.random() * 2000 + 500;

    const shouldFail = Math.random() < 0.2;

    setTimeout(() => {
        if(shouldFail){
            reject(new Error('Failed to fetch bookings'));
        }
        else{
            resolve(initialBookings);
        }
    }, delay);
  });
};