using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinalRankingSystem : MonoBehaviour
{

    public ScoreSystem ScoreSystem;

    private float CurrentScore;
    private float ScoreCountUp;
    private float CountSpeed;
    public int Rank;

public float SecondsOfRanking;

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

    public GameObject TVStaticSpare;

    void Start()
    {

    }

    void Update()
    {

        //MoveCamera When Game Ends
        //Extract score from scoring system

        if (GameEnded == true)
        {
            CameraAnim.SetBool("GameEnded", true);
            CurrentScoreText.gameObject.SetActive(false);
            CurrentScore = ScoreSystem.CurrentScore;
            CountSpeed = CurrentScore / SecondsOfRanking;
            TVStaticSpare.SetActive(true);

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

        if (RankingStart == true && ScoreCountUp >= ScoreF && Rank == 0)
        {
            FStar.SetActive(true);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_SuccessfulHitSound");
            Rank += 1;
        }

        if (RankingStart == true && ScoreCountUp >= ScoreC && Rank == 1)
        {
            FStar.SetActive(false);
            CStar.SetActive(true);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_SuccessfulHitSound");
            Rank += 1;
        }

        if (RankingStart == true && ScoreCountUp >= ScoreB && Rank == 2)
        {
            CStar.SetActive(false);
            BStar.SetActive(true);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_SuccessfulHitSound");
            Rank += 1;
        }

        if (RankingStart == true && ScoreCountUp >= ScoreA && Rank == 3)
        {
            BStar.SetActive(false);
            AStar.SetActive(true);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_SuccessfulHitSound");
            Rank += 1;
        }

        if (RankingStart == true && ScoreCountUp >= ScoreS && Rank == 4)
        {
            AStar.SetActive(false);
            SStar.SetActive(true);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_SuccessfulHitSound");
            Rank += 1;
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

            //Create F Rank even if not reached

            if (CurrentScore < ScoreF)
            {
                RankingCounter.gameObject.SetActive(true);
                RankingCounter.text = string.Format("<mspace=0.55em>{0:0}</mspace>", ScoreCountUp);
                FStar.SetActive(true);
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
