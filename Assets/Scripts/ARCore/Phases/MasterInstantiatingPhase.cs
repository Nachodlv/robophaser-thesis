using GoogleARCore.Examples.CloudAnchors;
using UnityEngine;

namespace ARCore.Phases
{
    public class MasterInstantiatingPhase : Phase
    {
        private readonly NetworkUIController _networkUiController;
        private CloudAnchorsExampleController _cloudAnchors;

        public MasterInstantiatingPhase(PhaseManager phaseManager, NetworkUIController networkUiController,
            CloudAnchorsExampleController cloudAnchors) : base(phaseManager)
        {
            _networkUiController = networkUiController;
            _cloudAnchors = cloudAnchors;
        }

        public override void OnEnter()
        {
            _networkUiController.ShowDebugMessage("Tap a plane to place the area game");
            _cloudAnchors.OnPlaneTouch += Touch;
        }

        private void Touch(Vector3 position, Quaternion rotation)
        {
            _networkUiController.ShowDebugMessage($"Touch at: {position.ToString()} with rotation: {rotation.ToString()}");
        }
    }
}
