using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ToolMenuController : MonoBehaviour
{
	public SteamVR_Action_Vector2 axisValue;

	private BlockSpawner blockSpawner;

	public GameObject[] buttons;
	public Sprite handTool;
	public Sprite drawTool;
	public GameObject selected;
	private int currentSelect = 0;

	private float timer = 0;
	private const float cooldown = 0.5f;

	private Vector3 enableScale = new Vector3(0.001f, 0.001f, 0.001f);
	private bool isPrevHand = true;
	private bool viewing = false;

	void Start()
	{
		blockSpawner = GameObject.Find("BlockSpawner").GetComponent<BlockSpawner>();
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if (!viewing && GestureHandler.leftGrabClicked && !isInsideAnyBlock()) {
			isPrevHand = StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl;
			buttons[0].GetComponent<Image>().sprite = isPrevHand ? drawTool : handTool;
			if (StatusRecord.switchToRay()) {
				GetComponent<RectTransform>().localScale = enableScale;
				currentSelect = 0;
				viewing = true;
			}
		}
		else if (viewing) {
			if (timer > cooldown && axisValue.axis.x != 0) {
				currentSelect += axisValue.axis.x > 0 ? 1 : -1;
				currentSelect = (currentSelect + 5) % 5;
				timer = 0;
			}
			selected.GetComponent<RectTransform>().anchoredPosition = new Vector2(-440 + currentSelect * 220, 0);
			if (GestureHandler.leftGrabClicked || GestureHandler.rightGrabClicked) {
				closeMenu(isPrevHand);
			}
			if (GestureHandler.leftTriggerClicked || GestureHandler.rightTriggerClicked) {
				if (currentSelect == 0) {
					closeMenu(!isPrevHand);
				}
				else {
					blockSpawner.spawn(currentSelect - 1);
					closeMenu(true);
				}
			}
		}

	}

	private void closeMenu(bool isHand) {
		if ((isHand && StatusRecord.switchToHand()) || (!isHand && StatusRecord.switchToDraw())) {
			GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
			viewing = false;
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
