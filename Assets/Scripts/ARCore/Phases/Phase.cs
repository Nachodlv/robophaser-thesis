using UI;

namespace ARCore.Phases
{
    public abstract class Phase
    {
        protected Phase(PhaseManager phaseManager)
        {
            PhaseManager = phaseManager;
        }

        protected PhaseManager PhaseManager { get; }

        public abstract void OnEnter();
        public virtual void OnExit(){}

        public virtual void OpponentLeft()
        {
            PhaseManager.EndGameScreen.ShowVictory(EndGameReason.PlayerDisconnect);
            PhaseManager.EndGameScreen.DisableRematch();
        }
    }
}
