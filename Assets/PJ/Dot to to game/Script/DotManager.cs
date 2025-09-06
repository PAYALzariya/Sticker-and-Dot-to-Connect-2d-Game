// DotManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DotManager : MonoBehaviour
{
    public static DotManager instance;

    public GameObject[] pointsOfObject;
    [SerializeField]
    public int CurrentPoints, HighLightCounter, incorrectattemp;
    public List<Transform> selectedDot;
    public LineRenderer lr;
    public GameObject CurrentDrawSprite, CurrentDrawLine;
    public GameObject linepanel, que_mark;
    public bool istrue, starttimer, isavarage, isclick, traincome = true;
  
    public float timer, currentleveltimer, avaragetimer, currentstoretimer;
    public Animator train_anim, logo_anim;
    public Button nextbtn;
    public ParticleSystem dotConnect, sparkle1, smoke;
   

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        CurrentPoints = 0;
        HighLightCounter = 0;
        incorrectattemp = 0;
        isclick = true;
        train_anim.Play("Train_anim");
        //StartCoroutine(s());
        logo_anim.Play("title_anim");
    }

    Vector2 origin;
    RaycastHit2D hit;

    void Update()
    {
        if (isavarage == true)
        {
            currentleveltimer += Time.deltaTime * 1f;
        }

        if (starttimer == true)
        {
            timer += Time.deltaTime * 1f;

            if (timer >= 1)
            {
                pointsOfObject[HighLightCounter].gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                timer = 0;
                pointsOfObject[HighLightCounter].GetComponent<Animator>().enabled = true;
            }

        }
        if (EventSystem.current.currentSelectedGameObject != null &&
        (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null))
        {
            return;
        }

        if (istrue == false)
        {
            if (Input.GetMouseButton(0))
            {
                origin = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                               Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                hit = Physics2D.Raycast(origin, Vector2.zero, 0f);

                if (hit.collider != null)
                {
                    if (hit.collider.tag == "Dot")
                    {
                        if (CurrentPoints != pointsOfObject.Length)
                        {
                            if (hit.transform.name == pointsOfObject[0].transform.name)
                            {
                                lr.gameObject.SetActive(true);
                                dotConnect.gameObject.SetActive(true);
                                dotConnect.Play();
                            }

                            if (hit.transform.name == pointsOfObject[CurrentPoints].transform.name)
                            {
                               
                                selectedDot.Add(pointsOfObject[CurrentPoints].transform);
                                lr.positionCount = selectedDot.Count + 1;
                                CurrentPoints++;
                                pointsOfObject[HighLightCounter].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                                pointsOfObject[HighLightCounter].GetComponent<Animator>().enabled = false;
                                pointsOfObject[HighLightCounter].transform.localScale = new Vector3(1, 1, 1);
                                dotConnect.Play();
                                //AudioManager.instanceAM.Connect_Dot.Play();
                                /*AudioManager.instanceAM.mainAS.clip = AudioManager.instanceAM.Connect_Dot1;
                                AudioManager.instanceAM.mainAS.Play();*/

                                dotConnect.transform.position = pointsOfObject[CurrentPoints - 1].transform.position;
                            }
                            /*else if (hit.transform.name != pointsOfObject[CurrentPoints].transform.name &&
                                hit.transform.name != pointsOfObject[CurrentPoints - 1].transform.name && isclick == true)
                            {
                                incorrectattemp++;
                                isclick = false;
                                Debug.Log(incorrectattemp);
                            }*/
                            if (hit.transform.name == pointsOfObject[HighLightCounter].transform.name)
                            {
                                timer = 0;
                                if (HighLightCounter != (pointsOfObject.Length - 1))
                                {
                                    HighLightCounter++;
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < (lr.positionCount); i++)
                {
                    if (selectedDot.Count != 0)
                    {
                        if (i == (lr.positionCount - 1))
                        {
                            lr.SetPosition(i, new Vector3(origin.x, origin.y, 5));
                        }
                        else
                        {
                            lr.SetPosition(i, new Vector3(selectedDot[i].position.x, selectedDot[i].position.y, 5));
                        }
                    }
                }
                if (lr.positionCount == (2 + pointsOfObject.Length))
                {
                    CurrentDrawSprite.SetActive(true);
                    CurrentDrawLine.SetActive(false);
                    lr.gameObject.SetActive(false);
                    //AudioManager.instanceAM.Dot_Complete.Play();
                
                    //istrue = true;
                    if (istrue == false)
                    {
                     
                        StartCoroutine(ShiftToColor());
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                for (int i = 0; i < (lr.positionCount); i++)
                {
                    if (selectedDot.Count != 0)
                    {
                        if (i == (lr.positionCount - 1))
                        {
                            lr.SetPosition(i, new Vector3(selectedDot[i - 1].position.x, selectedDot[i - 1].position.y, 5));
                        }
                        else
                        {
                            lr.SetPosition(i, new Vector3(selectedDot[i].position.x, selectedDot[i].position.y, 5));
                        }
                    }
                }
            }
        }
    }

    public IEnumerator ShiftToColor()
    {
        istrue = true;

        sparkle1.gameObject.SetActive(true);
        sparkle1.Play();
        dotConnect.gameObject.SetActive(false);
        
      
        yield return new WaitForSeconds(3.2f);
        sparkle1.gameObject.SetActive(false);

      
        //linepanel.SetActive(false);
        CurrentDrawLine.SetActive(false);
        CurrentDrawSprite.SetActive(false);
        que_mark.SetActive(false);


        

        /*lastleveltimer = currentleveltimer;
        currentleveltimer = 0;*/
        starttimer = false;
        timer = 0;
    }

   
}


