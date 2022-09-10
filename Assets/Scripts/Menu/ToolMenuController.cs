using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ToolMenuController : MonoBehaviour
{
	public SteamVR_Action_Vector2 axisValue;

	public SpawnMenuController[] menus;
	
	public int num;
	private int row = 4;

	public GameObject buttonTool;
	public Sprite handTool;
	public Sprite drawTool;
	public GameObject selected;
	private int currentSelect = 0;

	private float timer = 0;
	private const float cooldown = 0.5f;

	private Vector3 enableScale = new Vector3(0.001f, 0.001f, 0.001f);
	private bool isPrevHand = true;
	[HideInInspector]
	public bool viewing = false;

	void Start()
	{
		GetComponent<RectTransform>().localScale = Vector3.zero;
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if (!viewing && GestureHandler.leftGrabClicked && !isInsideAnyBlock() && StatusRecord.tool != StatusRecord.ControllerStatus.Menu) {
			isPrevHand = StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl;
			buttonTool.GetComponent<Image>().sprite = isPrevHand ? drawTool : handTool;
			if (StatusRecord.switchToRay()) {
				GetComponent<RectTransform>().localScale = enableScale;
				currentSelect = 0;
				viewing = true;
			}
		}
		else if (viewing) {
			if (timer > cooldown && (axisValue.axis.x != 0 || axisValue.axis.y != 0)) {
				if (Mathf.Abs(axisValue.axis.x) > Mathf.Abs(axisValue.axis.y)) {
					if (axisValue.axis.x > 0) {
						currentSelect += 1;
						if (currentSelect == row) {
							currentSelect = 0;
						}
						if (currentSelect == row * 2) {
							currentSelect = row;
						}
					}
					else {
						currentSelect -= 1;
						if (currentSelect == -1) {
							currentSelect = row - 1;
						}
						if (currentSelect == row - 1) {
							currentSelect = row * 2 - 1;
						}
					}
				}
				else {
					if (axisValue.axis.y > 0) {
						currentSelect += row;
						currentSelect = currentSelect % (row * 2);
					}
					else {
						currentSelect += (row * 3);
						currentSelect = currentSelect % (row * 2);
					}
				}
				timer = 0;
				currentSelect = currentSelect < num - 1 ? currentSelect : num - 1;
			}
			selected.GetComponent<RectTransform>().anchoredPosition = new Vector2(-330 + 220 * (currentSelect % row), 110 - 220 * (currentSelect / row));
			if (GestureHandler.leftGrabClicked || GestureHandler.rightGrabClicked) {
				closeMenu(isPrevHand);
			}
			if ((GestureHandler.leftTriggerClicked || GestureHandler.rightTriggerClicked) && timer > cooldown) {
				if (currentSelect == 0) {
					closeMenu(!isPrevHand);
				}
				else {
					menus[currentSelect - 1].openMenu(isPrevHand);
					GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
					viewing = false;
				}
			}
		}

	}
	
	public void openMenu(bool isHand) {
		isPrevHand = isHand;
		GetComponent<RectTransform>().localScale = enableScale;
		currentSelect = 0;
		viewing = true;
		timer = 0;
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
