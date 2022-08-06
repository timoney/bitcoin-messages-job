namespace jobs.Messages;

using System.Text;
using jobs.Models;
using Newtonsoft.Json;

public class OpReturnSearch {

  Spelling spelling = new Spelling();

  public string findMessage(string hexMessage) {
    var message = Encoding.GetEncoding("ISO-8859-1").GetString(Convert.FromHexString(hexMessage));
    List<String> englishWords = spelling.findWords(message, DicLanguage.English);
    if (englishWords.Count > 0) {
      Console.WriteLine($"English words: {JsonConvert.SerializeObject(englishWords, Formatting.Indented)}");
      return message;
    }
    List<String> spanishWords = spelling.findWords(message, DicLanguage.Spanish);
    if (spanishWords.Count > 0) {
      Console.WriteLine($"Spanish words: {JsonConvert.SerializeObject(spanishWords, Formatting.Indented)}");
      return message;
    }
    List<String> frenchWords = spelling.findWords(message, DicLanguage.French);
    if (frenchWords.Count > 0) {
      Console.WriteLine($"French words: {JsonConvert.SerializeObject(frenchWords, Formatting.Indented)}");
      return message;
    }
    return null;
  }

 }