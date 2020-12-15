using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObjectSmooth : MonoBehaviour
{
    private Vector3 m_refPos;
    private Vector3 m_refRot;
    public Transform _hero;
    Vector3 startPosition;
    public float timeDelayPos = 0.1f;
    public float timeDelayRot = 0.1f;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void ToStartPos()
    {
        transform.position = _hero.position;
        transform.rotation = _hero.rotation;
    }

    void Update()
    {
        m_refPos *= Time.smoothDeltaTime;
        transform.position = new Vector3(Vector3.SmoothDamp(transform.position, _hero.position, ref m_refPos, timeDelayPos).x, transform.position.y, Vector3.SmoothDamp(transform.position, _hero.position, ref m_refPos, timeDelayPos).z) ;
        transform.rotation = SmoothDampRotation(transform.rotation, _hero.rotation, ref m_refRot, timeDelayRot);
    }

    public static Quaternion SmoothDampRotation(Quaternion ObjectToRotate, Quaternion WantedRotation, ref Vector3 SavedSmoothSteps, float RoationSpeed)
    {
        var euler = ObjectToRotate.eulerAngles;
        var eulerTarget = WantedRotation.eulerAngles;
        SavedSmoothSteps *= Time.smoothDeltaTime;
        euler.x = Mathf.SmoothDampAngle(euler.x, eulerTarget.x, ref SavedSmoothSteps.x, RoationSpeed);
        euler.y = Mathf.SmoothDampAngle(euler.y, eulerTarget.y, ref SavedSmoothSteps.y, RoationSpeed);
        euler.z = Mathf.SmoothDampAngle(euler.z, eulerTarget.z, ref SavedSmoothSteps.z, RoationSpeed);
        return Quaternion.Euler(euler);
    }
}
