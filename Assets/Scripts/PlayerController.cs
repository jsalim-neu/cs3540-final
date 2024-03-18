using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bullet;
    public AudioClip shootSFX, dashSFX;
    public float playerSpeed = 5f;
    public float JUMP_FORCE = 50;

    public float bulletSpeed = 20f;

    public float bulletCooldown = 0.1f;

    public float dashSpeed = 20f, dashDuration = 0.5f, dashCooldown = 3f;

    float bulletRefresh;

    float dashTimeLeft = 0, dashRefresh = 0;

    Vector3 dashDirection;

    Rigidbody rb;
    TrailRenderer dashTrail;

    Ray cameraRay;
    Plane groundPlane;

    GameObject gunPoint;

    CharacterController controller;

    PlayerAnimation animHandler;

    // Start is called before the first frame update
    void Start()
    {
        bulletRefresh = 0;
        rb = GetComponent<Rigidbody>();
        dashTrail = GetComponent<TrailRenderer>();
        dashTrail.emitting = false;
        groundPlane = new Plane(Vector3.up, Vector3.zero);
        gunPoint = GameObject.FindGameObjectWithTag("Gunpoint");
        FishEnemyBehavior.bulletHeight = gunPoint.transform.position.y;
        controller = GetComponent<CharacterController>();
        animHandler = GetComponentInChildren<PlayerAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelManager.isGameOver) {
            RotateWithMouse();

            if (bulletRefresh <= 0) {
                Shoot();
            }
            else {
                bulletRefresh -= Time.deltaTime;
            }

            //handle dash input
            if (Input.GetKeyDown(KeyCode.Space) && dashRefresh <= 0)
            {
                StartDash();
            }
        }
        

    }

    void FixedUpdate()
    {
        if (!LevelManager.isGameOver) {
            //translate inputs to movement vector
            Vector3 input = SetDirection();

            Vector3 moveDirection = input;

            //set speed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerSpeed = 10f;
                animHandler.isRunning = true;
            }
            else
            {
                playerSpeed = 5f;
                animHandler.isRunning = false;
            }

            moveDirection *= playerSpeed;

            //apply gravity

            moveDirection.y -= 9.81f;

            //handle dash behavior
            if (dashTimeLeft > 0) {
                Dash(Time.deltaTime);
                dashTimeLeft -= Time.deltaTime;
            }
            //player is not dashing
            else {
                dashTrail.emitting = false;
                controller.Move(moveDirection * Time.deltaTime);
                dashRefresh -= Time.deltaTime;
                
            }
            dashTimeLeft = Mathf.Clamp(dashTimeLeft, 0, dashDuration);
            dashRefresh = Mathf.Clamp(dashRefresh, 0, dashCooldown);

            //report movement direction to animation handler
            animHandler.SetMoveDirection(input.normalized);
            
        }
        Debug.Log("Dash time left: " + dashTimeLeft + "; dash refresh: " + dashRefresh);

    }

    

    void RotateWithMouse() {
        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength)) {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            // Debug.Log("pointToLook: " + pointToLook);
            Debug.DrawLine(transform.position, pointToLook, Color.blue);
            pointToLook = new Vector3(pointToLook.x, transform.position.y, pointToLook.z);
            transform.LookAt(pointToLook);
            //report current rotation to animation handler
            animHandler.SetPlayerRotation((pointToLook - transform.position).normalized);
        }
    }

    // shoots a bullet clone from the player to the direction of the mouse pointer
    //now physics-based, without gravity
    void Shoot() {
        if (Input.GetMouseButton(0))
        {
            FireBullet();
            AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, 0.2f);
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

    void StartDash()
    {
        dashDirection = SetDirection();
        //if player is not moving, dash in forward direction
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }

        dashTimeLeft = dashDuration;
        dashRefresh = dashCooldown;
        dashTrail.emitting = true;
        AudioSource.PlayClipAtPoint(dashSFX, Camera.main.transform.position, 0.7f);
    }

    void Dash(float timeElapsed)
    {
        //apply gravity
        dashDirection.y -= 9.81f;
        controller.Move(dashDirection * timeElapsed * dashSpeed);
    }

    private Vector3 SetDirection()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        return (Vector3.right * moveHorizontal + Vector3.forward * moveVertical).normalized;
    }

    /*

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
    
    void Shoot() {
        bullet.transform.position = transform.position;
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 10;
    }
    */
}