using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public Transform[] target;
    Transform newTarget;

    Animator bassAnimation;

    bool isMoving = false;

    public float speed = 2.0f;

    float randomOffset;

    public Transform[] bones;

    public ParticleSystem particles;

    public Floatable floatable;

    [HideInInspector]public bool caught;

    public Material[] matVarient;


    float caughtTime;

    Transform startParent;

    float freeTime;
    public float time; 

    public AudioSource audioSource;
    public AudioClip flap; 

    private void Start()
    {
        startParent = transform.parent; 

        int mat = Random.Range(0, matVarient.Length);

        GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = matVarient[mat];


        particles = GetComponentInChildren<ParticleSystem>();
        randomOffset = Random.Range(0, 1000);
        float fishScale = Random.Range(0.03f, 0.35f);// 0.1483863; 
  

        ParticleSystem.MainModule m = particles.main;
        m.startSize = fishScale /3f;

        if (fishScale < 0.12f)
        {
            freeTime = 3f;
            time = 5f;
        }
        else if (fishScale < 0.3f)
        {
            freeTime = 2f;
            time = 10f;
        }
        else if (fishScale < 0.4f)
        {
            freeTime = 1.4f;
            time = 20f;
        }

        transform.localScale = new Vector3(fishScale, fishScale, fishScale);
    }

    void Update()
    {
        if (!caught)
        {
            if (floatable.inWater && floatable.currentFish == null && (Time.time - caughtTime > 5f))
            {
                if (Vector3.Distance(transform.position, floatable.transform.position) < 3)
                {
                    float chance = Random.Range(0, 10000);

                    if (chance > 9930)
                    {
                        newTarget = floatable.transform;
                        floatable.currentFish = this;
                    }
                }
            }

            if (newTarget == floatable.transform && !floatable.inWater)
                isMoving = false;


            if (isMoving == false)
            {
                newTarget = target[Random.Range(0, target.Length)];
                isMoving = true;
            }

            transform.position = Vector3.MoveTowards(transform.position, newTarget.position, speed * Time.deltaTime);

            if (transform.position == newTarget.position)
            {
                if (newTarget == floatable.transform)
                {
                    caught = true;
                    AttachFishToFloat();
                    //floatable.currentFish = null; 
                }

                isMoving = false;
            }

            if (!caught)
            {
                if (newTarget)
                {
                    Vector3 relativePos = newTarget.position - transform.position;

                    if (relativePos != Vector3.zero)
                    {
                        Quaternion toRotation = Quaternion.LookRotation(relativePos);
                        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 3 * Time.deltaTime);
                    }
                }

                int i = 1;
                foreach (Transform b in bones)
                {
                    Vector3 euler = b.localEulerAngles;

                    euler.z = Mathf.Sin((Time.time + randomOffset) * 1.5f * speed * i) * 10;

                    b.localEulerAngles = euler;
                    i++;
                }
            }
        }
        else
        {
            particles.Stop();
            int i = 1;
            foreach (Transform b in bones)
            {
                Vector3 euler = b.localEulerAngles;
               // Debug.Log("flop");
                euler.z = Mathf.Sin((Time.time) * 15f * i) * 20;
                b.localEulerAngles = euler;
                i++;
            }

            if (Time.time - caughtTime > freeTime && floatable.inWater)
            {
                BreakFree();
            }
        }
    }

    public void BreakFree()
    {
        floatable.currentFish = null;


        transform.parent = startParent;
        caught = false;

        newTarget = target[Random.Range(0, target.Length)];
        isMoving = true;


        speed *= 4;

        StartCoroutine(SpeedBoostRoutine());

    }

    IEnumerator SpeedBoostRoutine()
    {
        yield return new WaitForSeconds(2f);
        speed /= 4;
    }

    public void AttachFishToFloat()
    {
        caughtTime = Time.time;
        caught = true;
        transform.parent = floatable.transform;
        Vector3 pos = Vector3.zero;

        pos.z = -(transform.localScale.z * 3.5f);

        transform.localPosition = pos;
        transform.localEulerAngles = Vector3.zero;

        floatable.PlayFishSplash();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void PlayFlapSound()
    {
        audioSource.pitch = 1.2f - transform.lossyScale.z;
        audioSource.clip = flap;
        audioSource.Play();
    }

    public void RespawnFish(Transform position)
    {
        transform.parent = startParent;
        caught = false;
        newTarget = null;

        int mat = Random.Range(0, matVarient.Length);

        GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = matVarient[mat];


        particles = GetComponentInChildren<ParticleSystem>();
        randomOffset = Random.Range(0, 1000);
        float fishScale = Random.Range(0.03f, 0.35f);// 0.1483863; 


        ParticleSystem.MainModule m = particles.main;
        m.startSize = fishScale / 3f;

        if (fishScale < 0.12f)
        {
            freeTime = 3f;
            time = 5f;
        }
        else if (fishScale < 0.3f)
        {
            freeTime = 2f;
            time = 10f;
        }
        else if (fishScale < 0.4f)
        {
            freeTime = 1.4f;
            time = 20f;
        }

        transform.localScale = new Vector3(fishScale, fishScale, fishScale);

        transform.position = position.position;

        gameObject.SetActive(true);
    }


}
