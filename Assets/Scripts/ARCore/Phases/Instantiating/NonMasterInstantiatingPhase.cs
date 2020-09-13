using GoogleARCore.Examples.CloudAnchors;

namespace ARCore.Phases
{
    public class NonMasterInstantiatingPhase : Phase
    {
        private readonly NetworkUIController _networkUiController;

        public NonMasterInstantiatingPhase(PhaseManager phaseManager, NetworkUIController networkUiController) : base(
            phaseManager)
        {
            _networkUiController = networkUiController;
        }

        public override void OnEnter()
        {
            _networkUiController.ShowDebugMessage("Waiting for the host to select an area for the game");
        }
    }
}
