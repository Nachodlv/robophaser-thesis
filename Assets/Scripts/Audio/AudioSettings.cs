using System;

namespace Audio
{
    [Serializable]
    public class AudioSettings
    {
        public AudioType audioType;
        public float volume = 1;
        public bool replicated;
        public bool spatial;
        public bool loop;
        public LoopSettings loopSettings;
    }

    [Serializable]
    public class LoopSettings
    {
        public float timeBetweenLoops;
    }
}
