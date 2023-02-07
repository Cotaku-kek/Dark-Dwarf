using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public Transform[] backgrounds;                                                                                                     // Array (list) for all kind of moving images, backgrounds, foregrounds
    private float[] parallaxScales;                                                                                                     // Proportion of the camera movement
    public float backgroundSmoothing;                                                                                                             // How smooth the parallax is moving

    public Transform ground;
    private float groundScales;
    public float groundSmoothing;

    private Transform camera;                                                                                                           // Reference to the main cameras transform
    private Vector3 previousCamPos;                                                                                                     // Position of the camera in the previous frame

    void Awake()                                                                                                                        // Awake is called before the Start Method
    {
        camera = Camera.main.transform;                                                                                                 // Set up the references above
    }

    void Start()
    {
        previousCamPos = camera.position;                                                                                               // Previous frame is equal to the current frames camera position
        
        parallaxScales = new float[backgrounds.Length];                                                                                 // Parallax list is as long as the backgrounds
        for(int i = 0; i < backgrounds.Length; i++) {
            parallaxScales[i] = backgrounds[i].position.z*-1;
        }

        groundScales = new float();                                                                                                     
        groundScales = ground.position.z*-1;    
    }

    void Update()                                                                                                                       
    {
        previousCamPos = camera.position;
        ParallaxMoving();
    }

    void ParallaxMoving()
    {
        BackgroundMoving();
        GroundBackgroundMoving();
    }

    void BackgroundMoving()
    {
        for(int i = 0; i < backgrounds.Length; i++) {                                                                                           // For each background
            float parallax = (previousCamPos.x - camera.position.x) * parallaxScales[i];                                                        // Parallax is the opposite of the camera position
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;
            Vector3 backgroundTargetPos = new Vector3 (backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);
            backgrounds[i].position = Vector3.Lerp (backgrounds[i].position, backgroundTargetPos, backgroundSmoothing * Time.deltaTime);        // deltaTime converts frames to seconds
        }
    }

    void GroundBackgroundMoving()
    {
        float groundParallax = (previousCamPos.x - camera.position.x) * groundScales;                                                   
        float groundTargetPosX = ground.position.x + groundParallax;
        Vector3 groundTargetPos = new Vector3 (groundTargetPosX, ground.position.y, ground.position.z);
        ground.position = Vector3.Lerp (ground.position, groundTargetPos, groundSmoothing * Time.deltaTime);
    }
}
