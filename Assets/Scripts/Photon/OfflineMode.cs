using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Photon
{
    public class OfflineMode : MonoBehaviour
    {
        private void Awake()
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.JoinOrCreateRoom("Room1", new RoomOptions(), TypedLobby.Default);
        }
    }
}
