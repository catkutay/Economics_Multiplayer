using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.VR;

public class SimpleMouseLook : MonoBehaviour
{
	// Using system mouselook behaviour with constraints which operate relative to
	// this gameobject's initial rotation.
	// Only rotates around local X and Y.
	// Works in local coordinates, so if this object is parented
	// to another moving gameobject, its local constraints will
	// operate correctly
	// (Think: looking out the side window of a car, or a gun turret
	// on a moving spaceship with a limited angular range)
	// to have no constraints on an axis, set the rotationRange to 360 or greater.

	//following not public as set here
	 Vector2 rotationRange = new Vector3 (100, 100);
	 float rotationSpeed = 10;
	 float dampingTime = 0.2f;

	bool relative = true;

	//use hips as pivot
	public Transform hip_pivot;
	private Vector3 m_TargetAngles;
	private Vector3 m_FollowAngles;
	private Vector3 m_FollowVelocity;
	private Quaternion m_OriginalRotation;
	public Vector3 dist;
	public GameObject cam;
	public float hitdist;
	//set in PlayerNewtorkController
	public bool _isLocalPlayer;

	void Start ()
	{
		m_OriginalRotation = transform.localRotation;

		//default to mixamo object - if not assigned
		if (hip_pivot == null)
			hip_pivot = transform.parent.parent.Find ("mixamorig:Hips").transform;
		//retain distance To Character
		dist = (transform.position - hip_pivot.position / 10);
		hitdist = dist.magnitude;
	}


	void LateUpdate ()
	{
		
		if (VRSettings.enabled == false && _isLocalPlayer) {// && Input.GetMouseButtonDown(0)) {
			// we make initial calculations from the original local rotation
			transform.localRotation = m_OriginalRotation;

			// read input from mouse or mobile controls
			float inputH;
			float inputV;
			if (relative) {
				inputH = CrossPlatformInputManager.GetAxis ("Mouse X");
				inputV = CrossPlatformInputManager.GetAxis ("Mouse Y");

				// wrap values to avoid springing quickly the wrong way from positive to negative
				if (m_TargetAngles.y > 180) {
					m_TargetAngles.y -= 360;
					m_FollowAngles.y -= 360;
				}
				if (m_TargetAngles.x > 180) {
					m_TargetAngles.x -= 360;
					m_FollowAngles.x -= 360;
				}
				if (m_TargetAngles.y < -180) {
					m_TargetAngles.y += 360;
					m_FollowAngles.y += 360;
				}
				if (m_TargetAngles.x < -180) {
					m_TargetAngles.x += 360;
					m_FollowAngles.x += 360;
				}


				// with mouse input, we have direct control with no springback required.
				m_TargetAngles.y += inputH * rotationSpeed;
				m_TargetAngles.x += inputV * rotationSpeed;


				// clamp values to allowed range
				m_TargetAngles.y = Mathf.Clamp (m_TargetAngles.y, -rotationRange.y * 0.5f, rotationRange.y * 0.5f);
				m_TargetAngles.x = Mathf.Clamp (m_TargetAngles.x, -rotationRange.x * 0.5f, rotationRange.x * 0.5f);
			} else {
				inputH = Input.mousePosition.x;
				inputV = Input.mousePosition.y;

				// set values to allowed range
				m_TargetAngles.y = Mathf.Lerp (-rotationRange.y * 0.5f, rotationRange.y * 0.5f, inputH / Screen.width);
				m_TargetAngles.x = Mathf.Lerp (-rotationRange.x * 0.5f, rotationRange.x * 0.5f, inputV / Screen.height);
			}

			// smoothly interpolate current values to target angles
			m_FollowAngles = Vector3.SmoothDamp (m_FollowAngles, m_TargetAngles, ref m_FollowVelocity, dampingTime);
			Quaternion angle = m_OriginalRotation * Quaternion.Euler (-m_FollowAngles.x, m_FollowAngles.y, m_FollowAngles.z);
		
			//Vector3 lookPos = angle * dist;
		
			cam.transform.localRotation = angle;
			// update the actual gameobject's rotation

			//move trasnform to camera view
			transform.position = transform.position + cam.transform.forward * hitdist * Time.deltaTime;

		}
	}
}

