using System;
using Photon.Pun;
using UnityEngine;

namespace Photon.GameControllers
{
    public class AvatarSetup : MonoBehaviourPun
    {
        [SerializeField] private int playerHealth;
        [SerializeField] private int playerDamage;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private AudioListener audioListener;

        public int PlayerDamage => playerDamage;

        private void Start()
        {
            if (photonView.IsMine)
            {
                photonView.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.Instance.CharacterType);
            }
            else
            {
                Destroy(playerCamera);
                Destroy(audioListener);
            }
        }

        [PunRPC]
        public void RPC_AddCharacter(int characterTypeToAdd)
        {
            var myTransform = transform;
            Instantiate(PlayerInfo.Instance.AllCharacters[characterTypeToAdd], myTransform.position, myTransform.rotation, myTransform);
        }

        public void DealDamage(int damage)
        {
            playerHealth -= damage;
        }
    }
}
