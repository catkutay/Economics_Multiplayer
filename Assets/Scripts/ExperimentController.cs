using UnityEngine;
using System.Collections;
using System;
using UnityEngine.VR;
using SimpleJSON;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

public class ExperimentController : NetworkBehaviour
{
	//set in playernetworkcontroller
	bool updating;
	public int participant_id;
	public int participant;

	//find in game
	public TextFileReader textFileReader;

	//move hand to enter coins - effector from Playernetworksetup

	public bool enter;
	public Transform button;
	[SyncVar]public bool ikActive;
	[SyncVar]public int round_id;
	public float transPos;
	public ExperimentController[] tokenBoxes;
	bool start=true;

	//first run of answer etc
	bool update = true;
	string url;


	public enum runState
	{
		start,
		wait,
		ask,
		answer,
		result,
		end}

	;


	public bool isHost;
	public bool waiting;

	//public PlayerNetworkSetup setupBox;
	public GameManager gameManager;
	public CoinManager coinManager;

	public int boxCount;
	public Text canvasText;

	string oldMessage;

	//set in playernetworksetup to create network authority
	public ParticipantController participantController;
	public ExperimentNetworking experimentNetworking;
	public bool _isLocalPlayer;

	//moving hands
	float tableHeight = -0.1f;
	Transform leftHandEffector;
	Transform rightHandEffector;
	Vector3 target;
	//Shared from host
	//[HideInInspector]

	[SyncVar] public int stage_number = 0;
	[SyncVar]public runState mode = runState.start;
	//stage number when result recorded
	int resultStage = 0;
	//public ExperimentController[] tokenBoxes;
	IKBody ikBody = null;
	//fixme
	string result;

	// Use this for initialization
	void Start ()
	{
		//start at stage 0


		update = true;

		coinManager = (CoinManager)GetComponent<CoinManager> ();

	
		mode = runState.start;
	
	}
	void setupHost ()
	{

		gameManager = GameObject.Find ("NetworkManager").GetComponent<GameManager> ();
		//reduce error in scene setup
		gameManager = GameObject.Find ("NetworkManager").GetComponent<GameManager> ();
		boxCount = gameManager.boxCount;
		textFileReader = GameObject.Find ("NetworkManager").GetComponent<TextFileReader> ();
		button = transform.Find ("Capsule");
		experimentNetworking = GetComponent<ExperimentNetworking> ();

		if (isHost && tokenBoxes.Length == 0) {
			List <ExperimentController> list = new List<ExperimentController> ();
			foreach (GameObject box in gameManager.tokenBoxes) {

				list.Add (box.GetComponent<ExperimentController> ());
			}
			tokenBoxes = list.ToArray ();

			//Debug.Log(gameManager);
		}
		try{
		//update mode and message - player setupin PlayerNetwork
		coinManager.player.Cmd_GetMode(boxCount);
		start = false;
		}catch{}
	}



