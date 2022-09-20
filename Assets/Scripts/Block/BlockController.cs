using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
	private int blockID;

	private ColorMenuController colorMenu;
	private ColorMenuControllerHifi colorMenuHifi;
	private GameObject highlight;

	[HideInInspector]
	public Transform synchroBlock;
	private Transform blockTransform;
	private Rigidbody rb;
	private bool hasrb;
	private bool isReal;

	private Material originalMat;

	private float maxX;
	private float maxY;
	private float maxZ;

	private enum Direction {
		x, y, z
	}
	private Direction scaleDir;
	private Vector3 initScale;
	private bool isRotated = false;
	private bool hasMoved = false;
	private bool hasRotated = false;

	public enum BlockType {
		block,
		furniture,
		plane,
		wall,
		floor,
		drawing
	}
	public BlockType bt;

	[HideInInspector]
	public int currentMat;

	private bool isLofi;
	
	void Awake()
	{
		blockID = StatusRecord.blockCount;
		StatusRecord.blockCount++;

		if (bt != BlockType.furniture) {
			colorMenu = GameObject.Find("ColorMenu").GetComponent<ColorMenuController>();
		}
		else {
			colorMenuHifi = GameObject.Find("ColorMenu-HiFi").GetComponent<ColorMenuControllerHifi>();
		}
		highlight = transform.parent.GetChild(1).gameObject;

		blockTransform = transform.parent;

		isReal = blockTransform.parent == null;
		hasrb = isReal && (bt == BlockType.block || bt == BlockType.furniture);
		if (hasrb) {
			rb = blockTransform.gameObject.GetComponent<Rigidbody>();
		}
		else {
			Destroy(blockTransform.gameObject.GetComponent<MeshCollider>());
			Destroy(blockTransform.gameObject.GetComponent<Rigidbody>());
		}
		
		originalMat = GetComponent<Renderer>().material;
		
		calcBoundingBox();

		GameObject Lofi = GameObject.Find("[FOR LO-FI] ToolMenu-LoFi");
		isLofi = Lofi != null && Lofi.activeSelf;

	}

	// Update is called once per frame
	void Update()
	{

		if (bt != BlockType.furniture) {
			originalMat.color = new Color(originalMat.color.r, originalMat.color.g, originalMat.color.b, 0.9f);
			GetComponent<Renderer>().material = originalMat;
		}
		if (rb == null && bt != BlockType.drawing) {
			rb = synchroBlock.gameObject.GetComponent<Rigidbody>();
		}

		//Move

		hasMoved = false;
		Vector3 deltaPos = Vector3.zero;
		if (GestureHandler.leftTriggerPressed && !GestureHandler.rightTriggerPressed && leftHandInside() &&
			StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl &&
			(StatusRecord.currentBlock == blockID || StatusRecord.currentBlock == -1)
		) {
			deltaPos = GestureHandler.leftHandDeltaPos;
			StatusRecord.currentBlock = blockID;
			if (hasrb) {
				rb.isKinematic = true;
			}
			hasMoved = true;
		}
		else if (GestureHandler.rightTriggerPressed && !GestureHandler.leftTriggerPressed && rightHandInside() &&
			StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl &&
			(StatusRecord.currentBlock == blockID || StatusRecord.currentBlock == -1)
		) {
			deltaPos = GestureHandler.rightHandDeltaPos;
			StatusRecord.currentBlock = blockID;
			if (hasrb) {
				rb.isKinematic = true;
			}
			hasMoved = true;
		}
		else if (StatusRecord.currentBlock == blockID && !hasRotated) {
			if (hasrb) {
				rb.isKinematic = false;
			}
			StatusRecord.currentBlock = -1;
		}
		blockTransform.position += deltaPos;
		if (bt != BlockType.drawing) {
			synchroBlock.transform.localPosition = blockTransform.localPosition;
		}
		if (isReal) {
			switch (bt) {
				case BlockType.floor:
					if (!isLofi) {
						blockTransform.position = new Vector3(blockTransform.position.x, 3f, blockTransform.position.z);
						if (bt != BlockType.drawing) {
							synchroBlock.transform.localPosition = new Vector3(blockTransform.position.x, 3f, blockTransform.position.z);
						}
					}
					break;
				case BlockType.wall:
					if (!isLofi) {
						blockTransform.position = new Vector3(blockTransform.position.x, blockTransform.position.y < 3 ? 1.5f : 4.5f, blockTransform.position.z);
						if (bt != BlockType.drawing) {
							synchroBlock.transform.localPosition = new Vector3(blockTransform.position.x, blockTransform.position.y < 3 ? 1.5f : 4.5f, blockTransform.position.z);
						}
					}
					break;
			}
		}
		
		Vector3 leftInitPos = GestureHandler.leftHandInitPos;
		Vector3 rightInitPos = GestureHandler.rightHandInitPos;
		Vector3 leftPos = GestureHandler.leftHandPos;
		Vector3 rightPos = GestureHandler.rightHandPos;
		if (!isReal) {
			leftPos = blockTransform.InverseTransformDirection(leftPos);
			rightPos = blockTransform.InverseTransformDirection(rightPos);
			leftInitPos = blockTransform.InverseTransformDirection(leftInitPos);
			rightInitPos = blockTransform.InverseTransformDirection(rightInitPos);
		}

		//Scale
		hasRotated = false;
		if (
			((GestureHandler.leftTriggerDown && GestureHandler.rightTriggerDown) ||
			(GestureHandler.leftTriggerPressed && GestureHandler.rightTriggerDown) ||
			(GestureHandler.leftTriggerDown && GestureHandler.rightTriggerPressed)) &&
			(leftHandInside() || rightHandInside()) &&
			StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl
		) {
			if (bt == BlockType.wall) {
				scaleDir = Direction.z;
			}
			else if (
				Mathf.Abs(leftPos.x - rightPos.x) > Mathf.Abs(leftPos.y - rightPos.y) &&
				Mathf.Abs(leftPos.x - rightPos.x) > Mathf.Abs(leftPos.z - rightPos.z)
			) {
				scaleDir = !isReal ? Direction.x : !isRotated ? Direction.x : Direction.z;
			}
			else if  (
				(Mathf.Abs(leftPos.z - rightPos.z) > Mathf.Abs(leftPos.y - rightPos.y) &&
				Mathf.Abs(leftPos.z - rightPos.z) > Mathf.Abs(leftPos.x - rightPos.x)) ||
				bt == BlockType.plane || bt == BlockType.floor
			) {
				scaleDir = !isReal ? Direction.z : !isRotated ? Direction.z : Direction.x;
			}
			else {
				scaleDir = Direction.y;
			}
			initScale = blockTransform.localScale;
		}
		if (GestureHandler.leftTriggerPressed && GestureHandler.rightTriggerPressed && (leftHandInside() || rightHandInside()) &&
			StatusRecord.tool == StatusRecord.ControllerStatus.BlockControl &&
			(StatusRecord.currentBlock == blockID || StatusRecord.currentBlock == -1)
		) {
			float scalar = 1;
			switch (scaleDir) {
				case Direction.x:
					scalar = !isReal ? (leftPos.x - rightPos.x) / (leftInitPos.x - rightInitPos.x) :
						(!isRotated ?
						(leftPos.x - rightPos.x) / (leftInitPos.x - rightInitPos.x) :
						(leftPos.z - rightPos.z) / (leftInitPos.z - rightInitPos.z));
					break;
				case Direction.y:
					scalar = (leftPos.y - rightPos.y) / (leftInitPos.y - rightInitPos.y);
					break;
				case Direction.z:
					scalar = !isReal ? (leftPos.z - rightPos.z) / (leftInitPos.z - rightInitPos.z) :
						(!isRotated ?
						(leftPos.z - rightPos.z) / (leftInitPos.z - rightInitPos.z) :
						(leftPos.x - rightPos.x) / (leftInitPos.x - rightInitPos.x));
					break;
			}
			if (Single.IsNaN(scalar)) {
				scalar = 1;
			}
			scalar = Mathf.Clamp(scalar, 0.2f, 5f);
			float debugFloat = scalar;
			Vector3 debugVector1 = blockTransform.localScale;
			Vector3 debugVector2 = initScale;
			blockTransform.localScale = new Vector3(
				scaleDir == Direction.x ? initScale.x * scalar : initScale.x,
				scaleDir == Direction.y ? initScale.y * scalar : initScale.y,
				scaleDir == Direction.z ? initScale.z * scalar : initScale.z
			);
			if (bt != BlockType.drawing) {
				synchroBlock.transform.localScale = blockTransform.localScale;
			}
			StatusRecord.currentBlock = blockID;
			if (hasrb) {
				rb.isKinematic = true;
			}
		}
		else if (StatusRecord.currentBlock == blockID && !hasMoved) {
			StatusRecord.currentBlock = -1;
			if (hasrb) {
				rb.isKinematic = false;
			}
		}

		//Menu

		highlight.SetActive(leftHandInside() || rightHandInside());
		highlight.transform.localScale = transform.localScale * 1.01f;
		if (canOpenMenu()) {
			if (StatusRecord.tool != StatusRecord.ControllerStatus.Menu) {
				if (bt != BlockType.furniture) {
					colorMenu.enableMenu(this);
				}
				else {
					colorMenuHifi.enableMenu(this);
				}
			}
		}

	}
	
	public bool canOpenMenu() {
		return (GestureHandler.leftGrabClicked && leftHandInside()) || (GestureHandler.rightGrabClicked && rightHandInside());
	}

	private bool leftHandInside() {
		return isInside(GestureHandler.leftHandPos);
	}
	private bool rightHandInside() {
		return isInside(GestureHandler.rightHandPos);
	}
	private bool isInside(Vector3 pos) {
		pos = blockTransform.InverseTransformPoint(pos);
		return
			pos.x < maxX &&
			pos.x > - maxX &&
			pos.y < maxY &&
			pos.y > - maxY &&
			pos.z < maxZ &&
			pos.z > - maxZ;
	}

	public void calcBoundingBox() {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		maxX = mesh.vertices[0].x;
		maxY = mesh.vertices[0].y;
		maxZ = mesh.vertices[0].z;
		for (int i=0;i<mesh.vertices.Length;i++) {
			maxX = Mathf.Max(maxX, mesh.vertices[i].x);
			maxY = Mathf.Max(maxY, mesh.vertices[i].y);
			maxZ = Mathf.Max(maxZ, mesh.vertices[i].z);
		}
		switch (bt) {
			case BlockType.plane:
				maxY = isReal ? 0.2f : 1f;
				break;
			case BlockType.floor:
				maxY = isReal ? 0.2f : 1f;
				break;
			case BlockType.wall:
				maxX = isReal ? 0.2f : 1f;
				break;
		}
		// if (!isReal) {
		// 	maxX /= 10;
		// 	maxY /= 10;
		// 	maxZ /= 10;
		// }
		maxX *= 1.01f;
		maxY *= 1.01f;
		maxZ *= 1.01f;
	}

	public void changeMat(int i, bool isActive) {
		if (i >= ColorMenuController.materials.Length || bt == BlockType.furniture) {
			return;
		}
		currentMat = i;
		originalMat = new Material(ColorMenuController.materials[currentMat]);
		GetComponent<Renderer>().material = originalMat;
		if (isActive && bt != BlockType.drawing) {
			synchroBlock.GetChild(0).gameObject.GetComponent<BlockController>().changeMat(i, false);
		}
	}

	public void deleteBlock(bool isActive) {
		if (isActive && bt != BlockType.drawing) {
			synchroBlock.GetChild(0).gameObject.GetComponent<BlockController>().deleteBlock(false);
		}
		Destroy(transform.parent.gameObject);
	}

	public void rotateBlock(bool isActive) {
		if (isActive && bt != BlockType.drawing) {
			synchroBlock.GetChild(0).gameObject.GetComponent<BlockController>().rotateBlock(false);
		}
		transform.parent.localRotation = Quaternion.Euler(0, transform.parent.localRotation.eulerAngles.y + 90, 0);
		isRotated = !isRotated;
	}
}
