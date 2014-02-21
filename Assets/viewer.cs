using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;


public class viewer : MonoBehaviour {


	public class ListWithDuplicates : List<KeyValuePair<GameObject, LineRenderer>>
	{
		public void Add(GameObject key, LineRenderer value)
		{
			var element = new KeyValuePair<GameObject, LineRenderer>(key, value);
			this.Add(element);
		}
		public void Remove(GameObject key, LineRenderer value) {
			var element = new KeyValuePair<GameObject, LineRenderer>(key, value);
			this.Remove(element);
		}
		public void Remove(LineRenderer value) {
			List<KeyValuePair<GameObject, LineRenderer>> deletable_elm = new List<KeyValuePair<GameObject, LineRenderer>>();
			foreach(KeyValuePair<GameObject, LineRenderer> elm in this) {
				if(elm.Value == value)
					deletable_elm.Add(new KeyValuePair<GameObject, LineRenderer>(elm.Key, elm.Value));
			}
			foreach(KeyValuePair<GameObject, LineRenderer> elm in deletable_elm) {
				this.Remove(elm);
			}
		}
		public List<LineRenderer> Get(GameObject elm) {
			List<LineRenderer> bunchOfData = new List<LineRenderer>();
			foreach (KeyValuePair<GameObject, LineRenderer> data in this) {
				if(data.Key == elm) {
					bunchOfData.Add(data.Value);
				}
			} 
			return bunchOfData;
		}
		public List<GameObject> GetKeys(LineRenderer line) {
			List<GameObject> bunchOfObjects = new List<GameObject>();
			foreach (KeyValuePair<GameObject, LineRenderer> data in this) {
				if(data.Value == line) {
					bunchOfObjects.Add(data.Key);
				}
			} 
			return bunchOfObjects;
		}
	}

	Vector3 mouse_pos;				//mouseposition
	Vector3 vec3;					//vector from camera to mouse, length 20 units
	Vector3 start;					//startvector of relation
	Vector3 end;					//endvector of relation
	Vector3 last_start;				//startvector of relation
	Vector3 last_end;				//endvector of relation
	Vector3 obj_pos;				//position of object
	Vector3 torchpos;				//position of selection light
	Vector3 focus;					//rotation focus
	GameObject start_obj;

	Vector3 minX;
	Vector3 maxX;
	Vector3 minY;
	Vector3 maxY;
	Vector3 minZ;
	Vector3 maxZ;
	Vector3 position;

	float tmp;
	float motionx;					//motion value for x-axe (cameramotion)
	float motiony;					//motion value for y-axe (cameramotion)
	float speed; 					//cameraspeed
	public GameObject lineToCopy;
	public GameObject coreelementToCopy;
	GameObject elm;
	public int move_mode;
	
	List<GameObject> coreelements;	//list of coreelements
	List<LineRenderer> relations;	//list of relations
	List<LineRenderer> tmp_list;

	bool element_locked;
	
	int i;							//default index
	int inspect_mode;
	int tool;						//type of tool
	int store;						//defines storeplace of gameobjects (for relations)
	public int rotation_mode;		//mode of motion
	Color c1 = Color.yellow;		//relation color-begin
	Color c2 = Color.red;			//relation color-end
	
	string element_details;
	GameObject torch = null;
	GameObject selectedObject = null;
	public GameObject locked_element;


	menu menu;

	ListWithDuplicates coreelements_relations = new ListWithDuplicates();
	ListWithDuplicates coreelements_relations_tmp;
	List<LineRenderer> selectedRelations;

	public void Start(){
		element_locked=false;
		locked_element=null;
		GameObject elm = null;
		rotation_mode=2;
		store = 1;
		move_mode = 1;
		menu = GameObject.Find("menu").GetComponent<menu>();

		coreelements=new List<GameObject>();
		relations=new List<LineRenderer>();
		tmp_list = new List<LineRenderer>();

		//Hastable project = menu.getProject();
		//menu.showMessage("Projekt geladen");

	}
	// Update is called once per frame
	void Update () {

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Debug.DrawRay (ray.origin,ray.direction*10,Color.cyan);
		vec3=ray.GetPoint(20);
		mouse_pos=new Vector3(vec3.x,vec3.y,vec3.z);

		if(Input.anyKey==true || Input.GetAxis("Mouse 3")!=0)
		{
			action_control();
		}
		else
		{
			if(mouselocked())
			{
				Screen.lockCursor=false;
				element_locked=false;
			}
		}
	}
	bool mouselocked(){
		if(Screen.lockCursor==true)
			return true;
		else
			return false;
	}
	void action_control()
	{
		objectdirection();
		construction_mode();
	}
	void presentation_mode()
	{

	}
	void fly_mode()
	{

	}
	void construction_mode()
	{
		if(Input.GetMouseButton (1))
			move_camera();
		if(Input.GetAxis("Mouse 3")!=0)
			zoom_camera();
		if(Input.GetMouseButton (2))
		{
			rotate_camera(rotation_mode);
		}
		if(menu.getOverMenu() != true) {
			if(Input.GetMouseButtonDown (0)==true && menu.getAction_mode()==1)
				create_coreelement ();
			if(Input.GetMouseButtonDown(0)==true && menu.getAction_mode()==2)
				relation_storage ();
			if(Input.GetMouseButtonDown(0)==true && menu.getAction_mode ()==0)
			{
				select_object();
			}
		}
	}