	void Update ()
	{
		//start at stage 0
		if( start)setupHost();
		//wait for updates from api
		round_id = gameManager.round_id;
	
		if (experimentNetworking.urlReturn && _isLocalPlayer) {
			
			// url calls in rest of update do not work
			if (isHost) {
				//find next stage and message

				updateMove ();
				//message etc is send on Command on server and to all players
				//the syncvar to localplayer
			}


		

			if (coinManager.player && mode != runState.start) {
				//set from the server syncvar
				leftHandEffector = coinManager.player.transform.GetComponent<IKBody> ().leftHandObj;
				rightHandEffector = coinManager.player.transform.GetComponent<IKBody> ().rightHandObj;

				if (ikBody != null & coinManager.player != null) {
					ikBody.ikActive = ikActive;

				} else
					//get component if null
					ikBody = coinManager.player.gameObject.GetComponent<IKBody> ();
			}
		
			switch (mode) {
			case runState.start:
					//show this at start

				coinManager.result = false;
				canvasText.text = "You will contribute effort in the form of coins";
			
				break;

			case runState.wait:
					//wait for next step - need signal that all have done it from api - FIXME?

				if (coinManager.isFinished) {
					waiting = true;
					coinManager.isFinished = false;
				}
					//arbitrary - set hands on table
				try {
					Vector3 pos = leftHandEffector.position;
					pos.y = tableHeight;
					leftHandEffector.position = pos;
					pos = rightHandEffector.position;
					pos.y = tableHeight;
					rightHandEffector.position = pos;
					//get rotation of box
					target = button.transform.position;
					leftHandEffector.rotation = coinManager.gameObject.transform.parent.rotation * Quaternion.Euler (-18f, -15f, 40f);
					rightHandEffector.rotation = coinManager.gameObject.transform.parent.rotation * Quaternion.Euler (-18f, -15f, -40f);
				} catch {
				}
					//don't change message so not rewritten
				break;
			case runState.ask:
					//enable ik and sync from participantcontroller when sitting
					//ikActive = true;
					//send to server then SyncVar
				if (ikActive) {
					if (coinManager.player != null)
						coinManager.player.Cmd_ikActive (boxCount, true);

					//set up to change coins
					coinManager._isLocalPlayer = true;
					//need to add rotation of box - eg 90 deg
					target = button.transform.position;
					leftHandEffector.rotation = coinManager.gameObject.transform.parent.rotation * Quaternion.Euler (-18f, -15f, 40f);


					leftHandEffector.transform.position = target;
					button.GetComponent<ClearButton> ()._isLocalPlayer = true;
					//send coin number	
					canvasText.text = experimentNetworking.message + " You have selected " + coinManager.currentCoins + " coins";
					if (coinManager.isFinished & _isLocalPlayer) {
						round_id = gameManager.round_id;
						//cannot enter anymore

						button.GetComponent<ClearButton> ()._isLocalPlayer = false;
						resultStage = stage_number;
						//send in result to ZTree
						canvasText.text = "Wait for others to finish";

						//enter result
						url = textFileReader.IP_Address + "/experiments/results?experiment_id=" + textFileReader.experiment_id + "&stage_number=" + stage_number + "&participant_id=" + participant_id + "&round_id=" + round_id.ToString () + "&name=CoinEffort&value=" + coinManager.currentCoins;
						StartCoroutine (experimentNetworking.FetchStage (url, "", "", mode));
						//not playing anymore
						ikActive = false;
						if (coinManager.player != null)
							coinManager.player.Cmd_ikActive (boxCount, false);
						url = "";
						mode = runState.wait;
						//cannot use button
						button.GetComponent<ClearButton> ().SetToClear (false);
					}
				}
				break;

			case runState.answer:
					//call for result once per participant
					//coinManager.result = true; - set after collect result
				if (update) {
					//get result for previous stage for each participant
					url = textFileReader.IP_Address + "/experiments/results?experiment_id=" + textFileReader.experiment_id + "&stage_number=" + (resultStage) + "&round_id=" + round_id.ToString () + "&name=Result&participant_id=" + participant_id;
					
					StartCoroutine (experimentNetworking.FetchResults (url, "Results", "", mode));

				}
				//Debug.LogWarning (experimentNetworking.message);
				if (experimentNetworking.contrib > -20){
					canvasText.text = experimentNetworking.message + " " + experimentNetworking.contrib.ToString ();
				experimentNetworking.contrib = -100;
					update = false;
				}
						//FIXME should go to wait, but get message change
						//mode = runState.wait;
					

				break;
			case runState.end:
				//in case get moved to next stage too soon
				 if (experimentNetworking.total > 0) {
					string resultMessage = experimentNetworking.message + " Your contribution was:"+experimentNetworking.contrib.ToString ()+"Your payoff: "+experimentNetworking.payoff.ToString ()+"Total return: " + experimentNetworking.total.ToString ();
					Debug.LogWarning (resultMessage);
					StartCoroutine (resultShow (resultMessage));
					//reset to stop
					experimentNetworking.total = -100;
					coinManager.player.Cmd_Clear();
					coinManager.player.Rpc_Clear();
					//experimentNetworking.message = "";
				}
					//gameManager.boxCount = -1;
				if(!updating)participantController.mode = ParticipantController.modes.stand;

				break;
			}


		
			//update effort until end
			if (mode != runState.end & mode != runState.answer) {

				//works when syncvar gives new broadcast message to player
				//	Debug.LogWarning("here");
				//put here so overwrite with local message
				if (oldMessage != experimentNetworking.message & !experimentNetworking.message.Equals ("") & !(experimentNetworking.message == "")) {
					canvasText.text = experimentNetworking.message;
					oldMessage = experimentNetworking.message;
				}
		
			}
		}

	}

	IEnumerator resultShow (string _resultMessage)
	{
		//make sure see return message before final result and before player stands
		updating=true;
		yield return StartCoroutine (WaitForSeconds (20f));
		//wait before send result
	
		canvasText.text = _resultMessage;
		updating=false;
		yield return updating;


	}



	void updateMove ()
	{
		//runs on host localplayer only
		//do not broadcast unless change as some are in waiting
		int _stage_number = stage_number;
	

		//returned data from url
		if (experimentNetworking.urlReturn) {

			if (mode != runState.end) {
				
				url = textFileReader.IP_Address + "/experiments/stages?experiment_id=" + textFileReader.experiment_id + "&stage_number=" + stage_number + "&round_id=" + round_id.ToString ();
				//Debug.Log (url);

				StartCoroutine (experimentNetworking.FetchStage (url, "type_stage", "stage_number", mode));
				url = "";

				//move on to next one
				if (experimentNetworking.returnInt > 0)
					stage_number = experimentNetworking.returnInt;
				
				if (experimentNetworking.returnString == "Request") {
					

					mode = runState.ask;

				} else if (experimentNetworking.returnString == "Response") {
					

					mode = runState.answer;


				} else if (experimentNetworking.returnString == "Wait") {

					mode = runState.wait;

				} else if (experimentNetworking.returnString == "End") {

					mode = runState.end;
					update = true;

				}

			}


			try {

				//fixMe is this a problem??
				if (_stage_number != stage_number & coinManager.player) {
					coinManager.player.Cmd_change_currentStage (stage_number, mode);
				
				}
			} catch {
			}

			//checks message update on networking class if message for mode is new one
		
			experimentNetworking.callUpdate ();

		}
	}

	IEnumerator WaitForSeconds (float num)
	{

		yield return new WaitForSeconds (num);

	}



}