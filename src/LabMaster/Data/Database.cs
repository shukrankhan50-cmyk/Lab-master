using Microsoft.Data.SqlClient;
namespace LabMaster.Data;
public static class Database { public static string ConnectionString { get; set; } = @"Server=.\SQLEXPRESS;Database=LabMasterDb;Trusted_Connection=True;TrustServerCertificate=True;"; public static SqlConnection CreateConnection()=>new(ConnectionString); }