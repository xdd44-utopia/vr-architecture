using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
	public EventSystem eventSystem;
	public StandaloneInputModule inputModule;

	private MeshRenderer hand;
	private MeshRenderer draw;
	// private float defaultLength = 3;
	
	void Start()
	{
		
	}

	void Awake() {
		hand = transform.GetChild(0).GetComponent<MeshRenderer>();
		draw = transform.GetChild(1).GetComponent<MeshRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		hand.enabled = StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl;
		draw.enabled = StatusRecord.tool == StatusRecord.ControllerStatus.Draw;
	}
	
}
