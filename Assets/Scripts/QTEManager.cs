using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Video;

public class QTEManager : MonoBehaviour
{
    private StateMachine m_stateMachine = new StateMachine();
    public VideoPlayer m_videoPlayer;

    [System.Serializable]
    public class ClipData
    {
        public string m_url;
        public string m_text;
    };
    public List<ClipData> m_clipData = new List<ClipData>();

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
        private List<ClipData> m_clipDataList;
        private VideoPlayer m_videoPlayer;

        public PlayClip(List<ClipData> _dataList, VideoPlayer _videoPlayer)
        {
            m_clipDataList = _dataList;
            m_videoPlayer = _videoPlayer;
            m_videoPlayer.loopPointReached += EndReached;
        }
        void EndReached(UnityEngine.Video.VideoPlayer vp)
        {
            OnEnter();
        }

        public void Tick()
        {
        }
        public void OnEnter()
        {
            // choose a clip
            int index = Random.Range(0, m_clipDataList.Count);
            m_videoPlayer.url = m_clipDataList[index].m_url;
            m_videoPlayer.Play();

            // start timers
        }

        public void OnExit() { }
    }

    // Start is called before the first frame update
    void Start()
    {
        // setup state machine
        MainMenu mainMenuState = new MainMenu();
        PlayClip playClipState = new PlayClip(m_clipData, m_videoPlayer);

        m_stateMachine.AddTransition(mainMenuState, playClipState, mainMenuState.IsComplete);
        m_stateMachine.SetState(mainMenuState);
    }

    // Update is called once per frame
    void Update()
    {
        m_stateMachine.Tick();
    }
}
