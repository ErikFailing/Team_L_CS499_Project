using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class SaveSystem
{

    //////////////INITIALIZATION/////////////////////

    /// <summary>
    /// Searches through the persistantDataPath for each game file and returns a list of all their names
    /// </summary>
    /// <returns></returns>
    public static List<string> GetSaves()
    {
        //Search game file folder and add each game file to a list of strings
        FileInfo[] fileInfo = new DirectoryInfo(Application.persistentDataPath).GetFiles("*.*");
        List<string> saveNames = new List<string>();
        foreach (FileInfo f in fileInfo)
        {
            //If it is a save file add it's name to a list
            if (f.Name.Contains("Save_"))
            {
                string saveName = f.Name.Replace("Save_", "");
                saveName = saveName.Replace(".json", "");
                saveNames.Add(saveName);
            }
        }
        return saveNames;
    }

    /// <summary>
    /// Takes in a saveName and returns a savePath string
    /// </summary>
    /// <param name="saveName"></param>
    /// <returns></returns>
    public static string SaveNameToFilePath(string saveName)
    {
        return Path.Combine(Application.persistentDataPath, "Save_" + saveName + ".json");
    }

    /// <summary>
    /// Takes in a saveFileName and deletes the file
    /// </summary>
    /// <param name="saveName"></param>
    public static void DeleteSaveFile(string saveName)
    {
        File.Delete(SaveNameToFilePath(saveName));
    }

    /// <summary>
    /// Takes the saveName and creates a json from it
    /// Returns the actual save name after changing it so a file isn't overwritten
    /// </summary>
    /// <param name="initialGameData"></param>
    public static string CreateSaveFile(string saveName)
    {
        string saveFileName, path;
        int count = 0;
        do
        {
            if (count == 0) saveFileName = "Save_" + saveName + ".json";
            else saveFileName = "Save_" + saveName + "_" + count + ".json";
            path = Path.Combine(Application.persistentDataPath, saveFileName);
            count += 1;
        } while (File.Exists(path));

        using (File.CreateText(path)) { }
        Debug.Log("Save file created at: " + path);

        if (count == 1) return saveName;
        else return saveName + "_" + (count - 1);

    }




    //////////////LOADING//////////////////////

    /// <summary>
    /// Loads the game at the currentGameFilePath into the scene
    /// </summary>
    public static void LoadSaveFile(string saveName)
    {
        string path = SaveNameToFilePath(saveName);

        Model.Data data;
        //Reads GameData from currentGameFilePath into a gameData
        using (StreamReader streamReader = File.OpenText(path))
        {
            string jsonString = streamReader.ReadToEnd();
            data = JsonUtility.FromJson<Model.Data>(jsonString);
            Debug.Log("Loading save from:" + path);
        }

        // Do all the things needed to load the saved data
        Ref.I.Model.RemoveEverything();

        Ref.I.Model.data.name = data.name;
        Ref.I.Model.data.pathingVersion = data.pathingVersion;
        foreach (var go in data.Rooms)
        {
            Ref.I.Model.AddRoomIfValid(go);
        }
        foreach (var go in data.Chests)
        {
            Ref.I.Model.AddChestIfValid(go);
        }
        foreach (var go in data.Tables)
        {
            Ref.I.Model.AddTableIfValid(go);
        }
        foreach (var go in data.Doors)
        {
            Ref.I.Model.AddDoorIfValid(go);
        }
        foreach (var go in data.VacuumStation)
        {
            Ref.I.Model.AddVacuumIfValid(go);
        }
        Ref.I.Model.ChangeFloorType(data.FlooringType);
        Ref.I.Model.data.RandomPaths = data.RandomPaths;
        Ref.I.Model.data.SpiralPaths = data.SpiralPaths;
        Ref.I.Model.data.WallfollowPaths = data.WallfollowPaths;
        Ref.I.Model.data.SnakingPaths = data.SnakingPaths;


        for (int i = 0; i < data.cleanablePointsVectors.Count; i++)
        {
            Ref.I.Model.cleanablePoints.Add(data.cleanablePointsVectors[i], 0);
        }

    }





    /////////////////SAVING///////////////////

    /// <summary>
    /// Saves the game to the currentGameFilePath
    /// </summary>
    public static void SaveToFile(string saveName)
    {
        // Get path name
        string path = SaveNameToFilePath(saveName);

        //Pre-save work

        foreach (Vector2 key in Ref.I.Model.cleanablePoints.Keys)
        {
            Ref.I.Model.data.cleanablePointsVectors.Add(key);
        }

        // Save file
        string jsonString = JsonUtility.ToJson(Ref.I.Model.data, true);
        using (StreamWriter streamWriter = new StreamWriter(path, false))
        {
            streamWriter.Write(jsonString);
        }
        Debug.Log("File saved to file at: " + path);
    }




}
