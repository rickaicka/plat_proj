using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform player;

    public Vector3 offset;
    
    public float zoomSpeed = 5f;
    
    public float minZoom = 5f;
    
    public float maxZoom = 15f;
    
    public float pitch = 2f;
    
    private float currentZoom = 10f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    private void LateUpdate()
    {
        transform.position = player.position - offset * currentZoom;
        transform.LookAt(player.position + Vector3.up * pitch);
    }
}
