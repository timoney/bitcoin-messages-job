using jobs.Messages;
using jobs.Database;

DatabaseUtils.initializeDatabase();

await MessageFinder.findLatestBlockMessages();

DatabaseUtils.selectMostRecentTenMessages();