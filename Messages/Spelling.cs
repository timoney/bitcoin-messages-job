namespace jobs.Messages;

using WeCantSpell.Hunspell;

public class Spelling {
  public List<string> findEnglishWords(string possibleMessage) {
    var dictionary = WordList.CreateFromFiles(@"English (American).dic");

    List<string> englishWords = new List<string>();
    string[] tokens = possibleMessage.Split(' ');
    foreach (string token in tokens) {
      if (token.Length > 1 && dictionary.Check(token)) {
        englishWords.Add(token);
      }
    }
    return englishWords;
  }

}