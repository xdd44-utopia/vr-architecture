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
	public GameObject[] buttons;
	public Sprite unselected;
	public Sprite selected;
	public Material[] mats;
	public static Material[] materials;

	private float timer = 0;
	private const float cooldown = 0.5f;
	private bool viewing = false;

	[HideInInspector]
	public int currentMat = -1;
	private Vector3 enableScale = new Vector3(0.001f, 0.001f, 0.001f);
	void Start()
	{
		materials = mats;
		buttons = new GameObject[materials.Length];
		for (int i=0;i<materials.Length;i++) {
			buttons[i] = Instantiate(buttonPrefab, transform);
			buttons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-288 + 192 * (i % 4), 96 - 192 * (i / 4));
			buttons[i].GetComponent<Image>().material = materials[i];
		}
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if (currentBlock != null) {
			currentMat = currentBlock.currentMat;
		}
		if (viewing && (GestureHandler.leftGrabClicked || GestureHandler.rightGrabClicked)) {
			if (StatusRecord.switchToHand()) {
				GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
				viewing = false;
			}
		}
		if (viewing) {
			for (int i=0;i<materials.Length;i++) {
				buttons[i].GetComponent<Image>().sprite = i == currentMat ? selected : unselected;
			}
			if (timer > cooldown && (axisValue.axis.x != 0 || axisValue.axis.y != 0)) {
				if (Mathf.Abs(axisValue.axis.x) > Mathf.Abs(axisValue.axis.y)) {
					if (axisValue.axis.x > 0) {
						currentMat += 1;
						if (currentMat == 4) {
							currentMat = 0;
						}
						if (currentMat == 8) {
							currentMat = 4;
						}
					}
					else {
						currentMat -= 1;
						if (currentMat == -1) {
							currentMat = 3;
						}
						if (currentMat == 3) {
							currentMat = 7;
						}
					}
				}
				else {
					if (axisValue.axis.y > 0) {
						currentMat += 4;
						currentMat = currentMat % 8;
					}
					else {
						currentMat += 12;
						currentMat = currentMat % 8;
					}
				}
				timer = 0;
				currentBlock.changeMat(currentMat, true);
			}
		}
	}

	public void enableMenu(BlockController block) {
		if (StatusRecord.switchToRay()) {
			currentBlock = block;
			GetComponent<RectTransform>().localScale = enableScale;
			viewing = true;
		}
	}
	
}
