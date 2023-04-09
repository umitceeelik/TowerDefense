using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    [SerializeField] private float bottleLife;
    [SerializeField] private ParticleSystem[] particles;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    IEnumerator DestroyBottle()
    {
        yield return new WaitForSeconds(bottleLife);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3  particlePos = gameObject.transform.position;
        
       //Debug.Log(collision.relativeVelocity.magnitude);

        if (collision.relativeVelocity.magnitude > 30)
        {
            StartCoroutine(DestroyBottle());

            foreach (ParticleSystem particle in particles)
            {
                Instantiate(particle, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.parent);
            }
            Debug.Log(collision.relativeVelocity.magnitude);
        }
    }

}
