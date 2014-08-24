using UnityEngine;
using System.Collections;

public class smooth_rotation : MonoBehaviour {
	private Quaternion slerp_start;
	private Quaternion slerp_end;
	private Vector3 target_pos;
	private bool isSlerping;
	private float timeSinceStarted;
	private float timeStartedSlerping;
	public float timeTakenDuringSlerp;
	private float slerppercentage;
	// Use this for initialization
	public void StartSlerp (Vector3 target) 
	{
		timeTakenDuringSlerp=3.0f;
		timeStartedSlerping = Time.time;
		target_pos = target;
		slerp_start=transform.GetChild(0).transform.rotation;
		slerp_end=Quaternion.LookRotation(target_pos-transform.position);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(isSlerping)
		{
			timeSinceStarted = Time.time-timeStartedSlerping;
			slerppercentage = timeSinceStarted / timeTakenDuringSlerp;
			transform.rotation = Quaternion.Slerp(slerp_start,slerp_end,slerppercentage);
		}
		if(slerppercentage>=1.0f)
			isSlerping=false;
	}
}
