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
	// Start is called before the first frame update
	void Start()
	{
		tool = ControllerStatus.BlockControl; 
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void switchToDraw() {
		tool = ControllerStatus.Draw;
	}

	public void switchToHand() {
		tool = ControllerStatus.BlockControl;
	}
}
