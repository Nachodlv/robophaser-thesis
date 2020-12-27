using TMPro;
using UnityEngine;

namespace Utils
{
    public class TextAutoSizeController : MonoBehaviour
    {
        public TMP_Text[] textObjects;

        private void Awake()
        {
            RefreshAutoSize();
        }

        public void RefreshAutoSize()
        {
            if (textObjects == null || textObjects.Length == 0)
                return;

            // Iterate over each of the text objects in the array to find a good test candidate
            // There are different ways to figure out the best candidate
            // Preferred width works fine for single line text objects
            int candidateIndex = 0;
            float maxPreferredWidth = 0;

            for (int i = 0; i < textObjects.Length; i++)
            {
                float preferredWidth = textObjects[i].preferredWidth;
                if (preferredWidth > maxPreferredWidth)
                {
                    maxPreferredWidth = preferredWidth;
                    candidateIndex = i;
                }
            }

            // Force an update of the candidate text object so we can retrieve its optimum point size.
            textObjects[candidateIndex].enableAutoSizing = true;
            textObjects[candidateIndex].ForceMeshUpdate();
            float optimumPointSize = textObjects[candidateIndex].fontSize;

            // Disable auto size on our test candidate
            textObjects[candidateIndex].enableAutoSizing = false;

            // Iterate over all other text objects to set the point size
            for (int i = 0; i < textObjects.Length; i++)
                textObjects[i].fontSize = optimumPointSize;
        }
    }

}
