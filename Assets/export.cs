using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class export : MonoBehaviour {
	Hashtable project = new Hashtable();
	viewer viewer;
	menu menu;

	// Use this for initialization
	void Start () {
		viewer = GameObject.Find("viewer").GetComponent<viewer>();
		menu = GameObject.Find("menu").GetComponent<menu>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void save() {
		Hashtable project = menu.getProject ();
		// safe location
		string TopLevelPath = @project["Location"].ToString();
		// projectpath wich should be created
		string ProjectPath = TopLevelPath;

		// relation file path
		// and creation
		string elementsfile = System.IO.Path.Combine(ProjectPath, project["Name"].ToString() + "_elements" + ".csv");
		if (!File.Exists (elementsfile)) {
			File.CreateText(elementsfile);
		}
		// Create a file to write to.
		List<GameObject> coreelements = viewer.getCoreelements();
		using (StreamWriter sw = File.CreateText(elementsfile)) {
			foreach (GameObject obj in coreelements) {
				sw.WriteLine (obj.GetComponentInChildren<vars>().Id+";"+obj.GetComponentInChildren<TextMesh>().text+";"+obj.transform.position.x+";"+obj.transform.position.y+";"+obj.transform.position.z+";"+obj.GetComponentInChildren<vars>().Type);
			}
		}

		// relation file path
		// and creation
		string relationsfile = System.IO.Path.Combine(ProjectPath, project["Name"].ToString() + "_relations" + ".csv");
		if (!File.Exists (relationsfile)) {
			File.CreateText(relationsfile);
		}
		// Create a file to write to.
		List<LineRenderer> lines = viewer.getRelations ();
		using (StreamWriter sw = File.CreateText(relationsfile)) {
			foreach (LineRenderer line in lines) {

				sw.WriteLine (obj.GetComponentInChildren<vars>().Id+";"+obj.GetComponentInChildren<TextMesh>().text+";"+obj.transform.position.x+";"+obj.transform.position.y+";"+obj.transform.position.z+";"+obj.GetComponentInChildren<vars>().Type);
			}
		}
	}
}
