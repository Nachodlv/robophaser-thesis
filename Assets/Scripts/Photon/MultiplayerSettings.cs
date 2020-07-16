using UnityEngine;

namespace Photon
{
    [CreateAssetMenu(fileName = "Multiplayer Settings", menuName = "Multiplayer/Settings", order = 0)]
    public class MultiplayerSettings : ScriptableObject
    {
        public int maxPlayers;
        public int menuScene;
        [Tooltip("Scene index that will be loaded when the game starts")] public int multiplayerScene;
    }
}
