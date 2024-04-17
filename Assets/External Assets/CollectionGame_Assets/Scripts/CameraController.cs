using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;
    Vector3 offset;

    public float cameraHeight = 10f;

    public bool isTopDown = true;
    public float MouseCameraSensitivity = 1f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTopDown)
        {
            cameraHeight = LevelManager.cameraHeight;
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + cameraHeight, player.transform.position.z - 4f);
            transform.rotation = Quaternion.Euler(70, 0, 0);
        }
        else
        {
            transform.position = player.transform.position + offset;
            RotateWithMouse(Input.GetAxis("Mouse X") * MouseCameraSensitivity);
            PitchCamera(-Input.GetAxis("Mouse Y") * MouseCameraSensitivity);
        }
        // transform.position = player.transform.position + offset;

    }

    public void RotateWithMouse(float x) {
        transform.RotateAround(player.transform.position, Vector3.up, x);
    }

    void PitchCamera(float angle) {
        transform.RotateAround(player.transform.position, transform.right, angle);
    }
}
