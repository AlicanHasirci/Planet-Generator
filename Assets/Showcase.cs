using UnityEngine;
using System.Collections;

public class Showcase : MonoBehaviour {

	void Update () {
		this.transform.Rotate(15 * Time.deltaTime,15 * Time.deltaTime,15 * Time.deltaTime);
	}
}
