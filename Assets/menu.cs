using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class menu : MonoBehaviour {

	private Rect initWindow;
	private Rect projectWindow;
	private Rect inspectELementWindow;
	private Rect messageWindow;
	private Rect RelationWindowRect = new Rect(0,300,250,420);
	private Rect ToolbarWindowRect;					// toolbar with selectable tools
	private Rect ElementWindowRect; // shows each element as button
	
	private int buttonpressed = 0;
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
	
	private int selected = -1;
	private int gridWidth=1;
	
	private viewer viewer;
	
	private int action_mode; 				//move or create element
	private int elementnr; 					//sets type of element to place
	
	public GUISkin menuSkin;
	private Boolean overMenu;
	private int leftProjectWindow;
	private int topProjectWindow;
	private int leftInitWindow;
	private int topInitWindow;
	private int leftToolbarWindow;
	private int topToolbarWindow;
	private int leftElementWindow;
	private int topElementWindow;
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
		leftInitWindow = -220;
		leftProjectWindow = -260;
		topProjectWindow = 50;
		topInitWindow = 50;
		leftToolbarWindow = -220;
		topToolbarWindow = 50;
		leftElementWindow = 0;
		topElementWindow = 50;

		initWindow = new Rect(leftInitWindow,20,220,240);
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
		console("Debugmode: "+debugmode.ToString());
		overMenu = false;
	}
	
	/** initializes GUI with initWindow
	 * 
	 * 	@param	void
	 * 	@return void
	 */
	void OnGUI() {
		GUI.skin = menuSkin;
		switch(buttonpressed) {
		case (1):
			projectWindow = new Rect(leftProjectWindow,60,400,topProjectWindow);
			projectWindow = GUI.Window (1, projectWindow, projectWindowNew, "Neues Projekt");
			switch(save) {
			case(1):
				console ("project exists");
				showMessage("Das Projekt existiert bereits!");
				break;
			case(2):
				showMessage("Das Projekt wurde erfolgreich angelegt.");
				console ("saved");
				break;
			default:
				break;
			}
			save = 0;
			break;
		case (2):
			projectWindow = new Rect(leftProjectWindow,110,400,topProjectWindow);
			projectWindow = GUI.Window (1, projectWindow, projectWindowExisting, "Projekt öffnen");
			break;
		case (3):
			projectWindow = new Rect(leftProjectWindow,160,400,topProjectWindow);
			projectWindow = GUI.Window (1, projectWindow, projectWindowOptions, "Optionen");
			break;
		case (4):
			projectWindow = new Rect(leftProjectWindow,210,400,topProjectWindow);
			projectWindow = GUI.Window (1, projectWindow, projectWindowQuit, "Beenden");
			break;
		default:
			break;
		}
		GUI.BringWindowToBack(1);
		initWindow = GUI.Window (0, initWindow, initWindowFunc, "Start");
			
		ToolbarWindowRect = GUI.Window(3,ToolbarWindowRect,toolbox,"Werkzeuge");
		ElementWindowRect = GUI.Window (5,ElementWindowRect, showElements ,"Gadget Inspector");
		if(inspectElementVar) {
			inspectELementWindow = GUI.Window (4, inspectELementWindow, inspectElementForm, "Inspect Element:" + Element.name.ToString());
		}

		if(message) {
			messageWindow = new Rect (( Screen.width/2  - 400/2 ),( Screen.height/2  - 500/2 ),500,300);
			messageWindow = GUI.Window (6, messageWindow, messageOutput, "Nachricht");
		}
				
		if(action_mode == 2) {
			RelationWindowRect = GUI.Window (7, RelationWindowRect, relationOutput, "Relation");
		}

		if(inspectELementWindow.Contains(Event.current.mousePosition) || 
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
			leftToolbarWindow = -220;
			topToolbarWindow = 50;
			leftElementWindow = 0;
			topElementWindow = 50;

			if(leftInitWindow<0) {
				leftInitWindow += 20;
			} else {
				if(topInitWindow<240) {
					topInitWindow += 24;
				}
			}

			if(buttonpressed==1 || buttonpressed==2 || buttonpressed==3 || buttonpressed==4) {
				if(leftProjectWindow<220) {
					leftProjectWindow += 20;
				} else {
					if(topProjectWindow<255 && buttonpressed == 1) {
						topProjectWindow+= 255/10;
					}
					if(topProjectWindow<400 && buttonpressed == 2) {
						topProjectWindow+= 40;
					}
					if(topProjectWindow<460 && buttonpressed == 3) {
						topProjectWindow+= 46;
					}
					if(topProjectWindow<220 && buttonpressed == 4) {
						topProjectWindow+= 20;
					}
				}
			//	projectWindow = new Rect(leftProjectWindow,60,400,260);
			}
		} else {
			leftInitWindow = -220;
			topInitWindow = 50;
			topProjectWindow = 50;
			leftProjectWindow = 0;
			buttonpressed = 0;

			if(leftToolbarWindow<0) {
				leftToolbarWindow += 20;
			} else {
				if(topToolbarWindow<240) {
					topToolbarWindow += 24;
				}
			}

			if(leftElementWindow<300) {
				leftElementWindow += 30;
			} else {
				if(topElementWindow<500) {
					topElementWindow += 50;
				}
			}

		}
		initWindow = new Rect(leftInitWindow,20,220,topInitWindow);
		ToolbarWindowRect = new Rect(leftToolbarWindow,20,220,topToolbarWindow);
		ElementWindowRect = new Rect(Screen.width-leftElementWindow,20,300, topElementWindow);
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
	void relationOutput(int windowID) {
		GUI.Label(new Rect(10,50,250,200),viewer.getStart());
		GUI.Label(new Rect(10,200,250,200),viewer.getEnd());
	}
	void toolbox(int windowID)
	{
		if(GUI.Button (new Rect(0, 40, 220, 50),"Maus")) {
			action_mode=0;
			viewer.showTmp();
		}
		if(GUI.Button (new Rect (0, 90, 220, 50), "Element"))
		{
			action_mode=1;
			elementnr=2;
		}
		if (GUI.Button (new Rect (0, 140, 220, 50), "verbinden")) {
			action_mode = 2;
			viewer.relation_storage();
		}
		if (GUI.Button (new Rect (0, 190, 220, 50), "Menü")) {
			render = true;
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
		if(selected!=-1) {
			inspectElement(coreelements[selected]);
			viewer.enlight_object(coreelements[selected]);
			selected = -1;
		}
		GUI.EndScrollView();
	}
	
	private void inspectElementForm(int windowId) {
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
		
		if (GUI.Button (new Rect (10, 250, 200, 50), "Speichern")) {
			viewer.setCoreElementPosition(Element,position);
			inspectELementWindow = new Rect(250,200,0,0);
		}
		if (GUI.Button (new Rect (220, 250, 200, 50), "Löschen")) {
			viewer.destroyElement(Element);
			this.inspectElementVar = false;
			inspectELementWindow = new Rect(250,200,0,0);
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
			leftProjectWindow=-260;
			topProjectWindow = 50;
			buttonpressed = 1;
		}
		// Button -> open project
		if (GUI.Button (new Rect (0, 90, 220, 50), "Projekt öffnen...")) {
			console ("click -> open existing project (from: " + initWindowId + ")");
			leftProjectWindow=-260;
			topProjectWindow = 50;
			buttonpressed = 2;
		}
		// Button -> options
		if (GUI.Button (new Rect (0, 140, 220, 50), "Optionen")) {
			console ("click -> open options (from: " + initWindowId + ")");
			leftProjectWindow=-260;
			topProjectWindow = 50;
			buttonpressed = 3;
		}
		//Quit
		if (GUI.Button (new Rect (0, 190, 220, 50), "Beenden")) {
			console ("click -> quit (from: " + initWindowId + ")");
			console ("click -> open quitdialog (from: " + initWindowId + ")");
			leftProjectWindow=-260;
			topProjectWindow = 50;
			buttonpressed = 4;
		}
		
	}

	void projectWindowQuit(int windowID) {
		GUI.Label(new Rect(20,50,370,100),"Sind Sie sicher, dass Sie das Programm beenden wollen?");
		if (GUI.Button (new Rect (10, 150, 180, 50), "Ja")) {
			Application.Quit();
		}
		if (GUI.Button (new Rect (210, 150, 180, 50), "Nein")) {
			buttonpressed=0;
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
			buttonpressed = 0;
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
		GUI.Label (new Rect (20, 50, 120, 50), "Projektpfad:");
		Standard["Location"] = GUI.TextField(new Rect(140,50,250,40),Standard["Location"].ToString(),255);
		
		GUI.Label (new Rect (20, 100, 120, 50), "Debugmode:");
		debugmode = GUI.Toggle(new Rect(140, 100, 100, 50), debugmode, "An / Aus");
		
		
		GUI.Label (new Rect (20, 150, 400, 50), "Kamera (Geschwindigkeit):");
		GUI.Label (new Rect (40, 200, 100, 50), "Zoom:");
		Standard["zoom_speed"] = GUI.TextField(new Rect(140,200,250,40),Standard["zoom_speed"].ToString(),255);
		
		GUI.Label (new Rect (40, 250, 100, 50), "Bewegung:");
		Standard["move_speed"] = GUI.TextField(new Rect(140,250,250,40),Standard["move_speed"].ToString(),255);
		
		GUI.Label (new Rect (40, 300, 100, 50), "Drehen:");
		Standard["rotate_speed"] = GUI.TextField(new Rect(140,300,250,40),Standard["rotate_speed"].ToString(),255);
		
		GUI.Label (new Rect (40, 350, 100, 50), "Maus:");
		move_mode_conf = GUI.Toggle(new Rect(140, 350, 250, 40), move_mode_conf, " invertieren");
		
		Standard["rotate_speed"] = rgx.Replace(Standard["rotate_speed"].ToString(), "");
		Standard["move_speed"] = rgx.Replace(Standard["move_speed"].ToString(), "");
		Standard["zoom_speed"] = rgx.Replace(Standard["zoom_speed"].ToString(), "");
		
		// save project
		if (GUI.Button (new Rect (10, 400, 180, 50), "Speichern")) {
			save = saveOptions (initWindowId);
			Standard["move_mode"] = this.move_mode_conf.ToString();
			showMessage("Config geschrieben.");
		}
		if (GUI.Button (new Rect (210, 400, 180, 50), "Abbrechen")) {
			buttonpressed = 0;
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
	public void setInspectElementVar(Boolean value) {
		this.inspectElementVar = value;
	}
	public void inspectElement(GameObject Element) {
		this.Element = Element;
		position = new string[] {	Element.transform.position.x.ToString(),
			Element.transform.position.y.ToString(),
			Element.transform.position.z.ToString()
		};
		inspectELementWindow = new Rect(250,200,430,310);
		this.inspectElementVar = true;
	}
	
	public void console(object message) {
		if(debugmode)
			print(message);
	}
	
	public void showMessage(string message) {
		this.messageTxt = message.ToString();
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
			foreach(string key in Standard.Keys)
			{
				if(key != "debugmode" && key != "move_mode")
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
