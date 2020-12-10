using UnityEngine;

public class DetectionObjects : MonoBehaviour
{
    public PathFollower _hero;
    public BoxCollider _heroCollider;
    public Rigidbody _heroRigidbody;
    Transform collideCar;
    PathFollowerBot _bot;
    public float powerToCar = 100f;
    public float powerToBot = 100f;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CarBot"))
        {
            _bot = other.GetComponent<PathFollowerBot>();
            _bot.enabled = false;
            _bot._botRigidbody.isKinematic = false;

            

            _hero.enabled = false;
            //collideCar = other.transform;
            _heroCollider.isTrigger = false;
            //_heroRigidbody.isKinematic = false;
            _hero._camera.transform.parent = null;
            Vector3 direction = (transform.position - other.transform.position).normalized;
            transform.GetComponent<Rigidbody>().AddForce(direction * powerToCar);
            other.GetComponent<Rigidbody>().AddForce(-direction * powerToBot);

            _hero.Lose();
        }
    }
}
