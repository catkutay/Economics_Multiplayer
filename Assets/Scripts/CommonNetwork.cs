using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;
using UnityEngine.Networking;
public class CommonNetwork : NetworkBehaviour {
	//Network access for NetworkManager scripts
	public bool isHost=false;
	public bool update = true;

	public string IP_Address = null;

	// Update is called once per frame
	TextFileReader textFileReader;

	//will be set up first on server then collected by participatns
	[SyncVar] public int round_id=-1;
	[SyncVar]public int experiment_id;

	GameManager gameManager;

	//get from server
	public int max_participants;
	public int participant = 0;
	public int participant_id;



	public string Host_IP;
	public int Port;

	void Start ()
	{

		//get components to address 
		isHost=false;
		textFileReader = GetComponent<TextFileReader> ();
		gameManager = GetComponent<GameManager> ();


	}
	void Update ()
	{
		if (update) {
			//get setup values for server request - set gme host
			IP_Address = textFileReader.IP_Address;

			experiment_id = textFileReader.experiment_id;
		
			isHost = textFileReader.isHost;

			//set up for expereimenter as ishost
		
		//check to see if read host
			if (textFileReader.readHost!=null)update=false;


		}
	}
	public IEnumerator AddParticipant (string url)
	{
		//Debug.LogWarning(url);
		//yield return StartCoroutine (WaitForSeconds (.1f));
		//simple function for participant call only
		WWW www = new WWW (IP_Address+url);

		yield return StartCoroutine (WaitForRequest (www));
	}

	public IEnumerator FetchParticipant (string url)
	{
		//Debug.LogWarning(url);
		//yield return StartCoroutine (WaitForSeconds (.1f));
		//simple function for participant call only
		WWW www = new WWW (IP_Address+url);

		yield return StartCoroutine (WaitForRequest (www));
		// StringBuilder sb = new StringBuilder();
		string result = www.text;
		//Debug.Log(result);
		JSONNode node = JSON.Parse (result);
		//Debug.Log (node);
		if (node != null) {
			if ((node ["participant"] != null) & (Convert.ToInt32 (Math.Ceiling (node ["participant"].AsFloat)) >= 0)) {
				participant = Convert.ToInt32 ((Math.Ceiling (node ["participant"].AsFloat)) - 1);

				participant_id = Convert.ToInt32 (Math.Ceiling (node ["participant_id"].AsFloat));

				gameManager.boxCount = participant;

				//setup as host

			} else
				yield return false;


		} else {

			Debug.Log ("No Node Network Setup for new participant");
			yield return false;
		}


		yield break;
	}

	public IEnumerator FetchHost_IP (string url, string find, string findInt)
	{
		//Debug.LogWarning (url);
		//get IP and Port numbers - slowly
		yield return StartCoroutine (WaitForSeconds (.05f));

		WWW www = new WWW ( IP_Address+url);

		yield return StartCoroutine (WaitForRequest (www));

		// StringBuilder sb = new StringBuilder();
		string result = www.text;
		//Debug.Log (result);

		JSONNode node = JSON.Parse (result);
		//Debug.Log (node);
		if (node != null) {
			if (find.Length != 0) {
				//collect string values
				if (find=="Host_IP")Host_IP = node [find];
			
				//UNet bug - cannot use local host IP
		
				yield return true;
			} else if (findInt.Length != 0) {
				
				//Debug.LogWarning (findInt);
				//collect integer values
				int resultant;
				if (Int32.TryParse (node [findInt], out resultant)) {

					if (findInt == "Port")
						Port = resultant;
					else if (findInt == "max_participants")
						max_participants = resultant;
					else if (findInt=="round_id"){
						round_id = resultant;
						gameManager.update_round_id(round_id);
					
						//Debug.LogWarning (round_id);
					}else
						Debug.LogWarning ("incorrect call");

				} else {
					yield return true;
				}
			}
		} else {

			if (find.Length>0 || findInt.Length>0)Debug.LogWarning ("no node for " + find + " or " + findInt);

			yield return false;
		}

		yield return false ;
	
	}


	public IEnumerator WaitForRequest (WWW www)
	{
		yield return www;


	}
	IEnumerator WaitForSeconds (float num)
	{

		yield return new WaitForSeconds (num);

	}
}
