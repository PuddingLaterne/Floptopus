﻿using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float horizontalFollowSpeed = 5.0f, verticalFollowSpeed = 0.5f, horizontalMoveSpeed = 5.0f, verticalMoveSpeed = 5.0f, zoomSpeed = 3000.0f;
    public float minRadius = 10.0f, maxRadius = 50.0f;
    Vector3 targetPoint;
    float radius = 3f, angleX = 0f, angleY = -45f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        radius = (minRadius + maxRadius) / 2;
        targetPoint = target.position;
    }

    void Update()
    {
        radius -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed;
        if (radius > maxRadius) radius = maxRadius;
        if (radius < minRadius) radius = minRadius;
        if(Input.GetButton("Fire1"))
        {
            angleX -= Input.GetAxis("Mouse X") * Time.deltaTime * horizontalMoveSpeed;
            angleY -= Input.GetAxis("Mouse Y") * Time.deltaTime * verticalMoveSpeed;
            if (Mathf.Cos(angleY) <= 0.1f || Mathf.Cos(angleY) >= 0.9f)
                angleY += Input.GetAxis("Mouse Y") * Time.deltaTime * verticalMoveSpeed;
        }

        float x = radius * Mathf.Cos(angleX) * Mathf.Sin(angleY);
        float z = radius * Mathf.Sin(angleX) * Mathf.Sin(angleY);
        float y = radius * Mathf.Cos(angleY);
        targetPoint = new Vector3(Mathf.Lerp(targetPoint.x, target.position.x, Time.deltaTime * horizontalFollowSpeed), 
                                  Mathf.Lerp(targetPoint.y, target.position.y, Time.deltaTime * verticalFollowSpeed), 
                                  Mathf.Lerp(targetPoint.z, target.position.z, Time.deltaTime * horizontalFollowSpeed));
        transform.position = new Vector3(x + targetPoint.x,
                                         y + targetPoint.y,
                                         z + targetPoint.z);
        transform.LookAt(targetPoint);
    }
}
