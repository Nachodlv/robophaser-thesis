using UnityEngine;

namespace Cues
{
    public abstract class Cue : ScriptableObject
    {
        public abstract void Execute(Vector3 position, Quaternion rotation);
    }
}
