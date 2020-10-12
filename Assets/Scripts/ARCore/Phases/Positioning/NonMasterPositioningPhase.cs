using GoogleARCore.Examples.CloudAnchors;

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
            if (!success)
            {
                _networkUiController.ShowDebugMessage(
                    $"Cloud Anchor could not be resolved. Will attempt again. {response}");
            }
            else
            {
                _phaseManager.ChangePhase(_instantiatingPhase);
            }
        }
    }
}
