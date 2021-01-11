using Cues.Animations;
using Photon.Combat;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UI;
using UnityEngine;

namespace Photon.GameControllers
{
    public class AvatarSetup : MonoBehaviourPun, IDamageTaker
    {
        [SerializeField] private Vector3 characterPositionOffset = new Vector3(0, -1.22f, 1f);
        [SerializeField] private Quaternion characterRotationOffset = Quaternion.identity;

        private PhotonPlayer _photonPlayer;
        private RobotOrbAnimator _animator;
        private StatusEffects _statusEffect;
        private ImageFlash _cameraFlash;

        private RobotOrbAnimator Animator =>
            _animator != null ? _animator : _animator = GetComponentInChildren<RobotOrbAnimator>();

        public StatusEffects StatusEffect =>
            _statusEffect != null ? _statusEffect : _statusEffect = GetComponent<StatusEffects>();

        public string Id { get; private set; }

        public PhotonPlayer PhotonPlayer => _photonPlayer;

        private void Start()
        {
            if (photonView.IsMine)
            {
                PhotonRoom.Instance.OnAllPlayersReady += AddCharacter;
            }

            Invoke(nameof(TakeDamageBy1), 10);
        }
        void TakeDamageBy1() => TakeDamage(1);

        public void TakeDamage(int damage)
        {
            _photonPlayer.ReceiveDamage(damage);
            photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All);
        }

        public void SetPhotonPlayer(int photonViewId)
        {
            photonView.RPC(nameof(RPC_SetPhotonPlayer), RpcTarget.All, photonViewId, PhotonNetwork.LocalPlayer.UserId);
        }

        private void AddCharacter()
        {
            PhotonRoom.Instance.OnAllPlayersReady -= AddCharacter;
            photonView.RPC(nameof(RPC_AddNetworkCharacter), RpcTarget.OthersBuffered);
            AddLocalCharacter();
        }

        private void AddLocalCharacter()
        {
            var cameraMain = Camera.main;
            if (cameraMain != null)
            {
                transform.parent = cameraMain.transform;
                _cameraFlash = cameraMain.GetComponent<ImageFlash>();
            }
            var myTransform = transform;
            myTransform.localPosition = Vector3.zero;
            myTransform.localRotation = Quaternion.identity;
            var playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            var character = Instantiate(PlayerInfo.Instance.GetLocalCharacter(playerNumber), Vector3.zero,
                Quaternion.identity,
                myTransform);
            character.transform.localPosition = characterPositionOffset;
            character.transform.localRotation = characterRotationOffset;
        }

        [PunRPC]
        private void RPC_AddNetworkCharacter()
        {
            var playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            var character = Instantiate(PlayerInfo.Instance.GetNetworkCharacter(playerNumber), Vector3.zero,
                Quaternion.identity,
                transform);
            var localPosition = character.transform.localPosition;
            character.transform.localPosition =
                new Vector3(localPosition.x, characterPositionOffset.y, localPosition.z); // Why only on the y axis?
            character.transform.localRotation = characterRotationOffset;
        }

        [PunRPC]
        private void RPC_SetPhotonPlayer(int photonViewId, string userId)
        {
            Id = userId;
            _photonPlayer = PhotonNetwork.GetPhotonView(photonViewId).GetComponent<PhotonPlayer>();
        }

        [PunRPC]
        private void RPC_TakeDamage()
        {
            Animator.TakeDamage();
            if(photonView.IsMine) _cameraFlash.Flash();
        }
    }
}
