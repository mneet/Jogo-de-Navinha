using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Velocidade que a camera se move
    [SerializeField] private float moveSpeed = 1;

    private void LateUpdate() {
        //transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
    }
}
