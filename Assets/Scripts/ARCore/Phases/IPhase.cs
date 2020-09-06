namespace ARCore.Phases
{
    public abstract class IPhase
    {
        private readonly PhaseManager _phaseManager;

        protected IPhase(PhaseManager phaseManager)
        {
            _phaseManager = phaseManager;
        }

        public PhaseManager PhaseManager => _phaseManager;

        public abstract void OnEnter();
        public virtual void OnExit(){}
    }
}
