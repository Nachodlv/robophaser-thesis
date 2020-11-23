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
            CombatPhase combatPhase) : base(phaseManager)
        {
            _networkUiController = networkUiController;
            _combatPhase = combatPhase;
        }

        public override void OnEnter()
        {
            PhaseManager.ObstacleGenerator.OnFinishPlacingObstacles += FinishPlacingObstacle;
            _networkUiController.ShowDebugMessage("Waiting for the host to select an area for the game");
        }

        public override void OnExit()
        {
            PhaseManager.ObstacleGenerator.OnFinishPlacingObstacles -= FinishPlacingObstacle;
        }

        private void FinishPlacingObstacle()
        {
            PhaseManager.ChangePhase(_combatPhase);
        }
    }
}
