using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public string characterName;
    public Vector3 characterPostion;

    private void Awake()                                                    // Start a singleton MainManager Instance for all Scenes
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    
    [System.Serializable] class SaveData                                    // Serialize Data for Saving
    {
        public string characterName;
        public Vector3 characterPostion;
    }

        public void SaveGame()                                              // Save Game Procedure
    {
        SaveData data = new SaveData();
        data.characterName = characterName;
        data.characterPostion = characterPostion;

        string json = JsonUtility.ToJson(data);
    
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadGame()                                                  // Load Game Procedure
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            characterName = data.characterName;
            characterPostion = data.characterPostion;
        }
    }
}