using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Photon.GameControllers
{
    public class AvatarSetup : MonoBehaviourPun
    {
        [SerializeField] private int playerHealth;
        [SerializeField] private int playerDamage;

        public int PlayerDamage => playerDamage;

        private void Start()
        {
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(RPC_AddCharacter), RpcTarget.OthersBuffered);
            }
        }

        [PunRPC]
        public void RPC_AddCharacter()
        {
            var playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            var myTransform = transform;
            var position = Vector3.zero;
            position.y += 0.8f;
            Instantiate(PlayerInfo.Instance.GetNetworkCharacter(playerNumber), position, Quaternion.identity, myTransform);
        }

        public void DealDamage(int damage)
        {
            playerHealth -= damage;
        }
    }
}
