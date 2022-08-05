namespace jobs.Clients;

using Tweetinvi;
using Tweetinvi.Models;

public class TweetClient {

  TwitterClient client;

  public TweetClient() {
    string consumerKey = Environment.GetEnvironmentVariable("twitterApiKey");
    string consumerSecret = Environment.GetEnvironmentVariable("twitterKeySecret");
    string bearerToken = Environment.GetEnvironmentVariable("twitterToken");
    string accessToken = Environment.GetEnvironmentVariable("twitterAccessToken");
    string accessTokenSecret = Environment.GetEnvironmentVariable("twitterAccessTokenSecret");

    TwitterCredentials userCreds = new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
    client = new TwitterClient(userCreds);
  }

  async public Task<bool> publishTweet(string tweet) {
    try {
      var tweetResponse = await client.Tweets.PublishTweetAsync(tweet);
      Console.WriteLine($"tweetResponse: {tweetResponse}");
      return true;
    } catch (Exception ex) {
      Console.WriteLine($"error in publishTweet: {ex}");
      return false;
    }
  }
}