using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triangle {
	public List<int> indices;
	public List<Triangle> neighbours;

	public Triangle(int v1, int v2, int v3) {
		this.indices = new List<int>() {v1, v2, v3};
		this.neighbours = new List<Triangle>(3);
	}

	public List<int> SharedIndices (Triangle t2) {
		return indices.FindAll(i => t2.indices.Contains(i));
	}

	public int this[int i] {
		get {
			return indices[i];
		}
	}

	public static void AddNeighbours(Triangle t1, Triangle t2) {
		if ( !t1.neighbours.Contains(t2) ) t1.neighbours.Add(t2);
		if ( !t2.neighbours.Contains(t1) ) t2.neighbours.Add(t1);
	}

	public static void Reconstruct (ref Triangle t1, ref Triangle t2) {
		List<int> shared = t1.SharedIndices(t2);
		int a = t1.indices.FindIndex(i => !shared.Contains(i));
		int b = t2.indices.FindIndex(i => !shared.Contains(i));
		int t1Extra = t1[a];
		int t2Extra = t2[b];
		int t1Swap = t1.indices.FindIndex(i => i == shared[0]);
		int t2Swap = t2.indices.FindIndex(i => i == shared[1]);
		t1.indices[t1Swap] = t2Extra;
		t2.indices[t2Swap] = t1Extra;
	}
}
