using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject bullet;
    public AudioClip shootSFX, dashSFX, homingSFX;
    public float playerSpeed = 5f;
    public float JUMP_FORCE = 50;
    public float bulletSpeed = 20f;
    public float bulletCooldown = 0.1f, pulseCooldown = 1f;
    public GameObject homingProjectilePrefab;
    public GameObject throwablePrefab;
    public GameObject pulsePrefab;

    public float dashSpeed = 20f, dashDuration = 0.5f, dashCooldown = 3f;

    float bulletRefresh, pulseRefresh;

    float dashTimeLeft = 0, dashRefresh = 0;
    
    Vector3 knockbackDirection; public float knockbackTimer = 0.75f;


    UIController ui;

    Vector3 dashDirection;

    Rigidbody rb;
    TrailRenderer dashTrail;

    Ray cameraRay;
    Plane groundPlane;
    ThrowableBehaviour tb;
    GameObject gunPoint;

    public CharacterController controller;
    PlayerAnimation animHandler;

    // Start is called before the first frame update
    void Start()
    {
        bulletRefresh = 0;
        pulseRefresh = 0;
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
        controller = GetComponent<CharacterController>();
        animHandler = GetComponentInChildren<PlayerAnimation>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();

    }

    // Update is called once per frame
    void Update()
    {
        FishEnemyBehavior.bulletHeight = gunPoint.transform.position.y;
        if (!LevelManager.isGameOver & !ShopMenuBehavior.isGamePaused)
        {
            RotateWithMouse();

            if (bulletRefresh <= 0)
            {
                Shoot();
            }
            if (FlagManager.playerHasPulse && pulseRefresh <= 0)
            {
                Pulse();
            }
            bulletRefresh -= Time.deltaTime;
            bulletRefresh = Mathf.Clamp(bulletRefresh, 0, bulletCooldown);
            pulseRefresh -= Time.deltaTime;
            pulseRefresh = Mathf.Clamp(pulseRefresh, 0, pulseCooldown);
            //handle dash input
            if (Input.GetKeyDown(KeyCode.Space) && dashRefresh <= 0)
            {
                StartDash();
            }


        }

        SetSliders();
    }

        void FixedUpdate()
        {
            if (!LevelManager.isGameOver)
            {
                //translate inputs to movement vector
                Vector3 input = SetDirection();
                Vector3 moveDirection = input;

                //set speed
                if (Input.GetKey(KeyCode.LeftShift) & knockbackTimer <= 0)
                {
                    playerSpeed = 10f;
                    animHandler.isRunning = true;
                }
                else
                {
                    playerSpeed = 5f;
                    animHandler.isRunning = false;
                }

                if (knockbackTimer > 0)
                {
                    //hijack player movement to get knocked back
                    moveDirection = knockbackDirection * (knockbackTimer * 10f);
                    knockbackTimer -= Time.deltaTime;
                    knockbackTimer = Mathf.Clamp(knockbackTimer,0, 100);
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
                pulseRefresh = pulseCooldown;
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
            else if (Input.GetKeyDown(KeyCode.V) && FlagManager.playerHasHoming)
            {
                ShootHoming();
                AudioSource.PlayClipAtPoint(homingSFX, Camera.main.transform.position, 0.2f);
                bulletRefresh = bulletCooldown;
            }
            else if (Input.GetKeyDown(KeyCode.G) && FlagManager.playerHasGrenades)
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
                Debug.Log(controller.velocity);
                bulletClone.GetComponent<ThrowableBehaviour>().TriggerThrowable();
                //throw with more force, correct for player trajectory
                rb.AddForce(
                    transform.forward * speed * 0.75f + controller.velocity.normalized * 5, 
                    ForceMode.VelocityChange);
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
            
            GameObject homingProjectile = Instantiate(
                    homingProjectilePrefab,
                    gunPoint.transform.position,
                    gunPoint.transform.rotation
                );

            homingProjectile.transform.SetParent(
                    GameObject.FindGameObjectWithTag("BulletParent").transform
            );

            StartCoroutine(FireHomingProjectile(
                homingProjectile,
                homingTarget,
                1.5f
            ));

        }

        IEnumerator FireHomingProjectile(GameObject homingProjectile, GameObject homingTarget, float duration)
        {
            float time = 0;
            Transform tr = homingProjectile.transform;
            //is the projectile locked on to the enemy?
            bool lockedOn = false;

            while (homingProjectile != null)
            {
                if (homingTarget != null)
                {
                    //go forward, turn toward target gradually
                    Quaternion toRotation = Quaternion.FromToRotation(tr.forward, homingTarget.transform.position - tr.position);
                    //if the angle between the homing trajectory and the target direction is close enough, lock on
                    if (
                        (Vector3.Angle(tr.forward, homingTarget.transform.position - tr.position) <= 7.5f)
                        || Vector3.Distance(tr.position, homingTarget.transform.position) <= 3f)
                    {
                        lockedOn = true;
                    }

                    if (lockedOn)
                    {
                        tr.LookAt(homingTarget.transform);
                    }
                    else {
                        //rotation turns gradually toward enemy
                        tr.rotation = Quaternion.Slerp(
                            tr.rotation,
                            toRotation,
                            time / duration
                        );
                    }

                    if (time >= duration)
                    {
                        //lock on after a certain period
                        lockedOn = true;
                    }
                }
                // if the target is destroyed, let the projectile continue in the same direction; otherwise, move in turned direction
                tr.position = Vector3.MoveTowards(
                    tr.position,
                    tr.position + (tr.forward * 100),
                    Time.deltaTime * bulletSpeed * 0.7f
                );
                
                time += Time.deltaTime;
                yield return null;
            }

        }

        GameObject GetHomingTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject homingTarget = null;
            float minDistance = 65535f;

            //aim according to where the mouse is
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.transform.position.y - 2;

            Vector3 aimPosition = Camera.main.ScreenToWorldPoint(mousePos);            

            //loop through enemies, choose the one with the furthest position from the cursor
            foreach (GameObject enemy in enemies)
            {
                float dist = Vector3.Distance(enemy.transform.position, aimPosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    homingTarget = enemy;
                }
            }

            return homingTarget;
        }

        private void SetSliders()
        {
            
            //all slider values are capped between [0,1]
            ui.SetSlider(UISlider.RELOAD, (bulletCooldown - bulletRefresh)/bulletCooldown);
            ui.SetSlider(UISlider.DASH, (dashCooldown - dashRefresh)/dashCooldown);
            ui.SetSlider(UISlider.HOMING, (bulletCooldown - bulletRefresh)/bulletCooldown);
            ui.SetSlider(UISlider.GRENADE, (bulletCooldown - bulletRefresh)/bulletCooldown);
            ui.SetSlider(UISlider.PULSE, (pulseCooldown - pulseRefresh)/pulseCooldown);
        }

        public void Knockback(Vector3 moveDirection, float knockbackTime = 0.5f)
        {
            Debug.Log("Starting knockback in playercontroller");
            knockbackTimer = knockbackTime;
            knockbackDirection = moveDirection;

            
        }
    }