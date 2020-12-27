using UnityEngine;

namespace UI
{
    public class EndGameScreen : MonoBehaviour
    {
        [SerializeField] private GameObject victoryTitle;
        [SerializeField] private GameObject defeatTitle;

        public void ShowVictory()
        {
            gameObject.SetActive(true);
            victoryTitle.SetActive(true);
            defeatTitle.SetActive(false);
        }

        public void ShowDefeat()
        {
            gameObject.SetActive(true);
            victoryTitle.SetActive(false);
            defeatTitle.SetActive(true);
        }
    }
}
