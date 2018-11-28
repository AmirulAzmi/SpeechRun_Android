using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") {
			//ScoreManager.Instance.SaveScore ();
			//GameObject go = GameObject.Find ("GameOver");
			//go.GetComponent<Transform> ().GetChild (0).gameObject.SetActive (true);
			PlayerController.Instance.Dead ();
		}
	}
}
