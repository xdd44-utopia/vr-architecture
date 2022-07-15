using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushPointer : MonoBehaviour
{
	public Transform handTransform;
	// Update is called once per frame
	void Update()
	{
		transform.position = handTransform.position;
	}
}
