using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public class WaitSeconds
    {
        private readonly WaitForSeconds _waitTime;
        private readonly Action _callback;
        private readonly Func<WaitForSeconds, Action, IEnumerator> _waitFunction;
        private readonly MonoBehaviour _monoBehaviour;
        private Coroutine _coroutine;

        public WaitSeconds(MonoBehaviour monoBehaviour, Action callback, float time = 0)
        {
            _callback = callback;
            _waitTime = new WaitForSeconds(time);
            _monoBehaviour = monoBehaviour;
            _waitFunction = WaitForSeconds;
        }

        public void Wait(float time, Action callback)
        {
            Wait(new WaitForSeconds(time), callback);
        }

        public void Wait(float time)
        {
            Wait(new WaitForSeconds(time), _callback);
        }

        public void Wait()
        {
            Wait(_waitTime, _callback);
        }

        public void Stop()
        {
            if(_coroutine != null) _monoBehaviour.StopCoroutine(_coroutine);
        }

        private void Wait(WaitForSeconds waitingTime, Action callback)
        {
            _coroutine = _monoBehaviour.StartCoroutine(_waitFunction(waitingTime, callback));
        }

        private IEnumerator WaitForSeconds(WaitForSeconds waitingTime, Action callback)
        {
            yield return waitingTime;
            callback();
        }
    }
}
