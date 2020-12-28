//-----------------------------------------------------------------------
// <copyright file="CloudAnchorsExampleController.cs" company="Google LLC">
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

using System;
using Photon;
using Photon.Pun;
using Utils;

namespace GoogleARCore.Examples.CloudAnchors
{
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = GoogleARCore.InstantPreviewInput;
#endif

    /// <summary>
    /// Controller for the Cloud Anchors Example. Handles the ARCore lifecycle.
    /// See details in
    /// <a href="https://developers.google.com/ar/develop/unity/cloud-anchors/overview-unity">
    /// Share AR experiences with Cloud Anchors</a>
    /// </summary>
    public class CloudAnchorsExampleController : MonoBehaviour
    {
        [Header("ARCore")]

        /// <summary>
        /// The root for ARCore-specific GameObjects in the scene.
        /// </summary>
        public GameObject ARCoreRoot;

        /// <summary>
        /// The helper that will calculate the World Origin offset when performing a raycast or
        /// generating planes.
        /// </summary>
        public ARCoreWorldOriginHelper ARCoreWorldOriginHelper;

        [Header("ARKit")]

        /// <summary>
        /// The root for ARKit-specific GameObjects in the scene.
        /// </summary>
        public GameObject ARKitRoot;

        /// <summary>
        /// The first-person camera used to render the AR background texture for ARKit.
        /// </summary>
        public Camera ARKitFirstPersonCamera;

        public MultiplayerSettings settings;

        [Header("UI")]

        /// <summary>
        /// The network manager UI Controller.
        /// </summary>
        public NetworkUIController NetworkUIController;

        /// <summary>
        /// The Start Screen to see help information.
        /// </summary>
        public GameObject StartScreen;

        /// <summary>
        /// The AR Screen which display the AR view, return to lobby button and room number.
        /// </summary>
        public GameObject ARScreen;

        /// <summary>
        /// The Status Screen to display the connection status and cloud anchor instructions.
        /// </summary>
        public GameObject StatusScreen;

        public delegate void CloudAnchorCallback(bool success, string response);
        public delegate void TouchCallback(Vector3 position, Quaternion rotation);

        public event Action OnAnchorStartInstantiating;
        public event CloudAnchorCallback OnAnchorFinishHosting;
        public event CloudAnchorCallback OnAnchorFinishResolving;
        public event TouchCallback OnPlaneTouch;

        /// <summary>
        /// The key name used in PlayerPrefs which indicates whether
        /// the start info has displayed at least one time.
        /// </summary>
        private const string HasDisplayedStartInfoKey = "HasDisplayedStartInfo";

        /// <summary>
        /// The time between room starts up and ARCore session starts resolving.
        /// </summary>
        private const float ResolvingPrepareTime = 3.0f;

        /// <summary>
        /// Record the time since the room started. If it passed the resolving prepare time,
        /// applications in resolving mode start resolving the anchor.
        /// </summary>
        private float _timeSinceStart = 0.0f;

        /// <summary>
        /// Indicates whether passes the resolving prepare time.
        /// </summary>
        private bool _passedResolvingPreparedTime = false;

        /// <summary>
        /// A helper object to ARKit functionality.
        /// </summary>
        private readonly ARKitHelper _aRKit = new ARKitHelper();

        /// <summary>
        /// Indicates whether the Anchor was already instantiated.
        /// </summary>
        private bool _anchorAlreadyInstantiated = false;

        /// <summary>
        /// Indicates whether the Cloud Anchor finished hosting.
        /// </summary>
        private bool _anchorFinishedHosting = false;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error,
        /// otherwise false.
        /// </summary>
        private bool _isQuitting = false;

        /// <summary>
        /// The anchor component that defines the shared world origin.
        /// </summary>
        private Component _worldOriginAnchor = null;

        /// <summary>
        /// The last pose of the hit point from AR hit test.
        /// </summary>
        private Pose? _lastHitPose = null;

        /// <summary>
        /// The current cloud anchor mode.
        /// </summary>
        private ApplicationMode _currentMode = ApplicationMode.Ready;

        /// <summary>
        /// The current active UI screen.
        /// </summary>
        private ActiveScreen _currentActiveScreen = ActiveScreen.StartScreen;

        /// <summary>
        /// The Network Manager.
        /// </summary>
#pragma warning disable 618
        private PhotonRoom _networkManager;
#pragma warning restore 618

