using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum EndGameReason
    {
        PlayerDestroyed,
        PlayerDisconnect,
    }
    [Serializable]
    public class EndGameMessage
    {
        [SerializeField] private EndGameReason endGameReason;
        [SerializeField] private GameObject message;
        public EndGameReason EndGameReason => endGameReason;
        public GameObject Message => message;
    }
    public class EndGameScreen : MonoBehaviour
    {
        [SerializeField] private GameObject victoryTitle;
        [SerializeField] private GameObject defeatTitle;
        [SerializeField] private Button rematchButton;
        [SerializeField] private EndGameMessage[] victoryMessages;
        [SerializeField] private EndGameMessage[] defeatMessages;

        public void ShowVictory(EndGameReason endGameReason = EndGameReason.PlayerDestroyed)
        {
            gameObject.SetActive(true);
            victoryTitle.SetActive(true);
            defeatTitle.SetActive(false);
            HideMessages(defeatMessages);
            ShowMessage(victoryMessages, endGameReason);
        }

        public void ShowDefeat(EndGameReason endGameReason = EndGameReason.PlayerDestroyed)
        {
            gameObject.SetActive(true);
            victoryTitle.SetActive(false);
            defeatTitle.SetActive(true);
            HideMessages(victoryMessages);
            ShowMessage(defeatMessages, endGameReason);
        }

        public void DisableRematch()
        {
            rematchButton.interactable = false;
        }

        private void ShowMessage(EndGameMessage[] messages, EndGameReason endGameReason)
        {
            foreach (var endGameMessage in messages)
            {
                endGameMessage.Message.SetActive(endGameMessage.EndGameReason == endGameReason);
            }
        }

        private void HideMessages(EndGameMessage[] messages)
        {
            foreach (var endGameMessage in messages)
            {
                endGameMessage.Message.SetActive(false);
            }
        }
    }
}
