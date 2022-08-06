namespace jobs.Messages;

using jobs.Clients;
using jobs.Models;

public class TestMessageFinder {

  MessageFinder messageFinder = new MessageFinder();

  public async Task<bool> findKnownMessage() {
    string txid = "00bb146d2f16954bd5ccb454c742330f911daccb3a0f29d4bbbb939fc228517d";
    Tx tx = await BlockchainClient.getTransaction(txid);
    List<BlockchainMessage> blockMessages = messageFinder.findTransactionMessages(tx);

    foreach(BlockchainMessage blockchainMessage in blockMessages) {
      string tweet = $"Block: unknownForTest Tx:\n{blockchainMessage.transactionId}\n\n{blockchainMessage.message}";
      Console.WriteLine($"tweet:\n${tweet}");
    }

    return true;
  }

}