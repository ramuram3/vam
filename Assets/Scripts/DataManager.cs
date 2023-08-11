using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameData
{
    public bool[] hasKey = new bool[4];
    public int clearStage;
}
public class DataManager : MonoBehaviour
{
    public GameData gameData = new GameData();

    string path;
    string gameDataFilename = "saveGameData";
    void Awake()
    {
        path = Application.persistentDataPath + "/";
        gameData.hasKey[0] = true;
        gameData.hasKey[1] = true;
    }

    public void CheckHasKey()
    {
        if (gameData.clearStage >= 1)
        {
            gameData.hasKey[2] = true;
        }

        if (gameData.clearStage >= 3)
        {
            gameData.hasKey[3] = true;
        }
    }


    public void Save()
    {
        string _gameData = JsonUtility.ToJson(gameData);
        File.WriteAllText(path + gameDataFilename, _gameData);
    }

    public void Load()
    {
        if(!File.Exists(path + gameDataFilename))
        {
            return;
        }
        string _gameData = File.ReadAllText(path + gameDataFilename);
        if (_gameData == null) return;
        gameData = JsonUtility.FromJson<GameData>(_gameData);
    }
}
