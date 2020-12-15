using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class PathFollower : MonoBehaviour
{
    public List<PathCreator> allPathCreator = new List<PathCreator>();
    public List<GameObject> allBots = new List<GameObject>();
    public List<GameObject> allBotsPath = new List<GameObject>();
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    public float tempSpeed = 0f;
    float distanceTravelled;
    public float timeToMaxSpeed = 10f;
    public bool tapIsMove = true;

    public GameObject windowWin;
    public GameObject windowLose;
    public float delayWindowWin = 1f;
    public float delayWindowLose = 1f;
    [HideInInspector] bool isEnd = false;
    public GameObject windowRoad;
    [HideInInspector]bool isSelectRoad = false;
    public bool checkPeople = false;
    GameObject people;
    public DetectionObjects _detectObj;

    public GameObject[] roadArrows;
    public List<PeopleFollow> allPeople = new List<PeopleFollow>();

    public List<GameObject> wheels = new List<GameObject>();
    public GameObject trail;
    public List<GameObject> smoke = new List<GameObject>();
    public GameObject _camera;
    Vector3 cameraStartPos;
    Quaternion cameraStartRot;
    public CameraFollowObjectSmooth _cam;


    void Start()
    {
        Application.targetFrameRate = 60;
        cameraStartPos = _camera.transform.localPosition;
        cameraStartRot = _camera.transform.localRotation;
    }

    public void CheckRoad(int countRoad)
    {
        for (int i = 0; i < smoke.Count; i++)
        {
            smoke[i].SetActive(false);
        }
        for (int i = 0; i < allPathCreator.Count; i++)
        {
            allBots[i].SetActive(true);
            allBotsPath[i].SetActive(true);
            roadArrows[i].SetActive(false);
        }

        roadArrows[countRoad].SetActive(true);

        windowRoad.SetActive(false);
        pathCreator = allPathCreator[countRoad];
        for (int i = 0; i < allPathCreator.Count; i++)
        {
            if (i != countRoad)
            {
                allBots[i].SetActive(false);
                allBotsPath[i].SetActive(false);
            }
        }
        if (pathCreator != null)
        {
            pathCreator.pathUpdated += OnPathChanged;
        }
        isSelectRoad = true;
        if (!tapIsMove)
        {
            StartCoroutine("SpeedUp");
        }
        for (int i = 0; i < smoke.Count; i++)
        {
            smoke[i].SetActive(true);
        }
        isEnd = false;
        tempSpeed = 0f;
        distanceTravelled = 0.1f;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        _cam.ToStartPos();
    }

    void Update()
    {
        if (isSelectRoad)
        {
            if (pathCreator != null)
            {
                distanceTravelled += tempSpeed * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

                if (transform.position == pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1) & !isEnd)
                {
                    isEnd = true;
                    people.SetActive(true);
                    people.GetComponent<PeopleFollow>().SetUpFollowEnd();
                }
            }
            if (!checkPeople & !isEnd & !windowRoad.activeSelf & !windowLose.activeSelf & !windowWin.activeSelf)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (tapIsMove)
                    {
                        OffTrail();
                        StartCoroutine("SpeedUp");
                    }
                    else
                    {
                        if (tempSpeed > speed / 2f)
                            OnTrail();
                        StartCoroutine("SpeedDawn");
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (tapIsMove)
                    {
                        if(tempSpeed > speed / 2f)
                            OnTrail();
                        StartCoroutine("SpeedDawn");
                    }
                    else
                    {
                        OffTrail();
                        StartCoroutine("SpeedUp");
                    }
                }
            }
        }
    }

    GameObject tempTrail;

    void OnTrail()
    {
        for (int i = 0; i < wheels.Count; i++)
        {
            tempTrail = GameObjectPool.Spawn(trail, wheels[i].transform.position, Quaternion.identity);
            tempTrail.GetComponent<TrailRenderer>().Clear();
            tempTrail.transform.parent = wheels[i].transform;
        }
    }

    public void OffTrail()
    {
        for (int i = 0; i < wheels.Count; i++)
        {
            if (wheels[i].transform.childCount > 0)
            {
                wheels[i].transform.GetChild(0).parent = null;
            }
        }
    }

    IEnumerator SpeedUp()
    {
        StopCoroutine("SpeedDawn");
        while (tempSpeed < speed)
        {
            yield return null;
            if (!checkPeople & !isEnd)
            {
                if (tempSpeed + speed / timeToMaxSpeed <= speed)
                {
                    tempSpeed += speed / timeToMaxSpeed;
                }
                else
                {
                    tempSpeed = speed;
                    StopCoroutine("SpeedUp");
                    break;
                }
            }
        }
    }

    IEnumerator SpeedDawn()
    {
        StopCoroutine("SpeedUp");
        while (tempSpeed > 0)
        {
            yield return null;
            if (!checkPeople & !isEnd)
            {
                if (tempSpeed - speed / timeToMaxSpeed >= 0f)
                {
                    tempSpeed -= speed / timeToMaxSpeed;
                }
                else
                {
                    tempSpeed = 0f;
                    StopCoroutine("SpeedDawn");
                    break;
                }
            }
        }
    }

    PathFollowerBot _bot;

    public void SetBotSpeedOff()
    {
        for (int i = 0; i < allBots.Count; i++)
        {
            for (int k = 0; k < allBots[i].transform.childCount; k++)
            {
                if (allBots[i].transform.GetChild(k).GetComponent<PathFollowerBot>())
                {
                    _bot = allBots[i].transform.GetChild(k).GetComponent<PathFollowerBot>();
                    _bot.enabled = false;
                    _bot._botRigidbody.isKinematic = false;
                }
            }
        }
    }

    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }

    public void Win()
    {
        isEnd = true;
        tempSpeed = 0f;
        StartCoroutine(WinLevel());
    }

    public void Lose()
    {
        isEnd = true;
        tempSpeed = 0f;
        StartCoroutine(LoseLevel());
    }

    IEnumerator LoseLevel()
    {
        SetBotSpeedOff();
        yield return new WaitForSeconds(delayWindowLose);
        windowLose.SetActive(true);
    }

    IEnumerator WinLevel()
    {
        yield return new WaitForSeconds(delayWindowWin);
        windowWin.SetActive(true);
    }

    void RestartLevel()
    {
        _camera.transform.parent = transform;
        _camera.transform.localPosition = cameraStartPos;
        _camera.transform.localRotation = cameraStartRot;
        for (int i = 0; i < smoke.Count; i++)
        {
            smoke[i].SetActive(false);
        }
        for (int i = 0; i < wheels.Count; i++)
        {
            if (wheels[i].transform.childCount > 0)
            {
                GameObjectPool.Unspawn(wheels[i].transform.GetChild(0).gameObject);
                wheels[i].transform.GetChild(0).parent = null;
            }
        }
        distanceTravelled = 0;
        for (int i = 0; i < allPathCreator.Count; i++)
        {
            allBots[i].SetActive(false);
            allBotsPath[i].SetActive(false);
            roadArrows[i].SetActive(false);
            allPeople[i].GoToStart();

            for (int k = 0; k < allBots[i].transform.childCount; k++)
            {
                if (allBots[i].transform.GetChild(k).GetComponent<PathFollowerBot>())
                {
                    _bot = allBots[i].transform.GetChild(k).GetComponent<PathFollowerBot>();
                    _bot.distanceTravelled = 0;
                    _bot.enabled = true;
                    _bot._botRigidbody.isKinematic = true;
                }
            }
        }

        this.enabled = true;
        checkPeople = false;
        _detectObj._heroCollider.isTrigger = true;
        for (int i = 0; i < smoke.Count; i++)
        {
            smoke[i].SetActive(true);
        }
    }

    public void NextLevel()
    {
        windowRoad.SetActive(true);
        RestartLevel();
        windowWin.SetActive(false);
    }

    public void ReloadLevel()
    {
        windowRoad.SetActive(true);
        RestartLevel();
        windowLose.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("People"))
        {
            OnTrail();
            checkPeople = true;
            people = other.gameObject;
            //StartCoroutine("SpeedDawn");
            tempSpeed = 0;
            other.GetComponent<PeopleFollow>().target = transform;
            other.GetComponent<PeopleFollow>().SetUpFollow();
        }
    }
}