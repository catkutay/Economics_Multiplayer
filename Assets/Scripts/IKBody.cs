 /// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class IKBody : MonoBehaviour {
	
	protected Animator avatar;
	
	public bool ikActive = false;
	public Transform leftHandObj = null;
	public Transform rightHandObj = null;


	public float leftHandWeightPosition = 1;
	public float leftHandWeightRotation = 1;
	
	public float rightHandWeightPosition = 1;
	public float rightHandWeightRotation = 1;


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

				avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftHandWeightPosition);
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand,leftHandWeightRotation);
							
				avatar.SetIKPositionWeight(AvatarIKGoal.RightHand,rightHandWeightPosition);
				avatar.SetIKRotationWeight(AvatarIKGoal.RightHand,rightHandWeightRotation);

		
			
				if(leftHandObj != null)
				{
					avatar.SetIKPosition(AvatarIKGoal.LeftHand,leftHandObj.position);
					avatar.SetIKRotation(AvatarIKGoal.LeftHand,leftHandObj.rotation);
				}				
			
				if(rightHandObj != null)
				{
					avatar.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
					avatar.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
				}				
				
						
			}
			else
			{

				avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand,0);
							
				avatar.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				avatar.SetIKRotationWeight(AvatarIKGoal.RightHand,0);
				
			
				if(leftHandObj != null)
				{
					leftHandObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftHand);
					leftHandObj.rotation  = avatar.GetIKRotation(AvatarIKGoal.LeftHand);
				}				
				
				if(rightHandObj != null)
				{
					rightHandObj.position = avatar.GetIKPosition(AvatarIKGoal.RightHand);
					rightHandObj.rotation  = avatar.GetIKRotation(AvatarIKGoal.RightHand);
				}				
				
				
			
			}
		}
	}  
}
