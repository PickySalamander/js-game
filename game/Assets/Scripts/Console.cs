using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TNet;
using System.Net;
using System;

public class Console : SingletonBehavior<Console> {
	public string[] startupStrs = new string[] {
		"Picky Console (version 1.0)",
		"Copyright (C) 2015 PickySalamander Electronics",
		"",
		"",
		"Looking for server..."
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

		TNManager.SetPacketHandler(Packet.JSCodeResult, OnResult);
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

		input.collider.enabled = TNManager.isConnected;
	}

	private void OnSubmit() {
		string text = System.String.Copy(input.value);

		input.value = "";

		BinaryWriter writer = TNManager.BeginSend(Packet.RunJSCode);
		writer.Write(text);
		TNManager.EndSend(true);
	}

	private void OnResult(Packet response, BinaryReader reader, IPEndPoint source) {
		try {
			string result = reader.ReadString();
			addToConsole(result);
		}
		catch(Exception e) {
			Debug.LogError("Failed to console log result");
			Debug.LogException(e);
		}
	}
}