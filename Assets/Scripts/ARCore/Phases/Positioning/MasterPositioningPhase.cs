using System.Collections;
using ARCore.Phases.Instantiating;
using GoogleARCore.Examples.CloudAnchors;
using Photon;
using UnityEngine;

namespace ARCore.Phases
{
    public class MasterPositioningPhase : Phase
    {
        private readonly NetworkUIController _networkUiController;
        private readonly CloudAnchorsExampleController _cloudAnchorsController;
        private readonly MasterInstantiatingPhase _instantiatingPhase;
        private readonly bool _skipAR;

        public MasterPositioningPhase(PhaseManager phaseManager, NetworkUIController networkUiController,
            CloudAnchorsExampleController cloudAnchorsController, MasterInstantiatingPhase instantiatingPhase, bool skipAR) : base(
            phaseManager)
        {
            _networkUiController = networkUiController;
            _cloudAnchorsController = cloudAnchorsController;
            _instantiatingPhase = instantiatingPhase;
            _skipAR = skipAR;
        }

        public override void OnEnter()
        {

            _cloudAnchorsController.OnAnchorFinishHosting += AnchorsHosted;
            _cloudAnchorsController.OnAnchorStartInstantiating += StartInstantiating;
            PhotonRoom.Instance.OnAllPlayersReady += AllPlayersReady;
            _networkUiController.ShowDebugMessage("Find a plane, tap to create a Cloud Anchor.");

            if (_skipAR)
            {
                PhaseManager.StartCoroutine(WaitInitializationsAndHostAnchor());
            }
        }

        public override void OnExit()
        {
            _cloudAnchorsController.OnAnchorFinishHosting -= AnchorsHosted;
            _cloudAnchorsController.OnAnchorStartInstantiating -= StartInstantiating;
        }

        private void StartInstantiating()
        {
            _networkUiController.ShowDebugMessage("Hosting Cloud Anchor...");
        }

        private void AnchorsHosted(bool success, string response)
        {
#if !UNITY_EDITOR
            if (!success)
            {
                _networkUiController.ShowDebugMessage($"Cloud Anchor could not be hosted. {response}");
                return;
            }
#endif
            PhotonRoom.Instance.PlayerReady(PhotonRoom.Instance.LocalPlayer);
        }

        private void AllPlayersReady()
        {
            PhotonRoom.Instance.OnAllPlayersReady -= AllPlayersReady;
            PhaseManager.ChangePhase(_instantiatingPhase);
        }

        // TODO local player may not be awake. This is the easiest solution. It might need to be upgraded later.
        private IEnumerator WaitInitializationsAndHostAnchor()
        {
            yield return new WaitUntil(() => PhotonRoom.Instance.LocalPlayer != null);
            _cloudAnchorsController.SetWorldOriginWithoutHosting(PhaseManager.transform);
        }
    }
}
