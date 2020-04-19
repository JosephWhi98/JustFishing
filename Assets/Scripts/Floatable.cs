using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floatable : MonoBehaviour
{
    public Rigidbody rb;
    public AudioSource audioSource;
    public AudioClip[] audioClip;
    public ConfigurableJoint lineJoint;
    public float waterMass;
    float startMass;

    public bool inWater;

    public Fish currentFish;

    private void Start()
    {
        startMass = rb.mass; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            SoftJointLimit j = lineJoint.linearLimit;
            j.limit = Vector3.Distance(transform.position, lineJoint.connectedBody.transform.position);
            lineJoint.linearLimit = j;

            rb.mass = waterMass; 

            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.clip = audioClip[0]; 
            audioSource.Play();

            inWater = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            rb.mass = startMass;

            inWater = false; 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            //rb.isKinematic = true; 
            rb.velocity = Vector3.zero;  
            rb.AddForce(transform.up * 15);
        }
    }

    public void PlayFishSplash()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.clip = audioClip[1];
        audioSource.Play();
    }
}
