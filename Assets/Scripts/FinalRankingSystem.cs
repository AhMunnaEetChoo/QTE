using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Animator CameraAnim;

    public bool GameEnded;
    public bool RankingStart;
    public bool RankingFinished;

    void Start()
    {
        
    }

    void Update()
    {

        if (GameEnded == true)
        {
            CameraAnim.SetBool("GameEnded", true);

            if (CameraAnim.GetCurrentAnimatorStateInfo(0).IsName("CameraEndLoop"))
            {
                RankingStart = true;
            }
        }

        if (RankingStart == true && ScoreCountUp < CurrentScore)
        {
            ScoreCountUp += CountSpeed * Time.deltaTime;
        }

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

        if (RankingStart == true && ScoreCountUp >= CurrentScore)
        {
            ScoreCountUp = CurrentScore;
            RankingStart = false;
        }

    }
}
