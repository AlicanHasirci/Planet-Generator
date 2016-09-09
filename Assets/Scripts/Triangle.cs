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

	public int this[int i] {
		get {
			return indices[i];
		}
	}

	public List<int> SharedIndices (Triangle t2) {
		return indices.FindAll(i => t2.indices.Contains(i));
	}

	public static void AddNeighbours(Triangle t1, Triangle t2) {
		if ( !t1.neighbours.Contains(t2) ) t1.neighbours.Add(t2);
		if ( !t2.neighbours.Contains(t1) ) t2.neighbours.Add(t1);
	}

	public static void Reconstruct (ref Triangle t1, ref Triangle t2) {
		List<int> shared = t1.SharedIndices(t2);
		    
		int t1ExcIndex = t1.indices.FindIndex(i => !shared.Contains(i));
		int t2ExcIndex = t2.indices.FindIndex(i => !shared.Contains(i));
		int t1Exc = t1[t1ExcIndex];
		int t2Exc = t2[t2ExcIndex];
		int t1SwapIndex = t1.indices.FindIndex(i => i == shared[0]);
		int t2SwapIndex = t2.indices.FindIndex(i => i == shared[1]);
		t1.indices[t1SwapIndex] = t2Exc;
		t2.indices[t2SwapIndex] = t1Exc;

		Triangle t1Temp = t1;
		Triangle t2Temp = t2;
		int t1neighbourIndex = t1.neighbours.FindIndex(n => (!n.Equals(t2Temp) && n.indices.Contains(shared[0])));
		int t2neighbourIndex = t2.neighbours.FindIndex(n => (!n.Equals(t1Temp) && n.indices.Contains(shared[1])));
		Triangle t1OldNieghbour = t1.neighbours[t1neighbourIndex];
		Triangle t2OldNeighbour = t2.neighbours[t2neighbourIndex];
		t1.neighbours[t1neighbourIndex] = t2OldNeighbour;
		t2.neighbours[t2neighbourIndex] = t1OldNieghbour;
	}

	public override bool Equals (object obj) {
		if(obj is Triangle) {
			Triangle t = (Triangle) obj;
			return this[0] == t[0] && this[1] == t[1] && this[2] == t[2];
		} else {
			return base.Equals (obj);
		}
	}
}
