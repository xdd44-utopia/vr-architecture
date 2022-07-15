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
	public ModelTransform modelTransform;
	private LineRenderer curLine;
	private bool isTriggerDown = false;
	private bool isPrevTriggerDown = false;
	void Start()
	{
		GrabPinch.AddOnStateDownListener(triggerDown, handType);
		GrabPinch.AddOnStateUpListener(triggerUp, handType);
	}

	// Update is called once per frame
	void Update()
	{
		if (isTriggerDown) {
			if (!isPrevTriggerDown) {
				GameObject obj = Instantiate(drawingSegment, this.transform);
				curLine = obj.GetComponent<LineRenderer>();
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
		isPrevTriggerDown = isTriggerDown;
	}
	public void triggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		isTriggerDown = true;
	}
	public void triggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		isTriggerDown = false;
	}
}
