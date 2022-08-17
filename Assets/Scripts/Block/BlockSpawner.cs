using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BlockSpawner : MonoBehaviour
{
	public GameObject block;
	public Transform smallModel;
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		// if (GestureHandler.leftGrabClicked) {
		// 	spawn();
		// 	Destroy(this);
		// }
	}

	public void spawn() {
		GameObject realObject = Instantiate(block, GestureHandler.leftHandPos, Quaternion.identity);
		GameObject smallObject = Instantiate(block, smallModel);
		realObject.transform.GetChild(0).gameObject.GetComponent<BlockController>().synchroBlock = smallObject.transform;
		smallObject.transform.GetChild(0).gameObject.GetComponent<BlockController>().synchroBlock = realObject.transform;
		smallObject.transform.localPosition = realObject.transform.localPosition;
		smallObject.transform.localScale = realObject.transform.localScale;
		Destroy(this);
	}
}
