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

        private IPhase _currentState;

        private void Awake()
        {
            var masterPositioningPhase = new MasterPositioningPhase(this, networkUi, anchorsExampleController);
            var nonMasterPositioningPhase = new NonMasterPositioningPhase(this, networkUi, anchorsExampleController);
            var initialPhase = new InitialPhase(this, masterPositioningPhase, nonMasterPositioningPhase);

            ChangePhase(initialPhase);
        }

        public void ChangePhase(IPhase newPhase)
        {
            _currentState?.OnExit();
            _currentState = newPhase;
            _currentState.OnEnter();
        }


    }
}
