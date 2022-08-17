using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{

	private ColorMenuController colorMenu;

	[HideInInspector]
	public Transform synchroBlock;
	private Transform blockTransform;
	private bool isReal;

	private Material originalMat;
	private Material invertedMat;

	private enum Direction {
		x, y, z
	}
	private Direction scaleDir;
	private Vector3 initScale;

	private Color[] colors = {
		Color.blue,
		Color.cyan,
		Color.gray,
		Color.green,
		Color.magenta,
		Color.red,
		Color.white,
		Color.yellow,
	};
	[HideInInspector]
	public int currentColor;
	
	void Awake()
	{
		colorMenu = GameObject.Find("ColorMenu").GetComponent<ColorMenuController>();
		for (int i=0;i<colors.Length;i++) {
			colors[i].a = 0.4f;
		}

		blockTransform = transform.parent;

		isReal = blockTransform.parent == null;

		if (blockTransform.gameObject.name == "BlockContainer(Clone)" && isReal) {
			Material mat = new Material(GetComponent<Renderer>().material);
			currentColor = Random.Range(0, colors.Length);
			mat.color = colors[currentColor];
			GetComponent<Renderer>().material = mat;
			GameObject inverted = Instantiate(this.gameObject, this.transform);
			Destroy(inverted.GetComponent<BlockController>());
			inverted.transform.localPosition = Vector3.zero;
			inverted.transform.localRotation = Quaternion.identity;
			inverted.transform.localScale = Vector3.one;
			foreach (Transform child in inverted.transform) {
				Destroy(child.gameObject);
			}

			Mesh mesh = inverted.GetComponent<MeshFilter>().mesh;
			int[] newTriangles = new int[mesh.triangles.Length];
			for (int i=0;i<newTriangles.Length;i++) {
				newTriangles[i] = mesh.triangles[i / 3 * 3 + (3 - i % 3) % 3];
			}
			mesh.triangles = newTriangles;
			mesh.MarkModified();
			mesh.RecalculateNormals();

			invertedMat = inverted.GetComponent<Renderer>().material;
		}
		originalMat = GetComponent<Renderer>().material;
	}

	// Update is called once per frame
	void Update()
	{
		originalMat.color = new Color(originalMat.color.r, originalMat.color.g, originalMat.color.b, blockTransform.localScale.y > 2f ? 0.4f : 0.8f);

		//Move

		if (GestureHandler.leftTriggerPressed && !GestureHandler.rightTriggerPressed && leftHandInside() && !headInside() && StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl) {
			blockTransform.position += GestureHandler.leftHandDeltaPos;
			synchroBlock.transform.localPosition = blockTransform.localPosition;
		}
		if (GestureHandler.rightTriggerPressed && !GestureHandler.leftTriggerPressed && rightHandInside() && !headInside() && StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl) {
			blockTransform.position += GestureHandler.rightHandDeltaPos;
			synchroBlock.transform.localPosition = blockTransform.localPosition;
		}

		//Scale

		if (
			((GestureHandler.leftTriggerDown && GestureHandler.rightTriggerDown) ||
			(GestureHandler.leftTriggerPressed && GestureHandler.rightTriggerDown) ||
			(GestureHandler.leftTriggerDown && GestureHandler.rightTriggerPressed)) &&
			leftHandInside() && rightHandInside() && !headInside() &&
			StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl
		) {
			if (
				Mathf.Abs(GestureHandler.leftHandPos.x - GestureHandler.rightHandPos.x) > Mathf.Abs(GestureHandler.leftHandPos.y - GestureHandler.rightHandPos.y) &&
				Mathf.Abs(GestureHandler.leftHandPos.x - GestureHandler.rightHandPos.x) > Mathf.Abs(GestureHandler.leftHandPos.z - GestureHandler.rightHandPos.z)
			) {
				scaleDir = Direction.x;
			}
			else if  (
				Mathf.Abs(GestureHandler.leftHandPos.z - GestureHandler.rightHandPos.z) > Mathf.Abs(GestureHandler.leftHandPos.y - GestureHandler.rightHandPos.y) &&
				Mathf.Abs(GestureHandler.leftHandPos.z - GestureHandler.rightHandPos.z) > Mathf.Abs(GestureHandler.leftHandPos.x - GestureHandler.rightHandPos.x)
			) {
				scaleDir = Direction.z;
			}
			else {
				scaleDir = Direction.y;
			}
			initScale = blockTransform.localScale;
		}
		if (GestureHandler.leftTriggerPressed && GestureHandler.rightTriggerPressed && leftHandInside() && rightHandInside() && !headInside() && StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl) {
			float scalar = 1;
			switch (scaleDir) {
				case Direction.x:
					scalar = (GestureHandler.leftHandPos.x - GestureHandler.rightHandPos.x) / (GestureHandler.leftHandInitPos.x - GestureHandler.rightHandInitPos.x);
					break;
				case Direction.y:
					scalar = (GestureHandler.leftHandPos.y - GestureHandler.rightHandPos.y) / (GestureHandler.leftHandInitPos.y - GestureHandler.rightHandInitPos.y);
					break;
				case Direction.z:
					scalar = (GestureHandler.leftHandPos.z - GestureHandler.rightHandPos.z) / (GestureHandler.leftHandInitPos.z - GestureHandler.rightHandInitPos.z);
					break;
			}
			scalar = Mathf.Clamp(scalar, 0.2f, 5f);
			blockTransform.localScale = new Vector3(
				scaleDir == Direction.x ? initScale.x * scalar : initScale.x,
				scaleDir == Direction.y ? initScale.y * scalar : initScale.y,
				scaleDir == Direction.z ? initScale.z * scalar : initScale.z
			);
			synchroBlock.transform.localScale = blockTransform.localScale;
		}

		//Menu

		if (canOpenMenu()) {
			if (StatusRecord.tool != StatusRecord.ControllerStatus.Menu) {
				colorMenu.enableMenu(this);
			}
		}

	}
	
	public bool canOpenMenu() {
		return ((GestureHandler.leftGrabClicked && leftHandInside()) || (GestureHandler.rightGrabClicked && rightHandInside())) && ((isReal && !headInside()) || !isReal);
	}

	private bool leftHandInside() {
		return isInside(GestureHandler.leftHandPos);
	}
	private bool rightHandInside() {
		return isInside(GestureHandler.rightHandPos);
	}
	private bool headInside() {
		return isInside(GestureHandler.headPos);
	}
	private bool isInside(Vector3 pos) {
		return
			(isReal &&
			pos.x < blockTransform.position.x + 0.5 * blockTransform.localScale.x &&
			pos.x > blockTransform.position.x - 0.5 * blockTransform.localScale.x &&
			pos.y < blockTransform.position.y + 0.5 * blockTransform.localScale.y &&
			pos.y > blockTransform.position.y - 0.5 * blockTransform.localScale.y &&
			pos.z < blockTransform.position.z + 0.5 * blockTransform.localScale.z &&
			pos.z > blockTransform.position.z - 0.5 * blockTransform.localScale.z) ||
			(!isReal &&
			pos.x < blockTransform.position.x + 0.05f &&
			pos.x > blockTransform.position.x - 0.05f &&
			pos.y < blockTransform.position.y + 0.05f &&
			pos.y > blockTransform.position.y - 0.05f &&
			pos.z < blockTransform.position.z + 0.05f &&
			pos.z > blockTransform.position.z - 0.05f);
	}

	public void changeColor(int i) {
		currentColor = i;
		if (i >= colors.Length) {
			return;
		}
		originalMat.color = colors[i];
		if (isReal) {
			invertedMat.color = colors[i];
			synchroBlock.gameObject.GetComponent<BlockController>().changeColor(i);
		}
	}
}
