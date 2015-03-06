using System;
using TNet;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using PickySalamander.JSServer.Code;

namespace PickySalamander.JSServer {
	/// <summary>
	/// This is an example of a stand-alone server. You don't need Unity in order to compile and run it.
	/// Running it as-is will start a Game Server on ports 5127 (TCP) and 5128 (UDP), as well as a Lobby Server on port 5129.
	/// </summary>

	public class JSServer {
		private JSServeEngine engine;

		private GameServer gameServer = null;
		private LobbyServer lobbyServer = null;

		/// <summary>
		/// Start the server.
		/// </summary>

		public JSServer(string name, int tcpPort, int udpPort, string lobbyAddress, int lobbyPort, bool useTcp) {
			List<IPAddress> ips = Tools.localAddresses;
			string text = "\nLocal IPs: " + ips.size;

			for(int i = 0; i < ips.size; ++i) {
				text += "\n  " + (i + 1) + ": " + ips[i];
				if(ips[i] == TNet.Tools.localAddress) text += " (Primary)";
			}
			Console.WriteLine(text + "\n");
			{
				// Universal Plug & Play is used to determine the external IP address,
				// and to automatically open up ports on the router / gateway.
				UPnP up = new UPnP();
				up.WaitForThreads();

				if(up.status == UPnP.Status.Success) {
					Console.WriteLine("Gateway IP:  " + up.gatewayAddress);
				}
				else {
					Console.WriteLine("Gateway IP:  None found");
					up = null;
				}

				Console.WriteLine("External IP: " + Tools.externalAddress);
				Console.WriteLine("");

				if(tcpPort > 0) {
					gameServer = new GameServer();
					gameServer.name = name;
					gameServer.onCustomPacket = OnCustomPacket;

					if(!string.IsNullOrEmpty(lobbyAddress)) {
						// Remote lobby address specified, so the lobby link should point to a remote location
						IPEndPoint ip = Tools.ResolveEndPoint(lobbyAddress, lobbyPort);
						if(useTcp) gameServer.lobbyLink = new TcpLobbyServerLink(ip);
						else gameServer.lobbyLink = new UdpLobbyServerLink(ip);

					}
					else if(lobbyPort > 0) {
						// Server lobby port should match the lobby port on the client
						if(useTcp) {
							lobbyServer = new TcpLobbyServer();
							lobbyServer.Start(lobbyPort);
							if(up != null) up.OpenTCP(lobbyPort, OnPortOpened);
						}
						else {
							lobbyServer = new UdpLobbyServer();
							lobbyServer.Start(lobbyPort);
							if(up != null) up.OpenUDP(lobbyPort, OnPortOpened);
						}

						// Local lobby server
						gameServer.lobbyLink = new LobbyServerLink(lobbyServer);
					}

					// Start the actual game server and load the save file
					gameServer.Start(tcpPort, udpPort);
					gameServer.LoadFrom("server.dat");
				}
				else if(lobbyPort > 0) {
					if(useTcp) {
						if(up != null) up.OpenTCP(lobbyPort, OnPortOpened);
						lobbyServer = new TcpLobbyServer();
						lobbyServer.Start(lobbyPort);
					}
					else {
						if(up != null) up.OpenUDP(lobbyPort, OnPortOpened);
						lobbyServer = new UdpLobbyServer();
						lobbyServer.Start(lobbyPort);
					}
				}

				// Open up ports on the router / gateway
				if(up != null) {
					if(tcpPort > 0) up.OpenTCP(tcpPort, OnPortOpened);
					if(udpPort > 0) up.OpenUDP(udpPort, OnPortOpened);
				}

				engine = new JSServeEngine();

				for(; ; ) {
					Console.WriteLine("Press 'q' followed by ENTER when you want to quit.\n");
					string command = Console.ReadLine();
					if(command == "q") break;
				}
				Console.WriteLine("Shutting down...");

				// Close all opened ports
				if(up != null) {
					up.Close();
					up.WaitForThreads();
					up = null;
				}

				// Stop the game server
				if(gameServer != null) {
					gameServer.SaveTo("server.dat");
					gameServer.Stop();
					gameServer = null;
				}

				// Stop the lobby server
				if(lobbyServer != null) {
					lobbyServer.Stop();
					lobbyServer = null;
				}
			}
			Console.WriteLine("The server has shut down. Press ENTER to terminate the application.");
			Console.ReadLine();
		}

		/// <summary>
		/// UPnP notification of a port being open.
		/// </summary>

		private void OnPortOpened(UPnP up, int port, ProtocolType protocol, bool success) {
			if(success) {
				Console.WriteLine("UPnP: " + protocol.ToString().ToUpper() + " port " + port + " was opened successfully.");
			}
			else {
				Console.WriteLine("UPnP: Unable to open " + protocol.ToString().ToUpper() + " port " + port);
			}
		}

		private void OnCustomPacket(TcpPlayer player, TNet.Buffer buffer, BinaryReader reader, Packet request, bool reliable) {
			switch(request) {
				case Packet.RequestRunScript:
					string jsCode = reader.ReadString();
					string result = engine.Execute(jsCode);

					BinaryWriter writer = gameServer.BeginSend(Packet.ResponseRunScript);
					writer.Write(result);
					gameServer.EndSend(true, player);
					break;
				case Packet.RequestAddFunction:
					string functionName = reader.ReadString();
					Guid id = new Guid(reader.ReadBytes(16));

					engine.AddFunction(new JSFuncCallback(id, functionName));
					
					break;
			}
		}
	}
}