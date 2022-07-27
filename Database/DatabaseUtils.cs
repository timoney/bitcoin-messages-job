namespace jobs.Database;

using Polly;
using System;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using jobs.Models;
using Newtonsoft.Json;

public static class DatabaseUtils {
  private readonly static string connectionString = getMySqlConnectionString();

  public static void OpenWithRetry(this DbConnection connection) =>
    Policy
      .Handle<MySqlException>()
      .WaitAndRetry(new[]
      {
          TimeSpan.FromSeconds(1),
          TimeSpan.FromSeconds(2),
          TimeSpan.FromSeconds(5)
      })
      .Execute(() => connection.Open());

  public static string getMySqlConnectionString() {
    var connectionString = new MySqlConnectionStringBuilder() {
        SslMode = MySqlSslMode.None,
        Server = Environment.GetEnvironmentVariable("DB_HOST"),   // e.g. '127.0.0.1'
        UserID = Environment.GetEnvironmentVariable("DB_USER"),   // e.g. 'my-db-user'
        Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
        Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
    };
    connectionString.Pooling = true;
    connectionString.MaximumPoolSize = 5;
    connectionString.MinimumPoolSize = 0;
    connectionString.ConnectionTimeout = 5;
    connectionString.ConnectionLifeTime = 1800;
    Console.WriteLine($"connectionString: {connectionString.ConnectionString}");
    return connectionString.ConnectionString;
  }

  public static void initializeDatabase() {
    Console.WriteLine("InitializeDatabase");
    using(DbConnection connection = new MySqlConnection(connectionString))
    {
      connection.OpenWithRetry();
      using (var createTableCommand = connection.CreateCommand())
      {
        createTableCommand.CommandText = @"
            create table if not exists 
            blockMessage (
              blockHeight INTEGER, 
              transactionId TEXT, 
              message TEXT,
              INDEX(blockHeight)
            )";
        createTableCommand.ExecuteNonQuery();
      }
    }
    Console.WriteLine("InitializeDatabase complete");
  }

  public static async Task<bool> insertBlockMessage(BlockchainMessage b) {
    Console.WriteLine("insertBlockMessage");
    try {
      using(var connection = new MySqlConnection(connectionString)) { 
        connection.OpenWithRetry();
        using (var command = connection.CreateCommand()) {
          command.CommandText =$"insert into blockMessage (blockHeight, transactionId, message) values ({b.blockHeight}, '{b.transactionId}', '{b.message}')";
          await command.ExecuteNonQueryAsync();
        }
      }
      Console.WriteLine("insertBlockMessage complete");
      return true;
    }
    catch (Exception ex) {
      Console.WriteLine($"error in insertBlockMessage: {ex}");
      return false;
    }
  }

  public static async Task<List<BlockchainMessage>> selectMostRecentTenMessages() {
    Console.WriteLine("selectMostRecentTenMessages");
    List<BlockchainMessage> blockchainMessages = new List<BlockchainMessage>();
    try {
      using(var connection = new MySqlConnection(connectionString)) { 
        connection.OpenWithRetry();
        using (var command = connection.CreateCommand()) {
          command.CommandText = "select blockHeight, transactionId, message from blockMessage GROUP BY 1, 2, 3 ORDER BY 1 DESC LIMIT 10";
          using (var reader = await command.ExecuteReaderAsync()) {
            while (await reader.ReadAsync()) {
              blockchainMessages.Add(new BlockchainMessage(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), null));
            }
          }
        }
      }
      Console.WriteLine($"blockchainMessages: {JsonConvert.SerializeObject(blockchainMessages)}");
      return blockchainMessages;
    } catch (Exception ex) {
      Console.WriteLine($"error in selectMostRecentTenMessages: {ex}");
      return new List<BlockchainMessage>();
    }
  }

  public static async Task<int> selectMaxBlockHeight() {
    Console.WriteLine("selectMaxBlockHeight");
    try {
      using(var connection = new MySqlConnection(connectionString)) { 
        connection.OpenWithRetry();
        using (var command = connection.CreateCommand()) {
          command.CommandText = "select MAX(blockHeight) from blockMessage";
          using (var reader = await command.ExecuteReaderAsync()) {
            while (await reader.ReadAsync()) {
              return reader.GetInt32(0);
            }
          }
        }
      }
      return -1;
    }
    catch (Exception ex) {
      Console.WriteLine($"error in selectMaxBlockHeight: {ex}");
      return -1;
    }
  }
}