using System;
using UnityEngine;

namespace Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance => _instance ? _instance : FindObjectOfType<T>();

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                return;
            }
            _instance = FindObjectOfType<T>();
        }
    }
}
