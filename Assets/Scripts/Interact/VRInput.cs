using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInput : BaseInput
{

	public Camera eventCamera;
	private DrawingController drawingController;

	protected override void Awake() {
		drawingController = GameObject.Find("DrawingController").GetComponent<DrawingController>();
		GetComponent<BaseInputModule>().inputOverride = this;
	}

	public override bool GetMouseButton(int button) {
		return drawingController.isTriggerDown & !drawingController.contains;
	}

	public override bool GetMouseButtonDown(int button) {
		return drawingController.isTriggerComingDown & !drawingController.contains;
	}

	public override bool GetMouseButtonUp(int button) {
		return drawingController.isTriggerComingUp & !drawingController.contains;
	}

	public override Vector2 mousePosition {
		get {
			return new Vector2(eventCamera.pixelWidth / 2, eventCamera.pixelHeight / 2);
		}
	}
}
