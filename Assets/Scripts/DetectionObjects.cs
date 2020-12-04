using UnityEngine;

public class DetectionObjects : MonoBehaviour
{
    public PathFollower _hero;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CarBot"))
        {
            _hero.Lose();
        }
    }
}
