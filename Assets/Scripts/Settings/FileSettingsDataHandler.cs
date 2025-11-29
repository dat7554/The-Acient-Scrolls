using System;
using System.IO;
using UnityEngine;

namespace Settings
{
    public class FileSettingsDataHandler : ISettingsDataHandler
    {
        private readonly string _saveDirectory;
        private readonly string _fileName;
        
        public FileSettingsDataHandler(string saveDirectory, string fileName)
        {
            _saveDirectory = saveDirectory;
            _fileName = fileName;
        }
        
        public void Save(SettingsData settingsData)
        {
            var fullPath = Path.Combine(_saveDirectory, _fileName);
            var directoryPath = Path.GetDirectoryName(fullPath);
            
            try
            {
                Directory.CreateDirectory(directoryPath);
                
                var json = JsonUtility.ToJson(settingsData, true);
                File.WriteAllText(fullPath, json);
                
                // TODO: Debug purpose only
                Debug.Log("Saved settings at: " + _saveDirectory);
            }
            catch (Exception e)
            {
                Debug.LogError("Error when save data: " + fullPath + "\n" + e);
            }
        }

        public SettingsData Load()
        {
            SettingsData tempData = null;
            
            var fullPath = Path.Combine(_saveDirectory, _fileName);
            if (File.Exists(fullPath))
            {
                var json = File.ReadAllText(fullPath);
                tempData = JsonUtility.FromJson<SettingsData>(json);
            }
            else
            {
                Debug.LogWarning("Setting file does not exists!");
            }
            
            return tempData;
        }
    }
}
