using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
	[HideInInspector]
	public Transform synchroBlock;
	private Transform blockTransform;
	private bool isReal;
	
	void Awake()
	{
		blockTransform = transform.parent;
		isReal = blockTransform.parent == null;
		if (blockTransform.gameObject.name == "BlockContainer(Clone)" && isReal) {
			Material mat = new Material(GetComponent<Renderer>().material);
			mat.color = new Color(Random.Range(0.25f, 0.75f), Random.Range(0.25f, 0.75f), Random.Range(0.25f, 0.75f), 0.5f);
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
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (GestureHandler.leftTriggerPressed && !GestureHandler.rightTriggerPressed && leftHandInside() && !headInside()) {
			blockTransform.position += GestureHandler.leftHandDeltaPos;
			synchroBlock.transform.localPosition = blockTransform.localPosition;
		}
		if (GestureHandler.rightTriggerPressed && !GestureHandler.leftTriggerPressed && rightHandInside() && !headInside()) {
			blockTransform.position += GestureHandler.rightHandDeltaPos;
			synchroBlock.transform.localPosition = blockTransform.localPosition;
		}
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
}
