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
        public Text snackbarText;

        /// <summary>
        /// The number of matches that will be shown.
        /// </summary>
        private const int MatchPageSize = 5;

        /// <summary>
        /// The current room number.
        /// </summary>
        private string _currentRoomNumber;


        /// <summary>
        /// Callback indicating that the Cloud Anchor was instantiated and the host request was
        /// made.
        /// </summary>
        /// <param name="isHost">Indicates whether this player is the host.</param>
        public void OnAnchorInstantiated(bool isHost)
        {
            if (isHost)
            {
                snackbarText.text = "Hosting Cloud Anchor...";
            }
            else
            {
                snackbarText.text =
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
                snackbarText.text = "Cloud Anchor successfully hosted! Tap to place more stars.";
            }
            else
            {
                snackbarText.text = "Cloud Anchor could not be hosted. " + response;
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
                snackbarText.text = "Cloud Anchor successfully resolved! Tap to place more stars.";
            }
            else
            {
                snackbarText.text =
                    "Cloud Anchor could not be resolved. Will attempt again. " + response;
            }
        }

        /// <summary>
        /// Use the snackbar to display the error message.
        /// </summary>
        /// <param name="debugMessage">The debug message to be displayed on the snackbar.</param>
        public void ShowDebugMessage(string debugMessage)
        {
            snackbarText.text = debugMessage;
        }




    }
}
