using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameDataIO
{
    private static readonly string _path = Application.persistentDataPath + @"\GameData.dat";

    public static void SaveData(GameData data)
    {
        DataIOBase.SaveData(data, _path);
    }

    public static GameData LoadData()
    {
        return DataIOBase.LoadData<GameData>(_path) ?? new GameData();
    }
}

[Serializable]
public class GameData
{
    /// <summary>
    /// int  - Number of level
    /// bool - State of level (Blocekd or not)
    /// </summary>
    public Dictionary<int, bool> Levels = new();

    public bool ShowAds = true;

    public bool IsLevelBlocked(int levelNumber)
    {
        if (!Levels.TryGetValue(levelNumber, out bool isBlocked))
            return true;

        return isBlocked;
    }

    public void SetLevelStatus(int levelNumber, bool isBlocked)
    {
        if (!Levels.ContainsKey(levelNumber))
            Levels.Add(levelNumber, isBlocked);
        else
            Levels[levelNumber] = isBlocked;
    }
}