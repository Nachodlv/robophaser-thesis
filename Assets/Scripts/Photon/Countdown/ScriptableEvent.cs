using System;
using UnityEngine;

namespace Photon.Countdown
{
    public abstract class ScriptableEvent<T> : ScriptableObject
    {
        public event Action<T> OnTriggerEvent;

        public void TriggerEvent(T t) => OnTriggerEvent?.Invoke(t);
    }

    public abstract class ScriptableEvent : ScriptableObject
    {
        public event Action OnTriggerEvent;

        public void TriggerEvent() => OnTriggerEvent?.Invoke();
    }
}