        /// <summary>
        /// Enumerates modes the example application can be in.
        /// </summary>
        private enum ApplicationMode
        {
            /// <summary>
            /// Enume mode that indicate the example application is ready to host or resolve.
            /// </summary>
            Ready,

            /// <summary>
            /// Enume mode that indicate the example application is hosting cloud anchors.
            /// </summary>
            Hosting,

            /// <summary>
            /// Enume mode that indicate the example application is resolving cloud anchors.
            /// </summary>
            Resolving,
        }

        /// <summary>
        /// Enumerates the active UI screens the example application can be in.
        /// </summary>
        private enum ActiveScreen
        {
            /// <summary>
            /// Enume mode that indicate the example application is on start screen.
            /// </summary>
            StartScreen,

            /// <summary>
            /// Enume mode that indicate the example application is on AR screen.
            /// </summary>
            ARScreen,
        }

        /// <summary>
        /// Gets a value indicating whether the Origin of the new World Coordinate System,
        /// i.e. the Cloud Anchor was placed.
        /// </summary>
        public bool IsOriginPlaced { get; private set; }

        /// <summary>
        /// Callback handling Start Now button click event.
        /// </summary>
        public void OnStartNowButtonClicked()
        {
            _SwitchActiveScreen(ActiveScreen.ARScreen);
            PhotonRoom.Instance.InstancePlayer();
        }

        public void OnNotNowButtonClicked()
        {
            _DoReturnToLobby();
        }

        /// <summary>
        /// Callback handling Learn More Button click event.
        /// </summary>
        public void OnLearnMoreButtonClicked()
        {
            Application.OpenURL(
                "https://developers.google.com/ar/cloud-anchors-privacy");
        }

        /// <summary>
        /// The Unity Awake() method.
        /// </summary>
        public void Awake()
        {
            // Enable ARCore to target 60fps camera capture frame rate on supported devices.
            // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
            Application.targetFrameRate = 60;
        }

        /// <summary>
        /// The Unity Start() method.
        /// </summary>
        public void Start()
        {
            _ResetStatus();
            _networkManager = FindObjectOfType<PhotonRoom>();

            if (PhotonNetwork.IsMasterClient) OnEnterHostingModeClick();
            else OnEnterResolvingModeClick();

            gameObject.name = "CloudAnchorsExampleController";
            ARCoreRoot.SetActive(false);
            ARKitRoot.SetActive(false);
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _UpdateApplicationLifecycle();

            if (_currentActiveScreen != ActiveScreen.ARScreen)
            {
                return;
            }

            // If we are neither in hosting nor resolving mode then the update is complete.
            if (_currentMode != ApplicationMode.Hosting &&
                _currentMode != ApplicationMode.Resolving)
            {
                return;
            }

            // Give ARCore session some time to prepare for resolving and update the UI message
            // once the preparation time passed.
            if (_currentMode == ApplicationMode.Resolving && !_passedResolvingPreparedTime)
            {
                _timeSinceStart += Time.deltaTime;

                if (_timeSinceStart > ResolvingPrepareTime)
                {
                    _passedResolvingPreparedTime = true;
                }
            }

            // If the origin anchor has not been placed yet, then update in resolving mode is
            // complete.
            if (_currentMode == ApplicationMode.Resolving && !IsOriginPlaced)
            {
                return;
            }

            // If the player has not touched the screen then the update is complete.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            // Ignore the touch if it's pointing on UI objects.
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            TrackableHit arcoreHitResult = new TrackableHit();
            _lastHitPose = null;

            // Raycast against the location the player touched to search for planes.
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                if (ARCoreWorldOriginHelper.Raycast(touch.position.x, touch.position.y,
                        TrackableHitFlags.PlaneWithinPolygon, out arcoreHitResult))
                {
                    _lastHitPose = arcoreHitResult.Pose;
                }
            }
            else
            {
                Pose hitPose;
                if (_aRKit.RaycastPlane(
                    ARKitFirstPersonCamera, touch.position.x, touch.position.y, out hitPose))
                {
                    _lastHitPose = hitPose;
                }
            }

