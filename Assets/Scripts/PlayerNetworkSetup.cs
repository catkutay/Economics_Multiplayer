using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;


using SimpleJSON;

public class PlayerNetworkSetup : NetworkBehaviour
{
	//setup common componentes for participants and run CMD under network control
	[SerializeField] public Camera FPCharacterCam;
	[SerializeField] AudioListener audioListener;
	public bool isHost;
	GameManager gameManager;
	float startHeight = -1.2f;
	CommonNetwork commonNetwork;
	ParticipantController participantController;

	public Transform spawnPoint;
	string returnParameter;
	public TextFileReader textFileReader;
	ExperimentController expController;

	GameObject tokenBox;
	int boxCount;
	Text canvasText;
	Canvas canvasgo;


	// Use this for initialization
	void Start ()
	{
		//have paricipant number from server  in setupServer
		gameManager = GameObject.Find ("NetworkManager").GetComponent<GameManager> ();
		commonNetwork = GameObject.Find ("NetworkManager").GetComponent<CommonNetwork> ();

		textFileReader = GameObject.Find ("NetworkManager").GetComponent<TextFileReader> ();
		//has been geenrated
		if (gameManager.boxCount > -2) {

			setup ();
		}
	}

	public void setup ()
	{


		//	Debug.LogWarning(isLocalPlayer);
		if (isLocalPlayer) {

			GameObject mainCamera = GameObject.Find ("Main Camera");
			if (mainCamera != null)
				mainCamera.SetActive (false);
			FPCharacterCam.gameObject.SetActive (true);
			FPCharacterCam.enabled = true;
			audioListener.enabled = true;
			//setup new characgter as player
		
		

			while (textFileReader.readHost == null) {
				//infact stays false as not playernetwork on host - FIXME
				isHost = false;

			}


			//random -FIXME
			if (spawnPoint)
				transform.position = spawnPoint.position;
			
			callBack ();
		}

	}



	void callBack ()
	{
		//when you have the participant number setup all the variables on participatncontroller to enable them to locate objects and select
		if (isLocalPlayer) {
			try {
				boxCount = gameManager.boxCount;
				///boxCount=1;
				tokenBox = gameManager.tokenBoxes [boxCount];
				tokenBox.GetComponent<CoinManager> ().player = this;

			} catch (Exception e) {
				if (gameManager.boxCount > 0) {
					if (canvasgo) {
						
						audioListener.enabled = false;
						canvasText.text = "You cannot enter this game. It is full or server is not started";
						NetworkManager networkManager = GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ();
						networkManager.StopClient ();
					}
					Debug.Log (e);
					return;
				}
			}
	

			//find local canvas inactive
			canvasgo = gameObject.GetComponentInChildren <Canvas> (true);
			if (canvasgo == null) {
				//above seems to not worksometimes
				try {
					canvasgo = transform.Find ("FPCharacterCam/Canvas").GetComponent <Canvas> ();
				} catch {
				}
			}
			if (canvasgo) {
				canvasgo.gameObject.SetActive (true);
				canvasText = canvasgo.transform.Find ("Text").gameObject.GetComponent<Text> ();
			}


			//this is a player so set up experiment controller
			if (canvasgo) {
				
				//if no canvas still on fpcontroller for experimenter
				participantController = gameObject.GetComponent<ParticipantController> ();

				participantController.participant = commonNetwork.participant;
				participantController = GetComponent<ParticipantController> ();
				participantController.box = tokenBox;
				participantController.canvasText = canvasText;
				participantController.coinManager = tokenBox.GetComponent<CoinManager> ();
				participantController.coinManager.boxCount = boxCount;
				participantController.participant_id = commonNetwork.participant_id;
				expController = tokenBox.GetComponent<ExperimentController> ();
				expController.participantController = participantController;
				expController.canvasText = canvasText;

				expController.boxCount = boxCount;
				//setup authority over coinbox
				//expController.player = gameObject;

				expController.button = tokenBox.transform.Find ("Capsule");

				//expController.setupBox = transform.GetComponent<PlayerNetworkSetup> ();
				expController.participant_id = commonNetwork.participant_id;
				expController.participant = commonNetwork.participant;
			
			
				//first two effectors on chairbox
				participantController.walkTarget = tokenBox.transform.parent.Find ("WalkTarget").gameObject;
				participantController.sitTarget = tokenBox.transform.parent.Find ("SitTarget").gameObject;

				spawnPoint = tokenBox.transform.parent.Find ("SpawnPoint");

				Vector3 spawnPointV = spawnPoint.position;
				//spawnpoint set on chair/box

			
				spawnPointV.y = startHeight;
				transform.position = spawnPointV;
			
			
				transform.rotation = spawnPoint.rotation;


				expController._isLocalPlayer = true;


				tokenBox.GetComponent<CoinManager> ()._isLocalPlayer = true;
				tokenBox.GetComponent<CoinManager> ().SetToClear ();
				Cmd_update_round_id (gameManager.round_id);


			}
		}
		SimpleMouseLook simpleMouseLook = gameObject.GetComponentInChildren<SimpleMouseLook> ();
		if (simpleMouseLook != null) {
			simpleMouseLook.enabled = true;
			simpleMouseLook._isLocalPlayer = true;
			///fixme - should come from in sml?
			Transform hips = transform.Find ("mixamorig:Hips");
			if (hips == null)
				hips = transform.Find ("Armature/mixamorig:Hips");
			if (spawnPoint != null) {
				Vector3 posspawn = spawnPoint.position;
				posspawn.y = hips.position.y;

				simpleMouseLook.dist = (simpleMouseLook.transform.position - posspawn);
			}
		} else
			Debug.LogWarning ("No Mouse Look Found");
		//Debug.LogWarning ("Mouse Look Done");
	}

