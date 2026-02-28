SELECT "Id",
       "RoomId",
       "UserId",
       "StartTime",
       "EndTime",
       "Status",
       "Capacity",
       "CreatedAt",
       "CancelledAt"
FROM public."Bookings"
LIMIT 1000;