using UnityEngine;

public class DetectionObjects : MonoBehaviour
{
    public PathFollower _hero;
    public BoxCollider _heroCollider;
    public Rigidbody _heroRigidbody;
    Transform collideCar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CarBot"))
        {
            _hero.enabled = false;
            collideCar = other.transform;
            _heroCollider.isTrigger = false;
            _heroRigidbody.isKinematic = false;
            _hero.Lose();
        }
    }
}
