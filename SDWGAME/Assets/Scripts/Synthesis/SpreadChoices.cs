﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class SpreadChoices : MonoBehaviour
{
    public bool allJFArrived;
    private bool[] JfArrived = new bool[8];
    public int cntJFArrived;
    private float speed = 8f;

    private bool initialDone;
    
    private bool waitUser;
    //excel data
    public Entity_Synthesis data;
    
    // 초급/중급/고급
    public int level;
    
    // 각 난이도 안의 stage index
    public static int stageIndex;
    
    //보기 이 게임에선 해파리
    public GameObject[] choices;
    public Transform[] choiceTransforms;
    
    //Crab
    public GameObject crab;
    
    // ui level과 stage표시 하기 위한 변수
    public Text quizLevel;
    public Text quizStage;
    
    //each stage jellyfish's texts
    public TextMeshPro[] choiceTexts;
    
    //correct jellyfish's position Index
    public List<int> corrAnsPosIndex;
    
    //Number of Correct Answers
    public int corrAnsCnt;
    
    //Number of Wrong Answers
    public int wrongAnsCnt;
    

    public GameObject[] PickedAnswer;

    public Transform[] PickedJfPos;
    //시간 테스트를 위한 임시 변수
    public Text timeText;

    // 타이머 관련 변수
    private float timer = 0f;
    private float timeLimit = 60f;
    public Stopwatch watch = new Stopwatch();
    
    //해파리 도망가는 지점
    public Transform FinishPosition;

    public Transform crabStartTransform;
    

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Level, StageIndex = ("+level+", "+stageIndex+")");
        QuizInit();
        
    }

    public void QuizInit()
    {
        crab.transform.position = crabStartTransform.position;
        if (level == 0)
        {
            crab.transform.localScale=new Vector3(0.2f ,0.2f,1);
        }

        if (level == 1)
        {
            crab.transform.localScale = new Vector3(0.25f,0.25f, 1);
        }
        if (level == 2)
        {
            crab.transform.localScale=new Vector3(0.3f,0.3f,1);
        }

        if (level == 3)
        {
            crab.transform.localScale=new Vector3(0.4f,0.4f,1);
        }
        //StartCoroutine(EnableCoroutine());
        EnableCoroutine();
    }

    void EnableCoroutine()
    {
        //quizLevel.text = "" + (level + 1);
        //quizStage.text = "" + (stageIndex + 1);
        //Debug.Log("level "+level);
        //Debug.Log("stageIndex "+stageIndex);
        //yield return new WaitForSecondsRealtime(GetComponent<AudioSource>().clip.length);
        //yield return new WaitForSeconds(1.0f);
        //CheckToggleGroup.SetAllTogglesOff();
        
        
        //정답의 index (corrAnsCnt: 초급-1,2개 중급-3개 고급-4개)
        //8개의 자리중 랜덤한 위치
        for (int i = 0; i < (corrAnsCnt+wrongAnsCnt); i++)
        {
            int tmp; 
            do {
                tmp = Random.Range(0, 8);
            } while (corrAnsPosIndex.Contains(tmp));
            //Debug.Log("corrAnsPosIndex[" + i + "] = " + tmp );
            corrAnsPosIndex.Add(tmp);
        }

        
        //정답 글자를 랜덤 위치에 넣는다.
        for (int i = 0; i < corrAnsCnt; i++)
        {
            choices[corrAnsPosIndex[i]].tag = "CorrectAns";
            if (i == 0)
            {
                //Debug.Log("level "+level);
                //Debug.Log("stageIndex "+stageIndex);
                //Debug.Log(corrAnsPosIndex[0]);
                //Debug.Log(data.sheets[level].list[stageIndex].정답1);
                PickedAnswer[0].GetComponent<TextMeshPro>().text 
                    = choiceTexts[corrAnsPosIndex[0]].text 
                    = data.sheets[level].list[stageIndex].정답1;
                
                //Debug.Log(choiceTexts[corrAnsPosIndex[0]].text);
            }

            if (i == 1)
            {
                PickedAnswer[1].GetComponent<TextMeshPro>().text 
                    = choiceTexts[corrAnsPosIndex[1]].text
                    = data.sheets[level].list[stageIndex].정답2;
            }
            if (i == 2)
            {
                PickedAnswer[2].GetComponent<TextMeshPro>().text 
                    = choiceTexts[corrAnsPosIndex[2]].text
                    = data.sheets[level].list[stageIndex].정답3;
            }
            if (i == 3)
            {
                PickedAnswer[3].GetComponent<TextMeshPro>().text 
                    = choiceTexts[corrAnsPosIndex[3]].text
                    = data.sheets[level].list[stageIndex].정답4;
            }
        }

        //정답이 아닌 글자들 처리
        for (int i = corrAnsCnt; i < (corrAnsCnt + wrongAnsCnt); i++)
        {
            choices[corrAnsPosIndex[i]].tag = "WrongAns";
            if (i == corrAnsCnt)
            {
                choiceTexts[corrAnsPosIndex[i]].text
                    = data.sheets[level].list[stageIndex].보기1;
            }
            if (i == corrAnsCnt+1)
            {
                choiceTexts[corrAnsPosIndex[i]].text
                    = data.sheets[level].list[stageIndex].보기2;
            }
            if (i == corrAnsCnt+2)
            {
                choiceTexts[corrAnsPosIndex[i]].text
                    = data.sheets[level].list[stageIndex].보기3;
            }
            if (i == corrAnsCnt+3)
            {
                choiceTexts[corrAnsPosIndex[i]].text
                    = data.sheets[level].list[stageIndex].보기4;
            }
        }

        //show quiz
        StartCoroutine(ShowAnswers());
        //waitUser = true;
    }

    IEnumerator ShowAnswers()
    {
        //Jellyfish comes out
        int cnt=0;
        int jellyfish_index;
        while (cnt < corrAnsCnt+wrongAnsCnt)
        {
            yield return new WaitForSeconds(0.0f); 
            //choices[maxChoiceNumber].SetActive(true);
            jellyfish_index = corrAnsPosIndex[cnt];
            
            //1. move to each choicePosition
            StartCoroutine(InitialJellyfish(jellyfish_index));
            
            cnt++;
        }

        //Debug.Log("End ShowAnswers");
        // Crab Speaks - no audio available right now
        /*yield return new WaitForSeconds(1.0f);
        string p = "02.Sounds/Stimulus/" + data.sheets[level].list[stageIndex].filename;
        crab.GetComponent<AudioSource>().loop = false;
        crab.GetComponent<AudioSource>().clip = Resources.Load(p) as AudioClip;
        crab.GetComponent<AudioSource>().Play();*/
    }

    IEnumerator InitialJellyfish(int i)
    {
        //Debug.Log("InitialJellyfish "+i);
        
        while (!JfArrived[i])
        {
            yield return new WaitForEndOfFrame();
            //Debug.Log("InitialJellyfish while문 "+i);
            float distance = Vector2.Distance(
                choices[i].transform.position, 
                choiceTransforms[i].position);
            //Debug.Log("distance: "+distance);
            if (distance > 0.02f)
            {
                MoveJellyfish(i,choiceTransforms[i].position);
            }
            else
            {
                JfArrived[i] = true;
                //Debug.Log("InitialJellyfish JF "+ i +" became true");
            }
            
        }
    }

    void MoveJellyfish(int i, Vector2 destination)
    {
        
        float step = speed * Time.deltaTime;
        Transform jellyfish = choices[i].transform;
        /*float distance = Vector2.Distance(choices[i].transform.position,
            destination);*/
        jellyfish.position = Vector2.MoveTowards(jellyfish.position,
                destination, step);
        
    }
    
    
    // Update is called once per frame
     void FixedUpdate()
     { 
         //Debug.Log("initialDone: "+initialDone);
         if (!initialDone && CheckAllArrived())
         {
             for (int i = 0; i < wrongAnsCnt + corrAnsCnt; i++)
             {
                 JfArrived[corrAnsPosIndex[i]] = false;
             }

             
             initialDone = true;
             
             //Debug.Log("In update Jfarrived Initialized");
         }
         /*Debug.Log("JFArrived : "
                   +JfArrived[0]+", "
                   +JfArrived[1]+", "
                   +JfArrived[2]+", "
                   +JfArrived[3]+", ");*/
         
         
         
     }

    public void GoNext()
    {
        if (stageIndex == 29)
        { 
            level++;
            stageIndex = 0;
        }
        else
        { 
            stageIndex++;
        }
        
        StartCoroutine(HideAnswers());
    }

    IEnumerator HideAnswers()
    {
        int i = 0;
        int jfIndex;
        while (i < wrongAnsCnt + corrAnsCnt)
        {
            
            yield return null;
            jfIndex = corrAnsPosIndex[i];
            StartCoroutine(ExitJellyFish(jfIndex));
            i++;
        }
        
       
        yield return new WaitForSeconds(4f);
        //Debug.Log("Re Quiz Init");

        if (level == 0 && stageIndex == 15)
        {
            //글자수 1 -> 2
            //초급인 건 그대로
            SceneManager.LoadScene("CrabLevel2");
        }

        if (level == 1 && stageIndex == 15)
        {
            //중급으로 넘어가기 
            SceneManager.LoadScene("CrabLevel3");
        }
        if (level == 2 && stageIndex==30) 
        {
            //고급으로 넘어가기
            SceneManager.LoadScene("CrabLevel4");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
            
        
        
    }

    IEnumerator ExitJellyFish(int i)
    {
        //Debug.Log("Enter Exit && JfArrived " + i +": " + JfArrived[i]);
        while (!JfArrived[i])
        {
            //Debug.Log("Jf" + i + " starts moving");
            float distance = Vector2.Distance(
                choices[i].transform.position, 
                FinishPosition.position
            );
            if (distance > 0.01f)
            {
                //Debug.Log(i+"th jf is moving to finish position");
                MoveJellyfish(i, FinishPosition.position);
            }
            else
            {
                JfArrived[i] = true;
                //Debug.Log("Exit JellyFish Jf " + i +" became true");
            }
            yield return null;
        }
    }

    bool CheckAllArrived()
    {
        bool flag = true;
        for (int i = 0; i < corrAnsCnt + wrongAnsCnt; i++)
        {
            //Debug.Log("CheckAllArrived JfArrived " + i + ": " + JfArrived[i]);
            
            if (JfArrived[corrAnsPosIndex[i]] == false)
            {
                flag = false;
                //Debug.Log("flag is false, break");
                break;
            }
        }
        
        //Debug.Log("return flag: "+flag);
        return flag;
    }
}
