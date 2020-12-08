using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class PathFollower : MonoBehaviour
{
    public List<PathCreator> allPathCreator = new List<PathCreator>();
    public List<MeshRenderer> allPathMesh = new List<MeshRenderer>();

    public List<GameObject> allBots = new List<GameObject>();
    public List<GameObject> allBotsPath = new List<GameObject>();
    [HideInInspector]public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    public float tempSpeed = 0f;
    float distanceTravelled;
    public float timeToMaxSpeed = 10f;
    public bool tapIsMove = true;

    public GameObject windowWin;
    public GameObject windowLose;
    public float delayWindowSet = 1f;
    bool isEnd = false;
    public GameObject windowRoad;
    bool isSelectRoad = false;
    [HideInInspector]public bool checkPeople = false;
    GameObject people;

    void Start()
    {
        Application.targetFrameRate = 60;
        
    }

    public void CheckRoad(int countRoad)
    {
        windowRoad.SetActive(false);
        pathCreator = allPathCreator[countRoad];
        for (int i = 0; i < allPathMesh.Count; i++)
        {
            if (i != countRoad)
            {
                allPathMesh[i].enabled = false;
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
            if (!checkPeople & !isEnd)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (tapIsMove)
                    {
                        StartCoroutine("SpeedUp");
                    }
                    else
                    {
                        StartCoroutine("SpeedDawn");
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (tapIsMove)
                    {
                        StartCoroutine("SpeedDawn");
                    }
                    else
                    {
                        StartCoroutine("SpeedUp");
                    }
                }
            }
        }
    }

    IEnumerator SpeedUp()
    {
        StopCoroutine("SpeedDawn");
        while (tempSpeed < speed)
        {
            yield return new WaitForSeconds(0.1f);
            if(tempSpeed + speed / timeToMaxSpeed <= speed)
            {
                tempSpeed += speed / timeToMaxSpeed;
            }
        }
    }

    IEnumerator SpeedDawn()
    {
        StopCoroutine("SpeedUp");
        while (tempSpeed > 0)
        {
            yield return new WaitForSeconds(0.1f);
            if(tempSpeed - speed / timeToMaxSpeed >= 0f)
            {
                tempSpeed -= speed / timeToMaxSpeed;
            }
        }
    }

    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }

    public void Win()
    {
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
        yield return new WaitForSeconds(delayWindowSet);
        windowLose.SetActive(true);
    }

    IEnumerator WinLevel()
    {
        yield return new WaitForSeconds(delayWindowSet);
        windowWin.SetActive(true);
    }

    public void NextLevel()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadSceneAsync(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("People"))
        {
            checkPeople = true;
            people = other.gameObject;
            StartCoroutine("SpeedDawn");
            other.GetComponent<PeopleFollow>().target = transform;
            other.GetComponent<PeopleFollow>().SetUpFollow();
        }
    }
}