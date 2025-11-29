using System.Collections.Generic;

namespace SaveLoadSystem.Interfaces
{
    public interface IGameDataHandler
    {
        void Save(GameData gameData, string profileId);
        GameData Load(string profileId);
        
        Dictionary<string, GameData> LoadAllGameDataProfiles();
        
        string GetRecentlyUpdatedProfileId();
    }
}