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

	private Vector3 prevPos = Vector3.zero;

	private float[] filter = new float[9]{1, 8, 28, 56, 70, 56, 28, 8, 1};

	private float timer = 0;
	private const float cooldown = 0.5f;

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if (StatusRecord.tool == StatusRecord.ControllerStatus.Draw) {
			if (GestureHandler.leftTriggerPressed && isInside()) {
				if (curSegment == null) {
					timer = 0;
					curSegment = Instantiate(drawingSegment, this.transform);
					curLine = curSegment.GetComponent<LineRenderer>();
					curLine.startWidth = 0.005f;
					curLine.endWidth = 0.005f;
					modelTransform.locked = true;
				}
				else {
					modelTransform.locked = true;
					Vector3 curPos = transform.InverseTransformPoint(GestureHandler.leftHandPos);
					if (Vector3.Distance(prevPos, curPos) > 0.05f) {
						curLine.positionCount += 1;
						curLine.SetPosition(curLine.positionCount - 1, curPos);
						prevPos = curPos;
					}
				}
			}
			else {
				modelTransform.locked = false;
			}
			if (!GestureHandler.leftTriggerPressed && curSegment != null && timer > cooldown) {
				if (curLine.positionCount >= filter.Length) {
					for (int i = filter.Length / 2;i < curLine.positionCount - filter.Length / 2 - 1;i++) {
						Vector3 sum = Vector3.zero;
						for (int j=-filter.Length / 2;j<=filter.Length / 2;j++) {
							sum += curLine.GetPosition(i + j) * filter[j + filter.Length / 2];
						}
						sum /= 256;
						curLine.SetPosition(i, sum);
					}
				}
				curSegment.GetComponent<DrawingModel>().finish();
				curSegment = null;
			}
		}
	}

	private bool isInside() {
		Vector3 handPos = transform.InverseTransformPoint(GestureHandler.leftHandPos);
		return !(handPos.x > 3.75f || handPos.x < -3.75f || handPos.y > 6 || handPos.y < 0 || handPos.z > 4f || handPos.z < -4f);
	}
}
