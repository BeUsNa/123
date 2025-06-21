using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class TowerData
{
    public string type;
    public Vector3 position;
    public int level;
}

[System.Serializable]
public class SaveData
{
    public string sceneName;
    public int wave;
    public int money;
    public int lives;
    public List<TowerData> towers;
}

public static class SaveSystem
{
    public static bool shouldLoadFromSave = false; // <- важно!
    private static readonly string filePath = Application.persistentDataPath + "/savegame.json";

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(filePath)) return null;
        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool SaveExists()
    {
        return File.Exists(filePath);
    }
}