using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public float defaultPlayerDistance = 1;
    public float minPlayerDistance = 1;
    public float maxPlayerDistance = 10;
    public float turnSpeed = 5;
    float playerDistance;
    Vector3 offset;

    private GameObject player;
 
	// Use this for initialization
	void Start ()
    {
        playerDistance = defaultPlayerDistance;
        player = GameObject.FindGameObjectWithTag("Player");
        offset = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 10.0f);
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
        transform.LookAt(player.transform);
      //  transform.position = player.transform.position + offset;
	}
}
