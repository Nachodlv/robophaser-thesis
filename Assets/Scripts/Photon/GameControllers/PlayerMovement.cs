using System;
using Photon.Pun;
using UnityEngine;

namespace Photon.GameControllers
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviourPun
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;

        private CharacterController _characterController;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!photonView.IsMine) return;
            MovePlayer();
            RotatePlayer();
        }

        private void MovePlayer()
        {
            var movement = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                movement += transform.forward;
            if (Input.GetKey(KeyCode.A))
                movement -= transform.right;
            if (Input.GetKey(KeyCode.S))
                movement -= transform.forward;
            if (Input.GetKey(KeyCode.D))
                movement += transform.right;
            _characterController.Move(movement * Time.deltaTime * movementSpeed);
        }

        private void RotatePlayer()
        {
            var mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
            transform.Rotate(new Vector3(0, mouseX, 0));
        }
    }
}