using System;
using Photon.Pun;
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
                photonView.RPC(nameof(RPC_AddCharacter), RpcTarget.AllBuffered, PlayerInfo.Instance.CharacterType);
            }
        }

        [PunRPC]
        public void RPC_AddCharacter(int characterTypeToAdd)
        {
            var myTransform = transform;
            Instantiate(PlayerInfo.Instance.AllCharacters[characterTypeToAdd], Vector3.zero, Quaternion.identity, myTransform);
        }

        public void DealDamage(int damage)
        {
            playerHealth -= damage;
        }
    }
}
