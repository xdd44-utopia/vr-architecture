using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ObjectController : MonoBehaviour
{
	public SteamVR_Action_Boolean TriggerShow;
	public SteamVR_Input_Sources handType;
	private bool showImage = false;
	// Start is called before the first frame update
	void Start()
	{
		TriggerShow.AddOnStateDownListener(triggerDown, handType);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
	public void triggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		showImage = !showImage;
	}
}
