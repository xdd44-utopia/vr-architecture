using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ColorMenuControllerHifi : MonoBehaviour
{
	public SteamVR_Action_Vector2 axisValue;
	private BlockController currentBlock;

	public GameObject buttonPrefab;
	public GameObject selected;
	private GameObject[] buttons;
	public Sprite[] sprites;

	private float timer = 0;
	private const float cooldown = 0.5f;
	[HideInInspector]
	public bool viewing = false;

	[HideInInspector]
	public int currentMat = 0;
	private Vector3 enableScale = new Vector3(0.001f, 0.001f, 0.001f);
	private bool prevDestroy = false;
	void Start()
	{
		buttons = new GameObject[sprites.Length];
		for (int i=0;i<sprites.Length;i++) {
			buttons[i] = Instantiate(buttonPrefab, transform);
			buttons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-96 + 192 * i, 0);
			buttons[i].GetComponent<Image>().sprite = sprites[i];
		}
		selected.transform.SetSiblingIndex(selected.transform.parent.childCount - 1);
		selected.GetComponent<RectTransform>().anchoredPosition = new Vector2(-96, 0);
		GetComponent<RectTransform>().localScale = Vector3.zero;
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if ((viewing && (GestureHandler.leftGrabClicked || GestureHandler.rightGrabClicked)) || prevDestroy) {
			if (StatusRecord.switchToHand()) {
				GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
				viewing = false;
			}
			else {
				Debug.Log("Failed");
			}
			prevDestroy = false;
		}
		if (viewing && timer > cooldown && (GestureHandler.leftTriggerClicked || GestureHandler.rightTriggerClicked)) {
			if (currentMat == 0) {
				currentBlock.rotateBlock(true);
			}
			else {
				currentBlock.deleteBlock(true);
				prevDestroy = true;
			}
			timer = 0;
		}
		if (viewing) {
			if (timer > cooldown && axisValue.axis.x != 0) {
				if (axisValue.axis.x > 0) {
					currentMat += 1;
					currentMat %= buttons.Length;
				}
				else {
					currentMat += buttons.Length - 1;
					currentMat %= buttons.Length;
				}
				timer = 0;
				selected.GetComponent<RectTransform>().anchoredPosition = new Vector2(-96 + 192 * currentMat, 0);
			}
		}
	}

	public void enableMenu(BlockController block) {
		if (StatusRecord.switchToRay()) {
			currentBlock = block;
			currentMat = currentBlock.currentMat;
			GetComponent<RectTransform>().localScale = enableScale;
			viewing = true;
		}
	}
	
}
