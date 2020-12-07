using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Photon.GameControllers
{
    public class AvatarSetup : MonoBehaviourPun
    {
        [SerializeField] private Vector3 characterOffset = new Vector3(0, -1.22f, 0.5f);

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
            photonView.RPC(nameof(RPC_AddNetworkCharacter), RpcTarget.OthersBuffered);
            AddLocalCharacter();
        }

        [PunRPC]
        private void RPC_AddNetworkCharacter()
        {
            var playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            Instantiate(PlayerInfo.Instance.GetNetworkCharacter(playerNumber), characterOffset, Quaternion.identity,
                transform);
        }

        private void AddLocalCharacter()
        {
            var playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            Instantiate(PlayerInfo.Instance.GetLocalCharacter(playerNumber), characterOffset, Quaternion.identity,
                transform);
        }
    }
}
