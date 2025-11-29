namespace Settings
{
    public interface ISettingsDataHandler
    {
        void Save(SettingsData settingsData);
        SettingsData Load();
    }
}
