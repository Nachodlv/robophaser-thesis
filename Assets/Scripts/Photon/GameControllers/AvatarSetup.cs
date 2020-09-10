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
                photonView.RPC(nameof(RPC_AddCharacter), RpcTarget.OthersBuffered, PlayerInfo.Instance.CharacterType);
            }
        }

        [PunRPC]
        public void RPC_AddCharacter(int characterTypeToAdd)
        {
            var myTransform = transform;
            var position = Vector3.zero;
            position.y += 0.8f;
            Instantiate(PlayerInfo.Instance.AllCharacters[characterTypeToAdd], position, Quaternion.identity, myTransform);
        }

        public void DealDamage(int damage)
        {
            playerHealth -= damage;
        }
    }
}
