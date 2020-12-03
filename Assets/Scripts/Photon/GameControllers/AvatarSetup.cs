using System;
using Photon.Pun;
using UnityEngine;

namespace Photon.GameControllers
{
    public class AvatarSetup : MonoBehaviourPun
    {
        private void Start()
        {
            if (photonView.IsMine)
            {
                PhotonRoom.Instance.OnAllPlayersReady += AddCharacter;
            }
        }

        private void AddCharacter()
        {
            PhotonRoom.Instance.OnAllPlayersReady -= AddCharacter;
            photonView.RPC(nameof(RPC_AddCharacter), RpcTarget.OthersBuffered, PlayerInfo.Instance.CharacterType);
        }

        [PunRPC]
        private void RPC_AddCharacter(int characterTypeToAdd)
        {
            var myTransform = transform;
            var position = Vector3.zero;
            position.y += 0.8f;
            Instantiate(PlayerInfo.Instance.AllCharacters[characterTypeToAdd], position, Quaternion.identity, myTransform);
        }
    }
}
