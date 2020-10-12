using System.IO;
using GoogleARCore.Examples.CloudAnchors;
using Photon.Pun;
using UnityEngine;
using WFC;

namespace ARCore.Phases.Instantiating
{
    public class MasterInstantiatingPhase : Phase
    {
        private readonly NetworkUIController _networkUiController;
        private readonly CloudAnchorsExampleController _cloudAnchors;
        private GameArea _gameArea;
        private bool _settingUpGameArea;

        public MasterInstantiatingPhase(PhaseManager phaseManager, NetworkUIController networkUiController,
            CloudAnchorsExampleController cloudAnchors) : base(phaseManager)
        {
            _networkUiController = networkUiController;
            _cloudAnchors = cloudAnchors;
        }

        public override void OnEnter()
        {
            SetInitialMessage();
            _cloudAnchors.OnPlaneTouch += Touch;
        }

        public override void OnExit()
        {
            _cloudAnchors.OnPlaneTouch -= Touch;
        }

        private void Touch(Vector3 position, Quaternion rotation)
        {
            if (_settingUpGameArea) return;
            _settingUpGameArea = true;
            _networkUiController.ShowDebugMessage("Configure the game area");

            if (_gameArea == null)
            {
                _gameArea = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Game Area"), position, rotation)
                    .GetComponent<GameArea>();
                _gameArea.OnConfirmChanges += GameAreaConfirmed;
                _gameArea.OnChangePosition += OnChangePosition;
            }

            _gameArea.ShowGameArea(position, rotation);
        }

        private void SetInitialMessage()
        {
            _networkUiController.ShowDebugMessage("Tap a plane to place the area game");
        }

        private void OnChangePosition()
        {
            SetInitialMessage();
            _settingUpGameArea = false;
        }

        private void GameAreaConfirmed(float width, float depth)
        {
            _networkUiController.ShowDebugMessage(
                $"Game area setup finished! Creating the obstacles.");
            var obstacleGenerator = PhotonNetwork
                .Instantiate(Path.Combine("WFC Prefabs", "Obstacle Generator"), Vector3.zero, Quaternion.identity)
                .GetComponent<ObstacleGenerator>();
            obstacleGenerator.CreateObstacles(_gameArea.GameAreaPosition, _gameArea.GameAreaRotation, width, depth);
        }
    }
}
