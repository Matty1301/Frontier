using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveFileManager : Singleton<SaveFileManager>
{
    [Serializable]
    private struct SaveFile
    {
        public int highscore;
    }

    private string saveFilePath;
    private SaveFile saveFile;

    protected override void Awake()
    {
        base.Awake();
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveGame.json");
    }

    public int LoadHighScore()
    {
        int highScore = 0;
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            saveFile = JsonUtility.FromJson<SaveFile>(json);
            highScore = saveFile.highscore;
        }
        return highScore;
    }

    public void SaveHighScore(int newhighScore)
    {
        saveFile.highscore = newhighScore;
        string json = JsonUtility.ToJson(saveFile);
        File.WriteAllText(saveFilePath, json);
    }

    public void ClearHighScore()
    {
        if (File.Exists(saveFilePath))
            File.Delete(saveFilePath);
    }
}
