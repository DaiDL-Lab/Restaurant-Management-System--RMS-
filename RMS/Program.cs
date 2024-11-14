using Microsoft.Data.Sqlite;
using System;
using System.IO;

class Program
{
    const string dbFilePath = "menu.db";

    static void Main()
    {
        //createMenu(dbFilePath);

        //showMenu();
        //addMenuItem("Tamago", "1", 2.10, "Food", "süßes Omelett");
        //addMenuItem("Inari", "2", 2.10, "Food", "Tofutasche");
        //addMenuItem("Shiitake", "3", 2.10, "Food", "Pilze");
        //addMenuItem("Avocado", "3a", 2.30, "Food", "");
        //addMenuItem("Sake-Avocado Temaki", "62", 3.20, "Food", "Lachs, Avocado, Kaviar");
        //deleteMenuItem(5);
        //updateMenuItem(6,"Avocado", "3a", 2.3, "Food", "Delicious Avocados");
        //searchMenuItem("3a");
        searchMenuItem("Lachs");
        //showMenu();
    }


    // FUNCTIONS //
    static void createMenu()
    {
        // Connect to the SQLite database
        using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
        {
            connection.Open();
            Console.WriteLine("Database connection opened.");

            // Create Items table if it doesn't exist
            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Menu (
                ItemID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Number TEXT,  -- Nullable for menu sets
                Price REAL,
                Category TEXT,
                Description TEXT
            );";

            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table 'Items' created or already exists.");
            }
        }
    }

    // Function to display the menu items from the database
    static void showMenu()
    {
        // Open a connection to the SQLite database
        using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
        {
            connection.Open();
            //Console.WriteLine("Database connection opened.");

            // Query to retrieve all items in the database
            string selectQuery = "SELECT ItemID, Name, Number, Price, Category, Description FROM Menu";

            using (var command = new SqliteCommand(selectQuery, connection))
            {
                // Execute the query and read the data
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("Items on the menu:");

                    // Check if there are any results
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No items found.");
                    }
                    else
                    {
                        // Loop through the results and print each item
                        while (reader.Read())
                        {
                            int itemId = reader.GetInt32(0); // ItemID column
                            string name = reader.GetString(1); // Name column
                            string number = reader.GetString(2); // Number column
                            double price = reader.GetDouble(3); // Price column
                            string category = reader.GetString(4); // Category column
                            string description = reader.GetString(5); // Category column

                            Console.WriteLine($"Item ID: {itemId}, Name: {name}, Number: {number}, Price: {price}€, Category: {category}, Description: {description}");
                        }
                    }
                }
            }
        }
    }

    static void addMenuItem(string name, string? number, double price, string category, string description)
    {
        // Connect to the SQLite database
        using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
        {
            connection.Open();
            //Console.WriteLine("Database connection opened.");

            // Prepare the SQL query to insert a new item
            string insertQuery = @"
                INSERT INTO Menu (Name, Number, Price, Category, Description) 
                VALUES (@Name, @Number, @Price, @Category, @Description);";

            // Create a command with the query and parameters
            using (var command = new SqliteCommand(insertQuery, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Number", string.IsNullOrEmpty(number) ? DBNull.Value : (object)number); // Handle nullable number
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Category", category);
                command.Parameters.AddWithValue("@Description", description);

                // Execute the query
                command.ExecuteNonQuery();
                Console.WriteLine($"Item [Name: {name}, Number: {number}, Price: {price}€, Category: {category}, Description: {description}] added.");
            }
        }
    }

    static void deleteMenuItem(int itemId)
    {
        // Connect to the SQLite database
        using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
        {
            connection.Open();
            //Console.WriteLine("Database connection opened.");

            // Define the query for deleting an item by ItemID
            string deleteQuery = "DELETE FROM Menu WHERE ItemID = @ItemID;";

            // Execute the delete command
            using (var command = new SqliteCommand(deleteQuery, connection))
            {
                // Add the parameter for the ItemID
                command.Parameters.AddWithValue("@ItemID", itemId);

                int rowsAffected = command.ExecuteNonQuery();

                // Check if any rows were deleted
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Item with ID {itemId} has been deleted.");
                }
                else
                {
                    Console.WriteLine($"No item found with ID {itemId}.");
                }
            }
        }
    }

    static void updateMenuItem(int itemId, string name, string number, double price, string category, string description)
    {
        // Connect to the SQLite database
        using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
        {
            connection.Open();
            //Console.WriteLine("Database connection opened.");

            // Define the query for updating an item
            string updateQuery = @"
                UPDATE Menu
                SET 
                    Name = @Name,
                    Number = @Number,
                    Price = @Price,
                    Category = @Category,
                    Description = @Description
                WHERE ItemID = @ItemID;";

            // Execute the update command
            using (var command = new SqliteCommand(updateQuery, connection))
            {
                // Add the parameters to the query
                command.Parameters.AddWithValue("@ItemID", itemId);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Number", number);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Category", category);
                command.Parameters.AddWithValue("@Description", description);

                int rowsAffected = command.ExecuteNonQuery();

                // Check if any rows were updated
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Item with ID {itemId} has been updated.");
                }
                else
                {
                    Console.WriteLine($"No item found with ID {itemId}.");
                }
            }
        }
    }

    static void searchMenuItem(string query)
    {
        // Open a connection to the SQLite database
        using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
        {
            connection.Open();
            //Console.WriteLine("Database connection opened.");

            // Query to retrieve all items in the database
            string selectQuery = "SELECT ItemID, Name, Number, Price, Category, Description FROM Menu WHERE Name LIKE @Query OR Number LIKE @Query OR Description LIKE @Query";

            using (var command = new SqliteCommand(selectQuery, connection))
            {
                // Add the parameter for the ItemID
                command.Parameters.AddWithValue("@Query", "%" + query + "%");   // Find in as subtring as well

                // Execute the query and read the data
                using (var reader = command.ExecuteReader())
                {
                    // Check if there are any results
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No item found.");
                    }
                    else
                    {
                        // Loop through the results and print each item
                        while (reader.Read())
                        {
                            int itemId = reader.GetInt32(0); // ItemID column
                            string name = reader.GetString(1); // Name column
                            string number = reader.GetString(2); // Number column
                            double price = reader.GetDouble(3); // Price column
                            string category = reader.GetString(4); // Category column
                            string description = reader.GetString(5); // Category column

                            Console.WriteLine($"Item ID: {itemId}, Name: {name}, Number: {number}, Price: {price}€, Category: {category}, Description: {description}");
                        }
                    }
                }
            }
        }
    }
}