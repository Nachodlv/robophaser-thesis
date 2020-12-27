using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Photon.GameControllers
{
    public class AvatarSetup : MonoBehaviourPun
    {
        [SerializeField] private Vector3 characterPositionOffset = new Vector3(0, -1.22f, 1f);
        [SerializeField] private Quaternion characterRotationOffset = Quaternion.identity;

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
            character.transform.localPosition = new Vector3(localPosition.x, characterPositionOffset.y, localPosition.z); // Why only on the y axis?
            character.transform.localRotation = characterRotationOffset;
        }

        private void AddLocalCharacter()
        {
            if (Camera.main != null) transform.parent = Camera.main.transform;
            var myTransform = transform;
            myTransform.localPosition = Vector3.zero;
            myTransform.localRotation = Quaternion.identity;
            var playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            var character = Instantiate(PlayerInfo.Instance.GetLocalCharacter(playerNumber), Vector3.zero, Quaternion.identity,
                transform);
            character.transform.localPosition = characterPositionOffset;
            character.transform.localRotation = characterRotationOffset;
        }
    }
}
