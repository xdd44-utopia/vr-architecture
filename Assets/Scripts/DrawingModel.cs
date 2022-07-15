using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingModel : MonoBehaviour
{
	public GameObject drawingWorld;
	private LineRenderer lrWorld;
	private LineRenderer lrModel;
	void Start()
	{
		GameObject obj = Instantiate(drawingWorld, new Vector3(0, 0, 0), Quaternion.identity);
		lrWorld = obj.GetComponent<LineRenderer>();
		lrWorld.positionCount = 0;
		lrModel = GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		while (lrWorld.positionCount < lrModel.positionCount) {
			lrWorld.positionCount += 1;
			lrWorld.SetPosition(lrWorld.positionCount - 1, lrModel.GetPosition(lrWorld.positionCount - 1));
		}
	}
}
