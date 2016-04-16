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
	public float contrib = -100;
	public float total = -100;
	public float payoff = -100;
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
		if (message != _message && !message.Equals ("")) {
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
		yield return StartCoroutine (WaitForSeconds (1.0f));
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

	public IEnumerator FetchResults (string _url)
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
			//Debug.LogWarning (node);


			//colelect all values
			returnString = node ["Contribution"];

			//	Debug.LogWarning (node);
		
			//hack to get results into message- the time delay
			//means you cannot pick this up in the state machine

			//	Debug.LogWarning ("Return" + returnString);
			if (float.TryParse (returnString, out contrib)) {
				//get back result from group submissions


				if (!coinManager.result) {

					//display returned amount and no effort coins
					
					//coinManager.currentCoins -= (int)Mathf.Floor (contrib);

					//		Debug.LogWarning (resultCoins);

				}
			}
			returnString = node ["Payoff"];
			if (float.TryParse (returnString, out payoff)) {

				//		Debug.LogWarning (message);

			}
			returnString = node ["Return"];

			if (float.TryParse (returnString, out total)) {
				//result in effort coins is reverse of the total
				coinManager.currentCoinsIdinArray=coinManager.maxCoins-(int)Mathf.Floor (total);
				coinManager.result = true;
				coinManager.player.Cmd_Update_Coins(coinManager.boxCount, coinManager.currentCoinsIdinArray, true);

			}


			urlReturn = true;
			yield return true;

			//message for localplayer/tokenbox only


			

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