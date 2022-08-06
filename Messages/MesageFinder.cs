namespace jobs.Messages;

using jobs.Models;
using jobs.Clients;
using jobs.Messages;
using Newtonsoft.Json;
using jobs.Database;

public class MessageFinder {
  TweetClient tweetClient = new TweetClient();
  DynamoClient dynamoClient = new DynamoClient();
  OpReturnSearch opReturnSearch = new OpReturnSearch();

  public List<BlockchainMessage> findTransactionMessages(Tx tx) {
    List<BlockchainMessage> messages = new List<BlockchainMessage>();
    foreach(Vout vout in tx.vout) {
      string[] tokens = vout.scriptpubkey_asm.Split(' ');
      bool hasOpReturn = false;
      foreach(string token in tokens) {
        if (token == "OP_RETURN") {
          hasOpReturn = true;
          break;
        }
      }
      if (hasOpReturn) {
        foreach(string token in tokens) {
          if (!token.StartsWith("OP_")) {
            string message = opReturnSearch.findMessage(token);
            if (message != null) {
              messages.Add(new BlockchainMessage(tx.txid, message, null));
            }
          }
        }
      }
    }
    return messages;
  }

  public async Task<bool> searchBlocks(List<Block> blocks) {
    foreach(Block block in blocks) {

      if (await dynamoClient.isBlockInDb(block.height)) {
        Console.WriteLine($"block already exists: {block.height}");
        continue;
      }

      if (!await dynamoClient.insertBlockInDb(block)) {
        continue;
      }
      
      List<string> blockTxIds = await BlockchainClient.getBlockTransactions(block.id);
      Console.WriteLine($"blockHeight: {block.height}; blockTransactionsCount: {blockTxIds.Count}");

      List<BlockchainMessage> blockMessages = new List<BlockchainMessage>();
      foreach(string txId in blockTxIds) {
        Tx tx = await BlockchainClient.getTransaction(txId);
        List<BlockchainMessage> transactionMessages = findTransactionMessages(tx);
        blockMessages.AddRange(transactionMessages);
      }

      foreach(BlockchainMessage blockchainMessage in blockMessages) {
        Console.WriteLine($"blockchainMessage: {JsonConvert.SerializeObject(blockchainMessage)}");
        string tweet = $"Block: {block.height} Tx:\n{blockchainMessage.transactionId}\n\n{blockchainMessage.message}";
        await tweetClient.publishTweet(tweet);
      }
      
    }
    return true;
  }

  public async Task<bool> findLatestBlockMessages() {
    List<Block> lastTenBlocks = await BlockchainClient.getTenBlocks();
    await searchBlocks(lastTenBlocks);
    return true;
  }

}