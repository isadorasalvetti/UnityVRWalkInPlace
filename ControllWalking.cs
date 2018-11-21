using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Valve.VR;

public class ControllWalking : MonoBehaviour {

	public Transform playerCharacter;
	public bool iShouldMove, iShouldTurn;
	
	public double currentRightPosition, currentLeftPosition; //attempting to get these as doubles to avoid errors
	
	[System.NonSerialized]
	public float maxWalkSpeed = 50.0f;

	double lastRightPosition, lastLeftPosition;
	float moveSpeed, lastMoveSpeed = 0;

	//List<float> lastMoveSpeeds;


	//Modifyers
	//trackerMod - multply the tracker data to avoid small values / speedMod - tune to modify the final speed output.
	public float trackerMod = 1000.0f;
	public float speedMod = 750.0f;
	public float minSpeed = 1;
	public int maxDelay = 40; //sets movement update speed to half of fixed update

	private bool inited = false;
	private int delay = 0;

	// Use this for initialization
	void Start () {
		playerCharacter.GetComponent<MovementPlayer>().maxWalkSpeed = maxWalkSpeed;
	}

	void InitPositions(){
		lastLeftPosition = currentLeftPosition;
		lastRightPosition = currentRightPosition;
		
		inited = true;
		Debug.Log("Tracker movement initialized");
	}

	public float tolerance;


	float getMovementSpeed(){

		// current right/ left positions are set by the trackers on every pose change.
		// last right/ left positions are set at the end of the last speed calculation.
		double deltaRight = (lastRightPosition - currentRightPosition) * trackerMod;
		double deltaLeft = (lastLeftPosition - currentLeftPosition) * trackerMod;//abs

		lastLeftPosition = currentLeftPosition;
		lastRightPosition = currentRightPosition;

		if (deltaLeft*deltaLeft + deltaRight*deltaRight < tolerance){
			//variations were too small.
			return 0; //lets not move.
		}
		
		//Calculate speed - average of the squares of deltas * mod.
		float speed = (float)((deltaLeft*deltaLeft + deltaRight*deltaRight)*speedMod);
		speed = Mathf.Max(speed, minSpeed);

		float rSpeed = 0.3f*speed + 0.7f*lastMoveSpeed;
		lastMoveSpeed = rSpeed;

		return rSpeed;
	}

	double lastLeftDirection, lastRightDirection;

	[System.NonSerialized]
	public Quaternion currentRightRotation, currentLeftRotation;

	void changeDirection(){
		/* 	Average the rotation on up axis of both trakers. Apply to character	
			Notes: Z is up */
		//To do: dampening on speed if there is a sharp change in rotation
		Quaternion tRotation = Quaternion.Lerp(currentLeftRotation, currentRightRotation, 0.5f);
		Vector3 playerDirection = tRotation*Vector3.forward;
		playerDirection.y = 0;
		playerDirection.Normalize();

		playerCharacter.GetComponent<MovementPlayer>().direction = playerDirection;

	}

	// Update is called once per frame
	void FixedUpdate () {
		if (!inited) InitPositions();
		else {
			//Rotation
			if (iShouldTurn) changeDirection();
			if (iShouldMove) {
				playerCharacter.GetComponent<MovementPlayer>().maxWalkSpeed = maxWalkSpeed; //DEBUG!
				moveSpeed = getMovementSpeed();
				if (moveSpeed > 0) {
					playerCharacter.GetComponent<MovementPlayer>().UpdateMovement(moveSpeed);
					delay = 0;
					}
				else {delay ++;
					if (delay> maxDelay){
						playerCharacter.GetComponent<MovementPlayer>().UpdateMovement(moveSpeed);
						delay = 0;
					}}
			}
		}
	}
	
}
