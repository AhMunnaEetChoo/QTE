using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class PlayClip : IState
{
    private QTEManager m_manager;
    private List<QTEManager.ClipData> m_clipDataList;
    private VideoPlayer m_videoPlayer;
    private int m_clipIndex = 0;
    private GameObject m_prompt;
    private float m_promptDestroyTime;
    private bool m_gameComplete = false;
    private float m_channelChangetimer = 0.0f;

    enum QTEStage
    {
        Initialising,
        ShowPrompt,
        Showresult
    }
    enum QTEResult
    {
        None,
        Perfect,
        Great,
        Cool,
        Nice,
        Shame,
        TooBad,
        Whoops
    }
    public bool GameComplete()
    {
        return m_gameComplete;
    }

    private void SetPrompt(GameObject _newPrompt, float _promptDestroyTime)
    {
        if (m_prompt != null)
        {
            GameObject.Destroy(m_prompt);
        }
        m_prompt = _newPrompt;
        m_promptDestroyTime = _promptDestroyTime;
    }

    private class QTEState
    {
        public QTEStage m_qteStage = QTEStage.Initialising;
        public QTEResult m_qteResult = QTEResult.None;
        public float m_qteResultTimer = 0.0f;
        public int m_mashCount = 0;
        public GameObject m_rhythmTarget;
        public GameObject m_rhythmCue;
    };
    private List<QTEState> m_qteStates = new List<QTEState>();

    private QTEManager.ClipData m_currentClipData;

    public PlayClip(QTEManager _manager, List<QTEManager.ClipData> _dataList, VideoPlayer _videoPlayer)
    {
        m_manager = _manager;
        m_clipDataList = _dataList;
        m_videoPlayer = _videoPlayer;
        m_videoPlayer.loopPointReached += EndReached;
    }
    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        // TODO: is final video ...

        OnExit();
        m_gameComplete = m_clipIndex >= m_clipDataList.Count;
        if (m_clipIndex < m_clipDataList.Count)
        {
            OnEnter();
        }
    }

    private bool QTECheckTime(QTEState _qteState, QTEManager.QTE _qteData)
    {
        bool outOfTime = m_videoPlayer.time > (_qteData.triggerTime + _qteData.coolBuffer);
        if (Keyboard.current.anyKey.wasPressedThisFrame || outOfTime)
        {
            if (((KeyControl)Keyboard.current[_qteData.button]).wasPressedThisFrame)
            {
                // now check against the correct time..
                float difference = Mathf.Abs(_qteData.triggerTime - (float)m_videoPlayer.time);

                if (_qteData.qteType == QTEManager.QTEType.Instruction && !_qteData.dontPress)
                {
                    if (difference < _qteData.perfectBuffer)
                    {
                        _qteState.m_qteResult = QTEResult.Perfect;
                    }
                    else if (difference < _qteData.greatBuffer)
                    {
                        _qteState.m_qteResult = QTEResult.Great;
                    }
                    else if (difference < _qteData.coolBuffer)
                    {
                        _qteState.m_qteResult = QTEResult.Cool;
                    }
                }
                else if (_qteData.qteType == QTEManager.QTEType.Rhythm)
                {
                    if (difference < _qteData.coolBuffer)
                    {
                        _qteState.m_qteResult = QTEResult.Nice;
                    }
                }
            }

            if (_qteState.m_qteResult == QTEResult.None)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Fail");
                _qteState.m_qteResult = (QTEResult)Random.Range((int)QTEResult.Shame, (int)QTEResult.Whoops + 1);
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_SuccessfulHitSound");
                if (m_prompt)
                {
                    GameObject.Destroy(m_prompt);
                }
            }

            return true;
        }
        return false;
    }
    private void QTEShowResult(QTEState _qteState, QTEManager.QTE _qteData, Vector3 _spawnPos)
    {
        _qteState.m_qteResultTimer += Time.deltaTime;
        if (_qteState.m_qteResult != QTEResult.None && _qteState.m_qteResultTimer > 0.4f)
        {
            //after 1 second
            GameObject toSpawn = m_manager.m_perfectPrefab;
            switch (_qteState.m_qteResult)
            {
                case QTEResult.Perfect:
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Perfect");
                    m_manager.m_scoreSystem.ScorePerfectTrigger = true;
                    toSpawn = m_manager.m_perfectPrefab;
                    break;
                case QTEResult.Great:
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Great");
                    m_manager.m_scoreSystem.ScoreGreatTrigger = true;
                    toSpawn = m_manager.m_greatPrefab;
                    break;
                case QTEResult.Cool:
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Cool");
                    m_manager.m_scoreSystem.ScoreCoolTrigger = true;
                    toSpawn = m_manager.m_coolPrefab;
                    break;
                case QTEResult.Nice:
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Rhythm_Nice");
                    m_manager.m_scoreSystem.ScoreNiceTrigger = true;
                    toSpawn = m_manager.m_nicePrefab;
                    break;
                case QTEResult.Shame:
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Shame");
                    if (_qteData.qteType == QTEManager.QTEType.Instruction)
                    {
                        m_manager.m_scoreSystem.ScoreFailInstructionTrigger = true;
                    }
                    else
                    {
                        m_manager.m_scoreSystem.ScoreFailRhythmTrigger = true;
                    }
                    toSpawn = m_manager.m_shamePrefab;
                    break;
                case QTEResult.TooBad:
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_TooBad");
                    if (_qteData.qteType == QTEManager.QTEType.Instruction)
                    {
                        m_manager.m_scoreSystem.ScoreFailInstructionTrigger = true;
                    }
                    else
                    {
                        m_manager.m_scoreSystem.ScoreFailRhythmTrigger = true;
                    }
                    toSpawn = m_manager.m_tooBadPrefab;
                    break;
                case QTEResult.Whoops:
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Whoops");
                    if (_qteData.qteType == QTEManager.QTEType.Instruction)
                    {
                        m_manager.m_scoreSystem.ScoreFailInstructionTrigger = true;
                    }
                    else
                    {
                        m_manager.m_scoreSystem.ScoreFailRhythmTrigger = true;
                    }
                    toSpawn = m_manager.m_whoopsPrefab;
                    break;
            }
            GameObject canvas = GameObject.Find("Canvas");
            GameObject newText = GameObject.Instantiate(toSpawn, Vector3.zero, Quaternion.identity, canvas.transform);
            newText.transform.localPosition = _spawnPos;

            _qteState.m_qteResult = QTEResult.None;
        }
    }

    private void InstructionUpdate(QTEState _qteState, QTEManager.QTE _qteData)
    {
        switch (_qteState.m_qteStage)
        {
            case QTEStage.Initialising:
                if (m_videoPlayer.time > _qteData.promptTime)
                {
                    GameObject canvas = GameObject.Find("Canvas");

                    // spawn the text
                    SetPrompt(GameObject.Instantiate(m_manager.m_qtePrefab, m_manager.m_qtePrefab.transform.position, Quaternion.identity, canvas.transform), _qteData.triggerTime + _qteData.coolBuffer);
                    m_prompt.transform.localPosition = _qteData.screenPos;
                    TMP_Text newText = m_prompt.GetComponent<TMP_Text>();
                    newText.text = _qteData.text.Replace("[" + _qteData.button + "]", "<sprite=" + QTEManager.charToSpriteIndex[_qteData.button].ToString() + ">");
                    _qteState.m_qteStage = QTEStage.ShowPrompt;
                }
                break;
            case QTEStage.ShowPrompt:
                break;
            case QTEStage.Showresult:
                QTEShowResult(_qteState, _qteData, _qteData.resultPos);
                break;
        }
    }

    void TriggerQTE(QTEState _qteState, QTEManager.QTE _qteData)
    {
        switch (_qteData.qteType)
        {
            case QTEManager.QTEType.Instruction:
                {
                    _qteState.m_qteResultTimer = 0.0f;
                    _qteState.m_qteStage = QTEStage.Showresult;
                    break;
                }
            case QTEManager.QTEType.Mash:
                break;
            case QTEManager.QTEType.Rhythm:
                {
                    _qteState.m_qteResultTimer = 0.0f;
                    _qteState.m_qteStage = QTEStage.Showresult;
                    GameObject.Destroy(_qteState.m_rhythmCue);
                    GameObject.Destroy(_qteState.m_rhythmTarget);
                    break;
                }
        }
    }

    private void MashUpdate(QTEState _qteState, QTEManager.QTE _qteData)
    {
        switch (_qteState.m_qteStage)
        {
            case QTEStage.Initialising:
                if (m_videoPlayer.time > _qteData.promptTime)
                {
                    GameObject canvas = GameObject.Find("Canvas");

                    // spawn the text
                    SetPrompt(GameObject.Instantiate(m_manager.m_qtePrefab, m_manager.m_qtePrefab.transform.position, Quaternion.identity, canvas.transform), _qteData.triggerTime + _qteData.coolBuffer);
                    m_prompt.transform.localPosition = _qteData.screenPos;
                    TMP_Text newText = m_prompt.GetComponent<TMP_Text>();
                    newText.text = _qteData.text.Replace("[" + _qteData.button + "]", "<sprite=" + QTEManager.charToSpriteIndex[_qteData.button].ToString() + ">");
                    _qteState.m_qteStage = QTEStage.ShowPrompt;
                }
                break;
            case QTEStage.ShowPrompt:
                {
                    // if player presses the right QTE button
                    if (((KeyControl)Keyboard.current[_qteData.button]).wasPressedThisFrame)
                    {
                        _qteState.m_mashCount++;
                        m_manager.m_scoreSystem.ScoreMashTrigger = true;
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Mash_Hit");
                        // TODO: fire a star
                    }
                    if (m_videoPlayer.time > _qteData.triggerTime)
                    {
                        // finished, display result
                        if (_qteState.m_mashCount > _qteData.awesomeMashCount)
                        {
                            FMODUnity.RuntimeManager.PlayOneShot("event:/Mash_SuccessfulHitSound");
                            _qteState.m_qteResult = QTEResult.Perfect;
                        }
                        else
                        {
                            _qteState.m_qteResult = (QTEResult)Random.Range((int)QTEResult.Shame, (int)QTEResult.Whoops + 1);
                            FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Fail");
                        }
                        _qteState.m_qteStage = QTEStage.Showresult;
                        _qteState.m_qteResultTimer = 0.0f;
                    }
                    break;
                }
            case QTEStage.Showresult:
                {
                    _qteState.m_qteResultTimer += Time.deltaTime;
                    if (_qteState.m_qteResult != QTEResult.None && _qteState.m_qteResultTimer > 0.4f)
                    {
                        //after 1 second
                        GameObject toSpawn = m_manager.m_perfectPrefab;
                        switch (_qteState.m_qteResult)
                        {
                            case QTEResult.Perfect:
                            case QTEResult.Great:
                            case QTEResult.Cool:
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Mash_Awesome");
                                m_manager.m_scoreSystem.ScoreAwesomeTrigger = true;
                                toSpawn = m_manager.m_awesomePrefab;
                                break;
                            case QTEResult.Shame:
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Shame");
                                m_manager.m_scoreSystem.ScoreFailMashTrigger = true;
                                toSpawn = m_manager.m_shamePrefab;
                                break;
                            case QTEResult.TooBad:
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_TooBad");
                                m_manager.m_scoreSystem.ScoreFailMashTrigger = true;
                                toSpawn = m_manager.m_tooBadPrefab;
                                break;
                            case QTEResult.Whoops:
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Whoops");
                                m_manager.m_scoreSystem.ScoreFailMashTrigger = true;
                                toSpawn = m_manager.m_whoopsPrefab;
                                break;
                        }
                        GameObject canvas = GameObject.Find("Canvas");
                        GameObject newText = GameObject.Instantiate(toSpawn, Vector3.zero, Quaternion.identity, canvas.transform);
                        newText.transform.localPosition = _qteData.resultPos;

                        _qteState.m_qteResult = QTEResult.None;
                        _qteState.m_qteResultTimer = 0.0f;
                    }
                }
                break;
        }
    }
    private void RhythmUpdate(QTEState _qteState, QTEManager.QTE _qteData)
    {
        switch (_qteState.m_qteStage)
        {
            case QTEStage.Initialising:
                if (m_videoPlayer.time > _qteData.promptTime)
                {
                    GameObject canvas = GameObject.Find("Canvas");
                    string buttonSprite = "<sprite=" + QTEManager.charToSpriteIndex[_qteData.button].ToString() + ">";
                    // spawn the text
                    SetPrompt(GameObject.Instantiate(m_manager.m_qtePrefab, m_manager.m_qtePrefab.transform.position, Quaternion.identity, canvas.transform), _qteData.triggerTime + _qteData.coolBuffer);
                    m_prompt.transform.localPosition = _qteData.screenPos;
                    TMP_Text newText = m_prompt.GetComponent<TMP_Text>();
                    newText.text = _qteData.text.Replace("[" + _qteData.button + "]", buttonSprite);
                    _qteState.m_qteStage = QTEStage.ShowPrompt;

                    _qteState.m_rhythmTarget = GameObject.Instantiate(m_manager.m_buttonPrefab, m_manager.m_qtePrefab.transform.position, Quaternion.identity, canvas.transform);
                    _qteState.m_rhythmTarget.transform.localPosition = _qteData.rhythmTargetPos;
                    TMP_Text buttonTargetText = _qteState.m_rhythmTarget.GetComponent<TMP_Text>();
                    buttonTargetText.text = buttonSprite;

                    _qteState.m_rhythmCue = GameObject.Instantiate(m_manager.m_buttonPrefab, m_manager.m_qtePrefab.transform.position, Quaternion.identity, canvas.transform);
                    _qteState.m_rhythmCue.transform.localPosition = _qteData.rhythmCueStartPos;
                    TMP_Text buttonCueText = _qteState.m_rhythmCue.GetComponent<TMP_Text>();
                    buttonCueText.text = buttonSprite;
                }
                break;
            case QTEStage.ShowPrompt:
                float t = ((float)m_videoPlayer.time - _qteData.promptTime) / (_qteData.triggerTime - _qteData.promptTime);
                _qteState.m_rhythmCue.transform.localPosition = Vector3.Lerp(_qteData.rhythmCueStartPos, _qteData.rhythmTargetPos, t);
                break;
            case QTEStage.Showresult:
                QTEShowResult(_qteState, _qteData, _qteData.resultPos);
                break;
        }
    }
    public void Tick()
    {
        if (m_channelChangetimer <= 0.0f)
        {
            m_manager.m_tvStatic.SetActive(false);
        }
        else
        {
            m_channelChangetimer -= Time.deltaTime;
        }

        if ((float)m_videoPlayer.time >= m_promptDestroyTime)
        {
            GameObject.Destroy(m_prompt);
        }

        if (m_currentClipData.qtes.Count == 0)
        {
            return;
        }

        QTEManager.QTE closestTotrigger = m_currentClipData.qtes[0];
        QTEState closestStateToTrigger = m_qteStates[0];
        float smallestDelta = float.PositiveInfinity;

        for (int i = 0; i < m_qteStates.Count; ++i)
        {
            QTEManager.QTE currentQTE = m_currentClipData.qtes[i];
            QTEState currentState = m_qteStates[i];

            // find the closest QTE to trigger
            float triggerDelta = Mathf.Abs((float)m_videoPlayer.time - currentQTE.triggerTime);
            if (currentState.m_qteStage == QTEStage.ShowPrompt && triggerDelta < smallestDelta)
            {
                closestTotrigger = currentQTE;
                closestStateToTrigger = currentState;
                smallestDelta = triggerDelta;
            }

            // update the different types
            switch (currentQTE.qteType)
            {
                case QTEManager.QTEType.Instruction:
                    InstructionUpdate(currentState, currentQTE);
                    break;
                case QTEManager.QTEType.Mash:
                    MashUpdate(currentState, currentQTE);
                    break;
                case QTEManager.QTEType.Rhythm:
                    RhythmUpdate(currentState, currentQTE);
                    break;

            }
        }

        // check to see if we can trigger the closest QTE
        if (closestStateToTrigger.m_qteStage == QTEStage.ShowPrompt &&
            closestTotrigger.qteType != QTEManager.QTEType.Mash &&
            QTECheckTime(closestStateToTrigger, closestTotrigger))
        {
            TriggerQTE(closestStateToTrigger, closestTotrigger);
        }
    }
    public void OnEnter()
    {
        // play the fuzzy transition thing
        FlickChannel();

        // choose a clip
        m_currentClipData = m_clipDataList[m_clipIndex];

        foreach (QTEManager.QTE qte in m_currentClipData.qtes)
        {
            m_qteStates.Add(new QTEState());
        }

        m_videoPlayer.url = m_currentClipData.url;
        m_videoPlayer.Play();
        m_videoPlayer.SetDirectAudioVolume(0, m_currentClipData.volume);
    }

    public void OnExit()
    {
        foreach(QTEState qteState in m_qteStates)
        {
            GameObject.Destroy(qteState.m_rhythmCue);
            GameObject.Destroy(qteState.m_rhythmTarget);
        }

        if (m_prompt)
        {
            GameObject.Destroy(m_prompt);
        }
        m_qteStates.Clear();
        m_clipIndex++;

        m_videoPlayer.Stop();
    }

    public void FlickChannel()
    {
        m_manager.m_tvStatic.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/ChannelChange");
        m_channelChangetimer = 0.5f;
    }
}