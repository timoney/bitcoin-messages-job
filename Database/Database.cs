namespace jobs.Database;

using System.Data.SQLite;

public class Database {
  static string connectionString = "Data Source=../../MyDatabase.sqlite;Version=3";

  public static void createNewDatabaseIfNotExists() {
    SQLiteConnection conn = new SQLiteConnection(connectionString);
    conn.Open();

    string createSql = "create table if not exists block (id TEXT, height INTEGER, timestamp INTEGER)";
    SQLiteCommand createCommand = new SQLiteCommand(createSql, conn);
    createCommand.ExecuteNonQuery();

    createSql = "create table if not exists blockMessage (blockHeight INTEGER, transactionId TEXT, message TEXT)";
    createCommand = new SQLiteCommand(createSql, conn);
    createCommand.ExecuteNonQuery();

    conn.Close();
  }

  public static void insert(string sql) {
    SQLiteConnection conn = new SQLiteConnection(connectionString);
    conn.Open();

    SQLiteCommand createCommand = new SQLiteCommand(sql, conn);
    createCommand.ExecuteNonQuery();

    conn.Close();
  }

  public static void select() {
    SQLiteConnection conn = new SQLiteConnection(connectionString);
    conn.Open();

    string sql = "select * from block order by height desc";
    SQLiteCommand command = new SQLiteCommand(sql, conn);
    SQLiteDataReader reader = command.ExecuteReader();
    Console.WriteLine("block table:");
    while (reader.Read()) {
      Console.WriteLine("----------------------------------------------------------");
      Console.WriteLine($"id: {reader["id"]} \nheight: {reader["height"]} \ntimestamp: {reader["timestamp"]}");
    }

    sql = "select * from blockMessage order by blockHeight desc";
    command = new SQLiteCommand(sql, conn);
    reader = command.ExecuteReader();
    Console.WriteLine("blockMessage table: ");
    while (reader.Read()) {
      Console.WriteLine("-----------------------------------------------------------");
      Console.WriteLine($"blockHeight: {reader["blockHeight"]} \ntransactionId: {reader["transactionId"]} \nmessage: {reader["message"]}");
    }

    conn.Close();
  }
}