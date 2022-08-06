using jobs.Messages;

var messageFinder = new MessageFinder();
await messageFinder.findLatestBlockMessages();

// for testing
//var testMessageFinder = new TestMessageFinder();
//await testMessageFinder.findFrenchMessage();