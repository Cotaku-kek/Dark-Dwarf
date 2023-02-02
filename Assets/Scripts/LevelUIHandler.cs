using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUIHandler : MonoBehaviour
{
    public void BacktoMenu()
    {
        SceneManager.LoadScene(0);
    }
}