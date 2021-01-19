using UnityEngine;

namespace Photon.Countdown
{
    [CreateAssetMenu(fileName = "New Start Countdown Event", menuName = "Event/Start Countdown", order = 0)]
    public class StartCountdownEvent : ScriptableEvent<float>
    {
    }

    [CreateAssetMenu(fileName = "New Finish Countdown Event", menuName = "Event/Finish Countdown", order = 0)]
    public class FinishCountdownEvent : ScriptableEvent
    {
    }
}
