using LabMaster.Data;
using Microsoft.Data.SqlClient;

namespace LabMaster.Services;

public sealed class BackupService
{
    public async Task BackupAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Backup file path is required.", nameof(filePath));

        var builder = new SqlConnectionStringBuilder(Database.ConnectionString);
        var database = builder.InitialCatalog;

        await using var c = Database.CreateConnection();
        await c.OpenAsync();

        const string sql = "BACKUP DATABASE @db TO DISK=@path WITH INIT, FORMAT;";
        await using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddWithValue("@db", database);
        cmd.Parameters.AddWithValue("@path", filePath);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task RestoreAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Restore backup file path is required.", nameof(filePath));

        var builder = new SqlConnectionStringBuilder(Database.ConnectionString);
        var database = builder.InitialCatalog;
        var masterBuilder = new SqlConnectionStringBuilder(Database.ConnectionString)
        {
            InitialCatalog = "master"
        };

        await using var c = new SqlConnection(masterBuilder.ConnectionString);
        await c.OpenAsync();

        var escapedDatabase = database.Replace("]", "]]");
        const string sql = """
            DECLARE @db sysname = @database;
            DECLARE @sql nvarchar(max);
            SET @sql = N'ALTER DATABASE [' + REPLACE(@db, ']', ']]') + N'] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;';
            EXEC(@sql);
            SET @sql = N'RESTORE DATABASE [' + REPLACE(@db, ']', ']]') + N'] FROM DISK = @backup WITH REPLACE, RECOVERY;';
            EXEC sp_executesql @sql, N'@backup nvarchar(4000)', @backup=@path;
            SET @sql = N'ALTER DATABASE [' + REPLACE(@db, ']', ']]') + N'] SET MULTI_USER;';
            EXEC(@sql);
            """;

        try
        {
            await using var cmd = new SqlCommand(sql, c);
            cmd.CommandTimeout = 0;
            cmd.Parameters.AddWithValue("@database", database);
            cmd.Parameters.AddWithValue("@path", filePath);
            await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            try
            {
                await using var recover = new SqlCommand(
                    $"ALTER DATABASE [{escapedDatabase}] SET MULTI_USER;",
                    c);
                await recover.ExecuteNonQueryAsync();
            }
            catch
            {
            }

            throw;
        }
    }
}