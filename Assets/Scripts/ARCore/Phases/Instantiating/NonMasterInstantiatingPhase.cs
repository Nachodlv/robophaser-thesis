using ARCore.Phases.Combat;
using GoogleARCore.Examples.CloudAnchors;
using WFC;

namespace ARCore.Phases
{
    public class NonMasterInstantiatingPhase : Phase
    {
        private readonly NetworkUIController _networkUiController;
        private readonly CombatPhase _combatPhase;

        public NonMasterInstantiatingPhase(PhaseManager phaseManager, NetworkUIController networkUiController,
            CombatPhase combatPhase, ObstacleGenerator obstacleGenerator) : base(phaseManager)
        {
            _networkUiController = networkUiController;
            _combatPhase = combatPhase;
            obstacleGenerator.OnFinishPlacingObstacles += FinishPlacingObstacle;
        }

        public override void OnEnter()
        {
            _networkUiController.ShowDebugMessage("Waiting for the host to select an area for the game");
        }

        private void FinishPlacingObstacle()
        {
            PhaseManager.ChangePhase(_combatPhase);
        }
    }
}
