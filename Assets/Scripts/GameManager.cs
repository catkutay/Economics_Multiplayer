using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class GameManager : NetworkBehaviour {
	//these are the player prefabs and must all be in the NetworkManager spawnPrefab list;
	public GameObject[] playerPrefabs;
	static public GameManager singleton;
	public GameObject [] tokenBoxes;
	[SyncVar] 
	public int boxCount; 
	[SyncVar] public int round_id;
	Text canvasText;
	Text canvasTextUp;
	public string message="";
	NetworkManager networkManager;
	//height off zero for character feet
	float startHeight=-1.2f;


	void Awake()
	{
		singleton = this;
	}
	// Use this for initialization
	void Start () 
	{
		//check registered all players
		networkManager = GetComponent<NetworkManager> ();
		foreach (GameObject playerPrefab in playerPrefabs)
			if (!networkManager.spawnPrefabs.Contains(playerPrefab)){

			Debug.LogError("The player prefabs on GameManager must be included in the spawnPrefabs in NetworkManager");
				}
		//setup  is -2 when assigning experimenter
		boxCount = -2;
		//testing only
	//	boxCount = -1;
	


	
	//	canvasText = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild (1).gameObject.GetComponent<Text>();
	//	canvasTextUp = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild (2).gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	//	canvasText.text = message;
	//	canvasTextUp.text= message;
	}
	void OnGUI()
	{
		//GUILayout.Label(boxCount.ToString());
	}



	public void Spawn()
	{

		//	GameObject.FindObjectOfType<AddPlayer>().Cmd_Spawn_Prefab();
	}

	// called on the server by Addplayer on esrver
	public void ServerRespawn(AddPlayer addPlayer, int boxCount)
	{
		
		//zero is expereimenter prefab
		GameObject playerPrefab = playerPrefabs[1+boxCount];
		Destroy (addPlayer.gameObject);
		Vector3 pos = new Vector3 (0, startHeight, 0);
		GameObject newPlayer = Instantiate<GameObject >( playerPrefab);

		newPlayer.transform.position=pos;
		NetworkServer.ReplacePlayerForConnection(addPlayer.connectionToClient, newPlayer,0);
	

	}

	public void update_round_id(int _round_id){

		round_id=_round_id;
	}





}
