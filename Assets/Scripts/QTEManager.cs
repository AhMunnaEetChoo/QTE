using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
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
    public ScoreSystem m_scoreSystem;
    public FinalRankingSystem m_finalRankingSystem;
    public VideoPlayer m_videoPlayer;

    public string m_jsonURL;
    public string m_extraJsonURL;
    public bool m_webDataRetrieved = true;
    private string m_mainGameJson;
    private string m_extraGameJson;

    public GameObject m_tvStatic;
    public FMODUnity.StudioEventEmitter m_creditsMusicEmitter;

    public GameObject m_qtePrefab;
    public GameObject m_awesomePrefab;
    public GameObject m_perfectPrefab;
    public GameObject m_greatPrefab;
    public GameObject m_coolPrefab;
    public GameObject m_nicePrefab;
    public GameObject m_shamePrefab;
    public GameObject m_tooBadPrefab;
    public GameObject m_whoopsPrefab;
    public GameObject m_buttonPrefab;

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
        public Vector2 rhythmTargetPos = new Vector2(0, 100);
        public Vector2 rhythmCueStartPos = new Vector2(800, 0);
        public Vector2 resultPos = Vector2.zero;
        public QTEType qteType = QTEType.Instruction;
        public bool dontPress = false;
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
        private bool m_readyToStart = false;
        private bool m_readyForExtra = false;
        private QTEManager m_manager;
        private GameObject m_startPrompt;

        public MainMenu(QTEManager _manager)
        {
            m_manager = _manager;
        }
        public bool ReadyToStart()
        {
#if !UNITY_EDITOR
            return (m_readyToStart || m_readyForExtra) && m_manager.m_webDataRetrieved;
#else
            return m_readyToStart || m_readyForExtra;
#endif
        }

        public void Tick()
        {
            m_readyToStart = m_readyToStart || (bool)Keyboard.current?.sKey.wasPressedThisFrame;
            m_readyForExtra = m_readyToStart || (bool)Keyboard.current?.eKey.wasPressedThisFrame;
        }
        public void OnEnter()
        {
            m_manager.m_tvStatic.SetActive(true);
            m_manager.m_scoreSystem.CurrentScoreText.gameObject.SetActive(false);

            GameObject canvas = GameObject.Find("Canvas");

            // spawn the text
            m_startPrompt = GameObject.Instantiate(m_manager.m_qtePrefab, m_manager.m_qtePrefab.transform.position, Quaternion.identity, canvas.transform);
            m_startPrompt.transform.localPosition = new Vector3(0.0f, 100.0f, 0.0f);
            TMP_Text newText = m_startPrompt.GetComponent<TMP_Text>();
            string pressSText = "Press [s] to Start\n Press [e] for Extra Mode";
            newText.text = pressSText.Replace("[s]", "<sprite=" + QTEManager.charToSpriteIndex["s"].ToString() + ">").Replace("[e]", "<sprite=" + QTEManager.charToSpriteIndex["e"].ToString() + ">");

        }

        public void OnExit()
        {
            if (m_readyToStart)
            {
                m_manager.m_gameData = JsonUtility.FromJson<GameData>(m_manager.m_mainGameJson);
            }
            else if (m_readyForExtra)
            {
                m_manager.m_gameData = JsonUtility.FromJson<GameData>(m_manager.m_extraGameJson);
            }

            m_manager.m_scoreSystem.CurrentScoreText.gameObject.SetActive(true);
            m_manager.m_tvStatic.SetActive(false);
            m_readyToStart = false;
            GameObject.Destroy(m_startPrompt);
        }
    }
    public class ShowScore : IState
    {
        private bool m_isComplete = false;
        private QTEManager m_manager;

        public ShowScore(QTEManager _manager)
        {
            m_manager = _manager;
        }

        public bool IsComplete()
        {
            return true;
        }
        public void Tick()
        {
        }
        public void OnEnter()
        {
            m_manager.m_tvStatic.SetActive(false);
            m_manager.m_finalRankingSystem.GameEnded = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/CreditsApplause");
            m_manager.m_creditsMusicEmitter.Play();
        }

        public void OnExit()
        {
            m_manager.m_creditsMusicEmitter.Stop();
        }
    }

   

    // Start is called before the first frame update
    void Start()
    {
        m_scoreSystem = GameObject.Find("ScoreSystem").GetComponent<ScoreSystem>();
        m_finalRankingSystem = GameObject.Find("FinalRankingSystem").GetComponent<FinalRankingSystem>();

#if UNITY_EDITOR
        m_mainGameJson = System.IO.File.ReadAllText(Application.dataPath + "/../docs/GameData.json");
        m_extraGameJson = System.IO.File.ReadAllText(Application.dataPath + "/../docs/GameDataExtra.json");
#else
        StartCoroutine(GetRequest(m_jsonURL, m_mainGameJson));
        StartCoroutine(GetRequest(m_extraJsonURL, m_extraGameJson));
        m_webDataRetrieved = false;
#endif

        // setup state machine
        MainMenu mainMenuState = new MainMenu(this);
        PlayClip playClipState = new PlayClip(this, m_videoPlayer);
        ShowScore scoreState = new ShowScore(this);

        m_stateMachine.AddTransition(mainMenuState, playClipState, mainMenuState.ReadyToStart);
        m_stateMachine.AddTransition(playClipState, scoreState, playClipState.GameComplete);
        m_stateMachine.SetState(mainMenuState);

    }
    IEnumerator GetRequest(string uri, string output)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    output = webRequest.downloadHandler.text;
                    m_webDataRetrieved = true;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_stateMachine.Tick();
    }

    void InitialiseFromJSON(string _jsonString)
    {
        m_gameData = JsonUtility.FromJson<GameData>(_jsonString);
    }

}
