using GoogleARCore.Examples.CloudAnchors;

namespace ARCore.Phases
{
    public class MasterPositioningPhase : Phase
    {
        private readonly NetworkUIController _networkUiController;
        private readonly CloudAnchorsExampleController _cloudAnchorsController;
        private readonly MasterInstantiatingPhase _instantiatingPhase;

        public MasterPositioningPhase(PhaseManager phaseManager, NetworkUIController networkUiController,
            CloudAnchorsExampleController cloudAnchorsController, MasterInstantiatingPhase instantiatingPhase) : base(
            phaseManager)
        {
            _networkUiController = networkUiController;
            _cloudAnchorsController = cloudAnchorsController;
            _instantiatingPhase = instantiatingPhase;
        }

        public override void OnEnter()
        {
            _cloudAnchorsController.OnAnchorFinishHosting += AnchorsHosted;
            _cloudAnchorsController.OnAnchorStartInstantiating += StartInstantiating;
            _networkUiController.ShowDebugMessage("Find a plane, tap to create a Cloud Anchor.");
        }

        public override void OnExit()
        {
            _cloudAnchorsController.OnAnchorFinishHosting -= AnchorsHosted;
        }

        private void StartInstantiating()
        {
            _networkUiController.ShowDebugMessage("Hosting Cloud Anchor...");
        }

        private void AnchorsHosted(bool success, string response)
        {
            _cloudAnchorsController.OnAnchorStartInstantiating -= StartInstantiating;
#if UNITY_EDITOR
            PhaseManager.ChangePhase(_instantiatingPhase);
#else
            if(!success) _networkUiController.ShowDebugMessage($"Cloud Anchor could not be hosted. {response}");
            else PhaseManager.ChangePhase(_instantiatingPhase);
#endif
        }
    }
}
