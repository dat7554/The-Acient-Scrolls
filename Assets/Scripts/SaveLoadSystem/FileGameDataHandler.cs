using System;
using System.Collections.Generic;
using System.IO;
using SaveLoadSystem.Interfaces;
using UnityEngine;

namespace SaveLoadSystem
{
    public class FileGameDataHandler : IGameDataHandler
    {
        private readonly string _saveDirectory;
        private readonly string _fileName;

        public FileGameDataHandler(string saveDirectory, string fileName)
        {
            _saveDirectory = saveDirectory;
            _fileName = fileName;
        }

        public void Save(GameData gameData, string profileId)
        {
            if (profileId is null) return;
            
            var fullPath = Path.Combine(_saveDirectory, profileId, _fileName);
            var directoryPath = Path.GetDirectoryName(fullPath);
            
            try
            {
                Directory.CreateDirectory(directoryPath);
                
                var json = JsonUtility.ToJson(gameData, true);
                File.WriteAllText(fullPath, json);
                
                // TODO: Debug purpose only
                GUIUtility.systemCopyBuffer = _saveDirectory;
                Debug.Log("Saved at: " + _saveDirectory);
            }
            catch (Exception e)
            {
                Debug.LogError("Error when save data: " + fullPath + "\n" + e);
            }
        }

        public GameData Load(string profileId)
        {
            if (profileId is null) return null;
            
            GameData tempData = null;
            
            var fullPath = Path.Combine(_saveDirectory, profileId, _fileName);
            if (File.Exists(fullPath))
            {
                var json = File.ReadAllText(fullPath);
                tempData = JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                Debug.LogError("File does not exists!");
            }
            
            return tempData;
        }
        
        public void DeleteGameSaveData(string profileId)
        {
            var fullPath = Path.Combine(_saveDirectory, profileId, _fileName);
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            else
            {
                Debug.LogError("File does not exists!");
            }
        }

        public Dictionary<string, GameData> LoadAllGameDataProfiles()
        {
            var gameDataProfilesDictionary = new Dictionary<string, GameData>();
            
            // Ensure save directory exists
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
                return gameDataProfilesDictionary; // return empty, nothing to load yet
            }
            
            IEnumerable<DirectoryInfo> directoryInfos = new DirectoryInfo(_saveDirectory).EnumerateDirectories();
            foreach (var directoryInfo in directoryInfos)
            {
                string profileId = directoryInfo.Name;

                // Check if a ".sav" file exist in this folder
                var fullPath = Path.Combine(_saveDirectory, profileId, _fileName);
                if (!File.Exists(fullPath)) continue;

                var gameData = Load(profileId);
                if (gameData is not null)
                    gameDataProfilesDictionary.Add(profileId, gameData);
            }
            
            return gameDataProfilesDictionary;
        }

        public string GetRecentlyUpdatedProfileId()
        {
            string recentProfileId = null;
            
            var gameDataProfilesDictionary = LoadAllGameDataProfiles();
            foreach (var (profileID, gameData) in gameDataProfilesDictionary)
            {
                if (gameData is null) continue;

                if (recentProfileId is null)
                {
                    recentProfileId = profileID;
                }
                else
                {
                    var recentDateTime = DateTime.Parse(gameDataProfilesDictionary[recentProfileId].dateTimeISO);
                    var gameDataDateTime = DateTime.Parse(gameData.dateTimeISO);

                    if (gameDataDateTime > recentDateTime)
                    {
                        recentProfileId = profileID;
                    }
                }
            }
            
            return recentProfileId;
        }
    }
}