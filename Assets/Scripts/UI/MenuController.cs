using Photon;
using UnityEngine;

namespace UI
{
    public class MenuController : MonoBehaviour
    {
        public void OnClickCharacterPick(int characterType)
        {
            if (PlayerInfo.Instance != null)
            {
                PlayerInfo.Instance.CharacterType = characterType;
            }
        }
    }
}