	[Command]

	//Called from playernetworksetup for assigning control of token box -not used!!
	public void Cmd_Get_Authority (int _boxCount)
	{
		GameObject box = gameManager.tokenBoxes [_boxCount].gameObject;

		//send round_id to server
		Cmd_update_round_id (gameManager.round_id);
		NetworkIdentity nwI = tokenBox.AddComponent<NetworkIdentity> ();
		nwI.localPlayerAuthority = true;
	
		GameObject newBox = Instantiate (box);
	
		NetworkServer.SpawnWithClientAuthority (newBox, connectionToClient);
		gameManager.tokenBoxes [boxCount] = newBox;
	}

	//from AddPlayer when have instatiated character
	[ClientRpc]
	public void	Rpc_set_prefab ()
	{
		if (isLocalPlayer) {
			//instnatied prefab
			setup ();
		}
	}
	//called form coin manager as has no authority

	[Command]

	public void Cmd_GetMode(int token ){
		//not getting mode from late start
		ExperimentNetworking exp_network_get = gameManager.tokenBoxes[0].GetComponent<ExperimentNetworking> ();
		ExperimentNetworking exp_network = gameManager.tokenBoxes[token].GetComponent<ExperimentNetworking> ();

		exp_network.message = exp_network_get.message;

		ExperimentController exp_cont_get = gameManager.tokenBoxes[0].GetComponent<ExperimentController> ();
		ExperimentController exp_cont = gameManager.tokenBoxes[token].GetComponent<ExperimentController> ();

		exp_cont.stage_number = exp_cont_get.stage_number;

		exp_cont.mode = exp_cont_get.mode;

	}
	[Command]
	public void Cmd_Update_Coins (int _boxCount, int _currentCoins, bool _result)
	{

		gameManager.tokenBoxes [_boxCount].GetComponent<CoinManager> ().currentCoins = _currentCoins;
		gameManager.tokenBoxes [_boxCount].GetComponent<CoinManager> ().result = _result;
		//use syncvar
		//Rpc_Update_Coins (_boxCount, _currentCoins, _result);
	}

	[ClientRpc]
	public void Rpc_Update_Coins (int _boxCount, int _currentCoins, bool _result)
	{

		if (gameManager) {
			gameManager.tokenBoxes [_boxCount].GetComponent<CoinManager> ().currentCoins = _currentCoins;
			gameManager.tokenBoxes [_boxCount].GetComponent<CoinManager> ().result = _result;
		}
	}

	[Command]
	//single mesge send
	public void Cmd_Set_Text (int _boxCount, string _message)
	{

		ExperimentNetworking exp_network = gameManager.tokenBoxes [_boxCount].GetComponent<ExperimentNetworking> ();
		exp_network.message = _message;

	}
	[ClientRpc]
	public void Rpc_Clear(){
		tokenBox.GetComponent<CoinManager> ().SetToClear ();

	}
	[Command]
	public void Cmd_Clear(){
		tokenBox.GetComponent<CoinManager> ().SetToClear ();


	}

	//caleld form experiment controller to send update messages from ZTree
	[Command]
	public void Cmd_broadcast (string _message)
	{

		//update as player enters
		foreach (GameObject  exp_conts in gameManager.tokenBoxes) {

			try {
					ExperimentNetworking exp_network = exp_conts.GetComponent<ExperimentNetworking> ();

				exp_network.message = _message;

			} catch (Exception e) {

				Debug.LogWarning (e);
			}

		}
		Rpc_change_message(_message);

	}

	[ClientRpc]
	public void Rpc_change_message (string _message)
	{
		foreach (GameObject  exp_conts in gameManager.tokenBoxes) {

			try {
				ExperimentNetworking exp_network = exp_conts.GetComponent<ExperimentNetworking> ();

				exp_network.message = _message;

			} catch (Exception e) {

				Debug.LogWarning (e);
			}
		}
	}

	//called form expereiment controlle r to update stage from Ztree
	[Command]
	public void Cmd_change_currentStage (int _stage_number, ExperimentController.runState _mode)
	{
		//Debug.LogWarning ("stage");
		foreach (GameObject  exp_conts in gameManager.tokenBoxes) {
			ExperimentController exp_cont = exp_conts.GetComponent<ExperimentController> ();
		
			exp_cont.stage_number = _stage_number;

			exp_cont.mode = _mode;


		}
		Rpc_change_currentStage (_stage_number, _mode);

	}

	[ClientRpc]
	public void Rpc_change_currentStage (int _stage_number, ExperimentController.runState _mode)
	{

		foreach (GameObject  exp_conts in gameManager.tokenBoxes) {
			ExperimentController exp_cont = exp_conts.GetComponent<ExperimentController> ();

			exp_cont.stage_number = _stage_number;

			exp_cont.mode = _mode;


		}

	}

	[Command]
	public void Cmd_ikActive (int _boxCount, bool _ikActive)
	{
		
		gameManager.tokenBoxes [_boxCount].GetComponent<ExperimentController> ().ikActive = _ikActive;

	}

	[Command]
	public void Cmd_update_round_id (int _round_id)
	{

		gameManager.update_round_id (_round_id);
	}
}