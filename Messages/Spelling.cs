namespace jobs.Messages;

using WeCantSpell.Hunspell;

public class Spelling {

  WordList dictionary = WordList.CreateFromFiles(@"English (American).dic");

  public List<string> findEnglishWords(string possibleMessage) {
    try {
      List<string> englishWords = new List<string>();
      string[] tokens = possibleMessage.Split(' ');
      foreach (string token in tokens) {
        if (token.Length > 1 && dictionary.Check(token)) {
          englishWords.Add(token);
        }
      }
      return englishWords;
    } catch (Exception ex) {
      Console.WriteLine($"error in findEnglishWords for message {possibleMessage}: {ex}");
      return new List<string>();
    }
  }

}