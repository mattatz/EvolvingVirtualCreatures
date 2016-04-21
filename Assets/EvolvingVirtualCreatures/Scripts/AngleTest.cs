using UnityEngine;
using System.Collections;

public class AngleTest : MonoBehaviour {

	public float Angle0 { 
		get {
			var parent = joint.connectedBody.transform;
			var center = parent.position;
			var anchor = transform.TransformPoint(joint.anchor);
			var dir = anchor - center;
			var axis = transform.TransformVector(joint.axis);
			return Vector3.Angle(dir, axis);
		} 
	}

	public float Angle1 { 
		get {
			var parent = joint.connectedBody.transform;
			var center = parent.position;
			var anchor = transform.TransformPoint(joint.anchor);
			var dir = anchor - center;
			var axis = transform.TransformVector(joint.swingAxis);
			return Vector3.Angle(dir, axis);
		} 
	}

	[SerializeField] CharacterJoint joint;

	void Start () {
	}
	
	void Update () {
		Debug.Log (Angle0);
	}

	void OnDrawGizmos () {
		var axis = transform.TransformVector(joint.axis);
		var swingAxis = transform.TransformVector(joint.swingAxis);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + axis);

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + swingAxis);

		var parent = joint.connectedBody.transform;
		var center = parent.position;
		var anchor = transform.TransformPoint(joint.anchor);
		var dir = anchor - center;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(anchor, anchor + dir.normalized);
	}

}
