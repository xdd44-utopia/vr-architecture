using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusRecord : MonoBehaviour
{
	public enum ControllerStatus {
		Draw,
		BlockControl,
		Menu
	}
	public static ControllerStatus tool;
	private GameObject teleport;
	private static float timer = 0;
	private const float cooldown = 0.1f; 
	// Start is called before the first frame update
	void Start()
	{
		tool = ControllerStatus.BlockControl;
		teleport = GameObject.Find("Teleporting");
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		teleport.SetActive(tool != ControllerStatus.Menu);
	}

	public static bool switchToDraw() {
		if (timer > cooldown) {
			tool = ControllerStatus.Draw;
			timer = 0;
			return true;
		}
		return false;
	}
	
	public static void switchToDraw2() {
		if (timer > cooldown) {
			tool = ControllerStatus.Draw;
			timer = 0;
		}
	}

	public static bool switchToHand() {
		if (timer > cooldown) {
			tool = ControllerStatus.BlockControl;
			timer = 0;
			return true;
		}
		return false;
	}

	public static bool switchToRay() {
		if (timer > cooldown) {
			tool = ControllerStatus.Menu;
			timer = 0;
			return true;
		}
		return false;
	}
}
