using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    private StateMachine m_stateMachine = new StateMachine();
    public List<ClipData> m_clipData = new List<ClipData>();

    public class ClipData
    {
        public string m_url;
        public string m_text;
    }

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
        
        public PlayClip(List<ClipData> _dataList)
        {
            m_clipDataList = _dataList;
        }
        public void Tick()
        {
        }
        public void OnEnter()
        {
            // choose a clip
            // start timers
        }

        public void OnExit() { }
    }

    // Start is called before the first frame update
    void Start()
    {
        // setup state machine
        MainMenu mainMenuState = new MainMenu();
        PlayClip playClipState = new PlayClip(m_clipData);

        m_stateMachine.AddTransition(mainMenuState, playClipState, mainMenuState.IsComplete);
        m_stateMachine.SetState(mainMenuState);
    }

    // Update is called once per frame
    void Update()
    {
        m_stateMachine.Tick();
    }
}
