namespace jobs.Clients;

using Newtonsoft.Json;
using jobs.Models;
using System.Threading;

public class BlockchainClient {
  public static HttpClient httpClient = new HttpClient();

  async public static Task<List<Block>> getTenBlocks(int startHeight = -1) {
    if (startHeight < 0) {
      string result = await httpClient.GetStringAsync($"https://blockstream.info/api/blocks");
      List<Block> tenBlocks = JsonConvert.DeserializeObject<List<Block>>(result);
      return tenBlocks;
    } else {
      string result = await httpClient.GetStringAsync($"https://blockstream.info/api/blocks/{startHeight}");
      List<Block> tenBlocks = JsonConvert.DeserializeObject<List<Block>>(result);
      return tenBlocks;
    }
  }

  async public static Task<Tx> getTransaction(string txId) {
    string result = await httpClient.GetStringAsync($"https://blockstream.info/api/tx/{txId}");
    return JsonConvert.DeserializeObject<Tx>(result);
  }

  async public static Task<List<Tx>> getBlockTransactions(string blockHash) {
    List<Tx> txs = new List<Tx>();
    try {
      string result = await httpClient.GetStringAsync($"https://blockstream.info/api/block/{blockHash}/txids");
      List<string> txIds = JsonConvert.DeserializeObject<List<string>>(result);
      Console.WriteLine($"getting {txIds.Count} transactions");
      foreach(string txId in txIds) {
        txs.Add(await getTransaction(txId));
      }
    } catch (Exception e) {
      Console.WriteLine($"caught exception {e}");
      Thread.Sleep(120000);
    }
    return txs;
  }
}