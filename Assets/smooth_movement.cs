using UnityEngine;
using System.Collections;

public class smooth_movement : MonoBehaviour {

	public float timeTakenDuringLerp = 3f;
	public float timeTakenDuringSlerp = 1f;
	public float wait_at_target = 1f;
	public bool isLerping;
	public bool isSlerping;
	public bool endedSlerping;
	public bool endedLerping;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private float timeStartedLerping;
	private viewer viewer;
	private float waited;
	private Quaternion slerp_start;
	private Quaternion slerp_end;
	private Vector3 target_pos;
	private float timeSinceStarted;
	private float timeStartedSlerping;
	private float slerppercentage;

	// Use this for initialization
	public void StartSlerp (Vector3 target) 
	{
		isSlerping=true;
		timeStartedSlerping = Time.time;
		target_pos = target;
		slerp_start=transform.GetChild(0).transform.rotation;
		slerp_end=Quaternion.LookRotation(target_pos-transform.position);
	}
	public void StartLerping(Vector3 target_pos)
	{
		isLerping = true;
		timeStartedLerping = Time.time;
		startPosition = transform.position;
		endPosition = target_pos;
	}
	
	//We do the actual interpolation in FixedUpdate(), since we're dealing with a rigidbody
	void FixedUpdate()
	{
		if(isLerping)
		{
			float timeSinceLerpStarted = Time.time - timeStartedLerping;
			float percentageComplete = timeSinceLerpStarted / timeTakenDuringLerp;
			transform.position = Vector3.Lerp (startPosition, endPosition, percentageComplete);

			//When we've completed the lerp, we set _isLerping to false
			if(percentageComplete >= 1.0f)
			{
				isLerping = false;
			}
		}
		if(isSlerping)
		{
			float timeSinceSlerpStarted = Time.time-timeStartedSlerping;
			float slerppercentage = timeSinceSlerpStarted / timeTakenDuringSlerp;
			transform.rotation = Quaternion.Slerp(slerp_start,slerp_end,slerppercentage);

			if(slerppercentage>=1.0f)
				isSlerping=false;
		}
		if(!isLerping && !isSlerping)
		{
			float to_wait = timeStartedLerping+wait_at_target;
			waited=Time.time-timeStartedLerping;
			if(waited>to_wait)
				end();
		}
	}
	public bool is_interpolating()
	{
		if(isLerping||isSlerping)
			return true;
		else
			return false;
	}
	void end()
	{
		transform.root.DetachChildren();
		Destroy (gameObject);
	}
	}