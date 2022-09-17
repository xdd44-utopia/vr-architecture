using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlaneConstructor : MonoBehaviour
{
	public GameObject planePrefab;
	private Vector3[] line1;
	private Vector3[] line2;
	void Start()
	{
		
	}

	void Update() {
		if ((GestureHandler.leftTriggerClicked || GestureHandler.rightTriggerClicked) && line1 != null && line2 != null) {
			constructPlane();
			line1 = null;
			line2 = null;
			GameObject[] lines = GameObject.FindGameObjectsWithTag("DrawnLine");
			foreach (GameObject line in lines)
			{
				Destroy(line);
			}
		}
	}

	public void receiveLine(LineRenderer lr) {
		if (line2 != null) {
			line1 = new Vector3[line2.Length];
			for (int i=0;i<line2.Length;i++) {
				line1[i] = line2[i];
			}
		}
		line2 = new Vector3[lr.positionCount];
		lr.GetPositions(line2);
	}
	private void constructPlane() {
		Vector3[] vertices = new Vector3[line1.Length + line2.Length];
		Vector2[] uv = new Vector2[line1.Length + line2.Length];
		for (int i=0;i<line1.Length;i++) {
			vertices[i] = line1[i];
		}
		if ((line1[0] - line2[0]).magnitude < (line1[0] - line2[line2.Length - 1]).magnitude) {
			for (int i=0;i<line2.Length;i++) {
				vertices[i + line1.Length] = line2[i];
			}
		}
		else {
			for (int i=0;i<line2.Length;i++) {
				vertices[i + line1.Length] = line2[line2.Length - 1 - i];
			}
		}
		float maxX = vertices[0].x;
		float maxY = vertices[0].y;
		float maxZ = vertices[0].z;
		float minX = vertices[0].x;
		float minY = vertices[0].y;
		float minZ = vertices[0].z;
		for (int i=0;i<line1.Length + line2.Length;i++) {
			maxX = Mathf.Max(maxX, vertices[i].x);
			maxY = Mathf.Max(maxY, vertices[i].y);
			maxZ = Mathf.Max(maxZ, vertices[i].z);
			minX = Mathf.Min(minX, vertices[i].x);
			minY = Mathf.Min(minY, vertices[i].y);
			minZ = Mathf.Min(minZ, vertices[i].z);
		}
		Vector3 offset = new Vector3((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);
		for (int i=0;i<line1.Length + line2.Length;i++) {
			vertices[i] -= offset;
		}

		List<int> trianglesList = new List<int>();
		int pointer1 = 0;
		int pointer2 = line1.Length;
		bool flipped = Vector3.Cross(vertices[pointer2] - vertices[pointer1], vertices[line1.Length - 1] - vertices[pointer1]).y > 0;
		while (pointer1 < line1.Length - 1 || pointer2 < line1.Length + line2.Length - 1) {
			if ((pointer1 <= pointer2 - line1.Length || pointer2 == line1.Length + line2.Length - 1) && pointer1 < line1.Length - 1) {
				trianglesList.Add(pointer1);
				trianglesList.Add(flipped ? pointer2 : pointer1 + 1);
				trianglesList.Add(flipped ? pointer1 + 1 : pointer2);
				pointer1++;
			}
			else {
				trianglesList.Add(pointer2);
				trianglesList.Add(flipped ? pointer2 + 1 : pointer1);
				trianglesList.Add(flipped ? pointer1 : pointer2 + 1);
				pointer2++;
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = trianglesList.ToArray();
		mesh.MarkModified();
		mesh.RecalculateNormals();
		GameObject newPlane = Instantiate(planePrefab, offset, Quaternion.identity);
		newPlane.transform.GetChild(0).GetComponent<MeshFilter>().mesh = mesh;
		newPlane.transform.GetChild(1).GetComponent<MeshFilter>().mesh = mesh;
		newPlane.transform.GetChild(2).GetComponent<MeshFilter>().mesh = mesh;
		newPlane.transform.GetChild(2).GetComponent<MeshCollider>().sharedMesh = mesh;
		newPlane.transform.GetChild(0).GetComponent<BlockController>().calcBoundingBox();

	}
	
}
