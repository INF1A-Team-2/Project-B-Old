namespace RestaurantManager;

class Table
{
    public int ID;
    public int Capacity;
    public bool MultipleGroupsAllowed;

    public Table(int id, int capacity, bool multipleGroupsAllowed)
    {
        this.ID = id;
        this.Capacity = capacity;
        this.MultipleGroupsAllowed = multipleGroupsAllowed;
    }

    private static Table Parse(List<object> data)
    {
        return new Table(
                 (int)(long)data[0],
            (int)(long)data[1],
                   (bool)data[2]);
    }
    
    public static List<Table> GetAllTables()
    {
        return DatabaseConnection.Execute("SELECT * FROM tables")
            .Select(Parse)
            .ToList();
    }
    
    public static Table? GetTableByID(int id)
    {
        List<List<object>> res = DatabaseConnection.Execute(
            "SELECT * FROM tables WHERE id = %s",
            id);

        if (res.Count == 0)
            return null;

        return Parse(res[0]);
    }
}