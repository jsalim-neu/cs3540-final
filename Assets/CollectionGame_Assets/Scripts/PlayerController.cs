using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float SPEED = 5f;
    public float JUMP_FORCE = 50;
    bool canShoot = false;

    Rigidbody rb;
    AudioSource jumpSound;
    Ray cameraRay;
    Plane groundPlane;
    GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        bullet = GameObject.Find("Bullet");
        rb = GetComponent<Rigidbody>();
        jumpSound = GetComponent<AudioSource>();
        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        RotateWithMouse();

        // if the player presses N then they will shoot a bullet from the player to the direction they are facing
        // bullet is Kinematic so it will not be affected by gravity
        if (Input.GetKeyDown(KeyCode.N))
        {
            canShoot = true;
        }
    }

    void FixedUpdate()
    {
        if (LevelManager.isGameOver) {
            //rb.velocity = Vector3.zero;
            // rb.angularVelocity = Vector3.zero;
            return;
        }
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 foreVector = new Vector3(moveHorizontal, 0.0f, moveVertical);
        //Debug.Log("foreVector: " + foreVector);


        //rb.AddForce(foreVector * SPEED);

        rb.transform.position = rb.transform.position + foreVector * SPEED * Time.deltaTime;

        // rotate in the forward direction
        // transform.rotation = Quaternion.LookRotation(foreVector);

        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (rb.velocity.y < 0.1f && rb.velocity.y > -0.1f)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.transform.position = rb.transform.position + Vector3.up * JUMP_FORCE * Time.deltaTime;
                //rb.AddForce(Vector3.up * JUMP_FORCE, ForceMode.Impulse);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            SPEED = 10f;
        }
        else
        {
            SPEED = 5f;
        }

        if (canShoot) {
            ShootToMouseDirection();
        }
    }

    void RotateWithMouse() {
        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength)) {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            // Debug.Log("pointToLook: " + pointToLook);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }

    // shoots a bullet clone from the player to the direction of the mouse pointer
    // bullet is Kinematic so it will not be affected by gravity
    void ShootToMouseDirection() {
        GameObject bulletClone = Instantiate(bullet, transform.position, transform.rotation);

        Vector3 bulletDirection = cameraRay.direction;
        bulletDirection.y = 0;
        bulletClone.transform.position = transform.position;
        bulletClone.GetComponent<Rigidbody>().velocity = bulletDirection * 1000;

        Debug.Log("bulletDirection: " + bulletDirection);
        Destroy(bulletClone, 2);
    }

    // shoots a bullet clone from the player to the direction of the player is facing
    void Shoot() {
        bullet.transform.position = transform.position;
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 10;
        canShoot = false;
    }
}
