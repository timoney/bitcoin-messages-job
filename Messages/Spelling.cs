namespace jobs.Messages;

using WeCantSpell.Hunspell;

public enum DicLanguage {
  English,
  Spanish
}

public class Spelling {

  WordList englishDic = WordList.CreateFromFiles(@"Dictionaries/English (American).dic");
  WordList spanishDic = null;

  private WordList getDictionary(DicLanguage language) {
    if (language == DicLanguage.English) {
      return englishDic;
    } else if (language == DicLanguage.Spanish) {
      return spanishDic;
    } else {
      return englishDic;
    }
  }

  public List<string> findWords(string possibleMessage, DicLanguage language) {
    try {
      List<string> words = new List<string>();
      string[] tokens = possibleMessage.Split(' ');
      WordList dic = getDictionary(language);
      foreach (string token in tokens) {
        if (token.Length > 1 && dic.Check(token)) {
          words.Add(token);
        }
      }
      return words;
    } catch (Exception ex) {
      Console.WriteLine($"error in findWords for message {possibleMessage}: {ex}");
      return new List<string>();
    }
  }

}