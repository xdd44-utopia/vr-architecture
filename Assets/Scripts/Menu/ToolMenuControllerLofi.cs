using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ToolMenuControllerLofi : MonoBehaviour
{
	public SteamVR_Action_Vector2 axisValue;

	public GameObject[] enables;
	public GameObject[] disables;

	private BlockSpawner blockSpawner;
	
	public int num;
	public int offset;
	private int row = 5;

	public GameObject[] buttons;
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
		blockSpawner = GameObject.Find("LofiSpawner").GetComponent<BlockSpawner>();
		foreach (GameObject go in enables) {
			go.SetActive(true);
		}
		foreach (GameObject go in disables) {
			go.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if (!viewing && GestureHandler.leftGrabClicked && !isInsideAnyBlock() && StatusRecord.tool != StatusRecord.ControllerStatus.Menu) {
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
			selected.GetComponent<RectTransform>().anchoredPosition = new Vector2(-440 + 220 * (currentSelect % row), 110 - 220 * (currentSelect / row));
			if (GestureHandler.leftGrabClicked || GestureHandler.rightGrabClicked) {
				closeMenu(isPrevHand);
			}
			if (GestureHandler.leftTriggerClicked || GestureHandler.rightTriggerClicked) {
				if (currentSelect == 0) {
					closeMenu(!isPrevHand);
				}
				else {
					blockSpawner.spawn(offset + currentSelect - 1);
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