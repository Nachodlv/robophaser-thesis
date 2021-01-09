using UnityEngine;

namespace Cues.Animations
{
    public class RobotOrbAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private static readonly int TakeDamageTrigger = Animator.StringToHash("TakeDamage");

        public void TakeDamage()
        {
            animator.SetTrigger(TakeDamageTrigger);
        }
    }
}