            // If there was an anchor placed, then instantiate the corresponding object.
            if (_lastHitPose != null)
            {
                if (!IsOriginPlaced && _currentMode == ApplicationMode.Hosting)
                {
                    if (Application.platform != RuntimePlatform.IPhonePlayer)
                    {
                        _worldOriginAnchor =
                            arcoreHitResult.Trackable.CreateAnchor(arcoreHitResult.Pose);
                    }
                    else
                    {
                        _worldOriginAnchor = _aRKit.CreateAnchor(_lastHitPose.Value);
                    }

                    SetWorldOrigin(_worldOriginAnchor.transform);
                    _InstantiateAnchor();
                    OnAnchorInstantiated(true);
                }
                else
                {
                    OnPlaneTouch?.Invoke(_lastHitPose.Value.position, _lastHitPose.Value.rotation);
                }
            }
        }

        /// <summary>
        /// Indicates whether the resolving prepare time has passed so the AnchorController
        /// can start to resolve the anchor.
        /// </summary>
        /// <returns><c>true</c>, if resolving prepare time passed, otherwise returns <c>false</c>.
        /// </returns>
        public bool IsResolvingPrepareTimePassed()
        {
            return _currentMode != ApplicationMode.Ready &&
                _timeSinceStart > ResolvingPrepareTime;
        }

        /// <summary>
        /// Sets the apparent world origin so that the Origin of Unity's World Coordinate System
        /// coincides with the Anchor. This function needs to be called once the Cloud Anchor is
        /// either hosted or resolved.
        /// </summary>
        /// <param name="anchorTransform">Transform of the Cloud Anchor.</param>
        public void SetWorldOrigin(Transform anchorTransform)
        {
            if (IsOriginPlaced)
            {
                Debug.LogWarning("The World Origin can be set only once.");
                return;
            }

            IsOriginPlaced = true;

            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                ARCoreWorldOriginHelper.SetWorldOrigin(anchorTransform);
            }
            else
            {
                _aRKit.SetWorldOrigin(anchorTransform);
            }
        }

        public void SetWorldOriginWithoutHosting(Transform anchorTransform)
        {
            SetWorldOrigin(anchorTransform);
            GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>().SpawnAnchorWithoutHosting();
        }

        /// <summary>
        /// Callback called when the resolving timeout is passed.
        /// </summary>
        public void OnResolvingTimeoutPassed()
        {
            if (_currentMode != ApplicationMode.Resolving)
            {
                Debug.LogWarning("OnResolvingTimeoutPassed shouldn't be called" +
                    "when the application is not in resolving mode.");
                return;
            }

            NetworkUIController.ShowDebugMessage("Still resolving the anchor." +
                "Please make sure you're looking at where the Cloud Anchor was hosted." +
                "Or, try to re-join the room.");
        }

        /// <summary>
        /// Handles user intent to enter a mode where they can place an anchor to host or to exit
        /// this mode if already in it.
        /// </summary>
        public void OnEnterHostingModeClick()
        {
            if (_currentMode == ApplicationMode.Hosting)
            {
                _currentMode = ApplicationMode.Ready;
                _ResetStatus();
                Debug.Log("Reset ApplicationMode from Hosting to Ready.");
            }

            _currentMode = ApplicationMode.Hosting;
        }

        /// <summary>
        /// Handles a user intent to enter a mode where they can input an anchor to be resolved or
        /// exit this mode if already in it.
        /// </summary>
        public void OnEnterResolvingModeClick()
        {
            if (_currentMode == ApplicationMode.Resolving)
            {
                _currentMode = ApplicationMode.Ready;
                _ResetStatus();
                Debug.Log("Reset ApplicationMode from Resolving to Ready.");
            }

            _currentMode = ApplicationMode.Resolving;
        }

        /// <summary>
        /// Callback indicating that the Cloud Anchor was instantiated and the host request was
        /// made.
        /// </summary>
        /// <param name="isHost">Indicates whether this player is the host.</param>
        public void OnAnchorInstantiated(bool isHost)
        {
            if (_anchorAlreadyInstantiated)
            {
                return;
            }

            _anchorAlreadyInstantiated = true;
            OnAnchorStartInstantiating?.Invoke();
        }

        /// <summary>
        /// Callback indicating that the Cloud Anchor was hosted.
        /// </summary>
        /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was hosted
        /// successfully.</param>
        /// <param name="response">The response string received.</param>
        public void OnAnchorHosted(bool success, string response)
        {
            _anchorFinishedHosting = success;
            OnAnchorFinishHosting?.Invoke(success, response);
        }

        /// <summary>
        /// Callback indicating that the Cloud Anchor was resolved.
        /// </summary>
        /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was resolved
        /// successfully.</param>
        /// <param name="response">The response string received.</param>
        public void OnAnchorResolved(bool success, string response)
        {
            NetworkUIController.OnAnchorResolved(success, response);
            OnAnchorFinishResolving?.Invoke(success, response);
        }

        /// <summary>
        /// Instantiates the anchor object at the pose of the m_LastPlacedAnchor Anchor. This will
        /// host the Cloud Anchor.
        /// </summary>
        private void _InstantiateAnchor()
        {
            // The anchor will be spawned by the host, so no networking Command is needed.
            GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
                .SpawnAnchor(Vector3.zero, Quaternion.identity, _worldOriginAnchor);
        }

        /// <summary>
        /// Sets the corresponding platform active.
        /// </summary>
        private void _SetPlatformActive()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                ARCoreRoot.SetActive(true);
                ARKitRoot.SetActive(false);
            }
            else
            {
                ARCoreRoot.SetActive(false);
                ARKitRoot.SetActive(true);
            }
        }

        /// <summary>
        /// Resets the internal status.
        /// </summary>
        private void _ResetStatus()
        {
            // Reset internal status.
            _currentMode = ApplicationMode.Ready;
            if (_worldOriginAnchor != null)
            {
                Destroy(_worldOriginAnchor.gameObject);
            }

            IsOriginPlaced = false;
            _worldOriginAnchor = null;
        }

        private void _SwitchActiveScreen(ActiveScreen activeScreen)
        {
            StatusScreen.SetActive(activeScreen != ActiveScreen.StartScreen);
            StartScreen.SetActive(activeScreen == ActiveScreen.StartScreen);
            ARScreen.SetActive(activeScreen == ActiveScreen.ARScreen);
            _currentActiveScreen = activeScreen;

            if (_currentActiveScreen == ActiveScreen.StartScreen)
            {
                PlayerPrefs.SetInt(HasDisplayedStartInfoKey, 1);
            }

            if (_currentActiveScreen == ActiveScreen.ARScreen)
            {
                // Set platform active when switching to AR Screen so the camera permission only
                // shows after Start Screen.
                _timeSinceStart = 0.0f;
                _SetPlatformActive();
            }
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            var sleepTimeout = SleepTimeout.NeverSleep;

#if !UNITY_IOS
            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                sleepTimeout = SleepTimeout.SystemSetting;
            }
