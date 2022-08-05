using jobs.Messages;
using jobs.Database;
using jobs.Clients;

var messageFinder = new MessageFinder();
await messageFinder.findLatestBlockMessages();