	void objectdirection()
	{
		foreach(GameObject element in coreelements)
		{
			element.transform.LookAt (Camera.main.transform.position);
		}
	}
	
	void fixRelations(GameObject element) {
		List<LineRenderer> bunchOfRelations = this.coreelements_relations.Get(element);
		foreach(LineRenderer line in bunchOfRelations) {
			List<GameObject> bunchOfObjects = this.coreelements_relations.GetKeys(line);
			line.SetPosition(0,bunchOfObjects[0].transform.position);
			line.SetPosition(1,bunchOfObjects[1].transform.position);
		}
	}

	void create_coreelement(){

		switch(menu.getElementnr()){
		case 1: 
			GameObject ce_plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			ce_plane.transform.position= mouse_pos;
			coreelements.Add (ce_plane);
			break;
		case 2: 
			GameObject ce_sphere = (GameObject)Instantiate(coreelementToCopy);
			ce_sphere.GetComponentInChildren<TextMesh>().text = ce_sphere.name.ToString();
			ce_sphere.transform.position= mouse_pos;
			coreelements.Add (ce_sphere);
			break;
		default:
			break;
		}
	}
	
	void move_camera()
	{
		speed=float.Parse(menu.getConfig("move_speed").ToString());

		if(menu.getConfig("move_mode").ToString()=="True") {
			move_mode = -1;
		} else {
			move_mode = 1;
		}
		//console(move_mode);
		float motionx = speed * Input.GetAxis("Mouse X")*Time.deltaTime;
		float motiony = speed * Input.GetAxis("Mouse Y")*Time.deltaTime*move_mode;
		Camera.main.transform.Translate(motionx,motiony,0);
	}
	void zoom_camera()
	{
		speed=int.Parse(menu.getConfig("zoom_speed").ToString());;
		if(Input.GetAxis ("Mouse 3")>0)
		Camera.main.transform.Translate(Vector3.forward*Time.deltaTime*speed);

		if(Input.GetAxis ("Mouse 3")<0)
		Camera.main.transform.Translate(Vector3.back*Time.deltaTime*speed);
	}
	/**
	 * visible_by_cam (true) -> returns closest rendered object from List coreelements 
	 * 				  (false)-> returns closest object from List coreelements
	 * 
	 * 
	 * 
	 * **/
	public GameObject getClosest_element(bool visible_by_cam)
	{
		float distance = 0f;

		if(coreelements.Count>0 && visible_by_cam)
		{
			if(visible_by_cam)
			{
				foreach(GameObject element in coreelements)
				{
					if(is_visible(element)==true)
					{
						float new_dist=(element.transform.position-Camera.main.transform.position).magnitude;
						if(distance==0f)
						{
							distance=new_dist;
							elm=element;
						}
						if(new_dist<distance)
						{
							if(coreelements.Count==1)
							{
								distance=new_dist;
								elm=element;
							}
							else
							{
								distance=new_dist;
								elm=element;
							}
						}
					}
				}
			}
			else
			{
				foreach(GameObject element in coreelements)
				{
					float new_dist=(element.transform.position-Camera.main.transform.position).magnitude;
					if(distance==0f)
					{
						distance=new_dist;
						elm=element;
					}
					if(new_dist<distance)
					{
						distance=new_dist;
						elm=element;
					}
				}
			}
		}
		return elm;
	}


	/*public Vector3 ()
	{
		float distance = 0f;
		Vector3 point = new Vector3();
		if(coreelements.Count>0)
			foreach(GameObject element in coreelements)
		{
			if(renderer.isVisible)
			{
				float new_dist=(element.transform.position-Camera.main.transform.position).magnitude;
				if(new_dist<distance)
				{
					distance=new_dist;
					elm=elment;
					point = elm.transform.position;
				}
			}
		}
		return point;
	}
	*/
	GameObject lock_element(GameObject element)
	{
		locked_element=element;
		return locked_element;
	}
	bool is_visible(GameObject element)
	{
		if(element.renderer.isVisible==true)
			return true;
		else
			return false;

	}

