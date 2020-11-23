using UI.Combat;
using UnityEngine;

namespace ARCore.Phases.Combat
{
    public class CombatPhase: Phase
    {
        private readonly PlayerUI _playerUI;

        public CombatPhase(PhaseManager phaseManager, PlayerUI playerUI) : base(phaseManager)
        {
            _playerUI = playerUI;
        }

        public override void OnEnter()
        {
            _playerUI.StartCombatPhase();
            Debug.Log("In Combat!");
        }
    }
}
