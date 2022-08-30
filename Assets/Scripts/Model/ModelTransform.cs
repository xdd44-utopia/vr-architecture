using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelTransform : MonoBehaviour
{
	public Transform headTransform;
	public Transform modelTransform;
	[HideInInspector]
	public bool locked;
	private const float dist = 0.25f;
	private float angle = 0;
	
	private Material mat;
	private Color insideColor = new Color(0, 0, 1, 0.8f);
	private Color outsideColor = new Color(0, 0, 1, 0.25f);
	private bool isInside = false;
	private bool isOperating = false;

	private bool modelEnabled = true;
	private const float enableScale = 0.05f;
	private const float disableScale = 0.005f;
	private const float enableHandleRadius = 0.25f;
	private const float disableHandleRadius = 0.1f;
	
	void Start()
	{
		locked = false;
		mat = new Material(GetComponent<Renderer>().material);
		GetComponent<Renderer>().material = mat;
	}

	// Update is called once per frame
	void Update()
	{
		isInside = Vector3.Distance(GestureHandler.rightHandPos, transform.position) < transform.lossyScale.x;
		mat.color = isInside ? insideColor : outsideColor;

		Vector3 target = headTransform.position + dist * new Vector3(Mathf.Sin(headTransform.rotation.eulerAngles.y * Mathf.PI / 180), -1.5f, Mathf.Cos(headTransform.rotation.eulerAngles.y * Mathf.PI / 180));

		if (GestureHandler.rightTriggerClicked && isInside) {
			modelEnabled = !modelEnabled;
		}

		if (GestureHandler.rightTriggerPressed && (isInside || isOperating)) {
			Vector3 deltaPos = headTransform.InverseTransformPoint(GestureHandler.rightHandDeltaPos) - headTransform.InverseTransformPoint(Vector3.zero);
			angle -= deltaPos.z * 250f;
			isOperating = true;
		}
		else {
			isOperating = false;
			if ((modelTransform.position - target).magnitude > 0.05f && !locked) {
				modelTransform.position = Vector3.Lerp(modelTransform.position, target, 0.025f);
			}
		}
		modelTransform.rotation = Quaternion.Euler(0, angle + headTransform.rotation.eulerAngles.y, 0);
		modelTransform.localScale = Vector3.one * (modelEnabled ? enableScale : disableScale);

		transform.localPosition = modelTransform.localPosition + new Vector3(Mathf.Sin(headTransform.rotation.eulerAngles.y * Mathf.PI / 180 + Mathf.PI / 2), 0.5f, Mathf.Cos(headTransform.rotation.eulerAngles.y * Mathf.PI / 180 + Mathf.PI / 2)) * (modelEnabled ? enableHandleRadius : disableHandleRadius);
	}
	
}
