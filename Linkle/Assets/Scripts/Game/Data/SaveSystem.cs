using System;
using System.IO;
using ProtoBuf;
using UnityEngine;

namespace Game.Data
{
    public static class SaveSystem
    {
        public static PlayerData LoadPlayerData(string fileName)
        {
            return Deserialize<PlayerData>(fileName);
        }
        
        public static void SavePlayerData(PlayerData playerData, string fileName)
        {
            Serialize(playerData, fileName);
        }
        
        public static LevelSaveDatabase LoadLevelDatabase(string fileName)
        {
            return Deserialize<LevelSaveDatabase>(fileName);
        }
        
        public static void SaveLevelDatabase(LevelSaveDatabase levelSaveDatabase, string fileName)
        {
            Serialize(levelSaveDatabase, fileName);
        }
        
        public static void Serialize<T>(T data, string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);
            try
            {
                using var stream = new FileStream(path, FileMode.Create);
                Serializer.Serialize<T>(stream, data);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Serialize '{typeof(T).Name}' to '{path}' failed ==> {e.Message}");
            }
        }
        
        public static T Deserialize<T>(string fileName)
        {
            T data = default;
            var path = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(path))
            {
                try
                {
                    using var stream = new FileStream(path, FileMode.Open);
                    data = Serializer.Deserialize<T>(stream);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Deserialize '{typeof(T).Name}' to '{path}' failed ==> {e.Message}");
                }
            }
            return data;
        }
    }
}