	void rotate_camera(int rotation_mode)
	{

		int mode=rotation_mode;
		switch(mode){
			case 1:
				speed= int.Parse(menu.getConfig("rotate_speed").ToString());
				float motionx = speed*Input.GetAxis ("Mouse X")*Time.deltaTime;
				float motiony = speed*Input.GetAxis ("Mouse Y")*Time.deltaTime;
				Camera.main.transform.Rotate (-motiony,motionx,0);
				break;
			case 2:

			if(is_visible(getClosest_element(true)))
			{
				Screen.lockCursor=true;
				if(element_locked==true)
				{
					speed = int.Parse(menu.getConfig("rotate_speed").ToString());
					motionx = speed*Input.GetAxis ("Mouse X")*Time.deltaTime;
					motiony = speed*Input.GetAxis ("Mouse Y")*Time.deltaTime;
					Vector3 focus = locked_element.transform.position;
					Camera.main.gameObject.transform.LookAt(focus);
					print (focus);
					Camera.main.transform.RotateAround(focus,Vector3.up,motionx);
					Camera.main.transform.RotateAround(focus,Vector3.left,motiony);
				}
				else
				{
					lock_element(getClosest_element(true));
					element_locked=true;
				}
			}
				/*Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					speed = int.Parse(menu.getConfig("rotate_speed").ToString());
					motionx = speed*Input.GetAxis ("Mouse X")*Time.deltaTime;
					motiony = speed*Input.GetAxis ("Mouse Y")*Time.deltaTime;

				float dist = 20f;
				Vector3 focus = ray.direction*dist;
				Camera.main.gameObject.transform.LookAt(focus);
				Camera.main.transform.RotateAround(focus,Vector3.up,motionx);
				Camera.main.transform.RotateAround(focus,Vector3.left,motiony);
				*/
				break;
			default:
				break;
		}
	}
	public void delight_all()
	{
		foreach(GameObject element in coreelements)
		{
			delight_object(element);
		}
	}
	public void delight_object(GameObject element)
	{
		if(element.gameObject.transform.childCount>1)
		{
			Transform[] children;
			children = element.GetComponentsInChildren<Transform>();
			foreach(Transform child in children)
			{
				if(child.tag == "torch")
					Destroy(child.gameObject);
			}
		}
	}
	public void enlight_object(GameObject element)
	{
		if(element.gameObject.transform.childCount>1)
		{
			Transform[] children;
			children = element.GetComponentsInChildren<Transform>();
			foreach(Transform child in children)
			{
				if(child.tag == "torch")
					Destroy(child.gameObject);
			}
		}
		if(element)
		{
			GameObject torch = new GameObject();
			torch.AddComponent<Light>();
			torch.light.color = Color.yellow;
			torch.light.type = LightType.Spot;
			torch.light.intensity = 10;
			torch.tag="torch";
			torch.transform.parent = element.transform;
			light_position (element);
			torch.transform.position = torchpos;
			torch.gameObject.transform.LookAt(element.transform.position);
		}
	}

	void light_position(GameObject element)
	{
		Vector3 obj = element.transform.position;
		Vector3 cam = Camera.main.gameObject.transform.position;
		Vector3 torchposnorm = (obj-cam).normalized;
		torchpos = obj-torchposnorm*2;
	}


