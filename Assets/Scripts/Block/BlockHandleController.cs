using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BlockHandleController : MonoBehaviour
{
	public SteamVR_Action_Boolean GrabPinch;
	public SteamVR_Input_Sources handType;
	public Transform block;
	[HideInInspector]
	public bool isTriggerDown = false;
	private bool isInside = false;

	private ModelTransform modelTransform;
	private Transform pointer;
	private Vector3 posDiff;
	private float defaultScale;

	private Material mat;
	private Color insideColor = new Color(0, 0, 1, 0.8f);
	private Color outsideColor = new Color(0, 0, 1, 0.25f);

	// Start is called before the first frame update
	void Start()
	{
		defaultScale = transform.localScale.x;
		block = transform.parent;
		mat = new Material(GetComponent<Renderer>().material);
		GetComponent<Renderer>().material = mat;
		GrabPinch.AddOnStateDownListener(triggerDown, handType);
		GrabPinch.AddOnStateUpListener(triggerUp, handType);
	}

	// Update is called once per frame
	void Update()
	{
		if (modelTransform == null) {
			modelTransform = GameObject.Find("ModelSmall").GetComponent<ModelTransform>();
		}
		if (pointer == null) {
			pointer = GameObject.Find("BrushPointer").transform;
		}

		isInside = Vector3.Distance(pointer.position, transform.position) < transform.lossyScale.x * 2f;
		mat.color = isInside ? insideColor : outsideColor;

		if (isTriggerDown) {
			transform.position = pointer.position - posDiff;
		}
		modelTransform.locked = isTriggerDown;
		transform.localScale = Vector3.one * 0.2f;

	}

	public void triggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		if (isInside) {
			isTriggerDown = true;
			posDiff = pointer.position - transform.position;
		}
	}
	public void triggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		isTriggerDown = false;
	}
}
