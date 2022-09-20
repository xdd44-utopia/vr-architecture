using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SpawnMenuController : MonoBehaviour
{
	public SteamVR_Action_Vector2 axisValue;

	public BlockSpawner blockSpawner;
	private ToolMenuController menu;
	
	public int num;
	
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
		menu = GameObject.Find("[FOR HI-FI] ToolMenu-HiFi").GetComponent<ToolMenuController>();
		GetComponent<RectTransform>().localScale = Vector3.zero;
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if (viewing) {
			if (timer > cooldown && axisValue.axis.x != 0) {
				if (axisValue.axis.x > 0) {
					currentSelect = (currentSelect + 1) % num;
				}
				else {
					currentSelect = (currentSelect + num - 1) % num;
				}
				timer = 0;
			}
			selected.GetComponent<RectTransform>().anchoredPosition = new Vector2(-440 + 220 * currentSelect, 0);
			if (GestureHandler.leftGrabClicked || GestureHandler.rightGrabClicked) {
				closeMenu(isPrevHand);
			}
			if ((GestureHandler.leftTriggerClicked || GestureHandler.rightTriggerClicked) && timer > cooldown) {
				if (currentSelect == 0) {
					GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
					viewing = false;
					menu.openMenu(isPrevHand);
				}
				else {
					blockSpawner.spawn(currentSelect - 1);
					closeMenu(true);
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

}
