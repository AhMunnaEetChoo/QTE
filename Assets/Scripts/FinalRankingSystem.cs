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
    public bool RankingFinished;

    void Start()
    {
        
    }

    void Update()
    {

        if (GameEnded == true)
        {
            CameraAnim.SetBool("GameEnded", true);
            CurrentScoreText.gameObject.SetActive(false);

            if (CameraAnim.GetCurrentAnimatorStateInfo(0).IsName("CameraEndLoop"))
            {
                RankingStart = true;
            }
        }

        if (RankingStart == true && ScoreCountUp < CurrentScore)
        {
            ScoreCountUp += CountSpeed * Time.deltaTime;

            RankingCounter.gameObject.SetActive(true);
            RankingCounter.text = string.Format("<mspace=0.55em>{0:0}</mspace>", ScoreCountUp);
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
