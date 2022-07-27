using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DrawingController : MonoBehaviour
{
	public SteamVR_Action_Boolean GrabPinch;
	public SteamVR_Input_Sources handType;
	public Transform handTransform;
	public GameObject drawingSegment;
	[HideInInspector]
	public bool contains;
	[HideInInspector]
	public bool isTriggerDown;
	[HideInInspector]
	public bool isTriggerComingDown;
	[HideInInspector]
	public bool isTriggerComingUp;
	private GameObject curSegment;
	public ModelTransform modelTransform;
	private LineRenderer curLine;
	private bool isPrevTriggerDown = false;
	void Start()
	{
		isTriggerDown = false;
		GrabPinch.AddOnStateDownListener(triggerDown, handType);
		GrabPinch.AddOnStateUpListener(triggerUp, handType);
	}

	// Update is called once per frame
	void Update()
	{
		contains = isInside();
		if (isTriggerDown && isInside()) {
			if (!isPrevTriggerDown) {
				curSegment = Instantiate(drawingSegment, this.transform);
				curLine = curSegment.GetComponent<LineRenderer>();
				curLine.SetWidth(0.01f, 0.01f);
				modelTransform.locked = true;
			}
			else {
				curLine.positionCount += 1;
				curLine.SetPosition(curLine.positionCount - 1, transform.InverseTransformPoint(handTransform.position));
				modelTransform.locked = true;
			}
		}
		else {
			modelTransform.locked = false;
		}
		isTriggerComingDown = isTriggerDown & !isPrevTriggerDown;
		isTriggerComingUp = !isTriggerDown & isPrevTriggerDown;
		if (isTriggerComingUp) {
			curSegment.GetComponent<DrawingModel>().finish();
		}
		isPrevTriggerDown = isTriggerDown;
	}

	private bool isInside() {
		Vector3 handPos = transform.InverseTransformPoint(handTransform.position);
		return !(handPos.x > 6 || handPos.x < -6 || handPos.y > 6 || handPos.y < 0 || handPos.z > 6 || handPos.z < -6);
	}

	public void triggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		isTriggerDown = true;
	}
	public void triggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		isTriggerDown = false;
	}
}
