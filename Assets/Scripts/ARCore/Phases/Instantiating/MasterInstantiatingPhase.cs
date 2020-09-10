using System.IO;
using GoogleARCore.Examples.CloudAnchors;
using Photon.Pun;
using UnityEngine;

namespace ARCore.Phases
{
    public class MasterInstantiatingPhase : Phase
    {
        private readonly NetworkUIController _networkUiController;
        private readonly CloudAnchorsExampleController _cloudAnchors;
        private GameArea _gameArea;

        public MasterInstantiatingPhase(PhaseManager phaseManager, NetworkUIController networkUiController,
            CloudAnchorsExampleController cloudAnchors) : base(phaseManager)
        {
            _networkUiController = networkUiController;
            _cloudAnchors = cloudAnchors;
        }

        public override void OnEnter()
        {
            _networkUiController.ShowDebugMessage("Tap a plane to place the area game");
            _cloudAnchors.OnPlaneTouch += Touch;
        }

        public override void OnExit()
        {
            _cloudAnchors.OnPlaneTouch -= Touch;
        }

        private void Touch(Vector3 position, Quaternion rotation)
        {
            _cloudAnchors.OnPlaneTouch -= Touch;
            _networkUiController.ShowDebugMessage(
                $"Touch at: {position.ToString()} with rotation: {rotation.ToString()}");
            _gameArea = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Game Area"), position, rotation)
                .GetComponent<GameArea>();
            _gameArea.OnConfirmChanges += GameAreaConfirmed;
            _gameArea.ShowGameArea(position, rotation);
        }

        private void GameAreaConfirmed(float width, float depth)
        {
            _networkUiController.ShowDebugMessage(
                $"Position: {_gameArea.GameAreaPosition}, Rotation: {_gameArea.GameAreaRotation}, Width: {width}, Depth: {depth}");
        }
    }
}
