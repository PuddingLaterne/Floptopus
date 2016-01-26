using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float horizontalFollowSpeed = 5.0f, verticalFollowSpeed = 0.5f, horizontalMoveSpeed = 5.0f, verticalMoveSpeed = 5.0f, zoomSpeed = 3000.0f;
    public float minRadius = 10.0f, maxRadius = 50.0f;
    DepthOfField dof;
    Vector3 targetPoint;
    Vector3 lastMovement;
    bool resettingRotation = false;
    float rotationInterpolation = 0.0f;
    public float radius = 3f, angleX = 110f, angleY = -45f;
    float playerViewDir;

    void Start()
    {
        angleX = angleX * Mathf.Deg2Rad;
        angleY = angleY * Mathf.Deg2Rad;
        dof = GetComponent<DepthOfField>();
        targetPoint = target.position;
    }

    void Update()
    {
        dof.focalLength = radius;
   
        radius -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed;
        radius -= Input.GetAxis("Zoom In") * Time.deltaTime * zoomSpeed * 0.1f;
        radius += Input.GetAxis("Zoom Out") * Time.deltaTime * zoomSpeed * 0.1f;
        angleX = angleX % (Mathf.PI * 2);

        if (radius > maxRadius) radius = maxRadius;
        if (radius < minRadius) radius = minRadius;
        if (Input.GetButton("Fire1"))
        {
            resettingRotation = false;
            rotationInterpolation = 0.0f;
            angleX -= Input.GetAxis("Mouse X") * Time.deltaTime * horizontalMoveSpeed;
            angleY -= Input.GetAxis("Mouse Y") * Time.deltaTime * verticalMoveSpeed;
            if (Mathf.Cos(angleY) <= 0.1f || Mathf.Cos(angleY) >= 0.9f)
                angleY += Input.GetAxis("Mouse Y") * Time.deltaTime * verticalMoveSpeed;
        }
        else
        {
            angleX -= Input.GetAxis("Camera X") * Time.deltaTime * horizontalMoveSpeed * 0.1f;
            angleY -= Input.GetAxis("Camera Y") * Time.deltaTime * verticalMoveSpeed * 0.1f;
            if (Mathf.Cos(angleY) <= 0.1f || Mathf.Cos(angleY) >= 0.9f)
                angleY += Input.GetAxis("Camera Y") * Time.deltaTime * verticalMoveSpeed * 0.1f;
        }

        if (Input.GetButton("CameraReset"))
        {
            resettingRotation = true;
            playerViewDir = target.localEulerAngles.y - 90;   
        }


        if(resettingRotation)
        {
            ResetRotation();
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

    void ResetRotation() //aligns camera rotation with player viewdirection
    {
        rotationInterpolation += Time.deltaTime;     
        angleX = Mathf.LerpAngle(angleX, (Mathf.Deg2Rad * -playerViewDir), rotationInterpolation);
        if (rotationInterpolation >= 1)
        {
            rotationInterpolation = 0.0f;
            resettingRotation = false;
        }
    }

}
