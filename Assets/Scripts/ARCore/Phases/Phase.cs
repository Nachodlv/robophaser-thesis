namespace ARCore.Phases
{
    public abstract class Phase
    {
        private readonly PhaseManager _phaseManager;

        protected Phase(PhaseManager phaseManager)
        {
            _phaseManager = phaseManager;
        }

        public PhaseManager PhaseManager => _phaseManager;

        public abstract void OnEnter();
        public virtual void OnExit(){}
    }
}
