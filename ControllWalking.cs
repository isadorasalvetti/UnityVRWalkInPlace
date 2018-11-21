using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Valve.VR;

public class ControllWalking : MonoBehaviour {

	public Transform playerCharacter;

	public double currentRightPosition, currentLeftPosition; //attempting to get these as doubles to avoid errors
	
	public float maxWalkSpeed = 50.0f;

	double lastRightPosition, lastLeftPosition;
	float moveSpeed, lastMoveSpeed = 0;

	private bool iShouldTurn;

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

	public static double absD(double number) {
        double num = number;           
        if(number<0) num = -1*number;
        return num;
 }

	float getMovementSpeed(){

		// current right/ left positions are set by the trackers on every pose change.
		// last right/ left positions are set at the end of the last speed calculation.
		double deltaRight = (lastRightPosition - currentRightPosition) * trackerMod;
		double deltaLeft = (lastLeftPosition - currentLeftPosition) * trackerMod;//abs

		lastLeftPosition = currentLeftPosition;
		lastRightPosition = currentRightPosition;

		if (absD(deltaLeft) + absD(deltaRight) < tolerance){
			//variations were too small.
			return 0; //lets not move.
		}
		
		//Calculate speed - average of the squares of deltas * mod.
		float speed = (float)((absD(deltaLeft)+absD(deltaRight))*speedMod);
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
		Quaternion tRotation = Quaternion.Slerp(currentLeftRotation, currentRightRotation, 0.5f);
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
			iShouldTurn = !iShouldTurn;
			if (iShouldTurn) changeDirection();

			//Speed
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
				}
			}
		}
	}

	public float getTrackerPosition(bool left, float scl){
		if (left) return (float) currentLeftPosition * scl;
		else return (float) currentRightPosition * scl;
	}
	
}
