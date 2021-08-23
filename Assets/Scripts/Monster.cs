using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour {

    public enum State {
        ALIVE, DYING, SINKING
    }

    public State monsterState = State.ALIVE;

    public GameObject player;
    public float attackRange;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private AudioSource audioSource;

    public AudioClip spawnClip;
    public AudioClip hitClip;
    public AudioClip dieClip;

    public int maxHealth;
    private int currHealth;

    public float sinkSpeed;

	// Use this for initialization
	void Start () {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(spawnClip);
        currHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
        if (monsterState == State.ALIVE) {
            navMeshAgent.SetDestination(player.transform.position);

            Vector3 distanceVector = transform.position - player.transform.position;
            distanceVector.y = 0;
            float distance = distanceVector.magnitude;

            if (distance <= attackRange) {
                animator.SetBool("Attack", true);
            }
        } else if (monsterState == State.SINKING) {
            float sinkDistance = sinkSpeed * Time.deltaTime;
            transform.Translate(new Vector3(0, -sinkDistance, 0));
        }
	}

    public void Attack() {
        audioSource.PlayOneShot(hitClip);
    }

    public void Hurt(int damage) {
        if (monsterState == State.ALIVE) {
            animator.SetTrigger("Hurt");
            currHealth -= damage;
            if (currHealth <= 0)
                Die();
        }
    }

    void Die() {
        monsterState = State.DYING;
        audioSource.PlayOneShot(dieClip);
        navMeshAgent.isStopped = true;
        animator.SetTrigger("Dead");
    }

    public void StartSinking() {
        monsterState = State.SINKING;
        navMeshAgent.enabled = false;
        Destroy(gameObject, 5);
    }
}
