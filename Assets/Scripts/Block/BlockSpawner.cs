using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BlockSpawner : MonoBehaviour
{
	public GameObject[] blocks;
	public Transform smallModel;
	public Transform head;
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

	public void spawn(int i) {
		if (i >= blocks.Length) {
			return;
		}
		GameObject realObject = Instantiate(blocks[i], GestureHandler.leftHandPos + head.forward, Quaternion.identity);
		GameObject smallObject = Instantiate(blocks[i], smallModel);
		realObject.transform.GetChild(0).gameObject.GetComponent<BlockController>().synchroBlock = smallObject.transform;
		smallObject.transform.GetChild(0).gameObject.GetComponent<BlockController>().synchroBlock = realObject.transform;
		smallObject.transform.localPosition = realObject.transform.localPosition;
		smallObject.transform.localScale = realObject.transform.localScale;
		realObject.transform.GetChild(0).gameObject.GetComponent<BlockController>().changeMat(realObject.transform.GetChild(0).gameObject.GetComponent<BlockController>().currentMat, true);
	}
}
