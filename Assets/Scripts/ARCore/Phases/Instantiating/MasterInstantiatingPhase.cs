using System.IO;
using ARCore.Phases.Combat;
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
        private readonly ObstacleGenerator _obstacleGenerator;
        private readonly CombatPhase _combatPhase;
        private GameArea _gameArea;
        private bool _settingUpGameArea;
        private bool _skipGameArea;

        public MasterInstantiatingPhase(PhaseManager phaseManager, NetworkUIController networkUiController,
            CloudAnchorsExampleController cloudAnchors, ObstacleGenerator obstacleGenerator, CombatPhase combatPhase,
            bool skipGameArea) : base(phaseManager)
        {
            _networkUiController = networkUiController;
            _cloudAnchors = cloudAnchors;
            _obstacleGenerator = obstacleGenerator;
            _combatPhase = combatPhase;
            _obstacleGenerator.OnFinishPlacingObstacles += FinishPlacingObstacles;
            _skipGameArea = skipGameArea;
        }

        public override void OnEnter()
        {
            SetInitialMessage();
            if (_skipGameArea)
            {
                Touch(Vector3.zero, Quaternion.identity);
                _gameArea.ConfirmChanges();
            }
            else
            {
                _cloudAnchors.OnPlaneTouch += Touch;
            }
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
            _obstacleGenerator.CreateObstacles(_gameArea.GameAreaPosition, _gameArea.GameAreaRotation, width, depth);
        }

        private void FinishPlacingObstacles()
        {
            PhaseManager.ChangePhase(_combatPhase);
        }
    }
}
