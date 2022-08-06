namespace jobs.Messages;

using System.Text;
using jobs.Models;

public class OpReturnSearch {

  Spelling spelling = new Spelling();

  public string findMessage(string hexMessage) {
    var message = Encoding.GetEncoding("ISO-8859-1").GetString(Convert.FromHexString(hexMessage));
    List<String> englishWords = spelling.findEnglishWords(message);
    if (englishWords.Count > 0) {
      return message;
    }
    return null;
  }

 }