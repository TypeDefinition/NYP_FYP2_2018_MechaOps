using UnityEngine;
using System.Collections;

//Did this with the help of http://wiki.unity3d.com/index.php?title=CameraFacingBillboard
public class Billboard : MonoBehaviour {

	public enum BillboardAxis { //Which axis do we use as our up axis.
		Up,
		Down,
		Left,
		Right,
		Forward,
		Backward,
	}

	public BillboardAxis m_BillboardUpAxis;
	public Camera m_MainCamera;
	public bool m_ReverseDirection;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (m_MainCamera == null) {
			if (Camera.main == null) {
				return;
			}
            m_MainCamera = Camera.main;
		}

		//This is the position we want to look at.
		Vector3 targetPosition;
		//When billboarding, the object need to have the same rotation as the camera, rather than just looking at the camera.
		//What I don't understand is why we are taking the camera's rotation and not the reverse of it.
		//If we want to face towards the camera, don't we need to face the opposite of where the camera is pointing?
		//EDIT: Alright, apparently this is cause Unity's Quad faces the wrong way.
		if (m_ReverseDirection) {
            targetPosition = transform.position + (m_MainCamera.transform.rotation * Vector3.back);
		} else {
            targetPosition = transform.position + (m_MainCamera.transform.rotation * Vector3.forward);
        }

		//Rotate our up axis by the quaternion.
		Vector3 targetOrientation = m_MainCamera.transform.rotation * GetAxis();
		gameObject.GetComponent<Transform>().LookAt(targetPosition, targetOrientation);
	}

	private Vector3 GetAxis() {
		switch (m_BillboardUpAxis) {
			case BillboardAxis.Up:
				return Vector3.up;
			case BillboardAxis.Down:
				return Vector3.down;
			case BillboardAxis.Left:
				return Vector3.left;
			case BillboardAxis.Right:
				return Vector3.right;
			case BillboardAxis.Forward:
				return Vector3.forward;
			case BillboardAxis.Backward:
				return Vector3.back;
			default:
				return new Vector3(0, 1, 0);
		}
	}

}