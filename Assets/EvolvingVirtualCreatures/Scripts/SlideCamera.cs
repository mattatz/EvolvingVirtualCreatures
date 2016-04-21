using UnityEngine;
using System.Collections;

public class SlideCamera : MonoBehaviour {

	[SerializeField] float speed = 1f;
	[SerializeField] Vector3 from = Vector3.left;
	[SerializeField] Vector3 to = Vector3.right;

	Vector3 origin;

	void Start () {
		origin = transform.position;
	}
	
	void Update () {
	}

	void FixedUpdate () {
		float t = Mathf.Sin(Time.timeSinceLevelLoad * speed);
		t = (t + 1f) * 0.5f;
		transform.position = Vector3.Lerp(origin + from, origin + to, t);
	}

}
