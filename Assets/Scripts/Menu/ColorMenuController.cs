using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMenuController : MonoBehaviour
{
	private BlockController currentBlock;
	[HideInInspector]
	public int currentColor = -1;
	private Vector3 enableScale = new Vector3(0.001f, 0.001f, 0.001f);
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (currentBlock != null) {
			currentColor = currentBlock.currentColor;
		}
		if (StatusRecord.tool == StatusRecord.ControllerStatus.Menu && (GestureHandler.leftGrabClicked || GestureHandler.rightGrabClicked)) {
			GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
			StatusRecord.tool = StatusRecord.ControllerStatus.BlockControl;
		}
	}

	public void enableMenu(BlockController block) {
		currentBlock = block;
		GetComponent<RectTransform>().localScale = enableScale;
		StatusRecord.tool = StatusRecord.ControllerStatus.Menu;
	}

	public void changeColor(int i) {
		currentBlock.changeColor(i);
	}
}
