using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private PlayerInputActions playerInputActions;                  // name of Input System
    private InputAction menu;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private bool isPaused;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        menu = playerInputActions.UI.Menu;                          // Path from our Input System
        menu.Enable();

        menu.performed += PauseGame;
    }

    void OnDisable()
    {
        menu.Disable();
    }

    void PauseGame(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;

        if(isPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }

    void ActivateMenu()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        isPaused = false;
    }
}
