using UnityEngine;

namespace Utils {

	public class MouseCameraControl : MonoBehaviour {
		public enum MouseButton { Left = 0, Right = 1, Middle = 2, None = 3 }
		
		[System.Serializable]
		// Handles left modifiers keys (Alt, Ctrl, Shift)
		public class Modifiers {
			public bool leftAlt;
			public bool leftControl;
			public bool leftShift;
			
			public bool checkModifiers() {
				return (!leftAlt ^ Input.GetKey(KeyCode.LeftAlt)) &&
					(!leftControl ^ Input.GetKey(KeyCode.LeftControl)) &&
						(!leftShift ^ Input.GetKey(KeyCode.LeftShift));
			}
		}
		
		[System.Serializable]
		// Handles common parameters for translations and rotations
		public class MouseControlConfiguration {
			public bool activate;
			public MouseButton mouseButton;
			public Modifiers modifiers;
			public float sensitivity;
			
			public bool isActivated() {
				return activate && Input.GetMouseButton((int)mouseButton) && modifiers.checkModifiers();
			}
		}
		
		[System.Serializable]
		// Handles scroll parameters
		public class MouseScrollConfiguration {
			public bool activate;
			public Modifiers modifiers;
			public float sensitivity;
			
			public bool isActivated() {
				return activate && modifiers.checkModifiers();
			}
		}
		
		// Yaw default configuration
		public MouseControlConfiguration yaw = new MouseControlConfiguration { mouseButton = MouseButton.Right, sensitivity = 10F };
		
		// Pitch default configuration
		public MouseControlConfiguration pitch = new MouseControlConfiguration { mouseButton = MouseButton.Right, modifiers = new Modifiers{ leftControl = true }, sensitivity = 10F };
		
		// Roll default configuration
		public MouseControlConfiguration roll = new MouseControlConfiguration();
		
		// Vertical translation default configuration
		public MouseControlConfiguration verticalTranslation = new MouseControlConfiguration { mouseButton = MouseButton.Middle, sensitivity = 2F };
		
		// Horizontal translation default configuration
		public MouseControlConfiguration horizontalTranslation = new MouseControlConfiguration { mouseButton = MouseButton.Middle, sensitivity = 2F };
		
		// Depth (forward/backward) translation default configuration
		public MouseControlConfiguration depthTranslation = new MouseControlConfiguration { mouseButton = MouseButton.Left, sensitivity = 2F };
		
		// Scroll default configuration
		public MouseScrollConfiguration scroll = new MouseScrollConfiguration { sensitivity = 2F };
		
		// Default unity names for mouse axes
		public string mouseHorizontalAxisName = "Mouse X";
		public string mouseVerticalAxisName = "Mouse Y";
		public string scrollAxisName = "Mouse ScrollWheel";
		
		void LateUpdate () {
			if (yaw.isActivated()) {
				float rotationX = Input.GetAxis(mouseHorizontalAxisName) * yaw.sensitivity;
				transform.Rotate(0, rotationX, 0);
			}

			if (pitch.isActivated()) {
				float rotationY = Input.GetAxis(mouseVerticalAxisName) * pitch.sensitivity;
				transform.Rotate(-rotationY, 0, 0);
			}

			if (roll.isActivated()) {
				float rotationZ = Input.GetAxis(mouseHorizontalAxisName) * roll.sensitivity;
				transform.Rotate(0, 0, rotationZ);
			}
			
			if (verticalTranslation.isActivated()) {
				// float translateY = Input.GetAxis(mouseVerticalAxisName) * verticalTranslation.sensitivity;
				// transform.Translate(0, translateY, 0);

				var pos = transform.position;
				var offset = transform.up * Input.GetAxis(mouseVerticalAxisName) * verticalTranslation.sensitivity;
				transform.position = pos + offset;
			}
			
			if (horizontalTranslation.isActivated()) {
				var pos = transform.position;
				var offset = transform.right * Input.GetAxis(mouseHorizontalAxisName) * horizontalTranslation.sensitivity;
				transform.position = pos + offset;
			}
			
			if (depthTranslation.isActivated()) {
				float translateZ = Input.GetAxis(mouseVerticalAxisName) * depthTranslation.sensitivity;
				transform.Translate(0, 0, translateZ);
			}
			
			if (scroll.isActivated()) {
				float translateZ = Input.GetAxis(scrollAxisName) * scroll.sensitivity;
				transform.Translate(0, 0, translateZ);
			}
		}
		
	}

}
