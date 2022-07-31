using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleHandle : MonoBehaviour
{
	public Transform positionHandle;
	public Transform block;
	public Transform synchroBlock;
	private BlockHandleController handle;
	private const float speed = 0.1f;
	private const float maxSpeed = 0.5f;
	private const float minScale = 0.05f;
	void Start()
	{
		handle = GetComponent<BlockHandleController>();
	}

	void Update()
	{
		if (handle.isTriggerDown) {
			Vector3 delta = transform.position - positionHandle.position - Vector3.one * 2.5f * transform.lossyScale.x;
			delta = delta.magnitude < maxSpeed ? delta : delta.normalized * maxSpeed;
			if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y) && Mathf.Abs(delta.x) >= Mathf.Abs(delta.z)) {
				delta = new Vector3(delta.x, 0, 0);
			}
			else if (Mathf.Abs(delta.y) >= Mathf.Abs(delta.x) && Mathf.Abs(delta.y) >= Mathf.Abs(delta.z)) {
				delta = new Vector3(0, delta.y, 0);
			}
			else if (Mathf.Abs(delta.z) >= Mathf.Abs(delta.x) && Mathf.Abs(delta.z) >= Mathf.Abs(delta.y)) {
				delta = new Vector3(0, 0, delta.z);
			}
			Vector3 newScale = block.localScale + delta * speed;
			newScale = new Vector3(newScale.x > minScale ? newScale.x : minScale, newScale.y > minScale ? newScale.y : minScale, newScale.z > minScale ? newScale.z : minScale);
			block.localScale = newScale;
			synchroBlock.localScale = block.localScale;
		}
		else {
			transform.position = positionHandle.position + Vector3.one * 2.5f * transform.lossyScale.x;
		}
	}
}
