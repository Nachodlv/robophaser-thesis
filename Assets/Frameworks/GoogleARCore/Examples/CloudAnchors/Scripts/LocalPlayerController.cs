//-----------------------------------------------------------------------
// <copyright file="LocalPlayerController.cs" company="Google LLC">
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
using System.IO;
using Photon.Pun;
using UnityEngine.Networking;

namespace GoogleARCore.Examples.CloudAnchors
{
    using UnityEngine;
    // using UnityEngine.Networking;

    /// <summary>
    /// Local player controller. Handles the spawning of the networked Game Objects.
    /// </summary>
    public class LocalPlayerController : MonoBehaviourPun
    {
        private CloudAnchorsExampleController _controller;

        private void Awake()
        {
            _controller = FindObjectOfType<CloudAnchorsExampleController>();
            if (photonView.IsMine) gameObject.name = "LocalPlayer";
        }

        /// <summary>
        /// Will spawn the origin anchor and host the Cloud Anchor. Must be called by the host.
        /// </summary>
        /// <param name="position">Position of the object to be instantiated.</param>
        /// <param name="rotation">Rotation of the object to be instantiated.</param>
        /// <param name="anchor">The ARCore Anchor to be hosted.</param>
        public void SpawnAnchor(Vector3 position, Quaternion rotation, Component anchor)
        {
            // Host can spawn directly without using a Command because the server is running in this
            // instance.
            var anchorPrefab =  PhotonNetwork.Instantiate(Path.Combine("ARCorePrefabs", "Anchor"), position, rotation);
            anchorPrefab.GetComponent<AnchorController>().HostLastPlacedAnchor(anchor);
        }

        public void SpawnAnchorWithoutHosting()
        {
            PhotonNetwork.Instantiate(Path.Combine("ARCorePrefabs", "Anchor"), Vector3.zero, Quaternion.identity);
            photonView.RPC(nameof(RPC_FinishHosting), RpcTarget.All);
        }

        /// <summary>
        /// A command run on the server that will spawn the Star prefab in all clients.
        /// </summary>
        /// <param name="position">Position of the object to be instantiated.</param>
        /// <param name="rotation">Rotation of the object to be instantiated.</param>
        public void SpawnStar(Vector3 position, Quaternion rotation)
        {
            // Instantiate Star model at the hit pose.
            // Spawn the object in all clients.
            PhotonNetwork.Instantiate(Path.Combine("ARCorePrefabs", "Star"), position, rotation);
        }

        [PunRPC]
        private void RPC_FinishHosting()
        {
            _controller.OnAnchorHosted(true, default);
            _controller.OnAnchorResolved(true, default);
        }
    }
}
