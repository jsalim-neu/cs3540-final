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
    public GameObject homingProjectilePrefab;
    public GameObject throwablePrefab;
    public GameObject pulsePrefab;

    public float dashSpeed = 20f, dashDuration = 0.5f, dashCooldown = 3f;

    float bulletRefresh;

    float dashTimeLeft = 0, dashRefresh = 0;

    Vector3 dashDirection;

    Rigidbody rb;
    TrailRenderer dashTrail;

    Ray cameraRay;
    Plane groundPlane;
    ThrowableBehaviour tb;
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
        bulletSpeed = bulletSpeed * 1;

        if (throwablePrefab != null)
        {
            tb = throwablePrefab.GetComponent<ThrowableBehaviour>();
        }
        gunPoint = GameObject.FindGameObjectWithTag("Gunpoint");
        FishEnemyBehavior.bulletHeight = gunPoint.transform.position.y;
        controller = GetComponent<CharacterController>();
        animHandler = GetComponentInChildren<PlayerAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelManager.isGameOver)
        {
            RotateWithMouse();

            if (bulletRefresh <= 0)
            {
                Shoot();
                Pulse();
            }
            else
            {
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
            if (!LevelManager.isGameOver)
            {
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
                if (dashTimeLeft > 0)
                {
                    Dash(Time.deltaTime);
                    dashTimeLeft -= Time.deltaTime;
                }
                //player is not dashing
                else
                {
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

        void RotateWithMouse()
        {
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayLength;

            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                // Debug.Log("pointToLook: " + pointToLook);
                Debug.DrawLine(transform.position, pointToLook, Color.blue);
                pointToLook = new Vector3(pointToLook.x, transform.position.y, pointToLook.z);
                transform.LookAt(pointToLook);
                //report current rotation to animation handler
                animHandler.SetPlayerRotation((pointToLook - transform.position).normalized);
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
        void Shoot()
        {
            if (Input.GetMouseButton(0))
            {
                FireBullet(bullet, bulletSpeed);
                AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, 0.2f);
                bulletRefresh = bulletCooldown;
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                ShootHoming();
                AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, 0.2f);
                bulletRefresh = bulletCooldown;
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                FireBullet(throwablePrefab);
                bulletRefresh = bulletCooldown;
            }
        }

        //fires a singular bullet/throws a throwable object
        void FireBullet(GameObject gameObject, float speed = 5)
        {
            Vector3 offset = new Vector3(0.1f, 0, 0.1f);

            GameObject bulletClone = Instantiate(
              gameObject,
              gunPoint.transform.position,
              gunPoint.transform.rotation
            ) as GameObject;

            Rigidbody rb = bulletClone.GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

            bulletClone.transform.SetParent(
                GameObject.FindGameObjectWithTag("BulletParent").transform
            );

            if (bulletClone.gameObject.tag == "Throwable")
            {
                bulletClone.GetComponent<ThrowableBehaviour>().TriggerThrowable();
                Destroy(bulletClone, 10f);
            }
            else
            {
                Destroy(bulletClone, 2f);
            }
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

        Vector3 SetDirection()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            return (Vector3.right * moveHorizontal + Vector3.forward * moveVertical).normalized;
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
    }