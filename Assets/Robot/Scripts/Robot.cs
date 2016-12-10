using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {

	[SerializeField]
	float speed = 2;
	[SerializeField]
	Transform playerTransform;
	[SerializeField]
	float rotateSpeed = 3;
	[SerializeField]
	GameObject[] bodyParts;
	[SerializeField]
	bool rotateRandomly = false;
	[SerializeField]
	float armMove = 1;
	[SerializeField]
	GameObject[] arms;
	[SerializeField]
	bool armMovement = true;
	[SerializeField]
	float playerDist = 5;

	void Start() {
		if(rotateRandomly == true)
			InvokeRepeating("rotate", 0, 5);
	}

	void Update () {
		StartCoroutine(moveArms());
		followPlayer();
	}

	void rotate() {
		foreach (GameObject part in bodyParts) {
			part.transform.Rotate(new Vector3(part.transform.rotation.eulerAngles.x + Random.value * rotateSpeed, 
				part.transform.rotation.eulerAngles.y + Random.value * rotateSpeed,
				part.transform.rotation.eulerAngles.z + Random.value * rotateSpeed));
		}
	}

	IEnumerator moveArms() {
		if (armMovement) {
			foreach (GameObject arm in arms) {
				arm.transform.position += armMove * arm.transform.forward;
				yield return new WaitForSeconds (0.1f);
				arm.transform.position -= armMove * arm.transform.forward;
			}
		}
	}

	void followPlayer() {
		Vector3 pos = Vector3.Lerp (transform.position, new Vector3(playerTransform.position.x+playerDist, playerTransform.position.y, playerTransform.position.z+playerDist), Time.deltaTime);
		pos.y = transform.position.y;
		transform.position = pos;
		transform.forward = (playerTransform.position - transform.position).normalized;
	}
}
