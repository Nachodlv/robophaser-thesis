using System;
using Photon.Pun;
using UnityEngine;

namespace WFC
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkSimpleTileWFC : SimpleTiledWFC
    {
        [SerializeField] private float scale = 1f;

        private PhotonView _photonView;

        public float Scale => scale;
        public float GridSize => gridsize * scale;

        private void Awake()
        {
            transform.localScale = new Vector3(scale, scale, scale);
            _photonView = GetComponent<PhotonView>();
        }

        protected override GameObject InstantiateGameObject(string prefabName, Vector3 position,
            Vector3 localEulerAngles)
        {
            var newGameObject = PhotonNetwork.Instantiate(prefabName.Substring(1, prefabName.Length - 1), Vector3.zero,
                Quaternion.identity);
            var newPhotonView = newGameObject.GetComponent<PhotonView>();
            Debug.Log(_photonView.ToString());
            _photonView.RPC(
                nameof(RPC_InitializeObstacle), RpcTarget.All, newPhotonView.ViewID, position, localEulerAngles);
            return newGameObject;
        }

        [PunRPC]
        private void RPC_InitializeObstacle(int obstacleViewId, Vector3 localPosition, Vector3 localEulerAngles)
        {

            var obstacle = PhotonView.Find(obstacleViewId);
            var parent = ObstacleParent;
            Transform obstacleTransofrm;
            (obstacleTransofrm = obstacle.transform).SetParent(parent);
            obstacleTransofrm.localPosition = localPosition;
            var fscale = obstacleTransofrm.localScale;
            obstacleTransofrm.localEulerAngles = localEulerAngles;
            obstacleTransofrm.localScale = fscale;
        }
    }
}
