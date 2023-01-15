namespace jobs.Messages;

using System.Text;
using jobs.Models;
using Newtonsoft.Json;

public class OpReturnSearch {

  Spelling spelling = new Spelling();

  List<String> dumbWords = new List<String>() {
    "zT", "Fl", "aggregate", "consolidate", "Hath"
  };

  // a hack for now. hopefully filter out messages that don't actually make any sense
  private bool isNormalWord(List<String> words) {
    var word = words[0];
    var isInDumbWords = dumbWords.FirstOrDefault(stringToCheck => stringToCheck.Contains(word));
    if (isInDumbWords != null) {
      Console.WriteLine($"word: {word} is a dumb word!!");
      return false;
    }
    return true;
  }

  private bool isStupidMessage(String message) {
    if (message.StartsWith("BERNSTEIN 2.0 REG")) {
      Console.WriteLine($"stupid message");
      return false;
    } else {
      return true;
    }
  }

  public string findMessage(string hexMessage) {
    //var encoding = "ISO-8859-1";
    var encoding = "UTF-8";
    var message = Encoding.GetEncoding(encoding).GetString(Convert.FromHexString(hexMessage));
    if (isStupidMessage(message)) return null;

    List<String> englishWords = spelling.findWords(message, DicLanguage.English);
    if (englishWords.Count > 0 && isNormalWord(englishWords)) {
      Console.WriteLine($"English words: {JsonConvert.SerializeObject(englishWords, Formatting.Indented)}");
      return message;
    }
    List<String> spanishWords = spelling.findWords(message, DicLanguage.Spanish);
    if (spanishWords.Count > 0 && isNormalWord(spanishWords)) {
      Console.WriteLine($"Spanish words: {JsonConvert.SerializeObject(spanishWords, Formatting.Indented)}");
      return message;
    }
    List<String> frenchWords = spelling.findWords(message, DicLanguage.French);
    if (frenchWords.Count > 0 && isNormalWord(frenchWords)) {
      Console.WriteLine($"French words: {JsonConvert.SerializeObject(frenchWords, Formatting.Indented)}");
      return message;
    }
    return null;
  }

 }