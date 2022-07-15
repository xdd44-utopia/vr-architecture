using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerIcon : MonoBehaviour
{
	public Transform player;
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		transform.localPosition = player.position;
		transform.localRotation = player.rotation;
	}
}
