using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace PathCreation.Examples
{
    public class PathFollowerBot : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        float distanceTravelled;

        float tempSpeed = 0f;
        float timeToMaxSpeed = 10f;

        bool speedIsUp = false;
        RaycastHit hit;
        public Transform forRaycast;
        Vector3 fwd;

        void Start()
        {
            tempSpeed = speed;
            if (pathCreator != null)
            {
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if (pathCreator != null)
            {
                distanceTravelled += tempSpeed * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        void FixedUpdate()
        {
            fwd = forRaycast.transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(forRaycast.transform.position, fwd * 10, Color.white);
            if (Physics.Raycast(forRaycast.transform.position, forRaycast.transform.forward, out hit, 10f))
            {
                if (hit.collider.CompareTag("CarBot"))
                {
                    tempSpeed = 0;
                }
                else
                {
                    tempSpeed = speed;
                }
            }
            else
            {
                tempSpeed = speed;
            }
        }

        void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }

        /*IEnumerator SpeedUp()
        {
            speedIsUp = true;
            StopCoroutine("SpeedDawn");
            while (tempSpeed < speed)
            {
                yield return new WaitForSeconds(0.1f);
                if (tempSpeed + speed / timeToMaxSpeed <= speed)
                {
                    tempSpeed += speed / timeToMaxSpeed;
                }
            }
        }

        IEnumerator SpeedDawn()
        {
            speedIsUp = false;
            StopCoroutine("SpeedUp");
            while (tempSpeed > 0)
            {
                yield return new WaitForSeconds(0.1f);
                if (tempSpeed - speed / timeToMaxSpeed >= 0f)
                {
                    tempSpeed -= speed / timeToMaxSpeed;
                }
            }
        }*/
    }
}