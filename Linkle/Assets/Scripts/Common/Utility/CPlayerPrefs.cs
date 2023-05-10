using UnityEngine;

namespace Common.Utility
{
    public static class CPlayerPrefs
    {
        public static bool GetBool(string key, bool defaultValue = false)
        {
            return HasKey(key) ? PlayerPrefs.GetInt(key) == 1 : defaultValue;
        }

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return HasKey(key) ? PlayerPrefs.GetInt(key) : defaultValue;
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static float GetFloat(string key, float defaultValue = 0)
        {
            return HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static string GetString(string key, string defaultValue = null)
        {
            return HasKey(key) ? PlayerPrefs.GetString(key) : defaultValue;
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}