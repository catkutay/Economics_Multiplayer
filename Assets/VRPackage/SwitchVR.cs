using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class SwitchVR : MonoBehaviour
{


	
	// Update is called once per frame
	void Update ()
	{
		//If V is pressed, toggle VRSettings.enabled
		if (Input.GetKeyDown(KeyCode.V))
		{
			if(VRSettings.enabled == true)
			{
				VRSettings.enabled = false;
			}
			else
			{
				VRSettings.enabled = true;
			}
				
			Debug.Log("Changed VRSettings.enabled to:"+VRSettings.enabled);
		}
	}
}
