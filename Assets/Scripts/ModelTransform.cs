using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ModelTransform : MonoBehaviour
{
	public SteamVR_Action_Boolean GrabPinch;
	public SteamVR_Input_Sources handType;
	public Transform handTransform;
	public Transform headTransform;
	[HideInInspector]
	public bool locked;
	private const float dist = 0.4f;
	private float angle = 0;
	private float scale = 1;
	private bool isTriggerDown = false;
	private Vector3 prevPos;
	// Start is called before the first frame update
	void Start()
	{
		GrabPinch.AddOnStateDownListener(triggerDown, handType);
		GrabPinch.AddOnStateUpListener(triggerUp, handType);
		locked = false;
	}

	// Update is called once per frame
	void Update()
	{
		Teleport.instance.CancelTeleportHint();
		Vector3 target = headTransform.position + dist * new Vector3(Mathf.Sin(headTransform.rotation.eulerAngles.y * Mathf.PI / 180), -1.5f, Mathf.Cos(headTransform.rotation.eulerAngles.y * Mathf.PI / 180));
		if (isTriggerDown) {
			Vector3 deltaPos = headTransform.InverseTransformPoint(handTransform.position) - prevPos;
			angle -= deltaPos.z * 250f;
			scale += deltaPos.x * 1f;
			scale = Mathf.Clamp(scale, 0.5f, 2f);
			prevPos = headTransform.InverseTransformPoint(handTransform.position);
		}
		else if ((transform.position - target).magnitude > 0.05f && !locked) {
			transform.position = Vector3.Lerp(transform.position, target, 0.025f);
		}
		transform.rotation = Quaternion.Euler(0, angle + headTransform.rotation.eulerAngles.y, 0);
		transform.localScale = new Vector3(scale, scale, scale);
	}
	public void triggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		isTriggerDown = true;
		prevPos = headTransform.InverseTransformPoint(handTransform.position);
	}
	public void triggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		isTriggerDown = false;
	}
}
