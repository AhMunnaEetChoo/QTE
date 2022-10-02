using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinalRankingSystem : MonoBehaviour
{
    public float CurrentScore;
    public float ScoreCountUp;
    public float CountSpeed;

    public int ScoreF;
    public int ScoreC;
    public int ScoreB;
    public int ScoreA;
    public int ScoreS;

    public GameObject FStar;
    public GameObject CStar;
    public GameObject BStar;
    public GameObject AStar;
    public GameObject SStar;

    public TMP_Text RankingCounter;
    public TMP_Text CurrentScoreText;

    public Animator CameraAnim;

    public bool GameEnded;
    public bool RankingStart;
    public bool RankingEnd;

    public float BaldManAnimationDelay;

    public GameObject BaldManMain;
    public Animator BaldMan;
    public GameObject BaldManSad;
    public GameObject BaldManMop;
    public GameObject BaldManFedora;
    public GameObject BaldManDog;
    public GameObject BaldManDogFedora;

    void Start()
    {

    }

    void Update()
    {

        //MoveCamera When Game Ends

        if (GameEnded == true)
        {
            CameraAnim.SetBool("GameEnded", true);
            CurrentScoreText.gameObject.SetActive(false);

            if (CameraAnim.GetCurrentAnimatorStateInfo(0).IsName("CameraEndLoop"))
            {
                RankingStart = true;
            }
        }

        //Count up score

        if (RankingStart == true && ScoreCountUp < CurrentScore)
        {
            ScoreCountUp += CountSpeed * Time.deltaTime;

            RankingCounter.gameObject.SetActive(true);
            RankingCounter.text = string.Format("<mspace=0.55em>{0:0}</mspace>", ScoreCountUp);
        }

        //Rankings appear

        if (RankingStart == true && ScoreCountUp >= ScoreF)
        {
            FStar.SetActive(true);
        }

        if (RankingStart == true && ScoreCountUp >= ScoreC)
        {
            FStar.SetActive(false);
            CStar.SetActive(true);
        }

        if (RankingStart == true && ScoreCountUp >= ScoreB)
        {
            CStar.SetActive(false);
            BStar.SetActive(true);
        }

        if (RankingStart == true && ScoreCountUp >= ScoreA)
        {
            BStar.SetActive(false);
            AStar.SetActive(true);
        }

        if (RankingStart == true && ScoreCountUp >= ScoreS)
        {
            AStar.SetActive(false);
            SStar.SetActive(true);
        }

        //Start Animation when ranking ends

        if (RankingStart == true && ScoreCountUp >= CurrentScore)
        {
            ScoreCountUp = CurrentScore;
            RankingStart = false;
            RankingEnd = true;
            RankingCounter.text = string.Format("<mspace=0.55em>{0:0}</mspace>", CurrentScore);

            if (RankingEnd == true)
            {
                BaldManAnimationDelay -= Time.deltaTime;
            }

            if (BaldManAnimationDelay <= 0)
            {

                RankingEnd = false;
                BaldMan.SetTrigger("Squish");

            }

        }

        //Props only appear when BaldSquish animation is playing

        if (BaldMan.GetCurrentAnimatorStateInfo(0).IsName("BaldSquish"))

        {

            if (CurrentScore >= ScoreS)
            {
                BaldManDog.SetActive(true);
                BaldManDogFedora.SetActive(true);
            }

            if (CurrentScore >= ScoreA && CurrentScore < ScoreS)
            {
                BaldManDog.SetActive(true);
            }

            if (CurrentScore >= ScoreB && CurrentScore < ScoreA)
            {
                BaldManFedora.SetActive(true);
            }

            if (CurrentScore >= ScoreC && CurrentScore < ScoreB)
            {
                BaldManMop.SetActive(true);
            }

            if (CurrentScore < ScoreC)
            {
                BaldManSad.SetActive(true);
                BaldManMain.SetActive(false);
            }

        }

    }
}
