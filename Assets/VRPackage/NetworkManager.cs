/*using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

    SpawnSpot[] spawnSpots;
    public GameObject myPlayerGO;

    // Use this for initialization
    void Start ()
	{
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        Connect();
	}
	//Connects to Photon
    void Connect()
    {
        //PhotonNetwork.offlineMode = true;
        PhotonNetwork.ConnectUsingSettings("v001");
    }
	//debug
    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
	//Calls once connected to Photon
    void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();

    }
    //Calls if ^ fails
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null);
    }
	//Calls when joined room
    void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        SpawnMyPlayer();
    }

	//spawns player and activiates their movement and camera
    void SpawnMyPlayer()
    {
		
        if (spawnSpots == null)
        {
            Debug.LogError("spawnSpots not assigned!");
            return;
        }

        SpawnSpot mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
        myPlayerGO = (GameObject)PhotonNetwork.Instantiate("FPController", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);

        ((MonoBehaviour)myPlayerGO.GetComponent("ThirdPersonUserControl")).enabled = true;
        myPlayerGO.transform.FindChild("MainCamera").gameObject.SetActive(true);
    }

}*/
