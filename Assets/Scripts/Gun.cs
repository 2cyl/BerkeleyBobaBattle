using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public GameObject trigger;

    public float releasedTriggerRotation = -90;
    public float pulledTriggerRotation = -135;

    private AudioSource audioSource;
    private Animator animator;
    private ParticleSystem particleSystem;

    public int damage;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        animator = transform.Find("Model").GetComponent<Animator>();
        particleSystem = transform.Find("MuzzleFlashEffect").GetComponent<ParticleSystem>();
	}
	
	public void SetTriggerRotation(float indexTriggerState) {
        float xRotation = Mathf.Lerp(releasedTriggerRotation, pulledTriggerRotation, indexTriggerState);
        trigger.transform.localEulerAngles = new Vector3(xRotation, 0, 0);
    }

    public void Fire() {
        audioSource.PlayOneShot(audioSource.clip);
        animator.SetTrigger("Fire");
        particleSystem.Play();

        RaycastHit hit;
        Vector3 origin = particleSystem.transform.position;
        Vector3 direction = particleSystem.transform.right;
        if (Physics.Raycast(origin, direction, out hit, 100f)) {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Monster")) {
                Monster monsterScript = hitObject.GetComponent<Monster>();
                monsterScript.Hurt(damage);
            }
        }
    }
}
