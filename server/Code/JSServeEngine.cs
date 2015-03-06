using Microsoft.ClearScript.V8;
using PickySalamander.JSServer.Code;
using System;

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

		public void AddFunction(JSFuncCallback func) {
			try {
				engine.Script[func.name] = func;
			}
			catch(Exception e) {
				Console.WriteLine("Unable to add to engine: " + e.ToString());
			}
		}
	}
}
