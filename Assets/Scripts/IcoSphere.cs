using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using URandom = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class IcoSphere : MonoBehaviour {

	public int recursionLevel = 3;
	public float radius = 1f;
	public int randomize = 10;
	public int seed;
	private Mesh mesh;

	public List<Vector3> vertList = new List<Vector3>();
	public List<Triangle> faces = new List<Triangle>();
	public Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

	public void Create() {
		mesh = GetMesh ();
		mesh.Clear ();
		vertList.Clear();
		faces.Clear();
		middlePointIndexCache.Clear();

		CreateIcoSphere ();
		RefineTriangles ();
		FindNeighbouringTriangles ();
		Reconstruct ();
		PopulateMesh ();
	}

	private void CreateIcoSphere () {
		float t = (1f + Mathf.Sqrt(5f)) / 2f;

		vertList.Add(new Vector3(-1f,  t,  0f).normalized * radius);
		vertList.Add(new Vector3( 1f,  t,  0f).normalized * radius);
		vertList.Add(new Vector3(-1f, -t,  0f).normalized * radius);
		vertList.Add(new Vector3( 1f, -t,  0f).normalized * radius);

		vertList.Add(new Vector3( 0f, -1f,  t).normalized * radius);
		vertList.Add(new Vector3( 0f,  1f,  t).normalized * radius);
		vertList.Add(new Vector3( 0f, -1f, -t).normalized * radius);
		vertList.Add(new Vector3( 0f,  1f, -t).normalized * radius);

		vertList.Add(new Vector3( t,  0f, -1f).normalized * radius);
		vertList.Add(new Vector3( t,  0f,  1f).normalized * radius);
		vertList.Add(new Vector3(-t,  0f, -1f).normalized * radius);
		vertList.Add(new Vector3(-t,  0f,  1f).normalized * radius);

		faces.Add(new Triangle(0, 11, 5));
		faces.Add(new Triangle(0, 5, 1));
		faces.Add(new Triangle(0, 1, 7));
		faces.Add(new Triangle(0, 7, 10));
		faces.Add(new Triangle(0, 10, 11));

		faces.Add(new Triangle(1, 5, 9));
		faces.Add(new Triangle(5, 11, 4));
		faces.Add(new Triangle(11, 10, 2));
		faces.Add(new Triangle(10, 7, 6));
		faces.Add(new Triangle(7, 1, 8));

		faces.Add(new Triangle(3, 9, 4));
		faces.Add(new Triangle(3, 4, 2));
		faces.Add(new Triangle(3, 2, 6));
		faces.Add(new Triangle(3, 6, 8));
		faces.Add(new Triangle(3, 8, 9));

		faces.Add(new Triangle(4, 9, 5));
		faces.Add(new Triangle(2, 4, 11));
		faces.Add(new Triangle(6, 2, 10));
		faces.Add(new Triangle(8, 6, 7));
		faces.Add(new Triangle(9, 8, 1));
	}

	private void RefineTriangles () {
		for (int i = 0; i < recursionLevel; i++) {
			List<Triangle> fragments = new List<Triangle>();
			foreach (Triangle tri in faces) {
				int a = GetMiddlePoint(tri[0], tri[1]);
				int b = GetMiddlePoint(tri[1], tri[2]);
				int c = GetMiddlePoint(tri[2], tri[0]);

				fragments.Add(new Triangle(tri[0], a, c));
				fragments.Add(new Triangle(tri[1], b, a));
				fragments.Add(new Triangle(tri[2], c, b));
				fragments.Add(new Triangle(a, b, c));
			}
			faces = fragments;
		}
	}

	private void FindNeighbouringTriangles () {
		for (int i = 0; i < faces.Count - 1; i++) {
			Triangle t1 = faces[i];
			for (int j = i + 1; j < faces.Count; j++) {
				Triangle t2 = faces[j];
				if (t1.SharedIndices(t2).Count == 2) {
					Triangle.AddNeighbours(t1, t2);
				}
			}
		}
	}

	public void Reconstruct () {
		for (int i = 0; i < faces.Count; i++) {
			Triangle triangle = faces[URandom.Range(0, faces.Count)];
			Triangle neighbour = triangle.neighbours[URandom.Range(0, 2)];
            Triangle.Reconstruct(ref triangle, ref neighbour);
		}
	}

	private void PopulateMesh () {
		mesh.vertices = vertList.ToArray();

		List< int > triList = new List<int>();
		for( int i = 0; i < faces.Count; i++ ) {
			triList.Add( faces[i][0] );
			triList.Add( faces[i][1] );
			triList.Add( faces[i][2] );
		}

		mesh.triangles = triList.ToArray();
		mesh.uv = new Vector2[mesh.vertices.Length];

		Vector3[] normales = new Vector3[ vertList.Count];
		for( int i = 0; i < normales.Length; i++ )
			normales[i] = vertList[i].normalized;

		mesh.normals = normales;

		mesh.RecalculateBounds();
		mesh.Optimize();
	}


	#region Utility
	private int GetMiddlePoint(int p1, int p2) {
		bool firstIsSmaller = p1 < p2;
		long smallerIndex = firstIsSmaller ? p1 : p2;
		long greaterIndex = firstIsSmaller ? p2 : p1;
		long key = (smallerIndex << 32) + greaterIndex;

		int ret;
		if (middlePointIndexCache.TryGetValue(key, out ret)) {
			return ret;
		}

		Vector3 point1 = vertList[p1];
		Vector3 point2 = vertList[p2];
		Vector3 middle = new Vector3 (
			(point1.x + point2.x) / 2f, 
			(point1.y + point2.y) / 2f, 
			(point1.z + point2.z) / 2f
		);

		int i = vertList.Count;
		vertList.Add( middle.normalized * radius );
		middlePointIndexCache.Add(key, i);
		return i;
	}

	private Mesh GetMesh() {
		MeshFilter mf = this.GetComponent<MeshFilter>();
		#if UNITY_EDITOR
		if(mf.sharedMesh == null) mf.sharedMesh = new Mesh();
		return mf.sharedMesh;
		#else
		return mf.mesh;
		#endif
	}
	#endregion
}
