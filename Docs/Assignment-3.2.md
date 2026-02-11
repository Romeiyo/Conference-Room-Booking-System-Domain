## 1. Why is removing a column more dangerous than adding one?
Removing a column is more dangerous that adding one because that column 
might have data and one that column is removed, so is the data, this could result in 
error from viewing information that may depend on that data, whereas adding columns does 
not result in loss of data because null values are created for that column.

## 2. Why are migrations preferred over manual SQL changes?
Migrations are preferred over manual over SQL changes because it every change is 
documented and you can see who made the migration via git, if changes are made manually 
there is no audit trail to view who made SQL changes and its more complex to go back 
to a last working migration if changes caused errors.

## 3. What could go wrong if two developers modify the schema without migrations?
Developers modifying the schema without migrations will risk breaking the entire 
system because changes made could cause irreversable damage.

## 4. Which of your schema changes would be risky in production, and why?
During my creation of the columns for conference rooms, I could not add new columns 
because I had data in my bookings already, so to fix that I deleted all of the data in my bookings 
in order to allow me to add my columns without conflicts, In production this would be risky because 
that data I deleted would've been necessary for future and past records and it would cause chaos 
for people who had already made room bookings 