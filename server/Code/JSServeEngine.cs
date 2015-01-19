using System;
using TNet;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System.IO;

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

		public string Execute(string command) {
			try {
				object result = engine.Evaluate(command);
				return result.ToString();
			}
			catch(Exception e) {
				return "Error running JS code ( \"" + command + "\" ): " + e.ToString();
			}
		}
	}
}
