using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolMenuController : MonoBehaviour
{
	private Vector3 enableScale = new Vector3(0.001f, 0.001f, 0.001f);
	private bool lastFrameClosed = false;
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (lastFrameClosed) {
			lastFrameClosed = false;
		}
		else if (StatusRecord.tool != StatusRecord.ControllerStatus.Menu && GestureHandler.leftGrabClicked && !isInsideAnyBlock()) {
			GetComponent<RectTransform>().localScale = enableScale;
			StatusRecord.tool = StatusRecord.ControllerStatus.Menu;
		}
		else if (StatusRecord.tool == StatusRecord.ControllerStatus.Menu && (GestureHandler.leftGrabClicked || GestureHandler.rightGrabClicked)) {
			GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
			StatusRecord.tool = StatusRecord.ControllerStatus.BlockControl;
			lastFrameClosed = true;
		}
	}

	private bool isInsideAnyBlock() {
		GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
		foreach (GameObject block in blocks) {
			if (block.GetComponent<BlockController>() != null && block.GetComponent<BlockController>().canOpenMenu()) {
				return true;
			}
		}
		return false;
	}
}
