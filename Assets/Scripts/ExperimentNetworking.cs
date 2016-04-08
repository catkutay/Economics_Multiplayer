using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;
using UnityEngine.Networking;

using UnityEngine.UI;

public class ExperimentNetworking : NetworkBehaviour
{
	//Network acces for Coin/Expereiment scripts
	public bool urlReturn;
	[SyncVar] string _message;
	//store resutls to dispaly on canvas in exp Controller
	public float resultCoins = -100;
	public float returnTotal = -100;

	public CoinManager coinManager;

	//returns from url
	public string returnString;
	public int returnInt;
	public float returnFloat;


	[SyncVar] public string message = "";


	void Start ()
	{
		//start at stage 0
		coinManager = GetComponent<CoinManager> ();
		urlReturn = true;
	}

	public void callUpdate ()
	{
		
		//message changed from expereiment controller request to server
		if (message != _message && !message.Equals("")) {
			//send update of result Message too for when it comes in

			coinManager.player.Cmd_broadcast (message);

		}
		//store lastes message
		_message = message;
	}

	public IEnumerator FetchStage (string _url, string find, string findInt, ExperimentController.runState _mode)
	{
		urlReturn = false;
		//Debug.LogWarning (_url);
		yield return StartCoroutine (WaitForSeconds (.5f));
		WWW www = new WWW (_url);

		yield return StartCoroutine (WaitForRequest (www));
		//go to next step when done
			
		// StringBuilder sb = new StringBuilder();
		string result = www.text;
		JSONNode node = JSON.Parse (result);

		if (node != null) {
			try {
				//get stage message
					
				if (node ["message"] != "" & !node ["message"].Equals (""))
					message = node ["message"];
			} catch {
				//message = null;
					
			}

			//Debug.LogWarning (node);

//looking for required part of node
			if (find.Length != 0) {

				returnString = node [find];
				returnFloat = -1;
				//	Debug.LogWarning (node);
				if (Int32.TryParse (node [findInt], out returnInt)) {
					urlReturn = true;
					//Debug.Log(returnInt);
					yield return true;
				}
				urlReturn = true;
				yield  return true;
			} else {

				if (Int32.TryParse (node [findInt], out returnInt)) {
					urlReturn = true;
					yield return true;
				}
			}
		} else {
			//Debug.LogWarning ("No node on api read for " + find + " or " + findInt);
			//canvas.message = "Errer in stages for experiment: " + node;
			urlReturn = true;
			yield return true;

		}
		urlReturn = true;
		yield break;
	}

	public IEnumerator FetchResults (string _url, string find, string findInt, ExperimentController.runState _mode)
	{
		urlReturn = false;
		//Debug.LogWarning (_url);

	
		WWW www = new WWW (_url);

		yield return StartCoroutine (WaitForRequest (www));
		//go to next step when done

		// StringBuilder sb = new StringBuilder();
		string result = www.text;
		JSONNode node = JSON.Parse (result);

		if (node != null) {
			
		//	Debug.LogWarning ("result");
		//	Debug.LogWarning (node);

			if (find.Length != 0) {

				returnString = node [find];

				//	Debug.LogWarning (node);
		
				//hack to get results into message- the time delay
				//mens you cannot pick this up in the state machine
				//	Debug.LogWarning (experimentController.mode);
				//	Debug.LogWarning ("Return" + returnString);
				if (float.TryParse (returnString, out resultCoins)) {
					//get back result from group submissions


					if (!coinManager.result) {

						//display returned amount and no effort coins
					
						coinManager.currentCoins -= (int)Mathf.Floor (resultCoins);
				//		Debug.LogWarning (message);
				//		Debug.LogWarning (resultCoins);
						coinManager.result = true;
					}
				
					returnString = node ["Total"];

					if (float.TryParse (returnString, out returnTotal)) {

						//		Debug.LogWarning (message);
						//		Debug.LogWarning (returnTotal);
					}


					urlReturn = true;
					yield return true;

					//message for localplayer/tokenbox only
				}
				urlReturn = true;
				yield return true;

			
			} else {

				if (Int32.TryParse (node [findInt], out returnInt)) {
					urlReturn = true;
					yield return true;
				}
			}
		} else {
			//Debug.LogWarning ("No node on api read for " + find + " or " + findInt);
			//canvas.message = "Errer in stages for experiment: " + node;
			urlReturn = true;
			yield return true;

		}
		urlReturn = true;
		yield break;
	}

	IEnumerator setupWait (float num)
	{
		yield return WaitForSeconds (num);


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