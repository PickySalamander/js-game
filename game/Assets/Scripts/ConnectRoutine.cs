using UnityEngine;
using TNet;
using System.IO;
using System;

public class ConnectRoutine : TaskRunner {
	private void Awake() {
		AddTask(new FindServer());
		AddTask(new WaitConnect());
	}

	private class FindServer : BaseTask {
		public override void Update() {
			List<ServerList.Entry> list = TNLobbyClient.knownServers.list;

			if(list.Count > 0) {
				ServerList.Entry ent = list[0];
				TNManager.Connect(ent.internalAddress, ent.internalAddress);
				Console.Instance.addToConsole("Connecting to " + ent.internalAddress.ToString());
				Finished();
			}
		}
	}

	private class WaitConnect : BaseTask {
		public override void Update() {
			if(!TNManager.isTryingToConnect) {
				if(TNManager.isConnected) {
					Finished();
					Console.Instance.addToConsole("Connected!");

					BinaryWriter writer = TNManager.BeginSend(Packet.RequestAddFunction);
					writer.Write("testObj");

					Guid id = Guid.NewGuid();
					Debug.Log("GUID: " + id.ToString());

					writer.Write(id.ToByteArray());
					TNManager.EndSend(true);

					Console.Instance.input.isSelected = true;
				}
				else {
					Console.Instance.addToConsole("Failed to connect, check log!");
					Runner.EnablePrevious(this);
				}
			}
		}
	}
}
