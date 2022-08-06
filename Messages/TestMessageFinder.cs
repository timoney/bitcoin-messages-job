namespace jobs.Messages;

using jobs.Clients;
using jobs.Models;

public class TestMessageFinder {

  MessageFinder messageFinder = new MessageFinder();

  private async Task<bool> findMessage(string txid) {
    Tx tx = await BlockchainClient.getTransaction(txid);
    List<BlockchainMessage> blockMessages = messageFinder.findTransactionMessages(tx);

    foreach(BlockchainMessage blockchainMessage in blockMessages) {
      string tweet = $"Block: unknownForTest\nTx:{blockchainMessage.transactionId}\n\n{blockchainMessage.message}";
      Console.WriteLine($"tweet:\n${tweet}");
    }

    return true;
  }

  public async Task<bool> findKnownMessage() {
    string txid = "00bb146d2f16954bd5ccb454c742330f911daccb3a0f29d4bbbb939fc228517d";
    await findMessage(txid);
    return true;
  }

  public async Task<bool> findSpanishMessage() {
    string txid = "e99efa5ac8560fe4d5e7b0b3efb7df75ae55f1d078ff532f36764d1bd8dec790";
    await findMessage(txid);
    return true;
  }

  public async Task<bool> findFrenchMessage() {
    string txid = "976312f3b02fe087b2e555c7679c4b3522ccb6c4b4b56f6b1e8bba46c346381a";
    await findMessage(txid);
    return true;
  }

}