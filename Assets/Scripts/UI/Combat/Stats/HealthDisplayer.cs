using System;
using System.Collections.Generic;
using Photon;
using Photon.GameControllers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Combat
{
    public class HealthDisplayer : MonoBehaviour
    {
        [SerializeField] private StatBarSlider slider;

        public void DisplayHealth(PhotonPlayer player)
        {
            if (!slider.gameObject.activeSelf) slider.gameObject.SetActive(true);
            slider.MaxValue = player.MaxHealth;
            slider.Value = player.MaxHealth;
            player.OnHealthUpdate += UpdateHealth;
        }

        private void UpdateHealth(int currentHealth)
        {
            slider.Value = currentHealth;
        }
    }
}
