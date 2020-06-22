using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

// Controls a ball that can go forward, back, turn left and right and jump.

public class BallController : MonoBehaviour
{
	public float movMult = 5f;
	public float jmpMult = 15f;

	public float straighteningMult = 3f;

	private float fixedMaxAngularVelocity = 50f;

    private Rigidbody rb;

    private float movInput;
    private bool jump;
    
    private bool canJump = false;

    void Awake()
    {
    	rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = fixedMaxAngularVelocity;
    }

    void Update()
    {
    	ReadInput();
    }

    void LateUpdate()
    {
      //  Debug.Log(canJump);

    	//Debug.Log("Velocity = " + rb.velocity.magnitude + " - Angular Velocity = " + rb.angularVelocity.magnitude);
    }

    void ReadInput()
    {
    	movInput = Input.GetAxis("Vertical");
    	//movInput = 1f;
    	if(Input.GetKey(KeyCode.Space) && canJump)
    		jump = true;
    }

     void OnCollisionEnter(Collision collision)
    {
    	ContactPoint[] contacts = new ContactPoint[collision.contactCount];
    	collision.GetContacts(contacts);
        foreach(ContactPoint contact in contacts)
        	if(contact.point.y < transform.position.y)
                canJump = true;
    }

    /*void OnCollisionEnter(Collision collision)
    {
    	ContactPoint[] contacts = new ContactPoint[collision.contactCount];
    	collision.GetContacts(contacts);
        foreach(ContactPoint contact in contacts)
            if(contact.point.y < transform.position.y)
                canJump++;
    }

    void OnCollisionExit(Collision collision)
    {
        ContactPoint[] contacts = new ContactPoint[collision.contactCount];
        collision.GetContacts(contacts);
        foreach(ContactPoint contact in contacts)
            if(contact.point.y < transform.position.y)
                canJump--;
    }*/

    void FixedUpdate()
    {
    	Move();
    	Jump();

    	//canJump = false;
    }

    void Move()
    {
    	if(movInput > 0f)
    	{
    		Vector3 d = CameraSharedData.directionVector;
			Vector3 p = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;

    		Vector3 forwardTorque = Vector3.Cross(d, Vector3.down) * movInput * movMult;
    		Vector3 turningTorque = Vector3.zero;

    		float dp = Vector3.Dot(d, p);

			if(dp < 0.99f)
				turningTorque = Vector3.Cross(p, Vector3.up) * straighteningMult;

			Debug.DrawRay(transform.position, forwardTorque + turningTorque, Color.green);
    		
			//Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			//Quaternion rot = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w); FIX REMOVED

    		rb.AddTorque(forwardTorque + turningTorque);

    		/*bool c = transform.position.Equals(pos);																				   DOESNT ACTUALLY WORK
																																	   AND INTRODUCES OTHER PROBLEMS
    		Debug.Log(transform.position + " " + c);

    		if((transform.position - pos).magnitude < 0.01f)
    		{
    			transform.position = pos;
    			transform.rotation = rot;
    		}*/
		}
		else if(movInput < 0f)
			rb.AddTorque(Vector3.Cross(rb.velocity.normalized, Vector3.down) * movInput * movMult);
    }

    void Jump()
    {
    	if(jump)
    	{
    		rb.AddForce(Vector3.up * jmpMult, ForceMode.Impulse);
    		jump = false;
    		canJump = false;
    	}        
    }
}