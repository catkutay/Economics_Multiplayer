﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using System;

public class CoinManager : NetworkBehaviour
{

	public GameObject[] effort;
	public GameObject[] resource;
	bool isPressed = false;
	public  bool clearBox=false;
	//should be updated onj server and client;
	[SyncVar (hook = "updateCoins")] public int currentCoinsIdinArray=-1;

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
		clearBox=false;
		//clear effort coins
		for (int i = maxCoins; i >= 0; i--) {
			effort [i].SetActive (false);
			resource [i].SetActive (true);
		}
		currentCoinsIdinArray = -1;
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
		//update clear material
		if (clearBox )SetToClear();
	}

	void HandlePlayerInput ()
	{
		//increment and decrement coin number on local box
		//set by callback in Playernetworkcontroller

		//Debug.Log ("Is Local");
		if (Input.GetKeyUp (KeyCode.W) && currentCoinsIdinArray < maxCoins) {
				
		

				currentCoinsIdinArray++;
				//Debug.Log (currentCoins);


		}

		if (Input.GetKeyUp (KeyCode.S) && currentCoinsIdinArray >= 0) {
			currentCoinsIdinArray--;

			

		}
		if (Input.GetAxis ("D-PadX") == 1.0f | Input.GetAxis ("D-PadX") == -1.0f) {
			//how to say finished
			isFinished = true;

			
		}
		if (Input.GetAxis ("D-PadY") > 0.0f && isPressed == false) {
				

			if (currentCoinsIdinArray <= maxCoins) {
				currentCoinsIdinArray++;
		
			}

			isPressed = true;
		}

		if (Input.GetAxis ("D-PadY") < 0.0f && isPressed == false && currentCoinsIdinArray > 1) {
			currentCoinsIdinArray--;

			
			isPressed = true;

		}

		if (Input.GetAxis ("D-PadY") == 0.0f) {
			isPressed = false;
			
		}
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetAxis ("A") == -1f) {
				

				isFinished = true;
			}
			//send to server
			
		if(_isLocalPlayer)player.Cmd_Update_Coins(boxCount, currentCoinsIdinArray, result);
	

	}
	void updateCoins(int _currentCoins){
		{

			currentCoinsIdinArray = _currentCoins;
			if (_isLocalPlayer) {
				
				if (currentCoinsIdinArray >= -1 & currentCoinsIdinArray<=maxCoins) {
					for (int i = maxCoins; i > currentCoinsIdinArray; i--) {
			
						effort [i].SetActive (false);
						resource [i].SetActive (true);
					}

					for (int i = 0; i <= currentCoinsIdinArray; i++) {
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
		if ( GetComponent<MeshRenderer> ().material != clear){
			GetComponent<MeshRenderer> ().material = clear;
		}
	}

	void OnGUI ()
	{
		//GUILayout.Label(currentCoins.ToString());
	}


}
