using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBullets : MonoBehaviour
{
    //assign
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    //stats
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    //damage
    public int explosionDamage;
    public float explosionRange;

    //lifetime
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    int collisions;
    PhysicMaterial physics_mat;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        //when to explode
        if (collisions > maxCollisions) Explode();

        //count down
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {
        //instantiate explosion
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        //check enemies
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            //get component of enemy and call take damage
            enemies[i].GetComponent<Monster>().Hurt(explosionDamage);

        }
        Invoke("Delay", 0.05f);
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //don't counter collisions
        if (collision.collider.CompareTag("Bullet")) return;

        //count up collisions
        collisions++;

        //explode if hits enemy and explodeOnTouch  is activated
        if (collision.collider.CompareTag("Monster") && explodeOnTouch) Explode(); 

    }

    private void Setup()
    {
        //create new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

        //assign materal
        GetComponent<SphereCollider>().material = physics_mat;

        //set gravity
    }
}
