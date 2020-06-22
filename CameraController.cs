using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	//[RequireComponent(typeof(Rigidbody))]
	public GameObject target;

	public float trnMult = 0.1f;

	[Range(0, 360f)]
    public float directionAngle = 0f;
    
    public bool smooth = true;

    [Range(0f, 1f)]
    public float smoothFactor = 1f;
    
    public float distance = 5f;
    public Vector3 angleOffset = Vector3.zero;

    /*************************************************************/

    private Rigidbody targetRb;

    private float turningInput = 0f;

    void Awake()
    {
    	targetRb = target.GetComponent<Rigidbody>();
    }

    void Update()
    {
    	ReadInput();
    }

    void LateUpdate()
    {
        UpdateDirection();

        UpdateCameraTransform();
    }

    void ReadInput()
    {
    	turningInput = Input.GetAxis("Horizontal");
    }

    void UpdateDirection()
    {
        directionAngle -= turningInput * trnMult;

        RestrainDirectionAngle();

        float rads = directionAngle * Mathf.PI / 180f;

        CameraSharedData.directionVector.x = Mathf.Cos(rads);
        CameraSharedData.directionVector.z = Mathf.Sin(rads);
    }

    void RestrainDirectionAngle()
    {
        if(directionAngle < 0f)
            directionAngle += 360f;
        else if(directionAngle > 360f)
            directionAngle -= 360f;
    }

    void UpdateCameraTransform()
    {
    	Vector3 d = CameraSharedData.directionVector; 	 // cam direction - black
    	Vector3 v = targetRb.velocity.normalized;     	 // velocity      - yellow
    	Vector3 p = new Vector3(v.x, 0, v.z).normalized; // velocity proj - orange (1f, 0.6f, 0f)

    	float dp = Vector3.Dot(d, p);					 // dot prod between _cam direction_ and _velocity proj_

    	Vector3 f = Vector3.Lerp(d, v, dp); // final result  - red


    	Vector3 desiredPosition = target.transform.position - f * distance;

    	if(smooth)
    		transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothFactor * Time.deltaTime * 100);
    	else
    		transform.position = desiredPosition;
    	

    	transform.LookAt(target.transform.position);

    	Debug.DrawRay(target.transform.position, d, Color.black);
    	Debug.DrawRay(target.transform.position, v, Color.yellow);    	
    	Debug.DrawRay(target.transform.position, p, new Color(1, 0.6f, 0));
    }
}