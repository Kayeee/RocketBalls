using UnityEngine;
using System.Collections;

public class Belt : MonoBehaviour {
	
	[SerializeField]
	float speed = 2;
	[SerializeField]
	int layerDetect = 8;

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == layerDetect) {
			other.transform.position += Time.deltaTime * speed * transform.forward;
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.layer == layerDetect) {
			other.transform.position += Time.deltaTime * speed * transform.forward;
		}
	}
}
