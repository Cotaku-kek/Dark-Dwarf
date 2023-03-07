using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMechanic : MonoBehaviour
{
    [SerializeField] GameObject switchOn, switchOff, doorOpen, doorClosed;
    [SerializeField] bool toggle = false;

    public void ToggleSwitch()
    {
        if (!toggle)
        {
            switchOn.SetActive(true);
            switchOff.SetActive(false);
            doorOpen.SetActive(true);
            doorClosed.SetActive(false);
            toggle = true;
        }
        else if (toggle)
        {
            switchOn.SetActive(false);
            switchOff.SetActive(true);
            doorOpen.SetActive(false);
            doorClosed.SetActive(true);
            toggle = false;
        }
        Debug.Log("My state is " + toggle);
    }
}