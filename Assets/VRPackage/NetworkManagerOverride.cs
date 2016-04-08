using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using SimpleJSON;
using System.Linq;


public class NetworkManagerOverride: NetworkManager
{
	public NetworkConnection _conn;

	public GameObject[] playerPrefabs;

	GameObject prefab;
	/*
	public override void OnStartServer()
	{



		base.OnStartServer();
	}

	public override void OnServerConnect(NetworkConnection conn)
	{
		_conn=conn;
		base.OnServerConnect(conn);
	}
	public override void OnClientConnect(NetworkConnection conn)
	{
		//change the prefab - does not work!
		int prefabIndex = gameObject.GetComponent<SetupServer> ().participant;
		//Debug.LogWarning( gameObject.GetComponent<SetupServer> ().participant);
		if (prefabIndex>=0)playerPrefab = playerPrefabs[prefabIndex];
		_conn=conn;

		base.OnClientConnect(conn);
	

	}

*/

}
