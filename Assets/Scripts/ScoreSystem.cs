using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public int CurrentScore;

    public int ScorePerfect;
    public int ScoreGreat;
    public int ScoreCool;
    public int ScoreAwesome;
    public int ScoreNice;
    public int ScoreMash;
    public int ScoreFailInstruction;
    public int ScoreFailMash;
    public int ScoreFailRhythm;

    public TMP_Text CurrentScoreText;

    public GameObject RainbowBurstParticle;
    public GameObject BlueBurstParticle;
    public GameObject BlueSingleParticle;
    public GameObject StarTarget;

    public bool ScorePerfectTrigger;
    public bool ScoreGreatTrigger;
    public bool ScoreCoolTrigger;
    public bool ScoreAwesomeTrigger;
    public bool ScoreNiceTrigger;
    public bool ScoreMashTrigger;
    public bool ScoreFailInstructionTrigger;
    public bool ScoreFailMashTrigger;
    public bool ScoreFailRhythmTrigger;

    void Start()
    {
        
    }

    void Update()
    {

        if (CurrentScore < 0)
        {
            CurrentScore = 0;
        }

        //Score display with 9 leading zeros

        if (CurrentScore <= 9)
        {
            CurrentScoreText.text = string.Format("<mspace=0.55em>00000000{0:0}</mspace>", CurrentScore);
        }

        if (CurrentScore <= 99 && CurrentScore > 9)
        {
            CurrentScoreText.text = string.Format("<mspace=0.55em>0000000{0:0}</mspace>", CurrentScore);
        }

        if (CurrentScore <= 999 && CurrentScore > 99)
        {
            CurrentScoreText.text = string.Format("<mspace=0.55em>000000{0:0}</mspace>", CurrentScore);
        }

        if (CurrentScore <= 9999 && CurrentScore > 999)
        {
            CurrentScoreText.text = string.Format("<mspace=0.55em>00000{0:0}</mspace>", CurrentScore);
        }

        if (CurrentScore <= 99999 && CurrentScore > 9999)
        {
            CurrentScoreText.text = string.Format("<mspace=0.55em>0000{0:0}</mspace>", CurrentScore);
        }

        if (CurrentScore <= 999999 && CurrentScore > 99999)
        {
            CurrentScoreText.text = string.Format("<mspace=0.55em>000{0:0}</mspace>", CurrentScore);
        }

        if (CurrentScore <= 9999999 && CurrentScore > 999999)
        {
            CurrentScoreText.text = string.Format("<mspace=0.55em>00{0:0}</mspace>", CurrentScore);
        }

        if (CurrentScore <= 99999999 && CurrentScore > 9999999)
        {
            CurrentScoreText.text = string.Format("<mspace=0.55em>0{0:0}</mspace>", CurrentScore);
        }

        if (CurrentScore > 99999999)
        {
            CurrentScoreText.text = string.Format("<mspace=0.55em>{0:0}</mspace>", CurrentScore);
        }

        //Bool Triggers from GameManager

        if (ScorePerfectTrigger == true)
        {
            ScorePerfectTrigger = false;
            Instantiate(RainbowBurstParticle, StarTarget.transform.position, StarTarget.transform.rotation);
            CurrentScore += ScorePerfect;
        }

        if (ScoreGreatTrigger == true)
        {
            ScoreGreatTrigger = false;
            Instantiate(BlueBurstParticle, StarTarget.transform.position, StarTarget.transform.rotation);
            CurrentScore += ScoreGreat;
        }

        if (ScoreCoolTrigger == true)
        {
            ScoreCoolTrigger = false;
            CurrentScore += ScoreCool;
        }

        if (ScoreAwesomeTrigger == true)
        {
            ScoreAwesomeTrigger = false;
            Instantiate(RainbowBurstParticle, StarTarget.transform.position, StarTarget.transform.rotation);
            CurrentScore += ScoreAwesome;
        }

        if (ScoreNiceTrigger == true)
        {
            ScoreNiceTrigger = false;
            CurrentScore += ScoreAwesome;
        }

        if (ScoreMashTrigger == true)
        {
            ScoreMashTrigger = false;
            Instantiate(BlueSingleParticle, StarTarget.transform.position, StarTarget.transform.rotation);
            CurrentScore += ScoreMash;
        }

        if (ScoreFailInstructionTrigger == true)
        {
            ScoreFailInstructionTrigger = false;
            CurrentScore += ScoreFailInstruction;
        }

        if (ScoreFailMashTrigger == true)
        {
            ScoreFailMashTrigger = false;
            CurrentScore += ScoreFailInstruction;
        }

        if (ScoreFailRhythmTrigger == true)
        {
            ScoreFailRhythmTrigger = false;
            CurrentScore += ScoreFailInstruction;
        }


    }
}
