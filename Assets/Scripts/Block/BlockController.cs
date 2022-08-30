using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{

	private ColorMenuController colorMenu;
	private GameObject highlight;

	//[HideInInspector]
	public Transform synchroBlock;
	private Transform blockTransform;
	public bool isPlane;
	private bool isReal;

	private Material originalMat;
	private Material invertedMat;
	private GameObject inverted;

	private enum Direction {
		x, y, z
	}
	private Direction scaleDir;
	private Vector3 initScale;

	[HideInInspector]
	public int currentMat;
	
	void Awake()
	{
		colorMenu = GameObject.Find("ColorMenu").GetComponent<ColorMenuController>();
		if (transform.parent.childCount > 1) {
			highlight = transform.parent.GetChild(1).gameObject;
		}

		blockTransform = transform.parent;

		isReal = blockTransform.parent == null;

		if (blockTransform.gameObject.name[blockTransform.gameObject.name.Length - 1] == ')' && isReal) {
			currentMat = Random.Range(0, ColorMenuController.materials.Length);
			inverted = Instantiate(this.gameObject, this.transform);
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

		}
		originalMat = GetComponent<Renderer>().material;
	}

	// Update is called once per frame
	void Update()
	{
		originalMat.color = new Color(originalMat.color.r, originalMat.color.g, originalMat.color.b, blockTransform.localScale.y > 2f ? 0.4f : 0.9f);
		GetComponent<Renderer>().material = originalMat;
		if (isReal) {
			invertedMat.color = new Color(originalMat.color.r, originalMat.color.g, originalMat.color.b, 0.4f);
			inverted.GetComponent<Renderer>().material = invertedMat;
		}

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
				(Mathf.Abs(GestureHandler.leftHandPos.z - GestureHandler.rightHandPos.z) > Mathf.Abs(GestureHandler.leftHandPos.y - GestureHandler.rightHandPos.y) &&
				Mathf.Abs(GestureHandler.leftHandPos.z - GestureHandler.rightHandPos.z) > Mathf.Abs(GestureHandler.leftHandPos.x - GestureHandler.rightHandPos.x)) || isPlane
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

		highlight.SetActive((leftHandInside() || rightHandInside()) && ((isReal && !headInside()) || !isReal));
		highlight.transform.localScale = transform.localScale * 1.01f;
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
			pos.x < blockTransform.position.x + 0.5f * blockTransform.localScale.x &&
			pos.x > blockTransform.position.x - 0.5f * blockTransform.localScale.x &&
			(
				(!isPlane &&
				pos.y < blockTransform.position.y + 0.5f * blockTransform.localScale.y &&
				pos.y > blockTransform.position.y - 0.5f * blockTransform.localScale.y) ||
				(isPlane &&
				pos.y < blockTransform.position.y + 0.2f &&
				pos.y > blockTransform.position.y - 0.2f)
			) &&
			pos.z < blockTransform.position.z + 0.5f * blockTransform.localScale.z &&
			pos.z > blockTransform.position.z - 0.5f * blockTransform.localScale.z) ||
			(!isReal &&
			pos.x < blockTransform.position.x + 0.05f * blockTransform.localScale.x &&
			pos.x > blockTransform.position.x - 0.05f * blockTransform.localScale.x &&
			(
				(!isPlane &&
				pos.y < blockTransform.position.y + 0.05f * blockTransform.localScale.y &&
				pos.y > blockTransform.position.y - 0.05f * blockTransform.localScale.y) ||
				(isPlane &&
				pos.y < blockTransform.position.y + 0.05f &&
				pos.y > blockTransform.position.y - 0.05f)
			) &&
			pos.z < blockTransform.position.z + 0.05f * blockTransform.localScale.z &&
			pos.z > blockTransform.position.z - 0.05f * blockTransform.localScale.z);
	}

	public void changeMat(int i, bool isActive) {
		if (i >= ColorMenuController.materials.Length) {
			return;
		}
		currentMat = i;
		originalMat = new Material(ColorMenuController.materials[currentMat]);
		GetComponent<Renderer>().material = originalMat;
		if (isReal) {
			invertedMat = new Material(originalMat);
			inverted.GetComponent<Renderer>().material = invertedMat;
		}
		if (isActive) {
			synchroBlock.GetChild(0).gameObject.GetComponent<BlockController>().changeMat(i, false);
		}
	}
}
