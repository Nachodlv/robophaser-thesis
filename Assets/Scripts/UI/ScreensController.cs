using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace UI
{
    public enum Screen
    {
        InitialScreen,
        WaitingScreen,
        GameScreen
    }

    [Serializable]
    public class ScreenGameObject: IDictionaryEntity<Screen, GameObject>
    {
        public GameObject gameObject;
        public Screen screen;
        public Screen TValue => screen;
        public GameObject TsValue => gameObject;
    }
    public class ScreensController: MonoBehaviour
    {
        [SerializeField] private ScreenGameObject[] screenGameObjects;
        [SerializeField] private Screen startingScreen;

        private Dictionary<Screen, GameObject> _screens;
        private GameObject _currentScreen;

        private void Awake()
        {
            _screens = ArrayToDictionary.ToDictionary<Screen, GameObject, ScreenGameObject>(screenGameObjects);
            HideScreens();
            _currentScreen = _screens[startingScreen];
            _currentScreen.SetActive(true);
        }

        public void ShowScreen(Screen screen)
        {
            _currentScreen.SetActive(false);
            if(!_screens.ContainsKey(screen)) return;
            _currentScreen = _screens[screen].gameObject;
            _currentScreen.SetActive(true);
        }

        private void HideScreens()
        {
            foreach (var screenGameObject in screenGameObjects)
            {
                screenGameObject.gameObject.SetActive(false);
            }
        }
    }
}
