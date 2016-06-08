using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace mattatz.EvolvingVirtualCreatures {

	public class Segment : Part, Effector {

		public List<Segment> Segments { get { return segments; } }

		public float[] Sensor { 
			get {
				var angles = transform.localRotation.eulerAngles / 360f;
				return new float[] {
					angles.x, angles.y, angles.z, 
					front.Contact01, back.Contact01, left.Contact01, right.Contact01, up.Contact01, down.Contact01
				};
			} 
		}
		public Transform Anchor { get { return anchor; } }
		public bool Root { get { return root; } }

		protected CharacterJoint joint;
		protected SideType side;
		protected Vector2 forceRange = new Vector2(-100f, 100f);
        protected float axisForce, swingAxisForce;
		protected List<Segment> segments = new List<Segment>();

		protected bool root;
		protected Transform anchor;
		protected Vector3 axis;
		protected Vector3 secondaryAxis;

		[SerializeField] protected Vector2 axisRange = new Vector2(-90f, 90f);
		[SerializeField] protected Vector2 secondaryAxisRange = new Vector2(-90f, 90f);

		[SerializeField, Range(0f, 1f)] float axisAngle = 0.5f;
		[SerializeField, Range(0f, 1f)] float secondaryAxisAngle = 0.5f;

		float toAxisAngle, toSecondaryAxisAngle;

		public void SetRoot(bool flag) {
			if(flag) {
				var body = gameObject.AddComponent<Rigidbody>();
				body.mass = transform.localScale.magnitude;
				body.interpolation = RigidbodyInterpolation.Interpolate;
			}
			root = flag;
		}

		// usage of CharacterJoint
		// http://d.hatena.ne.jp/hidetobara/20111005/1317841046
		CharacterJoint CreateJoint (float limit = 120f) {
			joint = gameObject.AddComponent<CharacterJoint>();

			var highTwistLimit = joint.highTwistLimit;
			highTwistLimit.limit = limit;
			joint.highTwistLimit = highTwistLimit;

			var lowTwistLimit = joint.lowTwistLimit;
			lowTwistLimit.limit = -limit;
			joint.lowTwistLimit = lowTwistLimit;

            var swing1Limit = joint.swing1Limit;
            swing1Limit.limit = limit;
            joint.swing1Limit = swing1Limit;

            var swing2Limit = joint.swing2Limit;
            swing2Limit.limit = limit;
            joint.swing2Limit = swing2Limit;

			joint.enableCollision = true;
			// joint.enableCollision = false;
			joint.enablePreprocessing = false;

			return joint;
		}

		public void Init () {
			/*
			Body.velocity *= 0f;
			Body.angularVelocity *= 0f;
			*/
			segments.ForEach(s => {
				s.Init (this);
			});

			toAxisAngle = axisAngle;
			toSecondaryAxisAngle = secondaryAxisAngle;
		}

		public void Init (Segment parent) {
			// Body.velocity *= 0f;
			// Body.angularVelocity *= 0f;
			// transform.position = parent.transform.position + Helper.directions[side] * 1.25f;

			var dir = Helper.directions[side];

			// var po = Vector3.Scale (dir, parent.transform.localScale) * 0.5f;
			// var lo = Vector3.Scale (dir, transform.localScale) * 0.5f;
			// var offset = po + lo + lo * 0.25f;

			var offset = dir;

			axis = Helper.directions[Helper.Axis(side)];
			secondaryAxis = Helper.directions[Helper.SwingAxis(side)];

			anchor = (new GameObject("Anchor")).transform;
			if(parent.root) {
				anchor.parent = parent.transform;
			} else {
				anchor.parent = parent.Anchor;
			}

			// parent.transform.localPosition * 2f;

			if(parent.root) {
				anchor.localPosition = offset * 0.5f;
			} else {
				anchor.localPosition = parent.transform.localPosition + Vector3.Scale(parent.transform.localScale, offset) * 0.5f;
			}

			transform.parent = anchor;
			// transform.localPosition = Vector3.Scale(transform.localScale, offset) * 0.5f;
			transform.localPosition = Vector3.Scale(transform.localScale, offset) * 0.75f;

			// anchor = - offset * 0.5f;

			// need to add HingeJoint after setting position
			/*
			if(this.joint == null) {
				ActivateJoint(parent, side);
			}
			*/

			Init ();
		}

		/*
		public void ActivateJoint (Segment parent, SideType side) {
			var joint = CreateJoint();
			joint.connectedBody = parent.Body;
			joint.axis = Helper.directions[Helper.Axis(side)];
			joint.swingAxis = Helper.directions[Helper.SwingAxis(side)];
			joint.anchor = Helper.directions[Helper.Inverse(side)] * 0.5f;
		}
		*/

		public void Connect (Segment parent, SideType side) {
			this.side = side;
			Init (parent);
			parent.AddSegment(this);
		}

		public void Sensing (List<float> sensors) {
			var sensor = Sensor;
			for(int i = 0, n = sensor.Length; i < n; i++) {
				sensors.Add(sensor[i]);
			}
			Segments.ForEach(segment => {
				segment.Sensing(sensors);
			});
		}

		public void WakeUp () {
			// Body.isKinematic = false;
			Segments.ForEach(segment => {
				segment.WakeUp();
			});
		}

		public void Sleep () {
			// Body.isKinematic = true;
			Segments.ForEach(segment => {
				segment.Sleep();
			});
		}

        void FixedUpdate () {
			/*
			if(anchor != null) {
				anchor.localRotation = 
					Quaternion.AngleAxis(Mathf.Lerp (axisRange.x, axisRange.y, axisAngle), axis) * 
					Quaternion.AngleAxis(Mathf.Lerp (secondaryAxisRange.x, secondaryAxisRange.y, secondaryAxisAngle), secondaryAxis);
			}
			*/

			// if(joint == null) return;
			// Body.AddRelativeTorque(joint.axis * axisForce);
			// Body.AddRelativeTorque(joint.swingAxis * swingAxisForce);

			axisAngle = Mathf.Lerp (axisAngle, toAxisAngle, Time.fixedDeltaTime);
			secondaryAxisAngle = Mathf.Lerp (secondaryAxisAngle, toSecondaryAxisAngle, Time.fixedDeltaTime);
			Apply();
        }

		public void AddSegment (Segment segment) {
			segments.Add(segment);
		}

		public int InputCount() {
			return 2;
		}

		public void Affect(float[] inputs, float dt) {
			if(anchor != null) {
				// axisAngle = Mathf.Lerp(axisAngle, inputs[0], dt);
				// secondaryAxisAngle = Mathf.Lerp(secondaryAxisAngle, inputs[1], dt);

				toAxisAngle = inputs[0];
				toSecondaryAxisAngle = inputs[1];
			}

			// if(joint == null) return;

			// joint.axis;
			// var axisLimit = joint.swing1Limit;
			// transform.RotateAround(joint.connectedAnchor, joint.axis, Mathf.Lerp(joint.lowTwistLimit.limit, joint.highTwistLimit.limit, inputs[0]));

            // axisForce = Mathf.Lerp (forceRange.x, forceRange.y, inputs[0]);
            // swingAxisForce = Mathf.Lerp (forceRange.x, forceRange.y, inputs[1]);
		}

		void Apply () {
			if(anchor == null) return;

			anchor.localRotation = 
				Quaternion.AngleAxis(Mathf.Lerp (axisRange.x, axisRange.y, axisAngle), axis) * 
				Quaternion.AngleAxis(Mathf.Lerp (secondaryAxisRange.x, secondaryAxisRange.y, secondaryAxisAngle), secondaryAxis);
		}

		void OnDrawGizmosSelected () {
			if(anchor == null) return;
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(anchor.position, 0.2f);
		}

	}

}

