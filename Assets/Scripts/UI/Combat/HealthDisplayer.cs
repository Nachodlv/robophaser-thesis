using System;
using System.Collections.Generic;
using Photon;
using Photon.GameControllers;
using UnityEngine;

namespace UI.Combat
{
    public class HealthDisplayer : MonoBehaviour
    {
        [SerializeField] private RectTransform hearthImage;

        private readonly List<RectTransform> _hearthsRemaining = new List<RectTransform>();

        public void DisplayHealth(PhotonPlayer player)
        {
            InstantiateHearths(player.MaxHealth);
            player.OnHealthUpdate += UpdateHealth;
        }

        private void InstantiateHearths(int maxHealth)
        {
            var myTransform = transform;
            for (var i = 0; maxHealth < i; i++)
            {
                _hearthsRemaining.Add(Instantiate(hearthImage, myTransform));
            }
        }

        private void UpdateHealth(int currentHealth)
        {
            for (var i = currentHealth; i < _hearthsRemaining.Count; i++)
            {
                _hearthsRemaining[currentHealth].gameObject.SetActive(false);
            }
        }
    }
}
