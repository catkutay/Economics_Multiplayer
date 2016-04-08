using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using System;

public class SetupServer : NetworkBehaviour
{
	//gets data from ZTree server for Host_IP, Port, max_participants and participant number
	//The latter is used to setup player prefab, so rest of code ahs to check this has been done

	string find;
	string findInt;
	bool connected = false;

	public string Host_IP;
	 int Port=7777;
	bool server = true;


	//is this the first running system - otherwise do not need to setup server

	CommonNetwork commonNetwork;


	NetworkManager networkManager;
	// Use this for initialization
	void Start ()
	{
		
		//get components to address 
		networkManager = GetComponent<NetworkManager> ();
		DontDestroyOnLoad (transform.gameObject);

		commonNetwork = GetComponent<CommonNetwork> ();
		Host_IP = null;
	
	}



	void OnGUI ()
	{

	
		//our own HUD
		if (!NetworkClient.active && !NetworkServer.active) {

		
			//reset values on server to set this machine as hosting, update round_id
			//option available only if set in textfile as host
			if (commonNetwork.isHost) {
				if (GUILayout.Button ("Reset ZTree server for hosting from this machine")) {

					find = "Host_IP";
					//	Debug.Log (Network.player.ipAddress);
					string url = "/experiments/setup?experiment_id=" + commonNetwork.experiment_id + "&Host_IP=" + Network.player.ipAddress;
					StartCoroutine (commonNetwork.FetchHost_IP (url, find, ""));
					findInt = "Port";
					url = "/experiments/setup?experiment_id=" + commonNetwork.experiment_id + "&Port="+Port.ToString();
					StartCoroutine (commonNetwork.FetchHost_IP (url, "", findInt));
					//reset participants
					url = "/experiments/participant?experiment_id=" + commonNetwork.experiment_id + "&participant=-1";
					StartCoroutine (commonNetwork.FetchHost_IP (url, "", ""));
					find = "Host_IP";
					url = "/experiments/setup?experiment_id=" + commonNetwork.experiment_id + "&Host_IP";
					StartCoroutine (commonNetwork.FetchHost_IP (url, find, ""));
					//repeat?
					find = "";
					url = "/experiments/setup?experiment_id=" + commonNetwork.experiment_id + "&Port";
					StartCoroutine (commonNetwork.FetchHost_IP (url, "", ""));
					//reset expereiment round - also remvoe from ztree FIXME
					findInt = "round_id";
					url = "/experiments/next_round?experiment_id=" + commonNetwork.experiment_id;
					StartCoroutine (commonNetwork.FetchHost_IP (url, "", findInt));
					//Debug.LogWarning("Setup Ztree");
					
				}

				//GUILayout.Label ("Network server is not running.");

				//textfile sets up server to avoid oculus need to select as server

				server = GUILayout.Toggle (server, "Run as host");
				//find host IP and max_participant number
			
				if (GUILayout.Button ("Start/Join Server")) { 
					//set up your IP

					StartCoroutine (setupLink ());
				}

			} else if (!commonNetwork.update) {
				//auto start as client if textfilereader found isHost not true
				//showServerInformation ();
				server = false;
				StartCoroutine (setupLink ());
			}
		
		
		}
	}



	IEnumerator setupLink ()
	{
		string url;
	
		if (Host_IP == null | Port == 0) {
			//find varibles for link
			find = "Host_IP";
			url = "/experiments/setup?experiment_id=" + commonNetwork.experiment_id + "&Host_IP";
		
			yield return StartCoroutine (commonNetwork.FetchHost_IP (url, find, ""));

			findInt = "Port";
			url = "/experiments/setup?experiment_id=" + commonNetwork.experiment_id + "&Port";
			yield return StartCoroutine (commonNetwork.FetchHost_IP (url, "", findInt));
	

			findInt = "max_participants";
			url = "/experiments/setup?experiment_id=" + commonNetwork.experiment_id + "&max_participants";
			yield return StartCoroutine (commonNetwork.FetchHost_IP (url, "", findInt));
			findInt = "";

			//reset expereiment round - also remvoe from ztree FIXME
			findInt = "round_id";
			url = "/experiments/get_round?experiment_id=" + commonNetwork.experiment_id;
			StartCoroutine (commonNetwork.FetchHost_IP (url, "", findInt));
			//get results
			Host_IP = commonNetwork.Host_IP;
			Port = commonNetwork.Port;
		} 
		if (!connected) {
			connected = true;
			//now can set up
			//Debug.Log(Host_IP);
			networkManager.networkAddress = Host_IP;
			networkManager.networkPort = Port;
			//Debug.LogWarning(networkManager.networkAddress+":"+networkManager.networkPort);
			//can start after set up
			find = "participant";
		
			if (server) {
				//different comment for participant = experimenters as do not add to ecperiment listmax
		
				url = "/experiments/participant?participant=0&experiment_id=" + commonNetwork.experiment_id;
				//uncomment for TESTING without experimenter
				//url = "/experiments/participant?participant=1&experiment_id=" + commonNetwork.experiment_id;

				yield return StartCoroutine (commonNetwork.FetchParticipant (url));
				networkManager.StartHost ();
				//Debug.LogWarning(commonNetwork.participant);
				//Debug.Log ("server");

			} else {


				url = "/experiments/participant?participant=1&experiment_id=" + commonNetwork.experiment_id;
				yield return StartCoroutine (commonNetwork.FetchParticipant (url));
			
				networkManager.StartClient ();

			}

		


		}
		yield break;
	}

	void showServerInformation ()
	{
		//display link info
		GUILayout.Label ("IP: " + networkManager.networkAddress + " Port: " + networkManager.networkPort);  
	}

}