	Boolean create_relation(Vector3 start, Vector3 end, GameObject obj1, GameObject obj2)
	{
		if(obj1 && obj2) {
			// kopiert prefab objekt
			GameObject line = (GameObject)Instantiate(lineToCopy);
			LineRenderer lRend = line.GetComponent<LineRenderer>();
			lRend.SetPosition (0,start);
			lRend.SetVertexCount (2);
			lRend.SetPosition (1,end);
			lRend.SetColors(c1,c2);
			lRend.SetWidth(1,1);
			// macht Probleme, wenn man es buildet und ausführt!!! (2h Fehlersuche -.-' )
			//lRend.material = new Material(Shader.Find("Particles/Additive"));
			relations.Add (lRend);
			coreelements_relations.Add(obj1,lRend);
			coreelements_relations.Add(obj2,lRend);
			return true;
		} else {
			store = 1;
			this.start=new Vector3(0,0,0);
			this.end=new Vector3(0,0,0);
			selectedObject = null;
			return false;
		}
	}
	Boolean select_object()
	{
		RaycastHit hit;
		if(Physics.Raycast(Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
		{
			selectedObject = hit.transform.gameObject;
			enlight_object(selectedObject);
		}
		else{
			selectedObject=null;
		}
		
		if(selectedObject) return true; else return false;
	}
	public void relation_storage()
	{
		if(store==1)
		{
			if(select_object()) {
				start=selectedObject.transform.position;
				start_obj = selectedObject;
				last_start = start;
				store=2;
			}
		}
		else
		{
			selectedObject = null;
			if(select_object()) {
				if(end==start)
					menu.showMessage("Relationen können nur zwischen verschiedenen Elementen erstellt werden.");
				else
				{
					end=selectedObject.transform.position;
					create_relation (this.start,this.end,start_obj,selectedObject);
					last_end = end;
					this.start=new Vector3 (0,0,0);
					this.end=new Vector3(0,0,0);
					selectedObject = null;
					store=1;
				}
			}
		}
	}
	public void showTmp() {
		string x="";
		foreach(var item in coreelements_relations)
		{
			x = x+"|"+item.Key.name.ToString()+" -> "+item.Value.name.ToString();
		}
	}
	public void destroyElement(GameObject element) {
		if(!relations.Count.Equals(0)) {
			coreelements_relations_tmp = new ListWithDuplicates();
			selectedRelations = new List<LineRenderer>();
			foreach(var relation in coreelements_relations)
			{
				if(relation.Key == element) {
					coreelements_relations_tmp.Add(element,relation.Value);
					selectedRelations.Add(relation.Value);
				}
			}
			foreach(var relation in coreelements_relations_tmp) {
				this.relations.Remove(relation.Value);
				relations.Remove(relation.Value);
				Destroy(relation.Value.gameObject, 0f);
				coreelements_relations.Remove(relation.Value);
				coreelements_relations.Remove(relation);
			}
		}
		this.showTmp();
		coreelements.Remove(element);
		Destroy (element, 0f);
	}
	/*
	 * 
	 * 
	 *				setter / getter  
	 * 
	 * 
	 * 
	 */
	public Vector3 minimapPosition() {

		List<GameObject> IntervalElements = new List<GameObject>();
		for(int i=0;i<coreelements.Count;i++) {
			if(coreelements[i].transform.position.x<minX.x) minX = coreelements[i].transform.position;
			if(coreelements[i].transform.position.y<minY.y) minY = coreelements[i].transform.position;
			if(coreelements[i].transform.position.z<minZ.z) minZ = coreelements[i].transform.position;

			if(coreelements[i].transform.position.x>maxX.x) maxX = coreelements[i].transform.position;
			if(coreelements[i].transform.position.y>maxY.y) maxY = coreelements[i].transform.position;
			if(coreelements[i].transform.position.z>maxZ.z) maxZ = coreelements[i].transform.position;
		}


		return position;
	}
	public List<GameObject> getCoreelements() {
		return coreelements;
	}
	public void console(object message) {
		menu.console(message);
	}
	public void setCoreElementPosition(GameObject Element, string[] position){
		
		if(position[0].Length != 0 &&
		   position[1].Length != 0 &&
		   position[2].Length != 0 &&
		   float.TryParse(position[0],out tmp) &&
		   float.TryParse(position[1],out tmp) &&
		   float.TryParse(position[2],out tmp)
		   ){
			menu.setInspectElementVar(false);
			
			Vector3 coords = new Vector3(float.Parse(position[0]),float.Parse(position[1]),float.Parse(position[2]));
			Element.transform.position=coords;
			Element.GetComponentInChildren<TextMesh>().text = Element.name.ToString();
			fixRelations(Element);
			console(position[0]+"|"+coords[0]+"|"+Element.transform.position.x);
		}
		else 
		{
			menu.showMessage("Die Eingaben sind nicht richtig.");
		}
	}
	public string getStart() {
		return "Punkt1:\n" +
			"       sx:"+last_start.x.ToString()+"("+start.x.ToString()+")"+
				"\n      sy:"+last_start.y.ToString()+"("+start.y.ToString()+")"+
				"\n      sz:"+last_start.z.ToString()+"("+start.z.ToString()+")";
	}
	public string getEnd() {
		return "Punkt2: \n" +
			"       ex:"+last_end.x.ToString()+"("+end.x.ToString()+")"+
				"\n      ey:"+last_end.y.ToString()+"("+end.y.ToString()+")"+
				"\n      ez:"+last_end.z.ToString()+"("+end.z.ToString()+")";
	}
}
