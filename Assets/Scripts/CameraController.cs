using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    void Update()
    {
        // Updates the position of the camera every frame, it changes only in x-achse
        transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);                            
    }
}
