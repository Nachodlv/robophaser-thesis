using System;
using Cues;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ExecuteCueWhenClicked : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Cue cue;

        private void Awake()
        {
            button.onClick.AddListener(ExecuteCue);
        }

        private void ExecuteCue()
        {
            cue.Execute(transform.position, Quaternion.identity);
        }
    }
}
