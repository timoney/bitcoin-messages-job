namespace jobs.Models;

public class Block {
  public string id { get; set; }
  public int height { get; set; }
  public long timestamp { get; set; }

  public Block(string id, int height, long timestamp) {
    this.id = id;
    this.height = height;
    this.timestamp = timestamp;
  }
}