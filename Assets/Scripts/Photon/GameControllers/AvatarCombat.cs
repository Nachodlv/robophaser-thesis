using System;
using Photon.Pun;
using UnityEngine;

namespace Photon.GameControllers
{
    [RequireComponent(typeof(AvatarSetup))]
    public class AvatarCombat : MonoBehaviourPun
    {
        [SerializeField] private Transform rayOrigin;

        private AvatarSetup _avatarSetup;

        private void Start()
        {
            _avatarSetup = GetComponent<AvatarSetup>();
        }

        private void Update()
        {
            if (!photonView.IsMine || !Input.GetMouseButtonDown(0)) return;
            photonView.RPC(nameof(RPC_Shoot), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_Shoot()
        {
            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out var hit,
                Mathf.Infinity))
            {
                Debug.DrawRay(rayOrigin.position, rayOrigin.forward * 1000, Color.green, 100);
                Debug.Log("Hit something");
                if (!hit.transform.CompareTag("Avatar")) return;
                var avatarSetup = hit.transform.GetComponentInParent<AvatarSetup>();
                if (avatarSetup != null)
                {
                    avatarSetup.DealDamage(_avatarSetup.PlayerDamage);
                }
                return;
            }

            Debug.DrawRay(rayOrigin.position, rayOrigin.forward * 1000, Color.red, 100);
            Debug.Log("Hit nothing");
        }
    }
}
