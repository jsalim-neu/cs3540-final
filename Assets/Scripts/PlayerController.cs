using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bullet;
    public float playerSpeed = 5f;
    public float JUMP_FORCE = 50;

    public float bulletSpeed = 20f;

    public float bulletCooldown = 0.1f;

    float bulletRefresh;

    Rigidbody rb;
    AudioSource jumpSound;
    Ray cameraRay;
    Plane groundPlane;

    GameObject gunPoint;

    CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        bulletRefresh = 0;
        rb = GetComponent<Rigidbody>();
        jumpSound = GetComponent<AudioSource>();
        groundPlane = new Plane(Vector3.up, Vector3.zero);
        gunPoint = GameObject.FindGameObjectWithTag("Gunpoint");
        FishEnemyBehavior.bulletHeight = gunPoint.transform.position.y;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        RotateWithMouse();

        if (bulletRefresh <= 0) {
            Shoot();
        }
        else {
            bulletRefresh -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!LevelManager.isGameOver) {
            //translate inputs to movement vector
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 moveDirection = (Vector3.right * moveHorizontal + Vector3.forward * moveVertical).normalized;

            //set speed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerSpeed = 10f;
            }
            else
            {
                playerSpeed = 5f;
            }

            moveDirection *= playerSpeed;

            //apply gravity

            moveDirection.y -= 9.81f;

            controller.Move(moveDirection * Time.deltaTime);

            // rotate in the forward direction
            // transform.rotation = Quaternion.LookRotation(foreVector);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Jump();
            }

            
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
    //now physics-based, without gravity
    void Shoot() {
        if (Input.GetMouseButton(0))
        {
            FireBullet();
            bulletRefresh = bulletCooldown;
        }
    }

    //fires a singular bullet
    void FireBullet() {
        Vector3 offset = new Vector3(0.1f, 0, 0.1f);

        GameObject bulletClone = Instantiate(bullet, gunPoint.transform.position, gunPoint.transform.rotation) as GameObject;

        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);

        bulletClone.transform.SetParent(
            GameObject.FindGameObjectWithTag("BulletParent").transform
        );

        Destroy(bulletClone, 2f);

    }

    private void Jump()
    {
        if (rb.velocity.y < 0.1f && rb.velocity.y > -0.1f)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.transform.position = rb.transform.position + Vector3.up * JUMP_FORCE * Time.deltaTime;
            //rb.AddForce(Vector3.up * JUMP_FORCE, ForceMode.Impulse);
        }
    }
    // shoots a bullet clone from the player to the direction of the player is facing
    /*
    void Shoot() {
        bullet.transform.position = transform.position;
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 10;
    }
    */
}