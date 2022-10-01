using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class QTEManager : MonoBehaviour
{
    private StateMachine m_stateMachine = new StateMachine();
    public VideoPlayer m_videoPlayer;
    public TMP_Text m_timerText;
    public GameObject m_qtePrefab;
    public GameObject m_resultPrefab;


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
    };

    [System.Serializable]
    public class ClipData
    {
        public string url;
        public List<QTE> qtes = new List<QTE>();
        public float volume = 1;
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
            KeyPressed
        }
        private QTEStage m_qteStage = QTEStage.Initialising;
        private ClipData m_currentClipData;
        private int m_currentQTE = 0;
        private GameObject m_currentPrompt;
        private GameObject m_currentResult;

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
            OnEnter();
        }

        public void Tick()
        {
            if (m_currentQTE >= m_currentClipData.qtes.Count)
            {
                return;
            }
            QTE currentQTE = m_currentClipData.qtes[m_currentQTE];

            switch (m_qteStage)
            {
                case QTEStage.Initialising:
                    if(m_videoPlayer.time > currentQTE.promptTime)
                    {
                        GameObject canvas = GameObject.Find("Canvas");

                        // spawn the text
                        m_currentPrompt = Instantiate(m_manager.m_qtePrefab, currentQTE.screenPos, Quaternion.identity, canvas.transform);
                        TMP_Text newText = m_currentPrompt.GetComponent<TMP_Text>();
                        newText.text = currentQTE.text;
                        m_qteStage = QTEStage.ShowPrompt;
                    }
                    break;
                case QTEStage.ShowPrompt:
                    // if player presses the right QTE button
                    if (((KeyControl)Keyboard.current[currentQTE.button]).isPressed)
                    {
                        // now check against the correct time..
                        float difference = Mathf.Abs(currentQTE.triggerTime - (float)m_videoPlayer.time);
                        string resultText = "";
                        if (difference < currentQTE.perfectBuffer)
                        {
                            resultText = "PERFECT!";
                        }
                        else if(difference < currentQTE.greatBuffer)
                        {
                            resultText = "GREAT!";
                        }
                        else if(difference < currentQTE.coolBuffer)
                        {
                            resultText = "Cool";
                        }
                        else
                        {
                            resultText = "Shame";
                            // fail
                        }

                        // for now spawn some test text
                        GameObject canvas = GameObject.Find("Canvas");
                        // spawn the text
                        m_currentResult = Instantiate(m_manager.m_resultPrefab, new Vector2(300.0f, 300.0f), Quaternion.identity, canvas.transform);
                        TMP_Text newText = m_currentResult.GetComponent<TMP_Text>();
                        newText.text = resultText;
                        m_qteStage = QTEStage.KeyPressed;

                        // for now destroy
                        GameObject.Destroy(m_currentPrompt);
                    }
                    break;
                case QTEStage.KeyPressed:
                    // nothing
                    break;
            }
        }
        public void OnEnter()
        {
            // choose a clip
            //int index = Random.Range(0, m_clipDataList.Count);
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
            m_currentQTE = 0;
            m_qteStage = QTEStage.Initialising;
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
