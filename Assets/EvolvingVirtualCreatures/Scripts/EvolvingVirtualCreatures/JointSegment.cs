using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace mattatz.EvolvingVirtualCreatures {

	public class JointSegment : Part, Effector {

		public List<JointSegment> Segments { get { return segments; } }

		public float[] Sensor { 
			get {
				var angles = transform.localRotation.eulerAngles / 360f;
				return new float[] {
					angles.x, angles.y, angles.z, 
					front.Contact01, back.Contact01, left.Contact01, right.Contact01, up.Contact01, down.Contact01
				};
			} 
		}

		public Rigidbody Body {
			get {
				if(body == null) {
					body = GetComponent<Rigidbody>();
				}
				return body;
			}
		}

		protected Rigidbody body;
		protected CharacterJoint joint;
		protected SideType side;
		[SerializeField] protected Vector2 forceRange = new Vector2(-100f, 100f);
        protected float axisForce, swingAxisForce;
		protected List<JointSegment> segments = new List<JointSegment>();

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
			segments.ForEach(s => {
				s.Init (this);
			});
		}

		public void Init (JointSegment parent) {
			Body.velocity *= 0f;
			Body.angularVelocity *= 0f;
			Body.mass = transform.localScale.magnitude;
			// transform.position = parent.transform.position + Helper.directions[side] * 1.25f;

			var dir = Helper.directions[side];
			var po = Vector3.Scale (dir, parent.transform.localScale) * 0.5f;
			var lo = Vector3.Scale (dir, transform.localScale) * 0.5f;
			var offset = po + lo + lo * 0.25f;
			transform.position = parent.transform.position + offset;

			// need to add HingeJoint after setting position
			if(this.joint == null) {
				ActivateJoint(parent, side);
			}

			Init ();
		}

		public void ActivateJoint (JointSegment parent, SideType side) {
			var joint = CreateJoint();
			joint.connectedBody = parent.Body;
			joint.axis = Helper.directions[Helper.Axis(side)];
			joint.swingAxis = Helper.directions[Helper.SwingAxis(side)];
			joint.anchor = Helper.directions[Helper.Inverse(side)] * 0.5f;
		}

		public void Connect (JointSegment parent, SideType side) {
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
			if(joint == null) return;
			Body.AddRelativeTorque(joint.axis * axisForce);
			Body.AddRelativeTorque(joint.swingAxis * swingAxisForce);
        }

		public void AddSegment (JointSegment segment) {
			segments.Add(segment);
		}

		public int InputCount() {
			return 2;
		}

		public void Affect(float[] inputs, float dt) {
			if(joint == null) return;

			// joint.axis;
			// var axisLimit = joint.swing1Limit;
			// transform.RotateAround(joint.connectedAnchor, joint.axis, Mathf.Lerp(joint.lowTwistLimit.limit, joint.highTwistLimit.limit, inputs[0]));

            axisForce = Mathf.Lerp (forceRange.x, forceRange.y, inputs[0]);
            swingAxisForce = Mathf.Lerp (forceRange.x, forceRange.y, inputs[1]);
		}

	}

}

