 /// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class IKDummyLookAt : MonoBehaviour {
	
	protected Animator avatar;
	
	public bool ikActive = false;

	public Transform lookAtObj = null;
	public Camera cam;

	public float lookAtWeight = 1.0f;
	
	// Use this for initialization
	void Start () 
	{
		avatar = GetComponent<Animator>();
	}

	void OnGUI()
	{

		//GUILayout.Label("Activate IK and move the Effectors around in Scene View");
		//ikActive = GUILayout.Toggle(ikActive, "Activate IK");
	}
		
    
	void OnAnimatorIK(int layerIndex)
	{		
		if(avatar)
		{
			if(ikActive)
			{
				
				
				avatar.SetLookAtWeight(lookAtWeight,0.3f,0.6f,1.0f,0.5f);


				if(lookAtObj != null)
				{
					cam.transform.LookAt(lookAtObj.position);
				}				
			}

		}
	}  
}
