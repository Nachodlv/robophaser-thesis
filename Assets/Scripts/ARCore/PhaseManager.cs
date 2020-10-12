using System;
using System.Collections.Generic;
using System.IO;
using ARCore.Phases;
using ARCore.Phases.Combat;
using ARCore.Phases.Instantiating;
using GoogleARCore.Examples.CloudAnchors;
using Photon.Pun;
using UI.Combat;
using UnityEngine;
using WFC;

namespace ARCore
{
    public class PhaseManager : MonoBehaviour
    {
        [SerializeField] private CloudAnchorsExampleController anchorsExampleController;
        [SerializeField] private NetworkUIController networkUi;
        [SerializeField] private PlayerUI playerUI;

        private Phase _currentState;
        private ObstacleGenerator _obstacleGenerator;

        private void Awake()
        {
            if (PhotonNetwork.IsMasterClient) InstantiateObstacleGenerator();
            InitializePhases();
        }

        public void ChangePhase(Phase newPhase)
        {
            _currentState?.OnExit();
            _currentState = newPhase;
            _currentState.OnEnter();
        }

        private void InitializePhases()
        {
            var initialPhase = new InitialPhase(
                this,
                InstantiateMasterPhases(),
                InstantiateNonMasterPhases());
            ChangePhase(initialPhase);
        }

        private void InstantiateObstacleGenerator()
        {
            _obstacleGenerator = PhotonNetwork
                .Instantiate(Path.Combine("WFC Prefabs", "Obstacle Generator"), Vector3.zero, Quaternion.identity)
                .GetComponent<ObstacleGenerator>();
        }

        private NonMasterPositioningPhase InstantiateNonMasterPhases()
        {
            var combatPhase = new CombatPhase(this, playerUI);
            var nonMasterInstantiatingPhase =
                new NonMasterInstantiatingPhase(this, networkUi, combatPhase, _obstacleGenerator);
            return new NonMasterPositioningPhase(this, networkUi, anchorsExampleController,
                nonMasterInstantiatingPhase);
        }

        private MasterPositioningPhase InstantiateMasterPhases()
        {
            var combatPhase = new CombatPhase(this, playerUI);
            var masterInstantiatingPhase =
                new MasterInstantiatingPhase(this, networkUi, anchorsExampleController, _obstacleGenerator,
                    combatPhase);
            return new MasterPositioningPhase(this, networkUi, anchorsExampleController, masterInstantiatingPhase);
        }
    }
}
