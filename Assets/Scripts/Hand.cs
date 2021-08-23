using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public Vector3 holdPosition = new Vector3(0, -0.025f, 0.03f);
    public Vector3 holdRotation = new Vector3(0, 180, 0);

    private bool holdingGun = false;
    private GameObject gun = null;

    public OVRInput.Controller controller;

    private float indexTriggerState = 0;
    private float handTriggerState = 0;
    private float oldIndexTriggerState = 0;

    // Update is called once per frame
    void Update() {
        oldIndexTriggerState = indexTriggerState;
        indexTriggerState = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);
        handTriggerState = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);

        if (holdingGun) {
            Gun gunScript = gun.GetComponent<Gun>();
            gunScript.SetTriggerRotation(indexTriggerState);

            if (indexTriggerState > 0.9f && oldIndexTriggerState < 0.9f)
                gunScript.Fire();

            if (handTriggerState < 0.9f)
                Release();
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Gun")) {
            if (handTriggerState > 0.9f && !holdingGun) {
                Grab(other.gameObject);
            }
        }
    }

    void Grab(GameObject obj) {
        holdingGun = true;
        gun = obj;

        gun.transform.parent = transform;

        gun.transform.localPosition = holdPosition;
        gun.transform.localEulerAngles = holdRotation;

        gun.GetComponent<Rigidbody>().useGravity = false;
        gun.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Release() {
        gun.transform.parent = null;

        Rigidbody rigidbody = gun.GetComponent<Rigidbody>();

        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;

        rigidbody.velocity = OVRInput.GetLocalControllerVelocity(controller);

        holdingGun = false;
        gun = null;
    }
}