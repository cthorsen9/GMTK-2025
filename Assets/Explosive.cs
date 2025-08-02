using UnityEngine;

public class Explosive : MonoBehaviour
{
    Rigidbody rigid;

    public float explosionForce = 1.2f;

    public float baseForceMult = 20000f;

    [SerializeField]
    GameObject explosion;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("coll with player");
            if (rigid == null) rigid = WheelController.singleton.rigid;

            Debug.Log("Trying to add force: " + (collision.contacts[0].normal * rigid.linearVelocity.magnitude * rigid.mass * explosionForce) + " Player vel " + rigid.linearVelocity);
            //Debug.DrawRay(transform.position, (collision.contacts[0].normal * rigid.linearVelocity.magnitude * rigid.mass * explosionForce));
            //Debug.Break();

            Debug.Log("spawning bomb");

            rigid.AddForce(-(collision.contacts[0].normal * rigid.linearVelocity.magnitude * rigid.mass * explosionForce) + -(collision.contacts[0].normal * baseForceMult));
            Debug.Log("spawning bomb");

            Instantiate<GameObject>(explosion, transform.position, Quaternion.identity);
            Debug.Log("destroying");
            Destroy(gameObject);

        }
    }

}
