using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class menu : MonoBehaviour {

	private Rect initWindowRect;
	private Rect projectWindowRect;
	private Rect inspectElementWindowRect;
	private Rect messageWindowRect;
	private Rect RelationWindowRect;
	private Rect chooseElementWindowRect;
	private Rect ToolbarWindowRect;					// toolbar with selectable tools
	private Rect ElementWindowRect; // shows each element as button
	private Rect SelectWindowRect;
	private Rect deselectAllWindowRect;
	private Rect selectAllWindowRect;

	private int initWindowRectSelect = 0;
	private int save = 0;
	
	private Boolean message = false;
	private string messageTxt;
	
	private Hashtable project = new Hashtable();
	private Hashtable Standard = new Hashtable ();
	private string windowLocation;
	private string tmp_windowLocation;
	
	// config display and debug
	private Boolean render = true;
	private Boolean debugmode = false;
	private Boolean move_mode_conf = false;
	private Boolean menu_animation = true;
	
	// For Elements
	private Boolean inspectElementVar = false;
	private GameObject Element;
	private string[] position;
	private Regex rgx = new Regex("[^0-9.-]"); // "[^0-9\s]"
	private float tmp;
	
	// inspector
	private int i;
	public int gi_mode = 1; 				//mode of gadgetinspector
	private List<GameObject> coreelements;	//list of coreelements
	private Vector2 scrollPos;
	private Vector2 scrollPosCon;
	private Vector2 scrollPosNotCon;
	
	private int selected = -1;
	private int gridWidth=1;
	
	private viewer viewer;
	
	private int action_mode; 				//move or create element
	private int submenu;
	private int subsubmenu;
	private int elementnr; 					//sets type of element to place
	
	public GUISkin menuSkin;
	private Boolean overMenu;
	private Dictionary<string, float> menu_pos_stand_visible = new Dictionary<string, float>();
	private Dictionary<string, float> menu_pos_stand_manipulated = new Dictionary<string, float>();
	private Dictionary<string, float> menu_pos_stand_invisible = new Dictionary<string, float>();
	private Dictionary<string, float> menu_pos = new Dictionary<string, float>();

	private Hashtable buttonStyle = new Hashtable();

	/** Constructor
	 * 
	 * reads configfile, if existing, otherwise it creates it with standardparameter
	 * reads from file into Standard[] 
	 * sets project[] with username and Standard[]
	 * 
	 * @param	void
	 * @return	void
	 */
	void Start () {
		menu_pos_stand_visible.Add("top_ProjectWindowRectNew",90);
		menu_pos_stand_visible.Add("left_ProjectWindowRectNew",220);
		menu_pos_stand_visible.Add("leftStep_ProjectWindowRectNew",46);
		menu_pos_stand_visible.Add("x_ProjectWindowRectNew",400);
		menu_pos_stand_visible.Add("y_ProjectWindowRectNew",260);
		menu_pos_stand_visible.Add("yStep_ProjectWindowRectNew",21);

		menu_pos_stand_visible.Add("top_ProjectWindowRectExisting",140);
		menu_pos_stand_visible.Add("left_ProjectWindowRectExisting",220);
		menu_pos_stand_visible.Add("leftStep_ProjectWindowRectExisting",48);
		menu_pos_stand_visible.Add("x_ProjectWindowRectExisting",400);
		menu_pos_stand_visible.Add("y_ProjectWindowRectExisting",400);
		menu_pos_stand_visible.Add("yStep_ProjectWindowRectExisting",35);

		menu_pos_stand_visible.Add("top_ProjectWindowRectOption",190);
		menu_pos_stand_visible.Add("left_ProjectWindowRectOption",220);
		menu_pos_stand_visible.Add("leftStep_ProjectWindowRectOption",52);
		menu_pos_stand_visible.Add("x_ProjectWindowRectOption",450);
		menu_pos_stand_visible.Add("y_ProjectWindowRectOption",510);
		menu_pos_stand_visible.Add("yStep_ProjectWindowRectOption",46);

		menu_pos_stand_visible.Add("top_ProjectWindowRectQuit",240);
		menu_pos_stand_visible.Add("left_ProjectWindowRectQuit",220);
		menu_pos_stand_visible.Add("leftStep_ProjectWindowRectQuit",48);
		menu_pos_stand_visible.Add("x_ProjectWindowRectQuit",400);
		menu_pos_stand_visible.Add("y_ProjectWindowRectQuit",210);
		menu_pos_stand_visible.Add("yStep_ProjectWindowRectQuit",16);

		menu_pos_stand_invisible.Add("top_ProjectWindowRectNew",100);
		menu_pos_stand_invisible.Add("left_ProjectWindowRectNew",-240);
		menu_pos_stand_invisible.Add("leftStep_ProjectWindowRectNew",-10);
		menu_pos_stand_invisible.Add("x_ProjectWindowRectNew",300);
		menu_pos_stand_invisible.Add("y_ProjectWindowRectNew",50);
		menu_pos_stand_invisible.Add("yStep_ProjectWindowRectNew",10);
		
		menu_pos_stand_invisible.Add("top_ProjectWindowRectExisting",100);
		menu_pos_stand_invisible.Add("left_ProjectWindowRectExisting",-260);
		menu_pos_stand_invisible.Add("leftStep_ProjectWindowRectExisting",-10);
		menu_pos_stand_invisible.Add("x_ProjectWindowRectExisting",300);
		menu_pos_stand_invisible.Add("y_ProjectWindowRectExisting",50);
		menu_pos_stand_invisible.Add("yStep_ProjectWindowRectExisting",10);
		
		menu_pos_stand_invisible.Add("top_ProjectWindowRectOption",460);
		menu_pos_stand_invisible.Add("left_ProjectWindowRectOption",-300);
		menu_pos_stand_invisible.Add("leftStep_ProjectWindowRectOption",-10);
		menu_pos_stand_invisible.Add("x_ProjectWindowRectOption",220);
		menu_pos_stand_invisible.Add("y_ProjectWindowRectOption",50);
		menu_pos_stand_invisible.Add("yStep_ProjectWindowRectOption",10);
		
		menu_pos_stand_invisible.Add("top_ProjectWindowRectQuit",220);
		menu_pos_stand_invisible.Add("left_ProjectWindowRectQuit",-260);
		menu_pos_stand_invisible.Add("leftStep_ProjectWindowRectQuit",48);
		menu_pos_stand_invisible.Add("x_ProjectWindowRectQuit",220);
		menu_pos_stand_invisible.Add("y_ProjectWindowRectQuit",50);
		menu_pos_stand_invisible.Add("yStep_ProjectWindowRectQuit",10);


		menu_pos_stand_visible.Add("left_ToolbarWindowRect",0);
		menu_pos_stand_visible.Add("leftStep_ToolbarWindowRect",22);
		menu_pos_stand_visible.Add("top_ToolbarWindowRect",50);
		menu_pos_stand_visible.Add("x_ToolbarWindowRect",220);
		menu_pos_stand_visible.Add("y_ToolbarWindowRect",240);
		menu_pos_stand_visible.Add("yStep_ToolbarWindowRect",19);

		menu_pos_stand_invisible.Add("left_ToolbarWindowRect",-220);
		menu_pos_stand_invisible.Add("leftStep_ToolbarWindowRect",-10);
		menu_pos_stand_invisible.Add("top_ToolbarWindowRect",50);
		menu_pos_stand_invisible.Add("x_ToolbarWindowRect",220);
		menu_pos_stand_invisible.Add("y_ToolbarWindowRect",50);
		menu_pos_stand_invisible.Add("yStep_ToolbarWindowRect",19);


		menu_pos_stand_visible.Add("left_InitWindowRect",0);
		menu_pos_stand_visible.Add("leftStep_InitWindowRect",22);
		menu_pos_stand_visible.Add("top_InitWindowRect",50);
		menu_pos_stand_visible.Add("x_InitWindowRect",220);
		menu_pos_stand_visible.Add("y_InitWindowRect",240);
		menu_pos_stand_visible.Add("yStep_InitWindowRect",19);

		menu_pos_stand_invisible.Add("left_InitWindowRect",-220);
		menu_pos_stand_invisible.Add("leftStep_InitWindowRect",-10);
		menu_pos_stand_invisible.Add("top_InitWindowRect",50);
		menu_pos_stand_invisible.Add("x_InitWindowRect",220);
		menu_pos_stand_invisible.Add("y_InitWindowRect",50);
		menu_pos_stand_invisible.Add("yStep_InitWindowRect",10);


		menu_pos_stand_invisible.Add("left_ElementWindowRect",Screen.width);
		menu_pos_stand_invisible.Add("leftStep_ElementWindowRect",10);
		menu_pos_stand_invisible.Add("top_ElementWindowRect",50);
		menu_pos_stand_invisible.Add("x_ElementWindowRect",300);
		menu_pos_stand_invisible.Add("y_ElementWindowRect",50);
		menu_pos_stand_invisible.Add("yStep_ElementWindowRect",10);

		menu_pos_stand_visible.Add("left_ElementWindowRect",Screen.width-300);
		menu_pos_stand_visible.Add("leftStep_ElementWindowRect",30);
		menu_pos_stand_visible.Add("top_ElementWindowRect",50);
		menu_pos_stand_visible.Add("x_ElementWindowRect",300);
		menu_pos_stand_visible.Add("y_ElementWindowRect",500);
		menu_pos_stand_visible.Add("yStep_ElementWindowRect",45);


		menu_pos_stand_visible.Add("left_RelationWindowRect",0);
		menu_pos_stand_visible.Add("leftStep_RelationWindowRect",22);
		menu_pos_stand_visible.Add("top_RelationWindowRect",240);
		menu_pos_stand_visible.Add("x_RelationWindowRect",220);
		menu_pos_stand_visible.Add("y_RelationWindowRect",310);
		menu_pos_stand_visible.Add("yStep_RelationWindowRect",26);

		menu_pos_stand_invisible.Add("left_RelationWindowRect",-220);
		menu_pos_stand_invisible.Add("leftStep_RelationWindowRect",-60);
		menu_pos_stand_invisible.Add("top_RelationWindowRect",420);
		menu_pos_stand_invisible.Add("x_RelationWindowRect",300);
		menu_pos_stand_invisible.Add("y_RelationWindowRect",50);
		menu_pos_stand_invisible.Add("yStep_RelationWindowRect",10);


		menu_pos_stand_visible.Add("left_chooseElementWindowRect",0);
		menu_pos_stand_visible.Add("leftStep_chooseElementWindowRect",22);
		menu_pos_stand_visible.Add("top_chooseElementWindowRect",140);
		menu_pos_stand_visible.Add("x_chooseElementWindowRect",220);
		menu_pos_stand_visible.Add("y_chooseElementWindowRect",310);
		menu_pos_stand_visible.Add("yStep_chooseElementWindowRect",26);
		
		menu_pos_stand_invisible.Add("left_chooseElementWindowRect",-220);
		menu_pos_stand_invisible.Add("leftStep_chooseElementWindowRect",-60);
		menu_pos_stand_invisible.Add("top_chooseElementWindowRect",140);
		menu_pos_stand_invisible.Add("x_chooseElementWindowRect",220);
		menu_pos_stand_invisible.Add("y_chooseElementWindowRect",50);
		menu_pos_stand_invisible.Add("yStep_chooseElementWindowRect",10);


		menu_pos_stand_visible.Add("left_selectAllWindowRect",220);
		menu_pos_stand_visible.Add("leftStep_selectAllWindowRect",28);
		menu_pos_stand_visible.Add("top_selectAllWindowRect",240);
		menu_pos_stand_visible.Add("x_selectAllWindowRect",60);
		menu_pos_stand_visible.Add("y_selectAllWindowRect",50);
		menu_pos_stand_visible.Add("yStep_selectAllWindowRect",0);
		
		menu_pos_stand_invisible.Add("left_selectAllWindowRect",-60);
		menu_pos_stand_invisible.Add("leftStep_selectAllWindowRect",10);
		menu_pos_stand_invisible.Add("top_selectAllWindowRect",240);
		menu_pos_stand_invisible.Add("x_selectAllWindowRect",60);
		menu_pos_stand_invisible.Add("y_selectAllWindowRect",50);
		menu_pos_stand_invisible.Add("yStep_selectAllWindowRect",0);


		menu_pos_stand_visible.Add("left_deselectAllWindowRect",220);
		menu_pos_stand_visible.Add("leftStep_deselectAllWindowRect",28);
		menu_pos_stand_visible.Add("top_deselectAllWindowRect",190);
		menu_pos_stand_visible.Add("x_deselectAllWindowRect",60);
		menu_pos_stand_visible.Add("y_deselectAllWindowRect",50);
		menu_pos_stand_visible.Add("yStep_deselectAllWindowRect",0);
		
		menu_pos_stand_invisible.Add("left_deselectAllWindowRect",-60);
		menu_pos_stand_invisible.Add("leftStep_deselectAllWindowRect",10);
		menu_pos_stand_invisible.Add("top_deselectAllWindowRect",190);
		menu_pos_stand_invisible.Add("x_deselectAllWindowRect",60);
		menu_pos_stand_invisible.Add("y_deselectAllWindowRect",50);
		menu_pos_stand_invisible.Add("yStep_deselectAllWindowRect",0);


		menu_pos_stand_visible.Add("left_inspectElementWindowRect",230);
		menu_pos_stand_visible.Add("leftStep_inspectElementWindowRect",230);
		menu_pos_stand_visible.Add("top_inspectElementWindowRect",50);
		menu_pos_stand_visible.Add("x_inspectElementWindowRect",Screen.width - 540);
		menu_pos_stand_visible.Add("y_inspectElementWindowRect",Screen.height - 100);
		menu_pos_stand_visible.Add("yStep_inspectElementWindowRect",Screen.height - 100);
		
		menu_pos_stand_invisible.Add("left_inspectElementWindowRect",230);
		menu_pos_stand_invisible.Add("leftStep_inspectElementWindowRect",0);
		menu_pos_stand_invisible.Add("top_inspectElementWindowRect",50);
		menu_pos_stand_invisible.Add("x_inspectElementWindowRect",0);
		menu_pos_stand_invisible.Add("y_inspectElementWindowRect",0);
		menu_pos_stand_invisible.Add("yStep_inspectElementWindowRect",0);

		foreach(KeyValuePair<string,float> pair in menu_pos_stand_invisible) {
			menu_pos.Add(pair.Key,pair.Value);
			menu_pos_stand_manipulated.Add(pair.Key,menu_pos_stand_visible[pair.Key]);
		}

		viewer = GameObject.Find("viewer").GetComponent<viewer>();
		// projectpath wich should be created
		string configFile = System.IO.Path.Combine(@Application.dataPath, "mindvision.conf");
		
		if (!File.Exists (configFile)) {
			// create projectpath
			string savePath = System.IO.Path.Combine(@Application.dataPath, "projects");
			System.IO.Directory.CreateDirectory(savePath);
			
			// Create a file to write to.
			using (StreamWriter sw = File.CreateText(configFile)) {
				sw.WriteLine ("Location>" + savePath);
				sw.WriteLine ("debugmode>False");
				sw.WriteLine ("move_mode>False");
				sw.WriteLine ("move_speed>100");
				sw.WriteLine ("zoom_speed>100");
				sw.WriteLine ("rotate_speed>100");
				sw.WriteLine ("menu_animation>True");
			}
			showMessage("Das Configfile wurde nicht gefunden. Standardwerte wurden geladen.");
			// project successful created
		}
		
		// Open the file to read from.
		using (StreamReader sr = File.OpenText(configFile)) {
			string s = "";
			string[] words;
			while ((s = sr.ReadLine()) != null) {
				words = s.Split(new char[] {'>'});
				if(words[0]!=null) {
					console (words[0].ToString());
					Standard.Add(words[0].ToString(),words[1].ToString());
					words[0] = null;
					words[1] = null;
				}
			}
		}
		
		project.Add ("Name", "new Project");
		project.Add ("Author", System.Environment.UserName);
		project.Add ("Location", Standard["Location"].ToString());
		windowLocation = Standard ["Location"].ToString ();
		tmp_windowLocation = windowLocation;
		debugmode = (Standard["debugmode"].ToString()=="True") ? true : false;
		move_mode_conf = (Standard["move_mode"].ToString()=="True") ? true : false;
		menu_animation = (Standard["menu_animation"].ToString()=="True") ? true : false;
		console("Debugmode: "+debugmode.ToString());
		overMenu = false;
	}
	
	/** initializes GUI with initWindowRect
	 * 
	 * 	@param	void
	 * 	@return void
	 */
	void OnGUI() {
		GUI.skin = menuSkin;
		switch(initWindowRectSelect) {
		case (1):
			projectWindowRect = GUI.Window (1, projectWindowRect, projectWindowNew, "Neues Projekt");
			switch(save) {
			case(1):
				console ("project exists");
				showMessage("Das Projekt existiert bereits!",50,50);
				break;
			case(2):
				showMessage("Das Projekt wurde erfolgreich angelegt.",50,50);
				console ("saved");
				break;
			default:
				break;
			}
			save = 0;
			break;
		case (2):
			projectWindowRect = GUI.Window (1, projectWindowRect, projectWindowExisting, "Projekt öffnen");
			break;
		case (3):
			projectWindowRect = GUI.Window (1, projectWindowRect, projectWindowOptions, "Optionen");
			break;
		case (4):
			projectWindowRect = GUI.Window (1, projectWindowRect, projectWindowQuit, "Beenden");
			break;
		default:
			break;
		}
		GUI.BringWindowToBack(1);
		initWindowRect = GUI.Window (0, initWindowRect, initWindowFunc, "Start");
			
		GUI.BringWindowToBack(3);
		ToolbarWindowRect = GUI.Window(3,ToolbarWindowRect,toolbox,"Werkzeuge");

		GUI.BringWindowToBack(8);
		SelectWindowRect = GUI.Window(8,SelectWindowRect,SelectWindowFunc,"");

		ElementWindowRect = GUI.Window (5,ElementWindowRect, showElements ,"Gadget Inspector");

		if(inspectElementVar) {
			inspectElementWindowRect = GUI.Window (4, inspectElementWindowRect, inspectElementForm, "Inspect Element:" + Element.name.ToString());
		}

		if(message) {
			messageWindowRect = GUI.Window (6, messageWindowRect, messageOutput, "Nachricht");
		}
				
		if(submenu == 0) {
			chooseElementWindowRect = GUI.Window(7,chooseElementWindowRect,chooseElementOutput,"Maus");
			if(subsubmenu == 1) {
				this.selectAllWindowRect = GUI.Window(9,selectAllWindowRect,selectAllOutput,"");
			}
			if(subsubmenu == 2) {
				this.deselectAllWindowRect = GUI.Window(9,deselectAllWindowRect,deselectAllOutput,"");
			}
			GUI.BringWindowToBack(9);
		}
		if(submenu == 2) {
			RelationWindowRect = GUI.Window (7, RelationWindowRect, relationOutput, "Relation");
		}

		if(inspectElementWindowRect.Contains(Event.current.mousePosition) || 
		   ToolbarWindowRect.Contains(Event.current.mousePosition) || 
		   ElementWindowRect.Contains(Event.current.mousePosition)
		   )  {
			overMenu = true; 
		}
		else { 
			overMenu = false;
		}
		
	}

	void FixedUpdate() {
		if(render) {
			showView(0);
		} else {
			showView(1);
		}
	}

	void showView(int view) {
		switch(view) {
		case 0:
			invisibleView(0);
			visibleWindow("InitWindowRect");
			switch(initWindowRectSelect) {
			case 1:
				visibleWindow("ProjectWindowRectNew");
				projectWindowRect = makeRect("ProjectWindowRectNew");
				break;
			case 2:
				visibleWindow("ProjectWindowRectExisting");
				projectWindowRect = makeRect("ProjectWindowRectExisting");
				break;
			case 3:
				visibleWindow("ProjectWindowRectOption");
				projectWindowRect = makeRect("ProjectWindowRectOption");
				break;
			case 4:
				visibleWindow("ProjectWindowRectQuit");
				projectWindowRect = makeRect("ProjectWindowRectQuit");
				break;
			default:
				break;
			}
			initWindowRect = makeRect("InitWindowRect");
			break;
		case 1:
			invisibleView(1);
			visibleWindow("ToolbarWindowRect");
			ToolbarWindowRect = makeRect("ToolbarWindowRect");
			visibleWindow("ElementWindowRect");
			ElementWindowRect = makeRect("ElementWindowRect");

			switch(submenu) {
			case 2:
				visibleWindow("RelationWindowRect");
				invisibleWindow("chooseElementWindowRect");
				break;
			case 0:
				invisibleWindow("RelationWindowRect");
				visibleWindow("chooseElementWindowRect");
				switch(subsubmenu) {
				case 1:
					print("selectAll");
					visibleWindow("selectAllWindowRect");
					invisibleWindow("deselectAllWindowRect");
					break;
				case 2:
					print("deselectAll");
					invisibleWindow("selectAllWindowRect");
					visibleWindow("deselectAllWindowRect");
					break;
				default:
					invisibleWindow("selectAllWindowRect");
					invisibleWindow("deselectAllWindowRect");
					break;
				}
				break;
			}
			if(this.inspectElementVar) {
				visibleWindow("inspectElementWindowRect");
			} else {
				invisibleWindow("inspectElementWindowRect");
			}
			selectAllWindowRect = makeRect("selectAllWindowRect");
			deselectAllWindowRect = makeRect("deselectAllWindowRect");
			RelationWindowRect = makeRect("RelationWindowRect");
			chooseElementWindowRect = makeRect("chooseElementWindowRect");
			inspectElementWindowRect = makeRect("inspectElementWindowRect");
			break;
		}
	}

	void invisibleView(int view) {
		switch (view) {
		case 0:
			invisibleWindow("ToolbarWindowRect");
			ToolbarWindowRect = makeRect("ToolbarWindowRect");
			invisibleWindow("ElementWindowRect");
			ElementWindowRect = makeRect("ElementWindowRect");
			invisibleWindow("RelationWindowRect");
			RelationWindowRect = makeRect("RelationWindowRect");
			action_mode = -1;
			submenu = -1;
			subsubmenu = -1;
			break;
		case 1:
			invisibleAllProjectWindows();
			invisibleWindow("InitWindowRect");
			initWindowRect = makeRect("InitWindowRect");
			projectWindowRect = makeRect("ProjectWindowRectNew");
			initWindowRectSelect = 0;
			break;
		default:
			break;
		}
	}

	void invisibleWindow(string key) {
		menu_pos["left_"+key] = menu_pos_stand_invisible["left_"+key];
		menu_pos["top_"+key] = menu_pos_stand_invisible["top_"+key];
		menu_pos["x_"+key] = menu_pos_stand_invisible["x_"+key];
		menu_pos["y_"+key] = menu_pos_stand_invisible["y_"+key];
	}

	void visibleWindow(string key) {
		if(this.getConfig("menu_animation").ToString()=="True") {
			if(menu_pos_stand_manipulated["left_"+key]>menu_pos_stand_invisible["left_"+key]) {
				if(menu_pos["left_"+key]<menu_pos_stand_manipulated["left_"+key]) {
					menu_pos["left_"+key] += menu_pos_stand_manipulated["leftStep_"+key];
				} else if(menu_pos["y_"+key]<menu_pos_stand_manipulated["y_"+key]) {
					menu_pos["y_"+key]+=menu_pos_stand_manipulated["yStep_"+key];
				}
			} else {
				if(menu_pos["left_"+key]>menu_pos_stand_manipulated["left_"+key]) {
					menu_pos["left_"+key] -= menu_pos_stand_manipulated["leftStep_"+key];
				} else if(menu_pos["y_"+key]<menu_pos_stand_manipulated["y_"+key]) {
					menu_pos["y_"+key]+=menu_pos_stand_manipulated["yStep_"+key];
				} 
			}
		} else {
			menu_pos["left_"+key] = this.menu_pos_stand_manipulated["left_"+key];
			menu_pos["y_"+key] = this.menu_pos_stand_manipulated["y_"+key];
		}
		menu_pos["x_"+key] = menu_pos_stand_manipulated["x_"+key];
		menu_pos["top_"+key] = menu_pos_stand_manipulated["top_"+key];
	}

	void invisibleAllWindows() {
		foreach(string key in menu_pos_stand_invisible.Keys) {
			//print(key+":"+menu_pos[key] + "->" + menu_pos_stand_invisible[key]);
			menu_pos[key] = this.menu_pos_stand_invisible[key];
		}
	}
	void manipulateMenuPos(string key,float value) {
		menu_pos_stand_manipulated[key] = value;
	}
	void standardMenuPos(string key) {
		menu_pos_stand_manipulated[key] = menu_pos_stand_visible[key];
		menu_pos[key] = menu_pos_stand_manipulated[key];
	}
	void invisibleAllProjectWindows() {
		invisibleWindow("ProjectWindowRectNew");
		invisibleWindow("ProjectWindowRectExisting");
		invisibleWindow("ProjectWindowRectOption");
		invisibleWindow("ProjectWindowRectQuit");
	}

	Rect makeRect(string key) {
		return new Rect(this.menu_pos["left_"+key.ToString()],
		                this.menu_pos["top_"+key.ToString()],
		                this.menu_pos["x_"+key.ToString()],
		                this.menu_pos["y_"+key.ToString()]);
	}
	/** 
	 * 
	 * 
	 * 
	 * 
	 * 					forms for GUI.Windows
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 */
	void SelectWindowFunc(int windowID) {
		if(GUI.Button (new Rect(0, 40, 220, 50),"desel")) {
			viewer.delight_all();
		}
	}
	void relationOutput(int windowID) {
		GUI.Label(new Rect(10,50,250,200),viewer.getStart());
		GUI.Label(new Rect(10,200,250,200),viewer.getEnd());
	}
	void toolbox(int windowID)
	{
		float[] y_menu = new float[4] {menu_pos_stand_manipulated["y_ToolbarWindowRect"]-200f, menu_pos_stand_manipulated["y_ToolbarWindowRect"]-150f,menu_pos_stand_manipulated["y_ToolbarWindowRect"]-100f,menu_pos_stand_manipulated["y_ToolbarWindowRect"]-50f};
		if(submenu == 0) {
			y_menu[0] = menu_pos_stand_visible["y_ToolbarWindowRect"]-200f;
		}
		if(submenu == 1) {
			y_menu[0] = menu_pos_stand_visible["y_ToolbarWindowRect"]-200f;
			y_menu[1] = menu_pos_stand_visible["y_ToolbarWindowRect"]-150f;
		}
		if(submenu == 2) {
			y_menu[0] = menu_pos_stand_visible["y_ToolbarWindowRect"]-200f;
			y_menu[1] = menu_pos_stand_visible["y_ToolbarWindowRect"]-150f;
			y_menu[2] = menu_pos_stand_visible["y_ToolbarWindowRect"]-100f;
		}
		
		if(GUI.Button (new Rect(0, y_menu[0], 220, 50),"Element")) {
			this.manipulateMenuPos("yStep_ToolbarWindowRect",31);
			this.manipulateMenuPos("y_ToolbarWindowRect",550);
			submenu = 0;
		}
		if(GUI.Button (new Rect (0, y_menu[1], 220, 50), "Ansicht"))
		{
			this.manipulateMenuPos("yStep_ToolbarWindowRect",31);
			this.manipulateMenuPos("y_ToolbarWindowRect",550);
			submenu = 1;
		}
		if (GUI.Button (new Rect (0, y_menu[2], 220, 50), "Relation")) {
			if(menu_pos["left_ToolbarWindowRect"]!=550) {
				this.manipulateMenuPos("yStep_ToolbarWindowRect",31);
				this.manipulateMenuPos("y_ToolbarWindowRect",550);
			}
			action_mode = 2;
			submenu = 2;
			viewer.relation_storage();
		}
		if (GUI.Button (new Rect (0, y_menu[3], 220, 50), "Menü")) {
			render = true;
			this.standardMenuPos("y_ToolbarWindowRect");
			this.standardMenuPos("yStep_ToolbarWindowRect");
		}
	}
	void chooseElementOutput(int windowID) {
		if(GUI.Button (new Rect(10, 0, 210, 50),"Neu")) {
			elementnr=2;
			action_mode = 1;
			subsubmenu = 0;
		}
		if(GUI.Button (new Rect(10, 50, 210, 50),"Abwählen")) {
			action_mode = 3;
			subsubmenu = 2;
		}
		if(GUI.Button (new Rect(10, 100, 210, 50),"Auswählen")) {
			elementnr=2;
			action_mode = 0;
			subsubmenu = 1;
		}
	}
	void selectAllOutput(int windowID) {
		if(GUI.Button(new Rect(0,0,60,50), "alle")) {
			viewer.enlight_all();
		}
	}
	void deselectAllOutput(int windowID) {
		if(GUI.Button(new Rect(0,0,60,50), "alle")) {
			viewer.delight_all();
		}
	}
	void showElements(int windowID)
	{
		coreelements = viewer.getCoreelements();
		i=coreelements.Count;
		string[] gridStrings = new string[i];
		int count = 0;
		foreach(GameObject coreelement in coreelements) {
			gridStrings[count] = coreelement.name.ToString();
			count++;
		}
		
		if(GUI.Button(new Rect(20,25,40,40),"inspect"))
			gi_mode=1;
		if(GUI.Button(new Rect(60,25,40,40),"relate"))
			gi_mode=2;
		GUI.Button(new Rect(100,25,40,40),"m3");
		
		Rect scrollViewRect = new Rect(5,100,290,390);					// defines size of window with scrollbar
		Rect gridRect = new Rect(0,0,270,50*i);							// defines area of elements
		scrollPos = GUI.BeginScrollView(scrollViewRect, scrollPos, gridRect);
		
		selected = GUI.SelectionGrid(gridRect, selected, gridStrings, gridWidth);
		//selected = GUI.Toolbar(gridRect,selected,gridStrings);
		if(selected!=-1) {
			viewer.delight_all();
			inspectElement(coreelements[selected]);
			viewer.enlight_object(coreelements[selected]);
			selected = -1;
		}
		GUI.EndScrollView();
	}
	
	private void inspectElementForm(int windowId) {
		List<GameObject> connected_objects = viewer.GetConnectedElements(Element);
		string[] connected = this.GameObjectListToStringList(connected_objects);
		List<GameObject> not_connected_objects = viewer.GetNotConnectedElements(Element);
		string[] not_connected = this.GameObjectListToStringList(not_connected_objects);

		Rect gridRectConnected = new Rect(0,0,(menu_pos["x_inspectElementWindowRect"]/2)-35,50*connected.Length);							// defines area of elements
		Rect scrollgridRectConnected = new Rect (10,300,(menu_pos["x_inspectElementWindowRect"]/2)-15,Screen.height - 500);							// defines area of elements
		Rect gridRectNotConnected = new Rect(0,0,(menu_pos["x_inspectElementWindowRect"]/2)-35,50*not_connected.Length);		
		Rect scrollgridRectNotConnected = new Rect ((menu_pos["x_inspectElementWindowRect"]/2)+15,300,(menu_pos["x_inspectElementWindowRect"]/2)-15,Screen.height - 500);							// defines area of elements// defines area of elements
		int connected_sel = -1;
		int notconnected_sel = -1;

		GUI.Label(new Rect(20,50,100,50),"Name:");
		Element.name = GUI.TextField(new Rect(120,50,290,40),Element.name.ToString(),255);
		
		GUI.Label(new Rect(20,100,100,50),"X:");
		position[0] = GUI.TextField(new Rect(120,100,290,40),position[0],255);
		
		GUI.Label(new Rect(20,150,100,50),"Y:");
		position[1] = GUI.TextField(new Rect(120,150,290,40),position[1],255);
		
		GUI.Label(new Rect(20,200,100,50),"Z:");
		position[2] = GUI.TextField(new Rect(120,200,290,40),position[2],255);
		
		position[0] = rgx.Replace(position[0], "");
		position[1] = rgx.Replace(position[1], "");
		position[2] = rgx.Replace(position[2], "");

		GUI.Label(new Rect(10,250,(menu_pos["x_inspectElementWindowRect"]/2)-15,50),"verknüpfte Elemente:");

		scrollPosCon = GUI.BeginScrollView(scrollgridRectConnected, scrollPosCon, gridRectConnected);
		connected_sel = GUI.SelectionGrid(gridRectConnected, connected_sel, connected, gridWidth);
		if(connected_sel != -1) {
			viewer.delete_relation(Element, connected_objects[connected_sel]);
		}
		GUI.EndScrollView();

		GUI.Label(new Rect(10+(menu_pos["x_inspectElementWindowRect"]/2)+15,250,(menu_pos["x_inspectElementWindowRect"]/2)-15,50),"nicht verknüpfte Elemente:");
		scrollPosNotCon = GUI.BeginScrollView(scrollgridRectNotConnected, scrollPosNotCon, gridRectNotConnected);
		GUI.Label(new Rect((menu_pos["x_inspectElementWindowRect"]/2)+15,250,(menu_pos["x_inspectElementWindowRect"]/2)-25,50),"nicht verknüpfte Elemente:");
		notconnected_sel = GUI.SelectionGrid(gridRectNotConnected, notconnected_sel, not_connected, gridWidth);
		if(notconnected_sel != -1) {
			viewer.create_relation(Element.transform.position,not_connected_objects[notconnected_sel].transform.position,Element,not_connected_objects[notconnected_sel]);
		}
		GUI.EndScrollView();
		if (GUI.Button (new Rect (10, Screen.height - 160, 200, 50), "Speichern")) {
			viewer.setCoreElementPosition(Element,position);
			invisibleWindow("inspectElementWindowRect");
			viewer.delight_object(Element);
		}
		if (GUI.Button (new Rect (220, Screen.height - 160, 200, 50), "Löschen")) {
			viewer.destroyElement(Element);
			this.inspectElementVar = false;
			invisibleWindow("inspectElementWindowRect");
		}
		
		GUI.DragWindow ();
	}
	
	private void messageOutput(int windowId) {
		GUI.Label (new Rect (20, 50, 460, 190), messageTxt);
		if (GUI.Button (new Rect (20, 240, 460, 50), "OK")) {
			console ("message closed");
			message = false;
		}
		
	}
	
	/** function for initWindow
	 * 
	 * @param	initWindowId	ID from initWindow (should be 0)
	 * @return	void
	 */
	void initWindowFunc(int initWindowId) {
		// Button -> new Project
		if (GUI.Button (new Rect (0, 40, 220, 50), "neues Projekt...")) {
			console ("click -> new project (from: " + initWindowId + ")");
			initWindowRectSelect = 1;
			invisibleAllProjectWindows();
		}
		// Button -> open project
		if (GUI.Button (new Rect (0, 90, 220, 50), "Projekt öffnen...")) {
			console ("click -> open existing project (from: " + initWindowId + ")");
			initWindowRectSelect = 2;
			invisibleAllProjectWindows();

		}
		// Button -> options
		if (GUI.Button (new Rect (0, 140, 220, 50), "Optionen")) {
			console ("click -> open options (from: " + initWindowId + ")");
			initWindowRectSelect = 3;
			invisibleAllProjectWindows();
		}
		//Quit
		if (GUI.Button (new Rect (0, 190, 220, 50), "Beenden")) {
			console ("click -> quit (from: " + initWindowId + ")");
			console ("click -> open quitdialog (from: " + initWindowId + ")");
			initWindowRectSelect = 4;
			invisibleAllProjectWindows();
		}
		
	}

	void projectWindowQuit(int windowID) {
		GUI.Label(new Rect(20,50,370,100),"Sind Sie sicher, dass Sie das Programm beenden wollen?");
		if (GUI.Button (new Rect (10, 150, 180, 50), "Ja")) {
			GameObject.Find("io").GetComponent<export>().save();
			Application.Quit();
		}
		if (GUI.Button (new Rect (210, 150, 180, 50), "Nein")) {
			initWindowRectSelect=0;
		}
	}
	
	/** function for projectWindow
	 * 
	 * 	creates new project
	 * 	
	 * 	@param	initWindowId	ID from initWindow (should be 0)
	 * 	@return	void
	 */
	void projectWindowNew(int initWindowId) {
		GUI.Label (new Rect (20, 50, 110, 50), "Name:");
		project["Name"] = GUI.TextField (new Rect(130, 50, 260, 40), project["Name"].ToString(), 255);
		GUI.Label (new Rect (20, 100, 110, 50), "Autor:");
		project["Author"] = GUI.TextField (new Rect (130, 100, 260, 40), (string)project["Author"].ToString(), 255);
		GUI.Label (new Rect (20, 150, 110, 50), "Speicherort:");
		project["Location"] = GUI.TextField (new Rect (130, 150, 260, 40), (string)project["Location"].ToString(), 255);
		
		// save project
		if (GUI.Button (new Rect (10, 200, 180, 50), "Erstellen")) {
			save = saveProject (initWindowId);
		}
		
		if (GUI.Button (new Rect (210, 200, 180, 50), "Abbrechen")) {
			initWindowRectSelect = 0;
		}
		//GUI.DragWindow ();
	}
	
	/** function for projectWindow
	 * 
	 * 	opens an existing project
	 * 	
	 * 	@param	initWindowId	ID from initWindow (should be 0)
	 * 	@return	void
	 */
	void projectWindowExisting(int initWindowId) {
		ArrayList buttons = new ArrayList();
		
		tmp_windowLocation = GUI.TextField (new Rect (20, 50, 250, 40), tmp_windowLocation, 255);
		if (GUI.Button (new Rect (280, 50, 100, 40), "Öffnen")) {
			if(System.IO.Directory.Exists(@tmp_windowLocation)) 
				windowLocation = tmp_windowLocation;
			else 
				tmp_windowLocation = windowLocation;
		}
		
		string pfad = @windowLocation;
		
		//if (!path.Equals ("default"))
		//				pfad = @path;
		System.IO.DirectoryInfo[] di = new System.IO.DirectoryInfo(pfad).GetDirectories();
		
		int selGridInt = -1;
		int i = 0;
		foreach (var dirInfo in di) {
			if (checkProject (System.IO.Path.Combine (@pfad, dirInfo.Name.ToString ()), dirInfo.Name.ToString ())) {
				buttons.Add(dirInfo.Name.ToString());
				i++;
			}
		}
		
		string[] buttonarray = new string[i];
		
		for(int e=0;e<i;e++) {
			buttonarray[e] = buttons[e].ToString();
		}
		
		
		Rect scrollViewRect = new Rect(10,100,385,290);					// defines size of window with scrollbar
		Rect gridRect = new Rect(0,0,350,50*i);							// defines area of elements
		
		scrollPos = GUI.BeginScrollView(scrollViewRect, scrollPos, gridRect);
		
		selGridInt = GUI.SelectionGrid (new Rect (0, 0, 360, 50*i), selGridInt, buttonarray, 1);
		if(selGridInt != -1) { 
			//print();
			if(setProject(@pfad,buttons[selGridInt].ToString())) {
				//viewer viewer = GameObject.Find("viewer").GetComponent<viewer>();
				render = false;
			}
		}
		GUI.EndScrollView();
		//GUI.DragWindow ();
	}
	
	/** function for projectWindow
	 * 
	 * 	opens Options
	 * 	
	 * 	@param	initWindowId	ID from initWindow (should be 0)
	 * 	@return	void
	 */
	void projectWindowOptions(int initWindowId) {
		GUI.Label (new Rect (20, 50, 170, 50), "Projektpfad:");
		Standard["Location"] = GUI.TextField(new Rect(190,50,250,40),Standard["Location"].ToString(),255);
		
		GUI.Label (new Rect (20, 100, 170, 50), "Debugmode:");
		debugmode = GUI.Toggle(new Rect(190, 100, 100, 50), debugmode, "An / Aus");

		GUI.Label (new Rect (20, 150, 170, 50), "Menu Animation:");
		menu_animation = GUI.Toggle(new Rect(190, 150, 100, 50), menu_animation, "An / Aus");
		
		GUI.Label (new Rect (20, 200, 450, 50), "Kamera (Geschwindigkeit):");
		GUI.Label (new Rect (40, 250, 150, 50), "Zoom:");
		Standard["zoom_speed"] = GUI.TextField(new Rect(190,250,250,40),Standard["zoom_speed"].ToString(),255);
		
		GUI.Label (new Rect (40, 300, 150, 50), "Bewegung:");
		Standard["move_speed"] = GUI.TextField(new Rect(190,300,250,40),Standard["move_speed"].ToString(),255);
		
		GUI.Label (new Rect (40, 350, 150, 50), "Drehen:");
		Standard["rotate_speed"] = GUI.TextField(new Rect(190,350,250,40),Standard["rotate_speed"].ToString(),255);
		
		GUI.Label (new Rect (40, 400, 150, 50), "");
		move_mode_conf = GUI.Toggle(new Rect(190, 400, 250, 40), move_mode_conf, " invertieren");
		
		Standard["rotate_speed"] = rgx.Replace(Standard["rotate_speed"].ToString(), "");
		Standard["move_speed"] = rgx.Replace(Standard["move_speed"].ToString(), "");
		Standard["zoom_speed"] = rgx.Replace(Standard["zoom_speed"].ToString(), "");
		
		// save project
		if (GUI.Button (new Rect (10, 450, 205, 50), "Speichern")) {
			save = saveOptions (initWindowId);
			Standard["move_mode"] = this.move_mode_conf.ToString();
			Standard["menu_animation"] = this.menu_animation.ToString();
			showMessage("Config geschrieben.");
		}
		if (GUI.Button (new Rect (230, 450, 205, 50), "Abbrechen")) {
			initWindowRectSelect = 0;
		}
		GUI.DragWindow ();
	}
	
	/*
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 					public methods
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 */
	public string[] GameObjectListToStringList(List<GameObject> Gameobjects) {
		string[] names = new string[Gameobjects.Count];
		i = 0;
		foreach(GameObject Gameobject in Gameobjects) {
			names[i] = Gameobject.name.ToString();
			i++;
		}
		return names;
	}
	public void setInspectElementVar(Boolean value) {
		this.inspectElementVar = value;
	}
	public void inspectElement(GameObject Element) {
		this.Element = Element;
		position = new string[] {	Element.transform.position.x.ToString(),
			Element.transform.position.y.ToString(),
			Element.transform.position.z.ToString()
		};
		this.inspectElementVar = true;
	}
	
	public void console(object message) {
		if(debugmode)
			print(message);
	}
	
	public void showMessage(string message) {
		this.messageTxt = message.ToString();
		messageWindowRect = new Rect (( Screen.width/2  - 400/2 ),( Screen.height/2  - 500/2 ),500,300);
		this.message = true;
	}

	public void showMessage(string message, int x_pos, int y_pos) {
		this.messageTxt = message.ToString();
		messageWindowRect = new Rect (x_pos,y_pos,500,300);
		this.message = true;
	}
	public Boolean getOverMenu() {
		return this.overMenu;
	}
	/** Checks project if its consistent
	 * 
	 * @param	pathProject	projecttopfolder
	 * @param	projectname
	 * @return	boolean		consitent or inconsistent
	 * 
	 */
	public Boolean checkProject(string PathProject,string projectname) {
		console("INFO: checking Project: "+PathProject);
		Boolean conf = false,rel=false,elm=false;
		System.IO.FileInfo[] fi = new System.IO.DirectoryInfo(PathProject).GetFiles();
		foreach(var fileInfo in fi)
		{
			if(fileInfo.Name.ToString() == projectname+".conf") {
				conf = true;
				console("INFO: checking : "+projectname+".conf -> passed");
			}
			else if(fileInfo.Name.ToString() == projectname+"_relations.csv") {
				rel = true;
				console("INFO: checking : "+projectname+"_relations.csv -> passed");
			}
			else if(fileInfo.Name.ToString() == projectname+"_elements.csv") {
				elm = true;
				console("INFO: checking : "+projectname+"_elements.csv -> passed");
			}
		}
		if(elm && rel && conf) {
			console("INFO: checking Project "+PathProject+" -> passed");
			return true;
		} else {
			console("INFO: checking Project "+PathProject+" -> fails");
			return false;
			
		}
	}
	
	public object getConfig(string name) {
		console(Standard.Contains(name.ToString()));
		string value = (this.Standard.Contains(name.ToString())) ? this.Standard[name].ToString() : "not found";
		return value;
	}
	
	public string getProjectConfig(string path,string projectName,string value = "Name") {
		string configFile = System.IO.Path.Combine(path,projectName+"/"+projectName+".conf");
		using (StreamReader sr = File.OpenText(configFile)) {
			string s = "";
			string[] words;
			while ((s = sr.ReadLine()) != null) {
				words = s.Split(new char[] {'>'});
				if(words[0]!=null) {
					if(words[0] == value) {
						return words[1];
					}
				}
			}
		}
		return "not found";
	}
	
	public Hashtable getProject() {
		return this.project;
	}
	
	public Boolean getRender() {
		return this.render;
	}
	
	public void setRender(Boolean render) {
		this.render = render;
	}
	
	public int getAction_mode() {
		return this.action_mode;
	}
	public int getElementnr() {
		return this.elementnr;
	}
	/*
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 					save window elements to config/options/...
	 * 
	 * 
	 * 
	 * 
	 */
	
	Boolean setProject(string projectLocation, string projectName) {
		console("INFO: Location:"+projectLocation);
		console("INFO: Name:"+projectName);
		if(checkProject(System.IO.Path.Combine(projectLocation,projectName),projectName)) {
			project["Location"] = System.IO.Path.Combine(projectLocation,projectName);
			project["Name"] = projectName;
			project["Author"] = getProjectConfig(projectLocation,projectName,"Author");
			console("INFO: "+project["Author"]);
			GameObject.Find("io").GetComponent<import>().setElements();
			return true;
		} else {
			console("ERROR: Bad project");
			return false;
		}
	}
	
	/** Saves the options from Standard[] in mindvision.conf
	 * 
	 * @param	id from window
	 * @return 	1 if succeded 0 if not
	 */
	int saveOptions(int projectWindowId) {
		console ("click -> save Standard  (from: " + projectWindowId + ")");
		
		// projectpath wich should be created
		string configFile = System.IO.Path.Combine(@Application.dataPath, "mindvision.conf");
		
		// create projectpath
		string savePath = System.IO.Path.Combine(@Application.dataPath, "projects");
		System.IO.Directory.CreateDirectory(savePath);
		
		tmp_windowLocation = Standard["Location"].ToString();
		windowLocation = Standard["Location"].ToString();
		
		// Create a file to write to.
		using (StreamWriter sw = File.CreateText(configFile)) {
			sw.WriteLine ("debugmode>" + debugmode.ToString());
			sw.WriteLine ("move_mode>" + move_mode_conf.ToString());
			sw.WriteLine ("menu_animation>" + move_mode_conf.ToString());
			foreach(string key in Standard.Keys)
			{
				if(key != "debugmode" && key != "move_mode" && key != "menu_animation")
					sw.WriteLine (key.ToString() + ">" + Standard[key].ToString());
			}
		}
		return 1;
	}
	
	/** saves new project
	 * 
	 * creates an folder, named after project name
	 * creates an file in folder, named after project name.
	 * 
	 * @param	id from window
	 * @return	2 if created, 1 if project already exists
	 */
	int saveProject(int projectWindowId) {
		console ("click -> save new Project (from: " + projectWindowId + " name: " + project["Name"].ToString() + " author: " + project["Author"].ToString() + " location: " +  project["Location"].ToString()  +")");
		// safe location
		string TopLevelPath = @project["Location"].ToString();
		
		// projectpath wich should be created
		string ProjectPath = System.IO.Path.Combine(TopLevelPath, project["Name"].ToString());
		
		// create projectpath
		System.IO.Directory.CreateDirectory(ProjectPath);
		
		// project file path
		// and creation
		string projectfile = System.IO.Path.Combine(ProjectPath, project["Name"].ToString() + ".conf");
		if (!File.Exists (projectfile)) {
			// Create a file to write to.
			using (StreamWriter sw = File.CreateText(projectfile)) {
				sw.WriteLine ("Author>" + project ["Author"].ToString ());
				sw.WriteLine ("Name>" + project ["Name"].ToString ());
			}
			// project successful created
		} else {
			// Project already exists
			return 1;
		}
		
		// relation file path
		// and creation
		string relationfile = System.IO.Path.Combine(ProjectPath, project["Name"].ToString() + "_relations" + ".csv");
		if (!File.Exists (relationfile)) {
			// Create a file to write to.
			File.CreateText(relationfile);
			// project successful created
		} else {
			// Project already exists
			return 1;
		}
		
		// relation file path
		// and creation
		string elementsfile = System.IO.Path.Combine(ProjectPath, project["Name"].ToString() + "_elements" + ".csv");
		if (!File.Exists (elementsfile)) {
			// Create a file to write to.
			File.CreateText(elementsfile);
			showMessage("Das Projekt wurde erfolgreich angelegt.");
			// project successful created
		} else {
			// Project already exists
			showMessage("Das Projekt existiert bereits!");
			return 1;
		}
		
		return 2;
		
		
	}
	
}
