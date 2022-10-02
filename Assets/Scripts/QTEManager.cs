using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class QTEManager : MonoBehaviour
{
    public static readonly Dictionary<string, int> charToSpriteIndex = new Dictionary<string, int>
    {
        { "1", 0 },
        { "2", 1 },
        { "3", 2 },
        { "4", 3 },
        { "5", 4 },
        { "6", 5 },
        { "space", 6 },
        { "7", 7 },
        { "8", 8 },
        { "9", 9 },
        { "a", 10 },
        { "b", 11 },
        { "c", 12 },
        { ".", 13 },
        { "~", 14 },
        { "d", 15 },
        { "e", 16 },
        { "f", 17 },
        { "g", 18 },
        { "h", 19 },
        { "i", 20 },
        { "j", 21 },
        { "k", 22 },
        { "l", 23 },
        { "m", 24 },
        { "n", 25 },
        { "o", 26 },
        { "p", 27 },
        { "q", 28 },
        { "r", 29 },
        { "s", 30 },
        { "t", 31 },
        { "u", 32 },
        { "v", 33 },
        { "w", 34 },
        { "x", 35 },
        { "y", 36 },
        { "z", 37 }
    };

    private StateMachine m_stateMachine = new StateMachine();
    public VideoPlayer m_videoPlayer;
    public TMP_Text m_timerText;

    public GameObject m_qtePrefab;
    public GameObject m_awesomePrefab;
    public GameObject m_perfectPrefab;
    public GameObject m_greatPrefab;
    public GameObject m_coolPrefab;
    public GameObject m_shamePrefab;
    public GameObject m_tooBadPrefab;
    public GameObject m_whoopsPrefab;

    public enum QTEType
    {
        Instruction,
        Mash,
        Rhythm
    }
        
    [System.Serializable]
    public class QTE
    {
        public string text = "Press [f] to pay respect";
        public string button = "f";
        public Vector2 screenPos = new Vector2(300,350);
        public float promptTime = 1.0f;
        public float triggerTime = 7.0f;
        public float perfectBuffer = 0.5f;
        public float greatBuffer = 0.8f;
        public float coolBuffer = 1.0f;
        public int awesomeMashCount = 10;
        public QTEType qteType = QTEType.Instruction;
    };

    [System.Serializable]
    public class ClipData
    {
        public string url;
        public List<QTE> qtes = new List<QTE>();
        public float volume = 1.0f;
    };

    [System.Serializable]
    public class GameData
    {
        public List<ClipData> clipData = new List<ClipData>();
    };
    public GameData m_gameData = new GameData();

    public class MainMenu : IState
    {
        private bool m_isComplete = false;
        public MainMenu()
        {
        }
        public bool IsComplete()
        {
            return true;
        }
        public void Tick()
        {
            m_isComplete = m_isComplete || (bool)Keyboard.current?.anyKey.wasPressedThisFrame;
        }
        public void OnEnter()
        {
        }

        public void OnExit() { }
    }

    public class PlayClip : IState
    {
        private QTEManager m_manager;
        private List<ClipData> m_clipDataList;
        private VideoPlayer m_videoPlayer;
        private int m_clipIndex = 0;

        enum QTEStage
        {
            Initialising,
            ShowPrompt,
            Showresult
        }
        private QTEStage m_qteStage = QTEStage.Initialising;
        private ClipData m_currentClipData;
        private int m_currentQTE = 0;
        private GameObject m_currentPrompt;
        private GameObject m_currentResult;
        enum QTEResult
        {
            None,
            Perfect,
            Great,
            Cool,
            Shame,
            TooBad,
            Whoops
        }
        private QTEResult m_qteResult = QTEResult.None;
        private float m_qteResultTimer = 0.0f;
        private int m_mashCount = 0;

        public PlayClip(QTEManager _manager, List<ClipData> _dataList, VideoPlayer _videoPlayer)
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
            if (m_clipIndex < m_clipDataList.Count)
            {
                OnEnter();
            }
        }

        private void InstructionUpdate()
        {
            QTE currentQTE = m_currentClipData.qtes[m_currentQTE];
            switch (m_qteStage)
            {
                case QTEStage.Initialising:
                    if (m_videoPlayer.time > currentQTE.promptTime)
                    {
                        GameObject canvas = GameObject.Find("Canvas");

                        // spawn the text
                        m_currentPrompt = Instantiate(m_manager.m_qtePrefab, currentQTE.screenPos, Quaternion.identity, canvas.transform);
                        TMP_Text newText = m_currentPrompt.GetComponent<TMP_Text>();
                        newText.text = currentQTE.text.Replace("[" + currentQTE.button + "]", "<sprite=" + QTEManager.charToSpriteIndex[currentQTE.button].ToString() + ">");
                        m_qteStage = QTEStage.ShowPrompt;
                    }
                    break;
                case QTEStage.ShowPrompt:
                    // if player presses the right QTE button
                    if (((KeyControl)Keyboard.current[currentQTE.button]).isPressed)
                    {
                        // now check against the correct time..
                        float difference = Mathf.Abs(currentQTE.triggerTime - (float)m_videoPlayer.time);
                        if (difference < currentQTE.perfectBuffer)
                        {
                            FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_SuccessfulHitSound");
                            m_qteResult = QTEResult.Perfect;
                        }
                        else if (difference < currentQTE.greatBuffer)
                        {
                            FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_SuccessfulHitSound");
                            m_qteResult = QTEResult.Great;
                        }
                        else if (difference < currentQTE.coolBuffer)
                        {
                            FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_SuccessfulHitSound");
                            m_qteResult = QTEResult.Cool;
                        }
                        else
                        {
                            FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Fail");
                            // pick a random failure
                            m_qteResult =  (QTEResult)Random.Range((int)QTEResult.Shame, (int)QTEResult.Whoops+1);
                        }
                        m_qteResultTimer = 0.0f;

                        // spawn the text
                        m_qteStage = QTEStage.Showresult;

                        // for now destroy
                        GameObject.Destroy(m_currentPrompt);
                    }
                    break;
                case QTEStage.Showresult:
                    m_qteResultTimer += Time.deltaTime;
                    if (m_qteResult != QTEResult.None && m_qteResultTimer > 1.0f)
                    {
                        //after 1 second
                        GameObject toSpawn = m_manager.m_perfectPrefab;
                        switch (m_qteResult)
                        {
                            case QTEResult.Perfect:
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Perfect");
                                toSpawn = m_manager.m_perfectPrefab;
                                break;
                            case QTEResult.Great:
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Great");
                                toSpawn = m_manager.m_greatPrefab;
                                break;
                            case QTEResult.Cool:
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Cool");
                                toSpawn = m_manager.m_coolPrefab;
                                break;
                            case QTEResult.Shame:
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Shame");
                                toSpawn = m_manager.m_shamePrefab;
                                break;
                            case QTEResult.TooBad:
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_TooBad");
                                toSpawn = m_manager.m_tooBadPrefab;
                                break;
                            case QTEResult.Whoops:
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Whoops");
                                toSpawn = m_manager.m_whoopsPrefab;
                                break;
                        }
                        GameObject canvas = GameObject.Find("Canvas");
                        m_currentResult = Instantiate(toSpawn, new Vector2(300.0f, 300.0f), Quaternion.identity, canvas.transform);

                        m_qteResult = QTEResult.None;
                    }
                    break;
            }
        }
        private void MashUpdate()
        {
            QTE currentQTE = m_currentClipData.qtes[m_currentQTE];
            switch (m_qteStage)
            {
                case QTEStage.Initialising:
                    if (m_videoPlayer.time > currentQTE.promptTime)
                    {
                        GameObject canvas = GameObject.Find("Canvas");

                        // spawn the text
                        m_currentPrompt = Instantiate(m_manager.m_qtePrefab, currentQTE.screenPos, Quaternion.identity, canvas.transform);
                        TMP_Text newText = m_currentPrompt.GetComponent<TMP_Text>();
                        newText.text = currentQTE.text.Replace("[" + currentQTE.button + "]", "<sprite=" + QTEManager.charToSpriteIndex[currentQTE.button].ToString() + ">");
                        m_qteStage = QTEStage.ShowPrompt;
                    }
                    break;
                case QTEStage.ShowPrompt:
                    {
                        // if player presses the right QTE button
                        if (((KeyControl)Keyboard.current[currentQTE.button]).isPressed)
                        {
                            m_mashCount++;
                            // play a small ping thing,
                            // fire a star
                        }
                        if (m_videoPlayer.time > currentQTE.triggerTime)
                        {
                            // finished, display result
                            if(m_mashCount > currentQTE.awesomeMashCount)
                            {
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Mash_SuccessfulHitSound");
                                m_qteResult = QTEResult.Perfect;
                            }
                            else
                            {
                                m_qteResult = (QTEResult)Random.Range((int)QTEResult.Shame, (int)QTEResult.Whoops+1);
                                FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Fail");
                            }
                            m_qteStage = QTEStage.Showresult;
                            m_qteResultTimer = 0.0f;
                        }
                        break;
                    }
                case QTEStage.Showresult:
                    {
                        m_qteResultTimer += Time.deltaTime;
                        if (m_qteResult != QTEResult.None && m_qteResultTimer > 1.0f)
                        {
                            //after 1 second
                            GameObject toSpawn = m_manager.m_perfectPrefab;
                            switch (m_qteResult)
                            {
                                case QTEResult.Perfect:
                                case QTEResult.Great:
                                case QTEResult.Cool:
                                    FMODUnity.RuntimeManager.PlayOneShot("event:/Mash_Awesome");
                                    toSpawn = m_manager.m_awesomePrefab;
                                    break;
                                case QTEResult.Shame:
                                    FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Shame");
                                    toSpawn = m_manager.m_shamePrefab;
                                    break;
                                case QTEResult.TooBad:
                                    FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_TooBad");
                                    toSpawn = m_manager.m_tooBadPrefab;
                                    break;
                                case QTEResult.Whoops:
                                    FMODUnity.RuntimeManager.PlayOneShot("event:/Instruction_Whoops");
                                    toSpawn = m_manager.m_whoopsPrefab;
                                    break;
                            }
                            GameObject canvas = GameObject.Find("Canvas");
                            m_currentResult = Instantiate(toSpawn, new Vector2(300.0f, 300.0f), Quaternion.identity, canvas.transform);

                            m_qteResult = QTEResult.None;
                            m_qteResultTimer = 0.0f;
                        }
                    }
                    break;
            }
        }
        private void RhythmUpdate()
        {

        }
        public void Tick()
        {
            if (m_currentQTE >= m_currentClipData.qtes.Count)
            {
                return;
            }
            QTE currentQTE = m_currentClipData.qtes[m_currentQTE];

            switch (currentQTE.qteType)
            {
                case QTEType.Instruction:
                    InstructionUpdate();
                    break;
                case QTEType.Mash:
                    MashUpdate();
                    break;
                case QTEType.Rhythm:
                    RhythmUpdate();
                    break;

            }
        }
        public void OnEnter()
        {
            // choose a clip
            m_currentClipData = m_clipDataList[m_clipIndex];
            m_videoPlayer.url = m_currentClipData.url;
            m_videoPlayer.Play();
            for(ushort i = 0; i < m_videoPlayer.audioTrackCount; ++i)
            {
                m_videoPlayer.SetDirectAudioVolume(i, m_currentClipData.volume);
            }
        }

        public void OnExit()
        {
            GameObject.Destroy(m_currentPrompt);
            GameObject.Destroy(m_currentResult);
            m_mashCount = 0;
            m_currentQTE = 0;
            m_qteStage = QTEStage.Initialising;
            m_qteResult = QTEResult.None;
            m_clipIndex++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // setup state machine
        MainMenu mainMenuState = new MainMenu();
        PlayClip playClipState = new PlayClip(this, m_gameData.clipData, m_videoPlayer);

        m_stateMachine.AddTransition(mainMenuState, playClipState, mainMenuState.IsComplete);
        m_stateMachine.SetState(mainMenuState);
    }

    // Update is called once per frame
    void Update()
    {
        double seconds = m_videoPlayer.time;
        double hundredseconds = m_videoPlayer.time * 100.0;
        m_timerText.text = string.Format("{0:0}", seconds);

        m_stateMachine.Tick();
    }

    void InitialiseFromJSON(string _jsonString)
    {
        m_gameData = JsonUtility.FromJson<GameData>(_jsonString);
    }
}
