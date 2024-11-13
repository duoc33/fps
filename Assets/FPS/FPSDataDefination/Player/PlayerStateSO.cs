using System.Collections;
using System.Collections.Generic;
using ScriptableObjectBase;
using UnityEngine;
using UnityEngine.InputSystem;
namespace FPS
{
    public class PlayerStateSO : SOBase
    {
        public int Health = 100;
        public float Height = 1.8f;
        public float Radius = 0.5f;
        private Transform camerHolder;
        public void Apply(GameObject player)
        {
            CapsuleCollider capsuleCollider = player.AddComponent<CapsuleCollider>();
            capsuleCollider.center = Vector3.up * ( Height / 2.0f );
            capsuleCollider.radius = Radius;
            capsuleCollider.height = Height;
            CharacterController characterController = player.AddComponent<CharacterController>();
            characterController.center = Vector3.up * ( Height / 2.0f );
            characterController.height = Height;
            characterController.radius = Radius;
            FPSPlayerState fPSPlayerState = player.AddComponent<FPSPlayerState>();
            fPSPlayerState.Health = Health;
            fPSPlayerState.Height = Height;
            fPSPlayerState.Width = Radius;
            camerHolder = player.transform.Find("CameraHolder");
            camerHolder.transform.SetParent(player.transform);
            camerHolder.transform.localPosition = Vector3.up * (Height * 0.8f);
            camerHolder.transform.localRotation = Quaternion.identity;

        }
        public Transform GetCamerHolder() => camerHolder;
    }
}

