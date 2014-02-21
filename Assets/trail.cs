using UnityEngine;
using System.Collections;

public class Trail: MonoBehaviour {

	public LineRenderer lineRenderer;
	private float schritte;
	private float entfernung;
	public float geschwindigkeitLinie;

	Vector3 aktuellerPunkt;

	public Transform punktA;
	public Transform punktB;


	// Use this for initialization
	void Start () {
		lineRenderer.SetPosition(0, punktA.position);
		lineRenderer.SetWidth(0.1f, 0.1f);

		entfernung = Vector3.Distance(punktA.position, punktB.position);
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(schritte < entfernung){

			schritte+= 0.1f/ geschwindigkeitLinie;

			float x = Mathf.Lerp(0, entfernung, schritte);

			aktuellerPunkt = x * Vector3.Normalize(punktB.position - punktA.position) + punktA.position;

			lineRenderer.SetPosition(1, aktuellerPunkt);
		}
	}
}
