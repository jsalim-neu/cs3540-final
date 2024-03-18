using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 5f;
    public float JUMP_FORCE = 50;
    public float bulletSpeed = 20f;
    public float bulletCooldown = 0.1f;
    public GameObject bullet;
    public GameObject homingProjectilePrefab;
    public GameObject throwablePrefab;
    public GameObject pulsePrefab;

    float bulletRefresh;

    Rigidbody rb;
    AudioSource jumpSound;
    Ray cameraRay;
    Plane groundPlane;
    ThrowableBehaviour tb;

    // Start is called before the first frame update
    void Start()
    {
        bulletRefresh = 0;
        rb = GetComponent<Rigidbody>();
        jumpSound = GetComponent<AudioSource>();
        groundPlane = new Plane(Vector3.up, Vector3.zero);
        bulletSpeed = bulletSpeed * 1;

        if (throwablePrefab != null)
        {
            tb = throwablePrefab.GetComponent<ThrowableBehaviour>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        RotateWithMouse();

        if (bulletRefresh <= 0) {
            Shoot();
            Pulse();
        }
        else {
            bulletRefresh -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!LevelManager.isGameOver) {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 foreVector = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
            //Debug.Log("foreVector: " + foreVector);


            //rb.AddForce(foreVector * playerSpeed);

            rb.transform.position = rb.transform.position + foreVector * playerSpeed * Time.deltaTime;

            // rotate in the forward direction
            // transform.rotation = Quaternion.LookRotation(foreVector);

            // if there is no input, stop the player
            if (moveHorizontal == 0 && moveVertical == 0)
            {
                rb.velocity = Vector3.zero;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerSpeed = 10f;
            }
            else
            {
                playerSpeed = 5f;
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

    void Pulse()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (pulsePrefab == null) return;

            GameObject pulse = Instantiate(
                pulsePrefab,
                transform.position,
                transform.rotation
            );

            Destroy(pulse, 2f);
            bulletRefresh = bulletCooldown;
        }
    }

    // shoots a bullet clone from the player to the direction of the mouse pointer
    //now physics-based, without gravity
    void Shoot() {
        if (Input.GetMouseButton(0))
        {
            FireBullet(bullet, bulletSpeed);
            bulletRefresh = bulletCooldown;
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            ShootHoming();
            bulletRefresh = bulletCooldown;
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            FireBullet(throwablePrefab);
            bulletRefresh = bulletCooldown;
        }
    }

    //fires a singular bullet/throws a throwable object
    void FireBullet(GameObject gameObject, float speed = 5) {
        Vector3 offset = new Vector3(0.1f, 0, 0.1f);

        GameObject bulletClone = Instantiate(
            gameObject,
            transform.position + transform.forward + offset,
            transform.rotation
        ) as GameObject;

        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

        bulletClone.transform.SetParent(
            GameObject.FindGameObjectWithTag("BulletParent").transform
        );

        if (bulletClone.gameObject.tag == "Throwable")
        {
            print("throwable");
            bulletClone.GetComponent<ThrowableBehaviour>().TriggerThrowable();
            Destroy(bulletClone, 10f);
        }
        else
        {
            Destroy(bulletClone, 2f);
        }
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

    void ShootHoming()
    {
        GameObject homingTarget = GetHomingTarget();

        if (homingTarget != null)
        {
            GameObject homingProjectile = Instantiate(
                homingProjectilePrefab,
                transform.position,
                transform.rotation
            );

            float distanceToTravel = Vector3.Distance(
                homingProjectile.transform.position,
                homingTarget.transform.position
            );

            StartCoroutine(FireHomingProjectile(
                homingProjectile,
                homingTarget,
                distanceToTravel / bulletSpeed
            ));
        }
    }

    IEnumerator FireHomingProjectile(GameObject homingProjectile, GameObject homingTarget, float duration)
    {
        float time = 0;
        Vector3 startPos = homingProjectile.transform.position;

        while (time < duration)
        {
            if (homingTarget != null)
            {
                homingProjectile.transform.position = Vector3.Lerp(
                    startPos,
                    homingTarget.transform.position,
                    time / duration
                );

                time += Time.deltaTime;
                yield return null;
            }
            else
            {
                // if the target is destroyed, let the projectile continue in the same direction
                homingProjectile.transform.position = Vector3.Lerp(
                    startPos,
                    startPos + homingProjectile.transform.forward * 100,
                    time / duration
                );
                time += Time.deltaTime;
                yield return null;
            }
        }

        homingProjectile.transform.position = homingTarget.transform.position;
    }

    GameObject GetHomingTarget()
    {
        GameObject homingTarget = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Enemy")
            {
                homingTarget = hit.collider.gameObject;
            }
        }

        return homingTarget;
    }

    // shoots a bullet clone from the player to the direction of the player is facing
    /*
    void Shoot() {
        bullet.transform.position = transform.position;
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 10;
    }
    */
}