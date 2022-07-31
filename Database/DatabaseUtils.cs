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

  public static MySqlConnectionStringBuilder newMysqlTCPConnectionString() {
    var connectionString = new MySqlConnectionStringBuilder() {
        SslMode = MySqlSslMode.None,
        Server = Environment.GetEnvironmentVariable("DB_HOST"),   // e.g. '127.0.0.1'
        UserID = Environment.GetEnvironmentVariable("DB_USER"),   // e.g. 'my-db-user'
        Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
        Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
    };
    return connectionString;
  }

  public static MySqlConnectionStringBuilder newMysqlUnixSocketConnectionString() {
    var connectionString = new MySqlConnectionStringBuilder() {
        SslMode = MySqlSslMode.None,
        Server = Environment.GetEnvironmentVariable("INSTANCE_UNIX_SOCKET"), // e.g. '/cloudsql/project:region:instance'
        UserID = Environment.GetEnvironmentVariable("DB_USER"),   // e.g. 'my-db-user
        Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
        Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
        ConnectionProtocol = MySqlConnectionProtocol.UnixSocket
    };
    return connectionString;
  }

  public static string getMySqlConnectionString()
  {
      MySqlConnectionStringBuilder connectionString; 
      if (Environment.GetEnvironmentVariable("DB_HOST") != null)
      {
          connectionString = newMysqlTCPConnectionString();
      }
      else
      {
          connectionString = newMysqlUnixSocketConnectionString();
      }
      connectionString.Pooling = true;
      connectionString.MaximumPoolSize = 5;
      connectionString.MinimumPoolSize = 0;
      connectionString.ConnectionTimeout = 5;
      connectionString.ConnectionLifeTime = 1800;
      //Console.WriteLine($"connectionString: {connectionString.ConnectionString}");
      return connectionString.ConnectionString;
  }

  public static void initializeDatabase() {
    Console.WriteLine("InitializeDatabase");
    using(DbConnection connection = new MySqlConnection(connectionString)) {
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
      using (var createTableCommand = connection.CreateCommand()) {
        createTableCommand.CommandText = @"
            create table if not exists 
            block (
              id TEXT, 
              height INT, 
              INDEX(height)
            )";
        createTableCommand.ExecuteNonQuery();
      }
    }
    Console.WriteLine("InitializeDatabase complete");
  }

  public static async Task<bool> insertBlockMessage(BlockchainMessage b) {
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

  public static async Task<bool> insertBlock(Block b) {
    try {
      using(var connection = new MySqlConnection(connectionString)) { 
        connection.OpenWithRetry();
        using (var command = connection.CreateCommand()) {
          command.CommandText =$"insert into block (id, height) values ('{b.id}', {b.height})";
          await command.ExecuteNonQueryAsync();
        }
      }
      Console.WriteLine("insertBlock complete");
      return true;
    }
    catch (Exception ex) {
      Console.WriteLine($"error in insertBlock: {ex}");
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
          command.CommandText = "select MAX(height) from block";
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

  public static async Task<List<Block>> selectMostRecentBlocks() {
    Console.WriteLine("selectMostRecentTenMessages");
    List<Block> blocks = new List<Block>();
    try {
      using(var connection = new MySqlConnection(connectionString)) { 
        connection.OpenWithRetry();
        using (var command = connection.CreateCommand()) {
          command.CommandText = "select height, id from block GROUP BY 1, 2 ORDER BY 1 DESC LIMIT 10";
          using (var reader = await command.ExecuteReaderAsync()) {
            while (await reader.ReadAsync()) {
              blocks.Add(new Block(reader.GetString(1), reader.GetInt32(0), 1L));
            }
          }
        }
      }
      Console.WriteLine($"blocks: {JsonConvert.SerializeObject(blocks)}");
      return blocks;
    } catch (Exception ex) {
      Console.WriteLine($"error in selectMostRecentBlocks: {ex}");
      return new List<Block>();
    }
  }

  public static async Task<bool> deleteBlocks() {
    Console.WriteLine("deleteBlocks");
    try {
      using(var connection = new MySqlConnection(connectionString)) { 
        connection.OpenWithRetry();
        using (var command = connection.CreateCommand()) {
          command.CommandText =$"delete from block";
          await command.ExecuteNonQueryAsync();
        }
      }
      await insertBlock(new Block("id", 1, 1L));
      Console.WriteLine("deleteBlocks complete");
      return true;
    }
    catch (Exception ex) {
      Console.WriteLine($"error in deleteBlocks: {ex}");
      return false;
    }
  }
}