#endif

            Screen.sleepTimeout = sleepTimeout;

            if (_isQuitting)
            {
                return;
            }

            // Quit if ARCore is in error status.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ReturnToLobbyWithReason("Camera permission is needed to run this application.");
            }
            else if (Session.Status.IsError())
            {
                _QuitWithReason("ARCore encountered a problem connecting. " +
                    "Please start the app again.");
            }
        }

        /// <summary>
        /// Quits the application after 5 seconds for the toast to appear.
        /// </summary>
        /// <param name="reason">The reason of quitting the application.</param>
        private void _QuitWithReason(string reason)
        {
            if (_isQuitting)
            {
                return;
            }

            NetworkUIController.ShowDebugMessage(reason);
            _isQuitting = true;
            Invoke("_DoQuit", 5.0f);
        }

        /// <summary>
        /// Returns to lobby after 3 seconds for the reason message to appear.
        /// </summary>
        /// <param name="reason">The reason of returning to lobby.</param>
        private void _ReturnToLobbyWithReason(string reason)
        {
            // No need to return if the application is currently quitting.
            if (_isQuitting)
            {
                return;
            }

            NetworkUIController.ShowDebugMessage(reason);
            _DoReturnToLobbyWithError(reason);
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Actually return to lobby scene.
        /// </summary>
        private void _DoReturnToLobbyWithError(string errorMessage)
        {
            PhotonNetwork.LeaveRoom();
            SceneLoader.Instance.LoadSceneAsyncWithError(settings.mainMenuScene, errorMessage);
        }

        private void _DoReturnToLobby()
        {
            PhotonNetwork.LeaveRoom();
            SceneLoader.Instance.LoadSceneAsync(settings.mainMenuScene);
        }
    }
}
