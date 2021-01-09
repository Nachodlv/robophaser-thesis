using System.Collections;
using GoogleARCore.Examples.CloudAnchors;
using Photon;
using UnityEngine;

namespace ARCore.Phases
{
    public class NonMasterPositioningPhase : Phase
    {
        private readonly NetworkUIController _networkUiController;
        private readonly CloudAnchorsExampleController _cloudAnchorsController;
        private readonly NonMasterInstantiatingPhase _instantiatingPhase;
        private readonly PhaseManager _phaseManager;

        public NonMasterPositioningPhase(PhaseManager phaseManager, NetworkUIController networkUiController,
            CloudAnchorsExampleController cloudAnchorsController, NonMasterInstantiatingPhase instantiatingPhase) :
            base(phaseManager)
        {
            _networkUiController = networkUiController;
            _cloudAnchorsController = cloudAnchorsController;
            _instantiatingPhase = instantiatingPhase;
            _phaseManager = phaseManager;
        }

        public override void OnEnter()
        {
            _networkUiController.ShowDebugMessage(
                "Look at the same scene as the hosting phone.");
            _cloudAnchorsController.OnAnchorStartInstantiating += StartInstantiating;
            _cloudAnchorsController.OnAnchorFinishResolving += FinishResolving;
            PhotonRoom.Instance.OnAllPlayersReady += AllPlayersReady;
        }

        public override void OnExit()
        {
            _cloudAnchorsController.OnAnchorStartInstantiating -= StartInstantiating;
            _cloudAnchorsController.OnAnchorFinishResolving -= FinishResolving;
        }

        private void StartInstantiating()
        {
            _networkUiController.ShowDebugMessage("Cloud Anchor added to session! Attempting to resolve anchor...");
        }

        private void FinishResolving(bool success, string response)
        {
#if !UNITY_EDITOR

            if (!success)
            {
                _networkUiController.ShowDebugMessage(
                    $"Cloud Anchor could not be resolved. Will attempt again. {response}");
                return;
            }
#endif
            PhaseManager.StartCoroutine(WaitForLocalPlayer());
        }

        private void AllPlayersReady()
        {
            PhotonRoom.Instance.OnAllPlayersReady -= AllPlayersReady;
            _phaseManager.ChangePhase(_instantiatingPhase);
        }

        // TODO, this should be fixed
        private IEnumerator WaitForLocalPlayer()
        {
            yield return new WaitUntil(() => PhotonRoom.Instance.LocalPlayer != null);
            PhotonRoom.Instance.PlayerReady(PhotonRoom.Instance.LocalPlayer);
        }
    }
}
