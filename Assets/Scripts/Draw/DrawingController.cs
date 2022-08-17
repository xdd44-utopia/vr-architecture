using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DrawingController : MonoBehaviour
{
	public GameObject drawingSegment;
	private GameObject curSegment;
	public ModelTransform modelTransform;
	private LineRenderer curLine;

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (StatusRecord.tool == StatusRecord.ControllerStatus.Draw) {
			if (GestureHandler.leftTriggerPressed && isInside()) {
				if (GestureHandler.leftTriggerDown) {
					curSegment = Instantiate(drawingSegment, this.transform);
					curLine = curSegment.GetComponent<LineRenderer>();
					curLine.startWidth = 0.01f;
					curLine.endWidth = 0.01f;
					modelTransform.locked = true;
				}
				else {
					curLine.positionCount += 1;
					curLine.SetPosition(curLine.positionCount - 1, transform.InverseTransformPoint(GestureHandler.leftHandPos));
					modelTransform.locked = true;
				}
			}
			else {
				modelTransform.locked = false;
			}
			if (GestureHandler.leftTriggerUp) {
				curSegment.GetComponent<DrawingModel>().finish();
			}
		}
	}

	private bool isInside() {
		Vector3 handPos = transform.InverseTransformPoint(GestureHandler.leftHandPos);
		return !(handPos.x > 6 || handPos.x < -6 || handPos.y > 6 || handPos.y < 0 || handPos.z > 6 || handPos.z < -6);
	}
}
