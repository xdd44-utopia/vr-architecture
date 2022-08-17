using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
	public EventSystem eventSystem;
	public StandaloneInputModule inputModule;

	private LineRenderer lr;
	private MeshRenderer mr;
	private float defaultLength = 3;
	
	void Start()
	{
		
	}

	void Awake() {
		mr = transform.GetChild(0).GetComponent<MeshRenderer>();
		lr = transform.GetChild(1).GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		mr.enabled = !(StatusRecord.tool == StatusRecord.ControllerStatus.Menu);
		lr.enabled = StatusRecord.tool == StatusRecord.ControllerStatus.Menu;
		if (lr.enabled) {
			updateLength();
		}
	}

	private void updateLength() {
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, getEnd());
	}

	private Vector3 getEnd() {
		float distance = getCanvasDistance();
		Vector3 endPos = calculateEnd(defaultLength);
		if (distance != 0) {
			endPos = calculateEnd(distance);
		}
		return endPos;
	}

	private Vector3 calculateEnd(float distance) {
		return transform.position + transform.forward * distance;
	}

	private float getCanvasDistance() {
		PointerEventData eventData = new PointerEventData(eventSystem);
		eventData.position = inputModule.inputOverride.mousePosition;
		List<RaycastResult> results = new List<RaycastResult>();
		eventSystem.RaycastAll(eventData, results);
		RaycastResult closestResult = findFirstRaycast(results);
		return Mathf.Clamp(closestResult.distance, 0, defaultLength);
	}

	private RaycastResult findFirstRaycast(List<RaycastResult> results) {
		foreach (RaycastResult result in results) {
			if (!result.gameObject) {
				continue;
			}
			return result;
		}
		return new RaycastResult();
	}
}
