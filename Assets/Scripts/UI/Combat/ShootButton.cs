using System;
using System.Threading.Tasks;
using Photon.GameControllers;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Combat
{
    public class ShootButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private Shooter _shooter;

        private void Awake()
        {
            button.onClick.AddListener(Shoot);
        }

        public void Show(Shooter shooter)
        {
            _shooter = shooter;
        }

        private void Shoot()
        {
            if(_shooter != null) _shooter.Shoot();
        }
    }
}
