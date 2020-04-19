using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishingPole : MonoBehaviour
{
    [SerializeField] Transform mainEnd;
    [SerializeField] Transform actualStart;
    [SerializeField] Transform actualEnd;
    [SerializeField] Transform endPoint; 
    [SerializeField] Transform[] linePoints;

    Vector3 directionMain;
    Vector3 directionActual;

    [SerializeField] LineRenderer lineRend;
    [SerializeField] Transform lineEnd;

    bool lineCast = false;

    public ConfigurableJoint lineJoint;
    public Rigidbody hook; 
    public Floatable floatable;

    bool fighting;

    public AudioSource audioSource;
    public AudioClip reelClip;
    public AudioClip removeFish;

    float defaultZRot;
    float maxZRot;

    public Transform fishSpawn;

    private void Start()
    {
        defaultZRot = transform.localEulerAngles.z;
        maxZRot = defaultZRot + 40f;
    }

    public void Update()
    {
        if (floatable.inWater && audioSource.isPlaying)
            audioSource.Stop();


        if (Input.GetKeyDown(KeyCode.Mouse0) && !fighting && !GameStateManager.instance.gameOver)
        {
            if (lineCast == false)
            {
                lineCast = true;
                Debug.Log("Casting Line");
                SoftJointLimit j = lineJoint.linearLimit;
                j.limit = 40;
                lineJoint.linearLimit = j;
                hook.AddForce(Camera.main.transform.forward * 50, ForceMode.Impulse);

                audioSource.clip = reelClip;
                audioSource.Play();
            }
            else if (!floatable.currentFish || (floatable.currentFish && !floatable.currentFish.caught))
            {
                StartCoroutine(ReelInRoutine());
            }
            else if (floatable.currentFish && floatable.currentFish.caught)
            {
                StartCoroutine(ReelInFishRoutine());
            }
        }

        if(!fighting)
        {
            Vector3 targetRot = transform.localEulerAngles;
            targetRot.z = defaultZRot;

            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, targetRot, Time.deltaTime * 2f);
        }


       // lineRend.SetPosition(0, endPoint.position);
        lineRend.SetPosition(1, lineRend.transform.InverseTransformPoint(lineEnd.position));


        actualEnd.position = endPoint.position;
        

        directionMain = (mainEnd.position - actualStart.transform.position).normalized;
        directionActual = (actualEnd.transform.position - actualStart.transform.position).normalized;

        float actualSpace = Vector3.Distance(actualEnd.transform.position, actualStart.transform.position) / linePoints.Length;
        float mainSpace = Vector3.Distance(mainEnd.transform.position, actualStart.transform.position) / linePoints.Length;
        float t = 0;
        Vector3 mainPoint;
        Vector3 actualPoint;

        for (int i = 0; i < linePoints.Length; i++)
        {
            mainPoint = actualStart.transform.position + (directionMain * (i * mainSpace));
            actualPoint = actualStart.transform.position + (directionActual * (i * actualSpace));

            t += (1f / linePoints.Length);
            float m = 1;//((float)i / (float)linePoints.Length);
            float step = m > 0 ? t * (m) : t;

            linePoints[i].transform.position = Vector3.Lerp(mainPoint, actualPoint, step);
        }

        actualEnd.rotation = endPoint.rotation;

    }

    public IEnumerator ReelInFishRoutine()
    {
        fighting = true; 
        yield return null;

        int clicks = 0;
        int clickTarget = 2;

        float fishScale = floatable.currentFish.transform.lossyScale.z;

        if (fishScale < 0.06f)
            clickTarget = 1;
        else if (fishScale < 0.12f)
            clickTarget = 2;
        else if (fishScale < 0.16f)
            clickTarget = 3;
        else if (fishScale < 0.2f)
            clickTarget = 4;
        else if (fishScale < 0.25f)
            clickTarget = 5;
        else if (fishScale < 0.3f)
            clickTarget = 6;
        else if (fishScale < 0.4f)
            clickTarget = 7;

        float zRotTarget = defaultZRot; 

        while ((floatable.currentFish && floatable.currentFish.caught && clicks < clickTarget && floatable.inWater))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                clicks++;

                floatable.PlayFishSplash();

                clicks = Mathf.Clamp(clicks, 0, clickTarget);

                if (clicks > 0)
                    zRotTarget = (float)defaultZRot + (80f * ((float)clicks / (float)clickTarget));
                else
                    zRotTarget = defaultZRot;
            }

            Vector3 targetRot = transform.localEulerAngles;
            targetRot.z = zRotTarget;

            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, targetRot, Time.deltaTime * 5f);

            yield return null; 
        }

        
        if(floatable.currentFish && floatable.currentFish.caught && (clicks >= clickTarget ||  !floatable.inWater ) )
            StartCoroutine(ReelInRoutine());

        fighting = false; 
    }


    public IEnumerator ReelInRoutine()
    {
        SoftJointLimit j = lineJoint.linearLimit;

        float t = 0;

        audioSource.clip = reelClip;
        audioSource.Play();

        //floatable.rb.isKinematic = true;
       

        while (Vector3.Distance(hook.transform.position, lineJoint.connectedBody.transform.position) > 0.5f)
        {
            floatable.inWater = false;

            Vector3 dir = lineJoint.connectedBody.transform.position - floatable.rb.transform.position;
            Vector3 vel = (dir * 25);
            vel = Vector3.ClampMagnitude(vel, 25);
            floatable.rb.velocity = vel;

            yield return null; 
        }


        //floatable.rb.isKinematic = false;

        j.limit = 0f;
        lineJoint.linearLimit = j;

        audioSource.Stop();

        /*while (j.limit > 0)
        {
            j.limit -= 0.1f;
            floatable.inWater = false;
            hook.velocity = Vector3.zero; 
            lineJoint.linearLimit = j;
            yield return null;
        }
        yield return null;*/


        hook.velocity = Vector3.zero;
        hook.angularVelocity = Vector3.zero;



        if (floatable.currentFish && floatable.currentFish.caught)
        {
            if (GameStateManager.instance && !GameStateManager.instance.gameOver)
                GameStateManager.instance.AddToTime(floatable.currentFish.time);

            floatable.currentFish.PlayFlapSound();
            yield return new WaitForSeconds(2f);

            
            floatable.currentFish.StopAllCoroutines();
            Fish fish = floatable.currentFish; 
            floatable.currentFish = null;
            fish.gameObject.SetActive(false);
            audioSource.clip = removeFish;
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.Play();
            fish.RespawnFish(fishSpawn);
            yield return new WaitForSeconds(0.5f);


        }
        else if (floatable.currentFish)
        {
            floatable.currentFish = null; 
        }
        

        lineCast = false;
    }
}
