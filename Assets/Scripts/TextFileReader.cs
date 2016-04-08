using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;




public class TextFileReader :MonoBehaviour
{
	//store the variables for the experiment
	//int counter = 0;


	string line;
	public bool isHost;
	public string readHost;
	public string IP_Address;
	public int experiment_id;



	// Use this for initialization
	void Start ()
	{
		//reading from config file
		StreamReader file = new StreamReader ("Assets" + Path.DirectorySeparatorChar + "SetupData.txt");

		//StringBuilder IP_Builder = new StringBuilder ();
		//StringBuilder ExperimentNO_Builder = new StringBuilder ();

		while ((line = file.ReadLine ()) != null) {
			int index = line.IndexOf ("=");
			if (line.Contains ("IP_Address")) {
				IP_Address = line.Substring (index + 1).Trim ();
			
			} else if (line.Contains ("experiment_id")) {
				string exp_id = line.Substring (index + 1).Trim ();
				experiment_id = Int32.Parse (exp_id);
			

		} else if (line.Contains ("host")) {
				readHost = line.Substring (index + 1).Trim ();
				if (readHost=="True")
					isHost = true;
					else isHost=false;

		}

		}
	
		file.Close ();


	}




}
