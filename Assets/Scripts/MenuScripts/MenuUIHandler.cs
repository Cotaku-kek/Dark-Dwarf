using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class MenuUIHandler : MonoBehaviour
{
    // Variables
    public Button newGameButton;
    public Button loadGameButton;
    public Button exitGameButton;
    public Button startGameButton;
    public TextMeshProUGUI characterNameText;
    public GameObject characterNameField;

    public string characterName;

    // Update is called once per frame
    void update()
    {
        
    }

    public void StartGame()                                                 // Switch Scene to Main Game on Button Click
    {
        SceneManager.LoadScene(1);
    }

    public void NewGame()                                                   // Get Input Field for Name
    {
        characterNameField.gameObject.SetActive(true);
    }

    public void GetName()                                                   // Get Text From Input Field and spawn Start!
    {
        characterName = characterNameText.text;
        if(characterNameText.text != null)
            startGameButton.gameObject.SetActive(true);
    }

    public void SaveGame()                                                  // Trigger Save in Main manager
    {
        MainManager.Instance.SaveGame();
    }

    public void LoadGame()                                                  // Trigger Load in Main manager
    {
        MainManager.Instance.LoadGame();
    }

    public void Exit()                                                      // Close the Game, or if in editor stop simulation
    {
        MainManager.Instance.SaveGame();
        #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();                           // code to quit simulation
        #else
                Application.Quit();                                         // code to quit Unity player
        #endif
    }
}
