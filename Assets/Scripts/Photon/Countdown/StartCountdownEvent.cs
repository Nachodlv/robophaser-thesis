using UnityEngine;

namespace Photon.Countdown
{
    [CreateAssetMenu(fileName = "New Start Countdown Event", menuName = "Event/Start Countdown", order = 0)]
    public class StartCountdownEvent : ScriptableEvent<float>
    {
    }
}
