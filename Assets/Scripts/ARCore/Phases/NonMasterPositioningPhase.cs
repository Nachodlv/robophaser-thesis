using GoogleARCore.Examples.CloudAnchors;

namespace ARCore.Phases
{
    public class NonMasterPositioningPhase : IPhase
    {
        private readonly NetworkUIController _networkUiController;
        private readonly CloudAnchorsExampleController _cloudAnchorsController;

        public NonMasterPositioningPhase(PhaseManager phaseManager, NetworkUIController networkUiController,
            CloudAnchorsExampleController cloudAnchorsController) : base(
            phaseManager)
        {
            _networkUiController = networkUiController;
            _cloudAnchorsController = cloudAnchorsController;
        }

        public override void OnEnter()
        {
            _networkUiController.ShowDebugMessage(
                "Look at the same scene as the hosting phone.");
            _cloudAnchorsController.OnAnchorStartInstantiating += StartInstantiating;
            _cloudAnchorsController.OnAnchorFinishResolving += FinishResolving;
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
            _networkUiController.ShowDebugMessage(success
                ? "Cloud Anchor successfully resolved! Tap to place more stars."
                : $"Cloud Anchor could not be resolved. Will attempt again. {response}");
        }
    }
}
