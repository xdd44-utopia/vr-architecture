using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
	
	void Awake()
	{
		if (transform.parent.gameObject.name == "BlockContainer(Clone)") {
			Material mat = new Material(GetComponent<Renderer>().material);
			mat.color = new Color(Random.Range(0.25f, 0.75f), Random.Range(0.25f, 0.75f), Random.Range(0.25f, 0.75f), 0.5f);
			GetComponent<Renderer>().material = mat;
			GameObject inverted = Instantiate(this.gameObject, this.transform);
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
		
	}
}
