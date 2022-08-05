namespace jobs.Database;

using System;
using System.Reflection;
using System.Reflection.Emit;
using Amazon;
using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using jobs.Models;

public class DynamoClient {

  AmazonDynamoDBClient client;

  public DynamoClient() {
    string accessKey = Environment.GetEnvironmentVariable("AWSAccessKeyId");
    string secretKey = Environment.GetEnvironmentVariable("AWSSecretKey");

    BasicAWSCredentials creds = new BasicAWSCredentials(accessKey, secretKey);
    client = new AmazonDynamoDBClient(creds, RegionEndpoint.USEast1);
  }

  public async Task<bool> isBlockInDb(int blockHeight) {
    Table table = Table.LoadTable(client, "blocks");
    Document doc = await table.GetItemAsync(blockHeight);
    bool exists = doc != null;
    return exists;
  }

  public async Task<bool> insertBlockInDb(Block block) {
    Table table = Table.LoadTable(client, "blocks");
    Document doc = new Document();
    doc["blockHeight"] = block.height;
    try {
      await table.PutItemAsync(doc);
      Console.WriteLine($"created: {doc.ToJson()}");
      return true;
    } catch (Exception ex) {
      Console.WriteLine($"error inserting: {ex}");
      return false;
    }
  }

}