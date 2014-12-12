using System;
using TNet;

namespace PickySalamander.JSServer {
	public class ServerApp {
		/// <summary>
		/// Application entry point -- parse the parameters.
		/// </summary>
		public static int Main(string[] args) {
			if(args == null || args.Length == 0) {
				Console.WriteLine("No arguments specified, assuming default values.");
				Console.WriteLine("In the future you can specify your own ports like so:\n");
				Console.WriteLine("   -name \"Your Server\"         <-- Name your server");
				Console.WriteLine("   -tcp [port]                 <-- TCP port for clients to connect to");
				Console.WriteLine("   -udp [port]                 <-- UDP port used for communication");
				Console.WriteLine("   -udpLobby [address] [port]  <-- Start or connect to a UDP lobby");
				Console.WriteLine("   -tcpLobby [address] [port]  <-- Start or connect to a TCP lobby");
				Console.WriteLine("   -ip [ip]                    <-- Choose a specific network interface");
				Console.WriteLine("\nFor example:");
				Console.WriteLine("  TNServer -name \"My Server\" -tcp 5127 -udp 5128 -udpLobby 5129");

				args = new string[] { "TNet Server", "-tcp", "5127", "-udp", "5128", "-udpLobby", "5129" };
			}

			string name = "TNet Server";
			int tcpPort = 0;
			int udpPort = 0;
			string lobbyAddress = null;
			int lobbyPort = 0;
			bool tcpLobby = false;

			for(int i = 0; i < args.Length; ) {
				string param = args[i];
				string val0 = (i + 1 < args.Length) ? args[i + 1] : null;
				string val1 = (i + 2 < args.Length) ? args[i + 2] : null;

				if(val0 != null && val0.StartsWith("-")) {
					val0 = null;
					val1 = null;
				}
				else if(val1 != null && val1.StartsWith("-")) {
					val1 = null;
				}

				if(param == "-name") {
					if(val0 != null) name = val0;
				}
				else if(param == "-tcp") {
					if(val0 != null) int.TryParse(val0, out tcpPort);
				}
				else if(param == "-udp") {
					if(val0 != null) int.TryParse(val0, out udpPort);
				}
				else if(param == "-ip") {
					if(val0 != null) UdpProtocol.defaultNetworkInterface = Tools.ResolveAddress(val0);
				}
				else if(param == "-tcpLobby") {
					if(val1 != null) {
						lobbyAddress = val0;
						int.TryParse(val1, out lobbyPort);
					}
					else int.TryParse(val0, out lobbyPort);
					tcpLobby = true;
				}
				else if(param == "-udpLobby") {
					if(val1 != null) {
						lobbyAddress = val0;
						int.TryParse(val1, out lobbyPort);
					}
					else int.TryParse(val0, out lobbyPort);
					tcpLobby = false;
				}
				else if(param == "-lobby") {
					if(val0 != null) lobbyAddress = val0;
				}

				if(val1 != null) i += 3;
				else if(val0 != null) i += 2;
				else ++i;
			}

			new JSServer(name, tcpPort, udpPort, lobbyAddress, lobbyPort, tcpLobby);
			return 0;
		}
	}
}
