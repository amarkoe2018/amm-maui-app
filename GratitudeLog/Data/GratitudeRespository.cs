using GratitudeLog.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace GratitudeLog.Data
{
    /// <summary>
    /// Repository class for managing Gratitude in the database.
    /// </summary>
    public class GratitudeRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GratitudeRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public GratitudeRepository(ILogger<GratitudeRepository> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initializes the database connection and creates the Gratitude table if it does not exist.
        /// </summary>
        private async Task Init()
        {
            if (_hasBeenInitialized)
                return;

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            try
            {
                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Gratitude (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Entry TEXT NOT NULL,
                Repeated INTEGER NOT NULL
            );";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating Gratitude table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all gratitude entries from the database.
        /// </summary>
        /// <returns>A list of <see cref="Gratitude Items"/> objects.</returns>
        public async Task<List<GratitudeEntry>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM Gratitude";
            var gratitudeEntries = new List<GratitudeEntry>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                gratitudeEntries.Add(new GratitudeEntry
                {
                    ID = reader.GetInt32(0),
                    Entry = reader.GetString(1),
                    Repeated = reader.GetInt32(2)
                });
            }

            return gratitudeEntries;
        }

        /// <summary>
        /// Saves a GratitudeEntry to the database. If the GratitudeEntry ID is 0, a new GratitudeEntry is created; otherwise, the existing GratitudeEntry is updated.
        /// </summary>
        /// <param name="item">The GratitudeEntry to save.</param>
        /// <returns>The ID of the saved GratitudeEntry.</returns>
        public async Task<int> SaveItemAsync(GratitudeEntry item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            // First, check if the same entry already exists
            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = "SELECT ID, Repeated FROM Gratitude WHERE Entry = @entry";
            checkCmd.Parameters.AddWithValue("@entry", item.Entry);

            await using var reader = await checkCmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                // Entry exists – increment the Repeated count
                int existingId = reader.GetInt32(0);
                int existingRepeated = reader.GetInt32(1);
                reader.Close(); // must close reader before reusing the connection

                var updateCmd = connection.CreateCommand();
                updateCmd.CommandText = "UPDATE Gratitude SET Repeated = @repeated WHERE ID = @id";
                updateCmd.Parameters.AddWithValue("@repeated", existingRepeated + 1);
                updateCmd.Parameters.AddWithValue("@id", existingId);
                await updateCmd.ExecuteNonQueryAsync();

                return existingId;
            }
            else
            {
                // Entry does not exist – insert new
                reader.Close(); // must still close even if not used
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"
            INSERT INTO Gratitude (Entry, Repeated) VALUES (@entry, 1);
            SELECT last_insert_rowid();";
                insertCmd.Parameters.AddWithValue("@entry", item.Entry);

                var result = await insertCmd.ExecuteScalarAsync();
                item.ID = Convert.ToInt32(result);
                item.Repeated = 1;

                return item.ID;
            }
        }


        /// <summary>
        /// Deletes a GratitudeEntry from the database.
        /// </summary>
        /// <param name="item">The GratitudeEntry to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> DeleteItemAsync(GratitudeEntry item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Gratitude WHERE ID = @id";
            deleteCmd.Parameters.AddWithValue("@id", item.ID);

            return await deleteCmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Drops the Gratitude table from the database.
        /// </summary>
        public async Task DropTableAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var dropTableCmd = connection.CreateCommand();
            dropTableCmd.CommandText = "DROP TABLE IF EXISTS Gratitude";
            await dropTableCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }
    }
}