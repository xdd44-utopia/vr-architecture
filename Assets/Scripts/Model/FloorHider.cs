using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorHider : MonoBehaviour
{
	private Transform playerPos;
	private MeshRenderer mr;
	// Start is called before the first frame update
	void Start()
	{
		mr = GetComponent<MeshRenderer>();
		playerPos = GameObject.Find("Player").transform;
	}

	// Update is called once per frame
	void Update()
	{
		mr.enabled = playerPos.position.y > 2;
	}
}
