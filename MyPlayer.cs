using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour {

	float walkSpeed = 0;
	public float maxWalkSpeed = 50.0f;

	public Vector3 direction = new Vector3(0, 0, 0);

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();

    }
	
	public void UpdateMovement(float speed){
		direction = new Vector3(direction.x, 0, direction.z);
		walkSpeed = Mathf.Min(speed, maxWalkSpeed);
	}

	void FixedUpdate(){
		displayRotation(direction);
		displaySpeed(walkSpeed);
		rb.AddForce(direction*walkSpeed, ForceMode.Force);
	}


	/* DEBUG! */
	// Show current rotation
	void displayRotation(Vector3 dir){
		GameObject cube = GameObject.FindGameObjectWithTag("FxTemporaire");
		cube.transform.forward = dir;
	}
	void displaySpeed(float speed){
		GameObject text = GameObject.FindGameObjectWithTag("SpeedDebugText");
		text.GetComponent<TextMesh>().text = speed.ToString();
	}

}
