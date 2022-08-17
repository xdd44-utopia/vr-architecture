using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInput : BaseInput
{

	public Camera eventCamera;

	protected override void Awake() {
		GetComponent<BaseInputModule>().inputOverride = this;
	}

	public override bool GetMouseButton(int button) {
		return (GestureHandler.leftTriggerClicked || GestureHandler.leftTriggerPressed) && StatusRecord.tool == StatusRecord.ControllerStatus.Menu;
	}

	public override bool GetMouseButtonDown(int button) {
		return GestureHandler.leftTriggerDown && StatusRecord.tool == StatusRecord.ControllerStatus.Menu;
	}

	public override bool GetMouseButtonUp(int button) {
		return GestureHandler.leftTriggerUp && StatusRecord.tool == StatusRecord.ControllerStatus.Menu;
	}

	public override Vector2 mousePosition {
		get {
			return new Vector2(eventCamera.pixelWidth / 2, eventCamera.pixelHeight / 2);
		}
	}
}
