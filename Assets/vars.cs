using UnityEngine;
using System.Collections;

public class vars : MonoBehaviour {
	public int id;
	public int type;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public int Type {
		get {
			return type;
		}
		
		set {
			this.type = value;
		}
	}

	public int Id {
		get {
			return id;
		}

		set {
			this.id = value;
		}
	}
	
}
