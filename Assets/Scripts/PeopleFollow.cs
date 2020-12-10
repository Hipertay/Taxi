using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PeopleFollow : MonoBehaviour
{
    public Transform target;
    public float timeDelay = 2f;
    public float timeMove = 3f;
    public Transform endPosition;
    BoxCollider _collider;
    Vector3 startPosition;
    Quaternion startRotation;
    Animator _anim;
    SphereCollider _colliderSphere;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _colliderSphere = GetComponent<SphereCollider>();
        _anim = GetComponent<Animator>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void GoToStart()
    {
        _colliderSphere.enabled = true;
        transform.position = startPosition;
        transform.rotation = startRotation;
        _collider.enabled = true;
        gameObject.SetActive(true);
    }

    public void SetUpFollow()
    {
        _anim.SetInteger("animation", 1);
        StartCoroutine(FollowToTarget());
    }

    IEnumerator FollowToTarget()
    {
        _collider.enabled = false;
        yield return new WaitForSeconds(timeDelay);
        transform.DOMove(target.position, timeMove);
        yield return new WaitForSeconds(timeMove);
        
        
        //target.GetComponent<PathFollower>().StartCoroutine("SpeedUp");
    }


    public void SetUpFollowEnd()
    {
        StartCoroutine(FollowToEnd());
    }

    IEnumerator FollowToEnd()
    {
        target.GetComponent<PathFollower>().SetBotSpeedOff();
        _anim.SetInteger("animation", 1);
        transform.DOLookAt(endPosition.position, 0f);
        transform.position = target.position;
        transform.DOMove(endPosition.position, timeMove);
        yield return new WaitForSeconds(timeMove);
        _anim.SetInteger("animation", 0);
        target.GetComponent<PathFollower>().Win();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Taxi"))
        {
            _colliderSphere.enabled = false;
            target.GetComponent<PathFollower>().checkPeople = false;
            gameObject.SetActive(false);
            target.GetComponent<PathFollower>().OffTrail();
        }
    }
}
