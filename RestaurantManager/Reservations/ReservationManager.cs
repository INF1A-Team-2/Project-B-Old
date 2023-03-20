namespace RestaurantManager;

static class ReservationManager
{
    public static List<DateTime> GetAvailableTimes(DateTime date, int groupSize)
    { ;
        DateTime start = date.Add(Config.OpeningTime.ToTimeSpan());
        DateTime end = date.Add(Config.ClosingTime.ToTimeSpan());

        List<Reservation> reservations = Reservation.GetAllReservations()
            .Where(r => r.Time.Day == date.Day).ToList();
        
        List<DateTime> res = new List<DateTime>();
        
        for (DateTime i = start; i <= end.Subtract(Config.RoundTime); i = i.Add(Config.ReservationInterval))
        {
            List<Reservation> existingReservations = reservations.Where(
                    r => r.Time <= i || r.Time.Add(r.TotalTime()) > i).ToList();

            List<int> occupiedTables = existingReservations.Select(r => Table.GetTableByID(r.TableID).ID)
                .ToList();

            List<Table> availableTables = Table.GetAllTables()
                .Where(t => !occupiedTables.Contains(t.ID))
                .Where(t => t.Capacity >= groupSize)
                .OrderBy(t => t.Capacity)
                .ToList();

            if (availableTables.Count == 0)
                continue;

            res.Add(i);
        }

        return res;
    } 
    
    private static string GenerateCode()
    {
        List<string> existingCodes = DatabaseConnection.Execute(
                """SELECT code FROM reservations WHERE CAST(datetime as DATE) = CAST(CURDATE() as DATE)""")
            .Select(r => (string)r[0]).ToList();

        string code;
        do
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVW1234567890";

            Random random = new Random();

            char[] res = new char[6];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = chars[random.Next(chars.Length)];
            }

            code = new string(res);

        } while (existingCodes.Contains(code));

        return code;
    }
}