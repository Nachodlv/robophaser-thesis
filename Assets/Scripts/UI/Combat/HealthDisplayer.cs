using System;
using System.Collections.Generic;
using Photon;
using Photon.GameControllers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Combat
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HealthDisplayer : MonoBehaviour
    {
        [SerializeField] private RectTransform hearthImage;

        private readonly List<RectTransform> _hearthsRemaining = new List<RectTransform>();
        private HorizontalLayoutGroup _layoutGroup;

        private void Awake()
        {
            _layoutGroup = GetComponent<HorizontalLayoutGroup>();
        }

        public void DisplayHealth(PhotonPlayer player)
        {
            InstantiateHearths(player.MaxHealth);
            player.OnHealthUpdate += UpdateHealth;
        }

        private void InstantiateHearths(int maxHealth)
        {
            _layoutGroup.childControlWidth = true;
            var myTransform = transform;
            for (var i = 0; i < maxHealth; i++)
            {
                _hearthsRemaining.Add(Instantiate(hearthImage, myTransform));
            }
        }

        private void UpdateHealth(int currentHealth)
        {
            if(_layoutGroup.childControlWidth) _layoutGroup.childControlWidth = false;

            for (var i = currentHealth < 0 ? 0 : currentHealth; i < _hearthsRemaining.Count; i++)
            {
                _hearthsRemaining[i].gameObject.SetActive(false);
            }
        }
    }
}
