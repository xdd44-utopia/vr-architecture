using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GestureHandler : MonoBehaviour
{
	public SteamVR_Action_Boolean LeftTrigger;
	public SteamVR_Action_Boolean RightTrigger;
	public SteamVR_Action_Boolean LeftGrab;
	public SteamVR_Action_Boolean RightGrab;
	public SteamVR_Input_Sources LeftHand;
	public SteamVR_Input_Sources RightHand;
	public Transform leftHandTransform;
	public Transform rightHandTransform;
	public Transform headTransform;

	public static Vector3 headPos;

	public static bool leftTriggerClicked = false; //Pressed for a very short time
	public static bool leftTriggerPressed = false; //Is current frame down
	public static bool leftTriggerPrev = false; //Is previous frame down
	public static bool leftTriggerDown = false; //The frame when trigger is pressed down
	public static bool leftTriggerUp = false; //The frame when trigger is released
	public static bool leftGrabClicked = false;
	public static bool leftGrabPressed = false;
	public static Vector3 leftHandInitPos; //Position at the frame when trigger is pressed down
	public static Vector3 leftHandDeltaPos;
	public static Vector3 leftHandPos;
	private float leftTriggerTimer = 0;
	private int leftTriggerFrame = 0;
	private float leftGrabTimer = 0;
	private int leftGrabFrame = 0;

	public static bool rightTriggerClicked = false;
	public static bool rightTriggerPressed = false;
	public static bool rightTriggerPrev = false;
	public static bool rightTriggerDown = false;
	public static bool rightTriggerUp = false;
	public static bool rightGrabClicked = false;
	public static bool rightGrabPressed = false;
	public static Vector3 rightHandInitPos;
	public static Vector3 rightHandDeltaPos;
	public static Vector3 rightHandPos;
	private float rightTriggerTimer = 0;
	private int rightTriggerFrame = 0;
	private float rightGrabTimer = 0;
	private int rightGrabFrame = 0;

	private const float clickRecTime = 0.2f;

	void Start()
	{
		LeftTrigger.AddOnStateDownListener(leftTiggerDown, LeftHand);
		LeftTrigger.AddOnStateUpListener(leftTiggerUp, LeftHand);
		LeftGrab.AddOnStateDownListener(leftGrabDown, LeftHand);
		LeftGrab.AddOnStateUpListener(leftGrabUp, LeftHand);
		RightTrigger.AddOnStateDownListener(rightTiggerDown, RightHand);
		RightTrigger.AddOnStateUpListener(rightTiggerUp, RightHand);
		RightGrab.AddOnStateDownListener(rightGrabDown, RightHand);
		RightGrab.AddOnStateUpListener(rightGrabUp, RightHand);
		leftHandPos = leftHandTransform.position;
		rightHandPos = rightHandTransform.position;
	}

	// Update is called once per frame
	void Update()
	{
		Teleport.instance.CancelTeleportHint();
		headPos = headTransform.position;
		leftHandDeltaPos = leftHandTransform.position - leftHandPos;
		rightHandDeltaPos = rightHandTransform.position - rightHandPos;
		leftHandPos = leftHandTransform.position;
		rightHandPos = rightHandTransform.position;

		leftTriggerTimer += Time.deltaTime;
		if (leftTriggerClicked && leftTriggerFrame == 1) {
			leftTriggerClicked = false;
		}
		else {
			leftTriggerFrame++;
		}

		leftGrabTimer += Time.deltaTime;
		if (leftGrabClicked) {
			Debug.Log("Left grabbed");
		}
		if (leftGrabClicked && leftGrabFrame == 1) {
			leftGrabClicked = false;
		}
		else {
			leftGrabFrame++;
		}
		leftTriggerDown = !leftTriggerPrev && (leftTriggerClicked || leftTriggerPressed);
		leftTriggerUp = leftTriggerPrev && !(leftTriggerClicked || leftTriggerPressed);
		leftTriggerPrev = leftTriggerClicked || leftTriggerPressed;

		rightTriggerTimer += Time.deltaTime;
		if (rightTriggerClicked && rightTriggerFrame == 1) {
			rightTriggerClicked = false;
		}
		else {
			rightTriggerFrame++;
		}

		rightGrabTimer += Time.deltaTime;
		if (rightGrabClicked) {
			Debug.Log("Right grabbed");
		}
		if (rightGrabClicked && rightGrabFrame == 1) {
			rightGrabClicked = false;
		}
		else {
			rightGrabFrame++;
		}
		rightTriggerDown = !rightTriggerPrev && (rightTriggerClicked || rightTriggerPressed);
		rightTriggerUp = rightTriggerPrev && !(rightTriggerClicked || rightTriggerPressed);
		rightTriggerPrev = rightTriggerClicked || rightTriggerPressed;
	}

	public void leftTiggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		leftTriggerTimer = 0;
		leftHandInitPos = leftHandPos;
		leftTriggerPressed = true;
	}
	public void leftTiggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		if (leftTriggerTimer < clickRecTime) {
			leftTriggerClicked = true;
		}
		leftTriggerFrame = 0;
		leftTriggerPressed = false;
	}
	public void rightTiggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		rightTriggerTimer = 0;
		rightHandInitPos = rightHandPos;
		rightTriggerPressed = true;
	}
	public void rightTiggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		if (rightTriggerTimer < clickRecTime) {
			rightTriggerClicked = true;
		}
		rightTriggerFrame = 0;
		rightTriggerPressed = false;
	}
	public void leftGrabDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		leftGrabTimer = 0;
		leftGrabPressed = true;
	}
	public void leftGrabUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		if (leftGrabTimer < clickRecTime) {
			leftGrabClicked = true;
		}
		leftGrabFrame = 0;
		leftGrabPressed = false;
	}
	public void rightGrabDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		rightGrabTimer = 0;
		rightGrabPressed = true;
	}
	public void rightGrabUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		if (rightGrabTimer < clickRecTime) {
			rightGrabClicked = true;
		}
		rightGrabFrame = 0;
		rightGrabPressed = false;
	}
}
