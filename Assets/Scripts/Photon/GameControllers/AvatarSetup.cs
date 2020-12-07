using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
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
            photonView.RPC(nameof(RPC_AddNetworkCharacter), RpcTarget.OthersBuffered);
            AddLocalCharacter();
        }

        [PunRPC]
        private void RPC_AddNetworkCharacter()
        {
            var playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            var myTransform = transform;
            var position = Vector3.zero;
            position.y += 0.8f;
            Instantiate(PlayerInfo.Instance.GetNetworkCharacter(playerNumber), position, Quaternion.identity,
                myTransform);
        }

        private static void AddLocalCharacter()
        {
            var playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            var cameraMain = Camera.main;
            if (cameraMain == null) return;
            Instantiate(PlayerInfo.Instance.GetLocalCharacter(playerNumber), new Vector3(0, -1.22f, 0.5f),
                Quaternion.identity, cameraMain.transform);
        }
    }
}
