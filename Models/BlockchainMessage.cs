namespace jobs.Models;

public class BlockchainMessage {
  public int blockHeight { get; set; }
  public string transactionId { get; set; }
  public string message { get; set; }
  // only used for debuggin purposed right now. not persisted
  public List<string> validWords { get; set; }

  public BlockchainMessage(int blockHeight, string transactionId, string message, List<string> validWords) {
    this.blockHeight= blockHeight;
    this.transactionId = transactionId;
    this.message = message;
  }
}