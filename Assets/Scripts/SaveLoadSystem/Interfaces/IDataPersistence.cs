namespace SaveLoadSystem.Interfaces
{
    public interface IDataPersistence
    {
        void SaveData(GameData gameData);
        void LoadData(GameData gameData);
    }
}