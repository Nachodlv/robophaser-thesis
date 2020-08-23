using System;
using UnityEngine;

namespace Photon
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private GameObject[] allCharacters;

        public static PlayerInfo Instance;

        private int _characterType;
        private const string CharacterPlayerPrefsKey = "Character";

        public int CharacterType
        {
            get => _characterType;
            set { _characterType = value; PlayerPrefs.SetInt(CharacterPlayerPrefsKey, value);}
        }

        public GameObject[] AllCharacters => allCharacters;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
            DontDestroyOnLoad(this);
            if (PlayerPrefs.HasKey(CharacterPlayerPrefsKey))
            {
                _characterType = PlayerPrefs.GetInt(CharacterPlayerPrefsKey);
            }
            else
            {
                CharacterType = 0;
            }
        }
    }
}
