using UnityEngine;
using System.Collections;

public class LooseObject : MonoBehaviour 
{
    bool falling;
    bool fallen;
    float fallingTime;
    float fallAngle;

	void Start () 
    {
        fallingTime = 0;
        falling = false;
        fallen = false;
	}

    void Update()
    {
        if (falling && !fallen)
        {
            float timeFactor;
            if (fallingTime >= 1.0f)
                timeFactor = 5.0f;
            else
                timeFactor = 2.0f;
            fallingTime += Time.deltaTime * timeFactor;

            transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, fallAngle, Time.deltaTime * Mathf.Pow(fallingTime, 2)),
                transform.localEulerAngles.y, transform.localEulerAngles.z);

            if (Mathf.Abs(transform.localEulerAngles.x - 90) <= 0.1f)
                fallen = true;
        }
    }

    public void FallOver(Vector3 forceDirection)
    {
        if (Vector3.Angle(transform.forward, forceDirection) < 90)
            fallAngle = 90;
        else
            fallAngle = -90;
        falling = true;
    }
}
