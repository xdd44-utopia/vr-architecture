using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHandle : MonoBehaviour
{
	public Transform block;
	public Transform synchroBlock;
	private BlockHandleController handle;
	void Start()
	{
		handle = GetComponent<BlockHandleController>();
	}

	void Update()
	{
		if (handle.isTriggerDown) {
			block.position = transform.position;
			synchroBlock.localPosition = block.transform.localPosition;
		}
	}
}
