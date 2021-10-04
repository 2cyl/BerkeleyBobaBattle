using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunProjectile : MonoBehaviour
{
    //bullet
    public GameObject bullet;
    //bullet force
    public float shootForce, upwardForce;
    //trigger
    public GameObject trigger;

    //sound and animation
    private AudioSource audioSource;
    private Animator animator;
    private ParticleSystem particleSystem;
    public int damage;

    //gun stats
    public float timeBtwnShooting, spread, reloatTime, timeBtwnShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public int reloadTime;

    //recoil
    public Rigidbody playRb;
    public float recoilForce;

    //bools
    bool shooting, readyToShoot, reloading;

    //reference
    public Camera fpsCam;
    public Transform attackPoint;

    //graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    //bug debug
    public bool allowInvoke = true;

    private void Awake() {
        //magazine full
        bulletsLeft = magazineSize;
        readyToShoot = true;

        audioSource = GetComponent<AudioSource>();
        animator = transform.Find("Model").GetComponent<Animator>();
    }

    private void Update() {
        MyInput();

        //set ammo display
        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + "/" +
                magazineSize / bulletsPerTap);
        }
    }

    private void MyInput()
    {
        //change shooting to configurable buttons

        //check if can hold down shoot
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //reload
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        { Reload(); }
        //reload automatically when shooting without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
        { Reload(); }

        //shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0) {
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        audioSource.PlayOneShot(audioSource.clip);
        animator.SetTrigger("Fire");

        //hit position with raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //check if ray hits
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Monster"))
            {
                Monster monsterScript = hitObject.GetComponent<Monster>();
                monsterScript.Hurt(damage);
            }
        }
        else
        {
            targetPoint = ray.GetPoint(75); //far from player
        }

        //attack to target direction
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //make bullet/ projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        //rotate bullet
        currentBullet.transform.forward = directionWithSpread.normalized;
        //add forces
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);
        
        //muzzle flash
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBtwnShooting);
            allowInvoke = false;

            //recoil
            playRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);

        }

        //shoot more than one bullet per tap
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBtwnShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
