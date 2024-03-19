using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{

    //CharacterController controller;
    public float speed = 8f;
    public float jumpHeight = 10f;
    public float gravity = 9.81f;
    public float airControl = 10f;
    bool canShoot = false;
    public float cameraDistance = 10f;

    WeaponController weapon;

    Vector3 input, moveDirection;

    Rigidbody rb;

    AudioSource jumpSound;
    Ray cameraRay;
    Plane groundPlane;
    GameObject bullet, bulletParent;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        weapon = GetComponent<WeaponController>();
        // controller = GetComponent<CharacterController>();
        bulletParent = GameObject.FindWithTag("BulletParent");
        jumpSound = GetComponent<AudioSource>();
        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    // fixed update
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 foreVector = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
        //Debug.Log("foreVector: " + foreVector);


        //rb.AddForce(foreVector * playerSpeed);
        if (moveHorizontal == 0 && moveVertical == 0)
        {
            // we dont want the player's x or z coordinates to change if the player is not moving.
            // WE MUST NOT SET THE RIGIDBODY TO KINEMATIC, OTHERWISE THE PLAYER WILL NOT BE AFFECTED BY GRAVITY
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        else
        {
            rb.isKinematic = false;
            rb.transform.position = rb.transform.position + foreVector * speed * Time.deltaTime;
        }
        
        /*
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpSound.Play();
        }
        */

        if (Input.GetButton("Fire1"))
        {
            weapon.Fire();
        }

        RotateWithMouse();
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

    // keep camera looking at player from top-down view
    void LateUpdate()
    {
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + cameraDistance, transform.position.z - 2f);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newPos, 0.9f);
        //Camera.main.transform.LookAt(transform);
    }
}
