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
		public LineRenderer GetRelation(GameObject obj1, GameObject obj2) {
			List<LineRenderer> obj1_lines = this.Get(obj1);
			List<LineRenderer> obj2_lines = this.Get(obj2);
			foreach(LineRenderer line1 in obj1_lines) {
				foreach(LineRenderer line2 in obj2_lines) {
					if(line1 == line2) return line1;
				}
			}
			return null;
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

		public List<GameObject> GetConnectedElements(GameObject elm) {
			List<LineRenderer> bunchOfLines = Get(elm);
			List<GameObject> objects = new List<GameObject>();
			foreach(LineRenderer line in bunchOfLines) {
				List<GameObject> objectsForLine = GetKeys(line);
				foreach(GameObject Gameobject in objectsForLine) {
					if(!objects.Contains(Gameobject) && Gameobject != elm)
						objects.Add(Gameobject);
				}
			}
			return objects;
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
	Vector3 translation_fly;		//cameratranslation (fly_mode)
	Vector3 tilt_fly;				//cameratilt (fly_mode)
	Vector3 newPosition;
	Vector3 lerp_target;
	Vector3 home;					//set position for center_cam on button o
	float locked_dist;				//magnitude of vector3 camera.main to locked element
	float tmp;
	float motionx;					//motion value for x-axe (cameramotion)
	float motiony;					//motion value for y-axe (cameramotion)
	float speed; 					//cameraspeed

	float fly_speed;				//cameraspeed in fly_mode
	float strave;
	float tilt;
	float rise;

	float camera_distance;

	public GameObject lineToCopy;
	public GameObject coreelementToCopy;
	GameObject elm;
	public int move_mode;
	
	List<GameObject> coreelements;	//list of coreelements
	List<LineRenderer> relations;	//list of relations
	List<LineRenderer> tmp_list;

	bool element_locked;
	bool focus_locked;				
	bool input_disabled;			//keyboard and mouse input is disabled
	bool propulsion;				//there is movement while in fly_mode

	bool is_lerping;

	int i;							//default index
	int inspect_mode;
	int tool;						//type of tool
	int store;						//defines storeplace of gameobjects (for relations)
	int mode_count;
	int current_autopos;
	int next_autopos;
	int autopos_mode;

	public int rotation_mode;		//mode of motion
	Color c1 = Color.yellow;		//relation color-begin
	Color c2 = Color.red;			//relation color-end
	
	string element_details;
	GameObject torch = null;
	GameObject selectedObject = null;
	GameObject obj_to_lerp = null;
	public GameObject locked_element;
	public GameObject rotationfocus;

	public int look_mode;
	//string var1 = (bla == true) ? "wert 1" : "wert 2"; if -> wert 1 | else -> wert 2

	menu menu;
	presentationscript presentationscript;

	ListWithDuplicates coreelements_relations = new ListWithDuplicates();
	ListWithDuplicates coreelements_relations_tmp;
	List<LineRenderer> selectedRelations;
	List <GameObject> waypoints = new List<GameObject>();
	public float smooth = 5.0f;
	Vector3 newTarget;

	public void Start(){
		autopos_mode=1;
		next_autopos=0;
		lerp_target= new Vector3(10,10,10);
		newTarget = new Vector3(100,100,100);
		is_lerping = false;
		home = new Vector3(0,0,0);
		current_autopos = 1;						//current auto_move position

		mode_count=4;							//number of modes
		camera_distance = 10;					//default distance in automove
		input_disabled=false;
		rise = 0;
		fly_speed = 0;
		strave = 0;
		tilt = 0;
		look_mode=1;							//construction_mode 1 presentation_mode 2 fly_mode 3
		speed=100;
		locked_dist = 0f;
		element_locked=false;
		locked_element=null;
		elm = null;
		rotationfocus = null;
		rotation_mode=2;						//1 selfcenteres ; 2 by closest element
		store = 1;
		move_mode = 1;
		menu = GameObject.Find("menu").GetComponent<menu>();
		presentationscript = GameObject.Find("viewer").GetComponent<presentationscript>();

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
		Ray zaxe = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
		Debug.DrawRay (zaxe.origin,zaxe.direction*10,Color.gray);
		Ray yaxe = new Ray(Camera.main.transform.position,Camera.main.transform.up);
		Debug.DrawRay (yaxe.origin,yaxe.direction*10,Color.gray);
		Ray xaxe = new Ray(Camera.main.transform.position,Camera.main.transform.right);
		Debug.DrawRay (xaxe.origin,xaxe.direction*10,Color.gray);
		vec3=ray.GetPoint(20);
		mouse_pos=new Vector3(vec3.x,vec3.y,vec3.z);
		key_control();
	}
	void set_cam_lerp_distance(float distance)
	{
		camera_distance=distance;
	}
	void obj_lerp(GameObject obj, Vector3 target)
	{
		if(!got_parent (obj))
		{
			GameObject slerpobj=attach_gameobject_to(obj,"movehelp",obj.transform.position+(camera_distance*Camera.main.transform.forward),true,obj.transform.rotation);
			slerpobj.AddComponent("smooth_movement");
			smooth_movement sr = slerpobj.GetComponent<smooth_movement>();
			sr.StartLerping(target);
		}
		else
		{
			GameObject lerpobj=obj.transform.parent.gameObject;
			smooth_movement sm = obj.transform.parent.GetComponent <smooth_movement>();
			sm.StartLerping(target);
		}
	}
	GameObject attach_lerp(GameObject obj)
	{
		GameObject lerpobj=attach_gameobject_to(obj,"movehelp",obj.transform.position+(camera_distance*Camera.main.transform.forward),true,obj.transform.rotation);
		return lerpobj;
	}

	void obj_slerp(GameObject obj, Vector3 target)
	{
		if(!got_parent (obj))
		{
			GameObject slerpobj=attach_gameobject_to(obj,"movehelp",obj.transform.position+(camera_distance*Camera.main.transform.forward),true,obj.transform.rotation);
			slerpobj.AddComponent("smooth_movement");
			smooth_movement sr = slerpobj.GetComponent<smooth_movement>();
			sr.StartSlerp(target);
		}
		else
		{
			GameObject slerpobj=obj.transform.parent.gameObject;
			smooth_movement sm = obj.transform.parent.GetComponent <smooth_movement>();
			sm.StartSlerp(target);
		}
	}
	public GameObject attach_gameobject_to(GameObject obj,string name, Vector3 position, bool directed, Quaternion direction)
	{
		GameObject go =new GameObject();
		go.name=name;
		go.transform.position=position;
		if(directed)
			go.transform.rotation=direction;
		obj.transform.parent=go.transform;
		return go;
	}
	void disable_input()
	{
		input_disabled = true;
	}
	void enable_input()
	{
		input_disabled = false;
	}
	bool input_is_disabled()
	{
		if(input_disabled == false)
			return false;
		else
			return true;
	}
	void lock_mouse()
	{
		Screen.lockCursor = true;
	}
	void unlock_mouse ()
	{
		Screen.lockCursor = false;
	}
	bool mouselocked(){
		if(Screen.lockCursor==true)
			return true;
		else
			return false;
	}
	bool got_children(GameObject potential_parent)
	{
		if(potential_parent.transform.root.childCount > 0)
			return true;
		else
			return false;
	}
	bool got_parent(GameObject potential_child)
	{
		if(potential_child.transform.root!=potential_child.transform)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void set_look_mode(int mode)
	{
		unlock_mouse();
		look_mode=mode;
	}
	void key_control()
	{
		if(Input.anyKey || Input.GetAxis("Mouse 3")!=0 || look_mode==3 || look_mode==4)
		{
			action_control();
		}
		if(Input.GetMouseButtonUp(2))
		{	
			unlock_mouse();
			if(rotationfocus_locked())
				unlock_fokus();
		}
		if(input_disabled && look_mode==3)
			enable_input();
		if(input_disabled==false)
		{
			if(Input.GetKeyDown (KeyCode.Tab))
			{
				stop_propulsion();
				switch_look_mode();
			}
		}
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			stop_propulsion();
			set_look_mode (1);
		}
		if(Input.GetKeyDown(KeyCode.H))
		{
			stop_propulsion();
			center_cam(home,0f);
		}

		if(Input.GetKeyDown(KeyCode.O))
			set_home();
	}
	void action_control()
	{
		objectdirection();
		switch(look_mode)
		{
		case 1:
			construction_mode();
			break;
		case 2:
			presentationscript.presentation_mode();
			break;
		case 3:
			fly_mode();
			break;
		case 4:
			auto_move(autopos_mode);
			break;
		default:
			construction_mode();
			break;
		}
	}
	void set_home()
	{
		home=new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z);
	}
	void set_home(Vector3 position)
	{
		home = position;
	}
	void switch_look_mode()
	{
		if(look_mode==mode_count)
			set_look_mode(1);
		else
			look_mode+=1;
	}
	void fly_mode()
	{
		if(Input.GetKey("1"))
		   lock_mouse();
		if(Input.GetKey ("2"))
			unlock_mouse();
		if(Input.GetKey("w"))
			fly_speed = fly_speed+1;
		if(Input.GetKey("s"))
			fly_speed = fly_speed-1;
		if(Input.GetKey("a"))
			strave = strave-1;
		if(Input.GetKey("d"))
			strave=strave+1;
		if(Input.GetKey("q"))
			tilt=tilt-1;
		if(Input.GetKey("e"))
			tilt=tilt+1;
		if(Input.GetKey ("x"))
			rise=rise+1;
		if(Input.GetKey ("c"))
			rise=rise-1;
		if(Input.GetKeyDown("space"))
			reduce_propulsion();
		
		Vector3 tilt_fly = new Vector3 (0,0,tilt);
		Vector3 translation_fly = new Vector3(strave,rise,fly_speed);
		Camera.main.transform.Translate(translation_fly*Time.deltaTime);
		Camera.main.transform.Rotate(tilt_fly*Time.deltaTime);
		rotation_mode=1;
		rotate_camera(rotation_mode);

		if(fly_speed!=0 || strave!=0 || tilt != 0 || rise !=0)
			propulsion=true;
		else
			propulsion=false;
	}
	void stop_propulsion()
	{
		fly_speed = 0;
		strave = 0;
		tilt = 0;
		rise=0;
		propulsion=false;
	}
	void reduce_propulsion()
	{
		while(propulsion)
		{
			if (fly_speed > 0)
				fly_speed -= Time.deltaTime*0.01f;
			else
				fly_speed += Time.deltaTime;
			if (strave > 0)
				strave -= Time.deltaTime;
			else
				strave += Time.deltaTime;
			if (tilt > 0)
				tilt -= Time.deltaTime;
			else
				tilt += Time.deltaTime;
			if (rise > 0)
				rise -= Time.deltaTime;
			else
				rise += Time.deltaTime;

			if(fly_speed >-0.1 && fly_speed <0.1)
				fly_speed=0;
			if(strave >-0.1 && strave <0.1)
				strave =0;
			if(tilt >-0.1 && tilt <0.1)
				tilt=0;
			if(rise >-0.1 && rise <0.1)
				rise =0;

			if(fly_speed==0&&strave==0&&tilt==0&&rise==0)
				propulsion=false;
		}
	}
	void center_cam(GameObject focus, float distance)
	{
		Camera.main.transform.position = focus.transform.position+Vector3.one*distance;
		Camera.main.transform.LookAt(focus.transform.position);
	}
	void center_cam(Vector3 focus, float distance)
	{
		Camera.main.transform.position = focus+Vector3.one*distance;
		Camera.main.transform.LookAt(focus);
	}
	public void set_autopos_mode(int mode)
	{
		autopos_mode = mode;
	}
	void auto_move(int mode)
	{
		switch (mode)
		{
		case 1:
			if(coreelements.Count>1)
			{
				if(!got_parent(Camera.main.gameObject))
				{
					obj_lerp(Camera.main.gameObject,coreelements[next_autopos].transform.position);
					obj_slerp(Camera.main.gameObject,coreelements[next_autopos].transform.position);
					if(next_autopos<coreelements.Count-1)
					{
						next_autopos+=1;
					}
					else
					{
						next_autopos=0;
					}
				}
			}
			break;
		case 2:
			if(waypoints.Count>1)
			{
				obj_lerp(Camera.main.gameObject,waypoints[next_autopos].transform.position);
				obj_slerp(Camera.main.gameObject,waypoints[next_autopos].transform.position);
				if(next_autopos<waypoints.Count)
					next_autopos+=1;
				else
					next_autopos=1;
			}
			break;
		default:
			auto_move (1);
			break;
		}
	}
	void construction_mode()
	{
		if(menu.getOverMenu() != true) {
			if(Input.GetMouseButtonDown (0)==true && menu.getAction_mode()==1)
				create_coreelement ();
			if(Input.GetMouseButtonDown(0)==true && menu.getAction_mode()==2)
				relation_storage ();
			if(Input.GetMouseButtonDown(0)==true && menu.getAction_mode ()==0)
				select_object();
			if(Input.GetMouseButtonDown(0)==true && menu.getAction_mode ()==3)
				deselect_object();
			if(Input.GetMouseButton (1))
				move_camera();
			if(Input.GetAxis("Mouse 3")!=0)
				zoom_camera();
			if(Input.GetMouseButton (2))
				rotate_camera(rotation_mode);
		}
	}

	void objectdirection()
	{
		foreach(GameObject element in coreelements)
		{
			Ray zaxe = new Ray(element.transform.position, element.transform.forward);
			Debug.DrawRay(zaxe.origin,zaxe.direction*20,Color.red);
			Ray yaxe = new Ray(element.transform.position, element.transform.up);
			Debug.DrawRay(yaxe.origin,yaxe.direction*20,Color.red);
			Ray xaxe = new Ray(element.transform.position, element.transform.right);
			Debug.DrawRay(xaxe.origin,xaxe.direction*20,Color.red);
			element.transform.LookAt (Camera.main.transform.position,Camera.main.transform.up);
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
	public void create_coreelement(float x, float y, float z, string name, int type, int id)
	{
		Vector3 pos = new Vector3(x,y,z);
		switch(type){
		case 1: 
			GameObject ce_plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			ce_plane.transform.position= pos;
			ce_plane.transform.name=name;
			ce_plane.GetComponentInChildren<vars>().Id = id;
			ce_plane.GetComponentInChildren<vars>().Type = 1;
			coreelements.Add (ce_plane);
			break;
		case 2: 
			GameObject ce_sphere = (GameObject)Instantiate(coreelementToCopy);
			ce_sphere.GetComponentInChildren<TextMesh>().text = name;
			ce_sphere.transform.position= pos;
			ce_sphere.transform.name=name;
			ce_sphere.GetComponentInChildren<vars>().Id = id;
			ce_sphere.GetComponentInChildren<vars>().Type = 2;
			coreelements.Add (ce_sphere);
			break;
		default:
			break;
		}
	}
	void create_coreelement(){
		switch(menu.getElementnr()){
		case 1: 
			GameObject ce_plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			ce_plane.transform.position= mouse_pos;
			ce_plane.GetComponentInChildren<vars>().Id = coreelements.Count;
			ce_plane.GetComponentInChildren<vars>().Type = 1;
			coreelements.Add (ce_plane);
			break;
		case 2: 
			GameObject ce_sphere = (GameObject)Instantiate(coreelementToCopy);
			ce_sphere.GetComponentInChildren<TextMesh>().text = ce_sphere.name.ToString();
			ce_sphere.transform.position= mouse_pos;
			ce_sphere.GetComponentInChildren<vars>().Id = coreelements.Count;
			ce_sphere.GetComponentInChildren<vars>().Type = 2;
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
	 */
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

	GameObject lock_element(GameObject element, Camera cam)
	{
		locked_element=element;
		locked_dist=(element.transform.position-cam.transform.position).magnitude;
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
		switch(rotation_mode){
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
					if(element_locked==true&&rotationfocus_locked()==true)
					{
						rotate_by_point(Camera.main.gameObject,locked_element.transform.position);
					}
					else
					{
						lock_element(getClosest_element(true), Camera.main);
						element_locked=true;
						create_rotationfokus(getClosest_element(true).transform.position);
					}
				}
				break;
			default:
				break;
		}
	}

	void rotate_by_point(GameObject camera, Vector3 target)
	{
		if(focus_locked==true)
		{
			speed=float.Parse(menu.getConfig("move_speed").ToString());
			camera.transform.parent = rotationfocus.transform;
			float y =speed*Input.GetAxis("Mouse X")*Time.deltaTime;
			float x =speed*Input.GetAxis ("Mouse Y")*Time.deltaTime;
			rotationfocus.transform.Rotate(x,y,0);
		}
	}
	bool rotationfocus_locked()
	{
		if(focus_locked==true)
			return true;
		else
			return false;
	}
	void unlock_fokus()
	{
		if(focus_locked==true)
		{
			rotationfocus.transform.DetachChildren();
			Destroy(rotationfocus);
			focus_locked=false;
		}
	}
	void lock_focus()
	{
		focus_locked=true;
	}
	GameObject create_rotationfokus(Vector3 position)
	{
		rotationfocus=new GameObject();
		rotationfocus.transform.position=position;
		rotationfocus.transform.eulerAngles = Camera.main.transform.eulerAngles;
		rotationfocus.name="rotationfocus";
		Camera.main.transform.LookAt(rotationfocus.transform.position,rotationfocus.transform.up);
		lock_focus();
		return rotationfocus;
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
	public void enlight_all()
	{
		if(coreelements.Count>0)
		{
			foreach(GameObject element in coreelements)
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
		}
	}
	void light_position(GameObject element)
	{
		Vector3 obj = element.transform.position;
		Vector3 cam = Camera.main.gameObject.transform.position;
		Vector3 torchposnorm = (obj-cam).normalized;
		torchpos = obj-torchposnorm*2;
	}

	public Boolean delete_relation(GameObject obj1, GameObject obj2) {
		LineRenderer line = this.coreelements_relations.GetRelation(obj1,obj2);
		if(line != null) {
			relations.Remove(line);
			this.coreelements_relations.Remove(line);
			Destroy(line);
			return true;
		} else {
			return false;
		}
	}
	public Boolean create_relation(Vector3 start, Vector3 end, GameObject obj1, GameObject obj2)
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
	public void create_relation(GameObject obj1, GameObject obj2) {
		create_relation (new Vector3 (), new Vector3 (), obj1, obj2);
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
	Boolean deselect_object()
	{
		RaycastHit hit;
		if(Physics.Raycast(Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
		{
			selectedObject = hit.transform.gameObject;
			delight_object(selectedObject);
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
			delight_all();
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
		//this.showTmp();
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

	public List<LineRenderer> getRelations ()
	{
		return relations;
	}

	public List<GameObject> GetConnectedElements(GameObject Element) {
		return this.coreelements_relations.GetConnectedElements(Element);
	}
	public List<GameObject> GetNotConnectedElements(GameObject Element) {
		List<GameObject> not_connected = new List<GameObject>();
		List<GameObject> connected = coreelements_relations.GetConnectedElements(Element);
		foreach(GameObject coreelement in coreelements) {
			Boolean is_in = false;
			foreach(GameObject connected_obj in connected) {
				if(connected_obj == coreelement) { is_in = true; }
			}
			if(is_in == false && coreelement != Element) not_connected.Add(coreelement); 
		}
		return not_connected;
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
