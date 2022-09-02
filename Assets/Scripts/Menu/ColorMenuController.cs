using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ColorMenuController : MonoBehaviour
{
	public SteamVR_Action_Vector2 axisValue;
	private BlockController currentBlock;

	public GameObject buttonPrefab;
	public GameObject selected;
	private GameObject[] buttons;
	public Material[] mats;
	public Sprite[] sprites;
	public static Material[] materials;

	private float timer = 0;
	private const float cooldown = 0.5f;
	[HideInInspector]
	public bool viewing = false;

	[HideInInspector]
	public int currentMat = 0;
	private Vector3 enableScale = new Vector3(0.001f, 0.001f, 0.001f);
	private int row = 5;
	private bool prevDestroy = false;
	void Start()
	{
		materials = mats;
		buttons = new GameObject[sprites.Length];
		for (int i=0;i<sprites.Length;i++) {
			buttons[i] = Instantiate(buttonPrefab, transform);
			buttons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-384 + 192 * (i % row), 96 - 192 * (i / row));
			buttons[i].GetComponent<Image>().sprite = sprites[i];
		}
		selected.transform.SetSiblingIndex(selected.transform.parent.childCount - 1);
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
		if (viewing && (GestureHandler.leftTriggerClicked || GestureHandler.rightTriggerClicked)) {
			switch (currentMat) {
				case 8:
					currentBlock.rotateBlock(true);
					break;
				case 9:
					currentBlock.deleteBlock(true);
					prevDestroy = true;
					break;
				default:
					currentBlock.changeMat(currentMat, true);
					break;
			}
		}
		if (viewing) {
			if (timer > cooldown && (axisValue.axis.x != 0 || axisValue.axis.y != 0)) {
				if (Mathf.Abs(axisValue.axis.x) > Mathf.Abs(axisValue.axis.y)) {
					if (axisValue.axis.x > 0) {
						currentMat += 1;
						if (currentMat == row) {
							currentMat = 0;
						}
						if (currentMat == row * 2) {
							currentMat = row;
						}
					}
					else {
						currentMat -= 1;
						if (currentMat == -1) {
							currentMat = row - 1;
						}
						if (currentMat == row - 1) {
							currentMat = row * 2 - 1;
						}
					}
				}
				else {
					if (axisValue.axis.y > 0) {
						currentMat += row;
						currentMat = currentMat % (row * 2);
					}
					else {
						currentMat += (row * 3);
						currentMat = currentMat % (row * 2);
					}
				}
				timer = 0;
				selected.GetComponent<RectTransform>().anchoredPosition = new Vector2(-384 + 192 * (currentMat % row), 96 - 192 * (currentMat / row));
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
