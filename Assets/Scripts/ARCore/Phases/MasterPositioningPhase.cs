using GoogleARCore.Examples.CloudAnchors;

namespace ARCore.Phases
{
    public class MasterPositioningPhase : IPhase
    {
        private readonly NetworkUIController _networkUiController;
        private readonly CloudAnchorsExampleController _cloudAnchorsController;

        public MasterPositioningPhase(PhaseManager phaseManager, NetworkUIController networkUiController,
            CloudAnchorsExampleController cloudAnchorController) : base(phaseManager)
        {
            _networkUiController = networkUiController;
            _cloudAnchorsController = cloudAnchorController;
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
            _networkUiController.ShowDebugMessage(success
                ? "Cloud Anchor successfully hosted! Tap to place more stars."
                : $"Cloud Anchor could not be hosted. {response}");
        }
    }
}
