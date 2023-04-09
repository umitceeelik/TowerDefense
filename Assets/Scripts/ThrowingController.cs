using System;
using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ThrowingController : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject bottle;
    [SerializeField] private Transform bottlePosition;
    [SerializeField] private float upForce, forwardForce;


    [SerializeField] private Transform ReleasePosition;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    private int LinePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float TimeBetweenPoints = 0.1f;

    Rigidbody rb;

    [SerializeField] private GameObject throwableObject;
    private LayerMask BottleCollisionMask;

    [SerializeField] private PlayerController playerController;

    private void Awake()
    {
        var newBottle = Instantiate(bottle, bottlePosition.position, bottlePosition.rotation, bottlePosition);
        playerController.canThrow = true;

        throwableObject = newBottle;
        rb = throwableObject.GetComponent<Rigidbody>();

        int bottleLayer = bottle.gameObject.layer;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(bottleLayer, i))
            {
                BottleCollisionMask |= 1 << i; // magic
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && playerController.canThrow)
            lineRenderer.enabled = true;

        if(playerController.canThrow) DrawProjection();

        if (Input.GetMouseButtonUp(0))
            lineRenderer.enabled = false;
    }

    private void DrawProjection()
    {
        lineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;
        Vector3 startPosition = throwableObject.transform.position;

        float realForce = (float)Math.Sqrt((float)Math.Pow(forwardForce, 2) + (float)Math.Pow(upForce, 2));
        Vector3 startVelocity = realForce * camera.transform.forward / rb.mass;
        int i = 0;
        lineRenderer.SetPosition(i, startPosition);
        for (float time = 0; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            lineRenderer.SetPosition(i, point);

            Vector3 lastPosition = lineRenderer.GetPosition(i - 1);

            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                (point - lastPosition).magnitude,
                BottleCollisionMask))
            {
                lineRenderer.SetPosition(i, hit.point);
                lineRenderer.positionCount = i + 1;
                return;
            }
        }
    }

    public void ThrowBottle()
    {
        RaycastHit hit;

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 100f))
            transform.LookAt(hit.point);

        throwableObject.transform.SetParent(transform.parent);

        rb.isKinematic = false;
        //Vector3 pos = new Vector3(0, 0.3f, 1);
        //Vector3 point = new Vector3();
        //Vector2 mousePos = new Vector2();
        //Event currentEvent = Event.current;

        //// Get the mouse position from Event.
        //// Note that the y position from Event is inverted.
        //mousePos.x = currentEvent.mousePosition.x;
        //mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;
        //point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        //Bottle.transform.LookAt(point);

        rb.AddForce(camera.transform.forward * forwardForce, ForceMode.Impulse);
        rb.AddForce(camera.transform.up * upForce, ForceMode.Impulse);
        playerController.canThrow = false;
    }


    //public void AnimationStartedTrigger() //Throw bottle
    //{
    //    Debug.Log("1");
    //    //playerController.animator.SetBool("Throw", true);

    //    // playerController.GetComponent<Animator>().GetComponent<AnimatorController>().GetComponent<Animation>().Stop();
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        playerController.animator.SetBool("Throw", true);
    //        // playerController.GetComponent<Animator>().GetComponent<AnimatorController>().GetComponent<Animation>().Play();
            
    //    }
    //    Debug.Log("3");
        
    //}

    public void AnimationContinueTrigger()
    {
        ThrowBottle();
        playerController.animator.SetBool("Throw", false);
    }



    public void AnimationFinishedTrigger()
    {
        StartCoroutine(InstantiateBottle());
        playerController.isThrowing = false;
    }


    IEnumerator InstantiateBottle()
    {
        yield return new WaitForSeconds(0.01f);

        var newBottle1 = Instantiate(bottle, bottlePosition.position, bottlePosition.rotation, bottlePosition);
        playerController.canThrow = true;
        throwableObject = newBottle1;
        rb = throwableObject.GetComponent<Rigidbody>();
    }
}