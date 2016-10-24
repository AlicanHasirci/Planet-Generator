using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using URandom = UnityEngine.Random;
using SysRandom = System.Random;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class IcoSphere : MonoBehaviour {

	public int recursionLevel = 3;
	[Range(0, 100)]
	public int randomness = 10;
	public Material subMaterial;
	public int seed;

	[HideInInspector]
	private List<Vector3> vertices = new List<Vector3>();
	private List<Polygon> polygons = new List<Polygon>();
	private List<Triangle> triangles = new List<Triangle>();
	public Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

	public List<Vector3> Vertices { get { return vertices; } }
	public List<Polygon> Polygons { get { return polygons; } }
	public List<Triangle> Triangles { get { return triangles; } }

	public void Create() {
		vertices.Clear();
		triangles.Clear();
		middlePointIndexCache.Clear();

		CreateIcoSphere ();
		RefineTriangles ();
		FindNeighbouringTriangles ();
		CreatePolygons();
		Reconstruct ();
		RelatePolygons ();

		CalculateCenteroids();
		CreateMeshesFromPolygons();
	}

	private void CreateIcoSphere () {
		float t = (1f + Mathf.Sqrt(5f)) / 2f;

		vertices.Add(new Vector3(-1f,  t,  0f).normalized);
		vertices.Add(new Vector3( 1f,  t,  0f).normalized);
		vertices.Add(new Vector3(-1f, -t,  0f).normalized);
		vertices.Add(new Vector3( 1f, -t,  0f).normalized);

		vertices.Add(new Vector3( 0f, -1f,  t).normalized);
		vertices.Add(new Vector3( 0f,  1f,  t).normalized);
		vertices.Add(new Vector3( 0f, -1f, -t).normalized);
		vertices.Add(new Vector3( 0f,  1f, -t).normalized);

		vertices.Add(new Vector3( t,  0f, -1f).normalized);
		vertices.Add(new Vector3( t,  0f,  1f).normalized);
		vertices.Add(new Vector3(-t,  0f, -1f).normalized);
		vertices.Add(new Vector3(-t,  0f,  1f).normalized);

		triangles.Add(new Triangle(0, 11, 5));
		triangles.Add(new Triangle(0, 5, 1));
		triangles.Add(new Triangle(0, 1, 7));
		triangles.Add(new Triangle(0, 7, 10));
		triangles.Add(new Triangle(0, 10, 11));

		triangles.Add(new Triangle(1, 5, 9));
		triangles.Add(new Triangle(5, 11, 4));
		triangles.Add(new Triangle(11, 10, 2));
		triangles.Add(new Triangle(10, 7, 6));
		triangles.Add(new Triangle(7, 1, 8));

		triangles.Add(new Triangle(3, 9, 4));
		triangles.Add(new Triangle(3, 4, 2));
		triangles.Add(new Triangle(3, 2, 6));
		triangles.Add(new Triangle(3, 6, 8));
		triangles.Add(new Triangle(3, 8, 9));

		triangles.Add(new Triangle(4, 9, 5));
		triangles.Add(new Triangle(2, 4, 11));
		triangles.Add(new Triangle(6, 2, 10));
		triangles.Add(new Triangle(8, 6, 7));
		triangles.Add(new Triangle(9, 8, 1));
	}

	private void RefineTriangles () {
		for (int i = 0; i < recursionLevel; i++) {
			List<Triangle> fragments = new List<Triangle>();
			foreach (Triangle tri in triangles) {
				int a = GetMiddlePoint(tri[0], tri[1]);
				int b = GetMiddlePoint(tri[1], tri[2]);
				int c = GetMiddlePoint(tri[2], tri[0]);

				fragments.Add(new Triangle(tri[0], a, c));
				fragments.Add(new Triangle(tri[1], b, a));
				fragments.Add(new Triangle(tri[2], c, b));
				fragments.Add(new Triangle(a, b, c));
			}
			triangles = fragments;
		}
	}

	private void FindNeighbouringTriangles () {
		for (int i = 0; i < triangles.Count - 1; i++) {
			Triangle t1 = triangles[i];
			for (int j = i + 1; j < triangles.Count; j++) {
				Triangle t2 = triangles[j];
				if (t1.SharedIndices(t2).Count == 2) {
					Triangle.AddNeighbours(t1, t2);
				}
			}
		}
	}

	private void CreatePolygons () {
		Polygon[] polygons = new Polygon [vertices.Count];
		foreach (Triangle t in triangles) {
			foreach (int index in t.indices) {
				if (polygons[index] == null) {
					polygons[index] = new Polygon (index);
				}
				polygons[index].AddTriangle(t);
			}
		}

		this.polygons.Clear();
		this.polygons.AddRange(polygons);
	}

	private void RelatePolygons () {
		foreach (Polygon p in polygons) {
			p.RelateTriangles();
		}
	}
		
	public void Reconstruct () {

		int rep = (int)(triangles.Count * (randomness * 0.01f));
		for (int i = 0; i < rep; i++) {
			int index = URandom.Range(0, triangles.Count);
			Triangle triangle = triangles[index];
			Triangle neighbour = triangle.neighbours[URandom.Range(0, 2)];

            Triangle.Perturb(ref triangle, ref neighbour);
		}
	}

	public void CalculateCenteroids () {
		foreach (Triangle t in triangles) {
			Vector3 v1 = vertices[t.indices[0]];
			Vector3 v2 = vertices[t.indices[1]];
			Vector3 v3 = vertices[t.indices[2]];
			Vector3 centeroid = new Vector3(
				(v1.x + v2.x + v3.x) / 3f,
				(v1.y + v2.y + v3.y) / 3f,
				(v1.z + v2.z + v3.z) / 3f
			);
			t.centeroid = centeroid.normalized;
		}
	}

	public void CreateMeshesFromPolygons () {
		foreach(Polygon p in polygons) {
			GameObject go = CreateGameObject(p);

			go.AddComponent<Showcase>();
		}
	}

	private GameObject CreateGameObject (Polygon p) {
		GameObject go = new GameObject();
		go.name = "Polygon#" + p.index;
		go.transform.SetParent(this.transform);
		MeshRenderer r = go.AddComponent<MeshRenderer>();
		r.sharedMaterial = subMaterial;
		MeshFilter f = go.AddComponent<MeshFilter>();

		Vector3 pivot = this.vertices[p.index];
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, Vector3.Normalize(pivot));

		go.transform.localPosition = pivot;
		go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, Vector3.Normalize(pivot));
		Matrix4x4 trs = Matrix4x4.TRS(
			Vector3.down,
			Quaternion.Inverse(rotation),
			Vector3.one
		);

		Mesh mesh = new Mesh();
		Vector3[] vertices = new Vector3[p.Triangles.Count + 1];
		Vector3[] normals = new Vector3[vertices.Length];
		int[] indices = new int[p.Triangles.Count * 3];
		vertices[0] = trs.MultiplyPoint3x4(this.vertices[p.index]);
		normals[0] = this.vertices[p.index];
		for (int i = 0, ii = 0; i < p.Triangles.Count; i++) {
			vertices[i + 1] = trs.MultiplyPoint3x4(p.Triangles[i].centeroid);
			normals[i + 1] = Vector3.Normalize(p.Triangles[i].centeroid);
			indices[ii++] = (i + 1).Mod(vertices.Length);
			indices[ii++] = (i + 2).Mod(vertices.Length);
			indices[ii++] = 0;
		}

		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.normals = normals;

		mesh.RecalculateBounds();
		mesh.Optimize();
		f.mesh = mesh;
		return go;
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

		Vector3 point1 = vertices[p1];
		Vector3 point2 = vertices[p2];
		Vector3 middle = new Vector3 (
			(point1.x + point2.x) / 2f, 
			(point1.y + point2.y) / 2f, 
			(point1.z + point2.z) / 2f
		);

		int i = vertices.Count;
		vertices.Add(middle.normalized);
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
