using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BlockSpawner : MonoBehaviour
{
	public SteamVR_Action_Boolean SpawnAction;
	public SteamVR_Input_Sources handType;
	public GameObject block;
	public Transform smallModel;
	private Transform pointer;
	// Start is called before the first frame update
	void Start()
	{
		SpawnAction.AddOnStateDownListener(triggerDown, handType);
	}

	// Update is called once per frame
	void Update()
	{

		if (pointer == null) {
			pointer = GameObject.Find("BrushPointer").transform;
		}

	}

	public void triggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
		GameObject realObject = Instantiate(block, pointer.position + pointer.forward, Quaternion.identity);
		GameObject smallObject = Instantiate(block, smallModel);
		smallObject.transform.localPosition = realObject.transform.localPosition;
		smallObject.transform.localScale = realObject.transform.localScale;
		realObject.transform.GetChild(1).GetComponent<PositionHandle>().synchroBlock = smallObject.transform;
		smallObject.transform.GetChild(1).GetComponent<PositionHandle>().synchroBlock = realObject.transform;
		realObject.transform.GetChild(2).GetComponent<ScaleHandle>().synchroBlock = smallObject.transform.GetChild(0).transform;
		smallObject.transform.GetChild(2).GetComponent<ScaleHandle>().synchroBlock = realObject.transform.GetChild(0).transform;
	}
}
