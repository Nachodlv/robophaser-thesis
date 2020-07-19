using UnityEngine;

namespace Photon
{
    [CreateAssetMenu(fileName = "Multiplayer Settings", menuName = "Multiplayer/Settings", order = 0)]
    public class MultiplayerSettings : ScriptableObject
    {
        [Header("Multiplayer parameters")]
        public int maxPlayers;

        [Header("Scenes index")]
        [Tooltip("Scene index that will be loaded when the game starts")] public int multiplayerScene;
        [Tooltip("Scene index of the main menu")]
        public int mainMenuScene;
    }
}
