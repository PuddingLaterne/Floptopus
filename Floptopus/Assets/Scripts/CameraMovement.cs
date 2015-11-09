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
 
	// Use this for initialization
	void Start ()
    {
		angle = 0;
        playerDistance = (minPlayerDistance + maxPlayerDistance) / 2;
        player = GameObject.FindGameObjectWithTag("Player");
        target = new Vector3(-player.transform.position.x, 10, -player.transform.position.z);
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
		playerDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * 100;
		if (playerDistance < minPlayerDistance)
			playerDistance = minPlayerDistance;
		if (playerDistance > maxPlayerDistance)
			playerDistance = maxPlayerDistance;
		if (Input.GetButton ("Fire1")) 
		{
			angle += Time.deltaTime * 100 * Input.GetAxis("Mouse X");
			target -= Vector3.up * Input.GetAxis ("Mouse Y");
			if(target.y > 20)
				target.y = 20;
			if(target.y < 3)
				target.y = 3;
		}
		offset = RotatePointAroundPivot(target, player.transform.position,  Quaternion.Euler(0, angle, 0)) * playerDistance;
		
        transform.LookAt(player.transform);
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x + offset.x, Time.deltaTime * 2),
		                                 Mathf.Lerp(transform.position.y, player.transform.position.y + offset.y, Time.deltaTime * 2),
		                                 Mathf.Lerp(transform.position.z, player.transform.position.z + offset.z, Time.deltaTime * 2));
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle) 
	{
		return angle * ( point - pivot) + pivot;
	}
}
