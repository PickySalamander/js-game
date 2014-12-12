using System;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace PickySalamander.JSServer {
	public class JSServeEngine {
		private V8ScriptEngine engine;

		public JSServeEngine() {
			Open();
		}

		public void Open() {
			engine = new V8ScriptEngine();
			engine.AddHostType("Console", typeof(Console));
		}

		public void Close() {
			engine.Dispose();
		}

		public void Execute(string command) {
			try {
				engine.Execute(command);
			}
			catch(Exception e) {
				Console.WriteLine("Error running JS code ( \"" + command + "\" ): " + e.ToString());
			}
		}
	}
}
