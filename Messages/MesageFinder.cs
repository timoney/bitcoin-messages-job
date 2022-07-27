namespace jobs.Messages;

using System.Text;
using jobs.Models;
using jobs.Clients;
using jobs.Messages;
using Newtonsoft.Json;
using jobs.Database;

public class MessageFinder {
  static Spelling spelling = new Spelling();

  public static List<BlockchainMessage> findTransactionMessages(int blockHeight, Tx tx) {
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
            var message = Encoding.GetEncoding("ISO-8859-1").GetString(Convert.FromHexString(token));
            //Console.WriteLine($"checking message: {message}");
            List<String> englishWords = spelling.findEnglishWords(message);
            if (englishWords.Count > 0) {
              messages.Add(new BlockchainMessage(blockHeight, tx.txid, message, englishWords));
            }
          }
        }
      }
    }
    return messages;
  }

  public static async Task<bool> searchBlocks(List<Block> blocks) {
    foreach(Block block in blocks) {
      List<Tx> blockTransactions = await BlockchainClient.getBlockTransactions(block.id);
      Console.WriteLine($"blockHeight: {block.height}; blockTransactionsCount: {blockTransactions.Count}");

      List<BlockchainMessage> blockMessages = new List<BlockchainMessage>();
      foreach(Tx tx in blockTransactions) {
        List<BlockchainMessage> transactionMessages = findTransactionMessages(block.height, tx);
        blockMessages.AddRange(transactionMessages);
      }

      foreach(BlockchainMessage blockchainMessage in blockMessages) {
        Console.WriteLine($"blockchainMessage: {JsonConvert.SerializeObject(blockchainMessage)}");
        DatabaseUtils.insertBlockMessage(blockchainMessage);
      }
    }
    return true;
  }

  public static async Task<bool> findKnownBlockMessages() {
    List<Block> knownBlocks = new List<Block>();
    knownBlocks.Add(new Block("00000000000000000004cfed06021279e3a2feabc9d6b26ec35f0a2134eba524", 723213, 0));
    await searchBlocks(knownBlocks);
    return true;
  }

  public static async Task<bool> findLatestBlockMessages() {
    List<Block> lastTenBlocks = await BlockchainClient.getTenBlocks();
    int lastSearchedBlock = await DatabaseUtils.selectMaxBlockHeight();
    Console.WriteLine($"lastSearchedBlock: {lastSearchedBlock}");
    await searchBlocks(lastTenBlocks.Where(b => b.height > lastSearchedBlock).ToList());
    return true;
  }

  public static async Task<bool> findBlockMessagesInRange(int startHeight, int endHeight) {
    while (startHeight <= endHeight) {
      Console.WriteLine($"Searching blocks: {startHeight} - {endHeight}");
      await searchBlocks(await BlockchainClient.getTenBlocks(startHeight));
      startHeight = startHeight + 10;
    }
    return true;
  }

}