using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    public float SPEED = 800f;
    public Light pointLight;
    public Light spotLight;

    // Fields for the Point Light and Spot Light
    void Start() {
        /*
        pointLight = GetComponent<Light>();
        spotLight = GetComponent<Light>();
        */
    }

    /*
    For the Point Light, change its color smoothl through interpolation. Use the code snippet:
    pointLight.color = Color.Lerp(Color.magenta, Color.yellow, Mathf.PingPong(Time.time, 1));

    As for the Spot Light, change its intensity constantly during the gameplay:
    spotLight.intensity = Mathf.PingPong(Time.time, 8);
    */
    void Update() {
        //Debug.Log("Time.time: " + Time.time);
        pointLight.color = Color.Lerp(Color.magenta, Color.yellow, Mathf.PingPong(Time.time, 1));
        spotLight.intensity = Mathf.PingPong(Time.time, 8);
    }
}
