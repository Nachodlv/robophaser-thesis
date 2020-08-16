//-----------------------------------------------------------------------
// <copyright file="NetworkManagerUIController.cs" company="Google LLC">
//
// Copyright 2018 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.CloudAnchors
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.Types;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary>
    /// Controller managing UI for joining and creating rooms.
    /// </summary>
    public class NetworkManagerUIController : MonoBehaviour
    {
        /// <summary>
        /// The snackbar text.
        /// </summary>
        public Text SnackbarText;

        /// <summary>
        /// The Label showing the current active room.
        /// </summary>
        public GameObject CurrentRoomLabel;

        /// <summary>
        /// The return to lobby button in AR Scene.
        /// </summary>
        public GameObject ReturnButton;

        /// <summary>
        /// The Cloud Anchors Example Controller.
        /// </summary>
        public CloudAnchorsExampleController CloudAnchorsExampleController;

        /// <summary>
        /// The Panel containing the list of available rooms to join.
        /// </summary>
        public GameObject RoomListPanel;

        /// <summary>
        /// Text indicating that no previous rooms exist.
        /// </summary>
        public Text NoPreviousRoomsText;

        /// <summary>
        /// The prefab for a row in the available rooms list.
        /// </summary>
        public GameObject JoinRoomListRowPrefab;

        /// <summary>
        /// The number of matches that will be shown.
        /// </summary>
        private const int k_MatchPageSize = 5;

        /// <summary>
        /// The current room number.
        /// </summary>
        private string m_CurrentRoomNumber;

        /// <summary>
        /// The Join Room buttons.
        /// </summary>
        private List<GameObject> m_JoinRoomButtonsPool = new List<GameObject>();

        /// <summary>
        /// The Unity Awake() method.
        /// </summary>
        public void Awake()
        {
            // Initialize the pool of Join Room buttons.
            for (int i = 0; i < k_MatchPageSize; i++)
            {
                GameObject button = Instantiate(JoinRoomListRowPrefab);
                button.transform.SetParent(RoomListPanel.transform, false);
                button.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0, -(100 * i));
                button.SetActive(true);
                button.GetComponentInChildren<Text>().text = string.Empty;
                m_JoinRoomButtonsPool.Add(button);
            }

        }


        /// <summary>
        /// Callback indicating that the Cloud Anchor was instantiated and the host request was
        /// made.
        /// </summary>
        /// <param name="isHost">Indicates whether this player is the host.</param>
        public void OnAnchorInstantiated(bool isHost)
        {
            if (isHost)
            {
                SnackbarText.text = "Hosting Cloud Anchor...";
            }
            else
            {
                SnackbarText.text =
                    "Cloud Anchor added to session! Attempting to resolve anchor...";
            }
        }

        /// <summary>
        /// Callback indicating that the Cloud Anchor was hosted.
        /// </summary>
        /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was hosted
        /// successfully.</param>
        /// <param name="response">The response string received.</param>
        public void OnAnchorHosted(bool success, string response)
        {
            if (success)
            {
                SnackbarText.text = "Cloud Anchor successfully hosted! Tap to place more stars.";
            }
            else
            {
                SnackbarText.text = "Cloud Anchor could not be hosted. " + response;
            }
        }

        /// <summary>
        /// Callback indicating that the Cloud Anchor was resolved.
        /// </summary>
        /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was resolved
        /// successfully.</param>
        /// <param name="response">The response string received.</param>
        public void OnAnchorResolved(bool success, string response)
        {
            if (success)
            {
                SnackbarText.text = "Cloud Anchor successfully resolved! Tap to place more stars.";
            }
            else
            {
                SnackbarText.text =
                    "Cloud Anchor could not be resolved. Will attempt again. " + response;
            }
        }

        /// <summary>
        /// Use the snackbar to display the error message.
        /// </summary>
        /// <param name="debugMessage">The debug message to be displayed on the snackbar.</param>
        public void ShowDebugMessage(string debugMessage)
        {
            SnackbarText.text = debugMessage;
        }




    }
}
