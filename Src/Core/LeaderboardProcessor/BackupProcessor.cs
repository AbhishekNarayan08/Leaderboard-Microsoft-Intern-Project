using CosmosDB.Interfaces;
using CosmosDB.Models;

namespace LeaderboardProcessor
{
    public class BackupProcessor
    {
        private readonly ICosmosDbUserLeaderboardWriteClient cosmosDbUserLeaderboardWriteClient;

        public BackupProcessor(ICosmosDbUserLeaderboardWriteClient cosmosDbUserLeaderboardWriteClinet)
        {
            this.cosmosDbUserLeaderboardWriteClient = cosmosDbUserLeaderboardWriteClinet;
        }

        public async Task UpdateCosmos(UserLeaderboard userLeaderboard)
        {
            await cosmosDbUserLeaderboardWriteClient.UpsertDocumentAsync<UserLeaderboard>("/userId", userLeaderboard);
        }
    }
}
