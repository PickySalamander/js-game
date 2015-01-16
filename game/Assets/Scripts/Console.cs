using UnityEngine;
using System.Collections.Generic;

public class Console : MonoBehaviour {
	public string[] startupStrs = new string[] {
		"Picky Console (version 1.0)",
		"Copyright (C) 2015 PickySalamander Electronics",
		"",
		"",
		"Starting console now..."
	};

	public UILabel output;
	public UIInput input;

	//TODO find collider

	private Queue<string> buffer = new Queue<string>();

	// Use this for initialization
	private void Start() {
		if(startupStrs != null) {
			addToConsole(startupStrs);
		}

		input.onSubmit.Add(new EventDelegate(OnSubmit));
	}

	public void addToConsole(IEnumerable<string> strs) {
		foreach(string str in strs) {
			addToConsole(str);
		}
	}

	public void addToConsole(string str) {
		buffer.Enqueue(str);
	}

	// Update is called once per frame
	private void Update() {
		while(buffer.Count != 0) {
			string elem = buffer.Dequeue();

			if(output.text.Length > 0) {
				elem = "\n" + elem;
			}

			output.text += elem;
		}
	}

	private void OnSubmit() {
	}
}