using jobs.Messages;
using jobs.Database;

Database.createNewDatabaseIfNotExists();

await MessageFinder.findLatestBlockMessages();

Database.select();