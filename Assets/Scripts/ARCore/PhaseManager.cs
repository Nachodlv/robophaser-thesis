using System;
using System.Collections.Generic;
using ARCore.Phases;
using GoogleARCore.Examples.CloudAnchors;
using Photon.Pun;
using UnityEngine;

namespace ARCore
{
    public class PhaseManager : MonoBehaviour
    {
        [SerializeField] private CloudAnchorsExampleController anchorsExampleController;
        [SerializeField] private NetworkUIController networkUi;
        [SerializeField] private GameArea gameArea;

        private Phase _currentState;

        private void Awake()
        {
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
            var masterInstantiatingPhase = new MasterInstantiatingPhase(this, networkUi, anchorsExampleController, gameArea);
            var masterPositioningPhase =
                new MasterPositioningPhase(this, networkUi, anchorsExampleController, masterInstantiatingPhase);
            var nonMasterPositioningPhase = new NonMasterPositioningPhase(this, networkUi, anchorsExampleController);
            var initialPhase = new InitialPhase(this, masterPositioningPhase, nonMasterPositioningPhase);

            ChangePhase(initialPhase);
        }
    }
}
