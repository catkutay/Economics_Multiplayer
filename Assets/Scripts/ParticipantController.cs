using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

using UnityEngine.Networking;

public class ParticipantController :NetworkBehaviour
{
	//data is inserted from playernetworksetup when it starts
	bool updating = true;
	public int participant;
	public int participant_id;
	float startHeight = -1.0f;
	Vector3 sitTargetV;
	Transform lookAtEffector;

	public enum modes
	{
		start,
		stand,
		walk,
		wait,
		sitting,
		sit,
		run}

	;

	float diff = 1;
	//targets assigned in PlyaerNetworksetup
	public GameObject box;

	public GameObject walkTarget;
	public GameObject sitTarget = null;

	Quaternion transformRotation;
	public Vector3 target;

	PlayerNetworkSetup playerNetwork;

	public CoinManager coinManager;
	public modes mode = modes.start;

	Animator animator;
	public NetworkConnection conn;


	Vector3 relativePos;
	//from playernewtork setup
	public Text canvasText;

	ExperimentController exp_cont;

	// Use this for initialization
	void Start ()
	{
		startHeight = transform.position.y;
		playerNetwork = transform.GetComponent<PlayerNetworkSetup> ();
		animator = GetComponent<Animator> ();

		//to focus on box
		try {
			lookAtEffector = GetComponentInChildren<SimpleMouseLook> ().transform;
		} catch {
			lookAtEffector = transform.GetChild (0).GetChild (0).GetComponent<SimpleMouseLook> ().transform;
		}
		mode = modes.start;

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isLocalPlayer) {

			switch (mode) {
			case modes.start:
				
				if (sitTarget != null)
					sitTargetV = sitTarget.transform.position; 
				sitTargetV.y = startHeight;
				

				//break;
		//	alternate walk start
				if (walkTarget != null) {
					//start walk
					animator.SetFloat ("Speed", 1);
					target = walkTarget.transform.position;
				
					mode = modes.walk;

				} else {
					animator.SetBool ("Sit", true);
					mode = modes.sitting;
				}
				break;
			case modes.stand:
			//end of game
				animator.SetFloat ("Speed", 0);
				animator.SetBool ("Sit", false);
			

				break;
			case modes.walk:
			//walking use walkTarget for direction the sittarget

				relativePos = target - transform.position;
				relativePos.y = 0f;
				transformRotation = Quaternion.LookRotation (relativePos);
				transform.rotation = Quaternion.Slerp (transform.rotation, transformRotation, Time.time * 1);	
			//	Debug.LogWarning (Vector3.Distance (transform.position, target));

				if ((Vector3.Distance (transform.position, target) < diff) && (transform.rotation.eulerAngles.y - transformRotation.eulerAngles.y < diff)) {
					//move between two effectors, first beside seat, next in front.
					//find target for sit or you are sitting
					if (walkTarget == null) {
						mode = modes.sit;
						sitTargetV.y = startHeight;
						transform.position = sitTargetV;
						animator.SetFloat ("Speed", 0);
					} else {
						
						diff = 0.4f;

						target = sitTargetV;
						walkTarget = null;
					}
				}

				break;
			case modes.sit:
				
			//sitting down

			
			
				animator.SetBool ("Sit", true);
				animator.SetFloat ("Speed", 0);


				transformRotation = sitTarget.transform.rotation;

	
				transform.rotation = Quaternion.Slerp (transform.rotation, transformRotation, Time.time * .5f);
				if (transform.rotation.eulerAngles.y - transformRotation.eulerAngles.y < .1f) {
					//go to sitting if finished sit motion
					if (animator.GetCurrentAnimatorStateInfo (0).IsName ("sitting_idle")) {
						mode = modes.sitting;

					}
				}


	
				break;
			case modes.sitting:
				
				//controler sits over centre of seat
			

				if (coinManager == null) {
					coinManager = box.GetComponent<CoinManager> ();
				}

					//start experiment when sitting
				mode = modes.run;
				exp_cont = coinManager.GetComponent<ExperimentController> ();

				exp_cont.ikActive = true;
						//exp_cont.mode = ExperimentController.runState.wait;

				//setup particpant on ZTree server
				CommonNetwork commonNetwork = GameObject.Find ("NetworkManager").GetComponent<CommonNetwork> ();

				string url = "/experiments/add_participant?participant_id=" + commonNetwork.participant_id + "&experiment_id=" + commonNetwork.experiment_id;
				StartCoroutine (commonNetwork.AddParticipant (url));



				updating = true;

				break;
			case modes.run:
				if (updating) {
					//lookAtEffector.position = box.transform.position;
					//does nto work, goes strait back to mouse position
					playerNetwork.FPCharacterCam.transform.LookAt (lookAtEffector.position);
					updating = false;
				}
				break;

			}
		}
	}


}