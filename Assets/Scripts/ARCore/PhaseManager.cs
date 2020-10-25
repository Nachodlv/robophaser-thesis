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
        public ObstacleGenerator ObstacleGenerator { get; private set; }

        private void Awake()
        {
            if (PhotonNetwork.IsMasterClient)
                InstantiateObstacleGenerator();
        }

        private void Start()
        {
            InitializePhases();
            // ChangePhase(new CombatPhase(this, playerUI));
        }

        public void ChangePhase(Phase newPhase)
        {
            _currentState?.OnExit();
            _currentState = newPhase;
            _currentState.OnEnter();
        }

        private void InitializePhases()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                InstantiateObstacleGenerator();
                ChangePhase(InstantiateMasterPhases());
            }
            else
            {
                ChangePhase(InstantiateNonMasterPhases());
            }
        }

        private void InstantiateObstacleGenerator()
        {
            ObstacleGenerator = PhotonNetwork
                .Instantiate(Path.Combine("WFC Prefabs", "Obstacle Generator"), Vector3.zero, Quaternion.identity)
                .GetComponent<ObstacleGenerator>();
        }

        private NonMasterPositioningPhase InstantiateNonMasterPhases()
        {
            var combatPhase = new CombatPhase(this, playerUI);
            var nonMasterInstantiatingPhase =
                new NonMasterInstantiatingPhase(this, networkUi, combatPhase);
            return new NonMasterPositioningPhase(this, networkUi, anchorsExampleController,
                nonMasterInstantiatingPhase);
        }

        private MasterPositioningPhase InstantiateMasterPhases()
        {
            var combatPhase = new CombatPhase(this, playerUI);
            var masterInstantiatingPhase =
                new MasterInstantiatingPhase(this, networkUi, anchorsExampleController, ObstacleGenerator,
                    combatPhase);
            return new MasterPositioningPhase(this, networkUi, anchorsExampleController, masterInstantiatingPhase);
        }
    }
}
