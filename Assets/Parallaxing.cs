using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public Transform[] backgrounds;
    private float[] parallaxScales;         //The proportion of the camres movement to move the backgrounds by 
    public float smoothing = 1;                 //How smooth the parralax is going to be. Make sure to set this above 0.

    private Transform cam;                  //Reference to the main camera transform.
    private Vector3 previousCamPosition;        //Store the position of the camera in the previous frame.

    //Called before start(). Great for references.
    private void Awake()
    {
        //Set up camera reference
        cam = Camera.main.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        //The previous frame had the current frame's camera position
        previousCamPosition = cam.position;

        parallaxScales = new float[backgrounds.Length];

        //Assigning corresponding parallax scales
        for(int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
    } 

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < backgrounds.Length; i++)
        {
            //The parallax is the oposite of the camera movement because the previous frame * scale
            float parallaxX = (previousCamPosition.x - cam.position.x) * parallaxScales[i];

            //Set a target x position which is the current position * parallax
            float backgroundTargetPositionX = backgrounds[i].position.x + parallaxX;

            //Create a target position wich is the background's current position with it's target x position
            Vector3 backgroundTargetPosition = new Vector3(backgroundTargetPositionX, backgrounds[i].position.y, backgrounds[i].position.z);

            //Fade between current position and target positon using lerp
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPosition, smoothing * Time.deltaTime);
        }

        previousCamPosition = cam.position;
    }
}
