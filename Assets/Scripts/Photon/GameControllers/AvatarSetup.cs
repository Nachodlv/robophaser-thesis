using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Photon.GameControllers
{
    public class AvatarSetup : MonoBehaviourPun
    {
        [SerializeField] private Vector3 characterOffset = new Vector3(0, -1.22f, 1f);

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
            var character = Instantiate(PlayerInfo.Instance.GetNetworkCharacter(playerNumber), Vector3.zero, Quaternion.identity,
                transform);
            var localPosition = character.transform.localPosition;
            character.transform.localPosition = new Vector3(localPosition.x, characterOffset.y, localPosition.z);
        }

        private void AddLocalCharacter()
        {
            if (Camera.main != null) transform.parent = Camera.main.transform;
            var playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            var character = Instantiate(PlayerInfo.Instance.GetLocalCharacter(playerNumber), Vector3.zero, Quaternion.identity,
                transform);
            character.transform.localPosition = characterOffset;
        }
    }
}
