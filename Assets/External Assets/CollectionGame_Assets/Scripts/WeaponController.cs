using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject bullet;
    public float bulletSpeed = 100f;
    public float timebetweenShots = 0.3f;
    public bool canShoot = true;
    public bool isFullAuto = false;

    public float range = 100f;

    GameObject newBullet;
    Transform bulletDebris;

    void Start()
    {
        if (bullet == null)
        {
            bullet = GameObject.FindGameObjectWithTag("Bullet");
        }

        bulletDebris = GameObject.Find("BulletDebris").transform;
    }

    // fires a bullet in the direction the player is facing
    public void Fire()
    {
        if (canShoot)
        {
            newBullet = Instantiate(bullet, transform.position, transform.rotation);

            // put bullet in the bulletDebris group
            newBullet.transform.SetParent(bulletDebris);

            newBullet.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
            canShoot = false;
            StartCoroutine(ShootDelay());
        }
    }

    // delay between shots
    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(timebetweenShots);
        canShoot = true;
    }
}
