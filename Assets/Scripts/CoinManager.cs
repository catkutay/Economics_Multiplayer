using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using System;

public class CoinManager : NetworkBehaviour
{

	public GameObject[] effort;
	public GameObject[] resource;
	bool isPressed = false;

	//should be updated onj server and client;
	[SyncVar (hook = "updateCoins")] public int currentCoins;

	public Material clear;
	public bool _isLocalPlayer = false;
	public int maxCoins = 19;
	[SyncVar] public bool result;
	public bool isFinished=false;
	public PlayerNetworkSetup player=null;
	public int boxCount;
	GameManager gameManager;
	// Use this for initialization
	void Start ()
	{
		//Debug.Log ("Start");
		for (int i = maxCoins; i >= 0; i--) {
			effort [i].SetActive (false);
			resource [i].SetActive (true);
		}
		currentCoins = 0;
		gameManager = GameObject.Find ("NetworkManager").GetComponent<GameManager> ();
			boxCount =gameManager.boxCount;
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (_isLocalPlayer){
		//Debug.Log (currentCoins);
		HandlePlayerInput ();

		//woudl like to do once

	
		}
	}

	void HandlePlayerInput ()
	{
		//increment and decrement coin number on local box
		//set by callback in Playernetworkcontroller

		//Debug.Log ("Is Local");
		if (Input.GetKeyUp (KeyCode.W)) {
				
			if (currentCoins >= 0 && currentCoins <= maxCoins) {
					

				currentCoins++;
				//Debug.Log (currentCoins);
			}

		}

		if (Input.GetKeyUp (KeyCode.S) && currentCoins > 0) {
			currentCoins--;

			

		}
		if (Input.GetAxis ("D-PadX") == 1.0f | Input.GetAxis ("D-PadX") == -1.0f) {
			//how to say finished
			isFinished = true;

			
		}
		if (Input.GetAxis ("D-PadY") > 0.0f && isPressed == false) {
				

			if (currentCoins <= maxCoins) {
				currentCoins++;
				//Debug.Log (currentCoins);
			}

			isPressed = true;
		}

		if (Input.GetAxis ("D-PadY") < 0.0f && isPressed == false && currentCoins > 1) {
			currentCoins--;

			
			isPressed = true;

		}

		if (Input.GetAxis ("D-PadY") == 0.0f) {
			isPressed = false;
			
		}
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetAxis ("A") == -1f) {
				

				isFinished = true;
			}
			//send to server
			
		if(_isLocalPlayer)player.Cmd_Update_Coins(boxCount, currentCoins, result);
	

	}
	//enact server update on coins
	//void FixedUpdate(){
//		if(_isLocalPlayer)
//		updateCoins (currentCoins);
///	}
	void updateCoins(int _currentCoins){
		{
			currentCoins = _currentCoins;
			if (_isLocalPlayer) {
				
				if (currentCoins >= 0 & currentCoins<=maxCoins) {
					for (int i = maxCoins; i >= currentCoins; i--) {
			
						effort [i].SetActive (false);
						resource [i].SetActive (true);
					}

					for (int i = 0; i < currentCoins; i++) {
						if (result) {
							effort [i].SetActive (false);
						} else
							effort [i].SetActive (true);
			
						resource [i].SetActive (false);
					}
				}
			}
		}
	}


	public void SetToClear ()
	{
		if (GetComponent<MeshRenderer> ().material != clear)
			GetComponent<MeshRenderer> ().material = clear;
	}

	void OnGUI ()
	{
		//GUILayout.Label(currentCoins.ToString());
	}


}
