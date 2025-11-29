using QuestSystem;
using UnityEngine;

namespace Events
{
    public class GameEventManager : MonoBehaviour
    {
        public static GameEventManager Instance;

        public PlayerEvent PlayerEventHandler { get; private set; }
        public InputEvent InputEventHandler { get; private set; }
        public QuestEvent QuestEventHandler { get; private set; }
        public UIEvent UIEventHandler { get; private set; }
        public DialogueEvent DialogueEventHandler { get; private set; }
        public SettingEvent SettingEventHandler { get; private set; }
        public MiscEvent MiscEventHandler { get; private set; }
        
        private void Awake()
        {
            if (Instance is null) 
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            PlayerEventHandler = new PlayerEvent();
            InputEventHandler = new InputEvent();
            QuestEventHandler = new QuestEvent();
            UIEventHandler = new UIEvent();
            DialogueEventHandler = new DialogueEvent();
            SettingEventHandler = new SettingEvent();
            MiscEventHandler = new MiscEvent();
        }
    }
}
