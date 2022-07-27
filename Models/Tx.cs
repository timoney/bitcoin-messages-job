namespace jobs.Models;

public class Vout {
  public string scriptpubkey_asm { get; set; }
}

public class Tx {
  public string txid { get; set; }
  public List<Vout> vout { get; set; }
}

public class TxId {
  public string txid { get; set; }
}