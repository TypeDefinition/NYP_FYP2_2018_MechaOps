using UnityEngine;
using System.Collections;

public class AxisAlignedBillboard : MonoBehaviour {

	public enum BillboardAxis {
		X,
		Y,
		Z,
	}

	[SerializeField] public BillboardAxis m_BillboardAxis = BillboardAxis.Y;
    [SerializeField] public bool m_ReverseDirection = false;

    private Camera m_MainCamera = null;

    // Use this for initialization
    void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
        m_MainCamera = Camera.main;
        // It is okay to not have a camera.
        if (m_MainCamera == null) {
            Debug.Log("asdassd");
            return;
		}

		Vector3 direction = m_MainCamera.transform.position - gameObject.transform.position;
        if (m_ReverseDirection)
        {
            direction = -direction;
        }
		Quaternion rotation = Quaternion.LookRotation(direction);
		Vector3 rotationEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

		switch (m_BillboardAxis) {
			case BillboardAxis.X:
                rotationEulerAngles.x = rotation.eulerAngles.x;
				break;
			case BillboardAxis.Y:
                rotationEulerAngles.y = rotation.eulerAngles.y;
				break;
			case BillboardAxis.Z:
                rotationEulerAngles.z = rotation.eulerAngles.z;
				break;
			default:
				print(gameObject.name + " Invalid Billboard Axis!");
				break;
		}
        
        rotation.eulerAngles = rotationEulerAngles;
		gameObject.transform.rotation = rotation;
	}
}