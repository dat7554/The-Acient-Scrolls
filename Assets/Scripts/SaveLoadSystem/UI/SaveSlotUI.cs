using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaveLoadSystem.UI
{
    public class SaveSlotUI : MonoBehaviour
    {
        [Header("Profile")]
        [SerializeField] private string profileId;
        
        [Header("Content")]
        [SerializeField] private GameObject noDataContent;
        [SerializeField] private GameObject hasDataContent;
        [SerializeField] private TextMeshProUGUI date;
        [SerializeField] private TextMeshProUGUI time;
        
        private Button _saveSlotUIButton;
        
        public bool HasData { get; private set; }
        public bool IsInteractable => _saveSlotUIButton.interactable;

        private void Awake()
        {
            _saveSlotUIButton = GetComponent<Button>();
        }

        public void SetGameData(GameData gameData)
        {
            if (gameData == null)
            {
                HasData = false;
                
                noDataContent.SetActive(true);
                hasDataContent.SetActive(false);
            }
            else
            {
                HasData = true;
                
                noDataContent.SetActive(false);
                hasDataContent.SetActive(true);
                
                date.text = DateTime.Parse(gameData.dateTimeISO).ToString("MM/dd/yyyy");
                time.text = DateTime.Parse(gameData.dateTimeISO).ToString("HH:mm");
            }
        }

        public string GetProfileId()
        {
            return this.profileId;
        }

        public void SetInteractable(bool interactable)
        {
            _saveSlotUIButton.interactable = interactable;
        } 
    }
}
