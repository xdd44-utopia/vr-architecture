using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButtonController : MonoBehaviour
{
	public int colorIndex;
	private ColorMenuController colorMenu;
	void Start()
	{
		colorMenu = GameObject.Find("ColorMenu").GetComponent<ColorMenuController>();
	}

	// Update is called once per frame
	void Update()
	{
		GetComponent<Button>().interactable = colorIndex != colorMenu.currentColor;
	}

	public void changeColor() {
		colorMenu.changeColor(colorIndex);
	}
}
