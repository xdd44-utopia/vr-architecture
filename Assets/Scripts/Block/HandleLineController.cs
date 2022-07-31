using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleLineController : MonoBehaviour
{
	public Transform pos;
	public Transform scale;
	private LineRenderer line;
	void Start()
	{
		line = GetComponent<LineRenderer>();
	}
	void Update()
	{
		line.SetPosition(0, pos.position);
		line.SetPosition(1, scale.position);
		line.startWidth = 0.025f * transform.lossyScale.x;
		line.endWidth = 0.025f * transform.lossyScale.x;
	}
}
