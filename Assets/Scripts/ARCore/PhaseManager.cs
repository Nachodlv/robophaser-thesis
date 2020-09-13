using System;
using System.Collections.Generic;
using ARCore.Phases;
using ARCore.Phases.Instantiating;
using GoogleARCore.Examples.CloudAnchors;
using Photon.Pun;
using UnityEngine;

namespace ARCore
{
    public class PhaseManager : MonoBehaviour
    {
        [SerializeField] private CloudAnchorsExampleController anchorsExampleController;
        [SerializeField] private NetworkUIController networkUi;

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
            var initialPhase = new InitialPhase(
                this,
                InstantiateMasterPhases(),
                InstantiateNonMasterPhases());
            ChangePhase(initialPhase);
        }

        private NonMasterPositioningPhase InstantiateNonMasterPhases()
        {
            var nonMasterInstantiatingPhase = new NonMasterInstantiatingPhase(this, networkUi);
            return new NonMasterPositioningPhase(this, networkUi, anchorsExampleController,
                nonMasterInstantiatingPhase);
        }

        private MasterPositioningPhase InstantiateMasterPhases()
        {
            var masterInstantiatingPhase = new MasterInstantiatingPhase(this, networkUi, anchorsExampleController);
            return new MasterPositioningPhase(this, networkUi, anchorsExampleController, masterInstantiatingPhase);
        }
    }
}
