using UnityEngine;

namespace Cues
{
    [CreateAssetMenu(fileName = "Cue", menuName = "Cue/Effect", order = 0)]
    public class Effect : Cue
    {
        [SerializeField] private Cue[] cues;

        public override void Execute(Vector3 position, Quaternion rotation)
        {
            foreach (var cue in cues)
            {
                cue.Execute(position, rotation);
            }
        }

        public override void StopExecution()
        {
            foreach (var cue in cues)
            {
                cue.StopExecution();
            }
        }
    }
}
