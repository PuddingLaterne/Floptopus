using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public float minPlayerDistance = 1;
    public float maxPlayerDistance = 10;

    float playerDistance;
	float angle;
    Vector3 offset;
	Vector3 target;

    private GameObject player;
    private PlayerMovement playerMovement;
    private bool following;
    private float lastChange;

	// Use this for initialization
	void Start ()
    {
        lastChange = 0.0f;
		angle = 0;
        playerDistance = (minPlayerDistance + maxPlayerDistance) / 2;
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        lastChange += Time.deltaTime;
        if (Input.GetButton("Fire2") && lastChange > 0.2f)
        {
            following = !following;
            lastChange = 0.0f;
            target = new Vector3(transform.position.x - player.transform.position.x, 10, transform.position.z -player.transform.position.z);
        }

        if (following)
            FollowPlayer();
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle) 
	{
		return angle * ( point - pivot) + pivot;
	}

    void FollowPlayer()
    {
        playerDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000;
        if (playerDistance < minPlayerDistance)
            playerDistance = minPlayerDistance;
        if (playerDistance > maxPlayerDistance)
            playerDistance = maxPlayerDistance;
        if (Input.GetButton("Fire1"))
        {
            angle += Time.deltaTime * 100 * Input.GetAxis("Mouse X");
            target -= Vector3.up * Input.GetAxis("Mouse Y");
            if (target.y > 50)
                target.y = 50;
            if (target.y < 3)
                target.y = 3;
        }


        offset = (RotatePointAroundPivot(target, player.transform.position, Quaternion.Euler(0, angle, 0))).normalized * playerDistance;

        transform.LookAt(player.transform.position);
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, Time.deltaTime * 2);
    }
}
