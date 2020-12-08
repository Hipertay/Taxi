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
    SphereCollider _collider;

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
    }

    public void SetUpFollow()
    {
        StartCoroutine(FollowToTarget());
    }

    IEnumerator FollowToTarget()
    {
        _collider.enabled = false;
        yield return new WaitForSeconds(timeDelay);
        transform.DOMove(target.position, timeMove);
        yield return new WaitForSeconds(timeMove);
        target.GetComponent<PathFollower>().checkPeople = false;
        gameObject.SetActive(false);
        //target.GetComponent<PathFollower>().StartCoroutine("SpeedUp");
    }


    public void SetUpFollowEnd()
    {
        StartCoroutine(FollowToEnd());
    }

    IEnumerator FollowToEnd()
    {
        transform.DOLookAt(endPosition.position, 0f);
        transform.position = target.position;
        transform.DOMove(endPosition.position, timeMove);
        yield return new WaitForSeconds(timeMove);
        target.GetComponent<PathFollower>().Win();
    }
}
