using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class presentationscript : MonoBehaviour {
	float t;
	bool presentation_focus;
	bool presentation_mode_running;
	bool presentation_moving;
	bool camera_moving;
	public GameObject presented_target;
	GameObject presented_current;
	GameObject focus;
	bool moving_started;
	private List<GameObject> coreelements;
	viewer viewer;
	// Use this for initialization
	void Start () {
		GameObject focus = null;
		GameObject presented_current = null;
		moving_started=false;
		focus=null;
		t=2.0f;
		camera_moving=false;
		presented_current=null;
		presented_target=null;
		presentation_focus=false;
		presentation_mode_running=false;
		presentation_moving=false;
		viewer = GameObject.Find("viewer").GetComponent<viewer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/**
	 * initialize presentationmode with first element in coreelements as startobject
	 */
	public void start_presentation_mode()
	{
		coreelements = viewer.getCoreelements();
		if(coreelements.Count>0)
		{
			start_presentation_mode(coreelements[1]);
		}
	}
	/**
	 * initialize presentationmode with startobject
	 */
	public void start_presentation_mode(GameObject current)
	{
		viewer.set_look_mode(2);
		presentation_mode_running=true;
		set_presented_object(current);
		GameObject focus = new GameObject();
		focus.transform.position = Camera.main.transform.position;
		focus.transform.position = focus.transform.position+new Vector3(0,0,5);
		Camera.main.transform.IsChildOf(focus.transform);
	}
	public void end_presentation_mode()
	{
		focus.transform.DetachChildren();
		Destroy (focus.transform);
	}
	public void presentation_mode()
	{
		if(presentation_mode_running)
		{
			if(camera_moving)
			{
				if(moving_started==false)
				{
					moving_started=true;
					presentation_moveto_object(focus,get_target());
				}
			}
			else
			{
				//zoom+rotation;
			}
		}
	}
	/**
	 * set next camera target
	 */
	public void set_presented_object(GameObject target) 
	{
		presented_target=target;
	}
	public GameObject get_target()
	{
		return presented_target;
	}
	void presentation_moveto_object(GameObject from,GameObject to)
	{
		Vector3 focus_pos=focus.transform.position;
		float t=0f;
		Vector3 start = from.transform.position;
		Vector3 end = to.transform.position;
		float startTime = Time.time;
		float speed = 1.0f;
		float journeyLength = Vector3.Distance (start,end);
		if(from!=to)
		{
			while(focus_pos!=end)
			{
				focus_pos=focus.transform.position;
				float distCovered = (Time.time - startTime) * speed;
				float fracJourney = distCovered / journeyLength;
				focus.transform.position = Vector3.Lerp(start, end, fracJourney);
			}
			from=to;
		}
		camera_moving=false;
		moving_started=false;
	}

	GameObject get_presented_object()
	{
		return presented_current;
	}

}
