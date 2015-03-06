using System;
using System.Dynamic;

namespace PickySalamander.JSServer.Code {
	public class JSFuncCallback : DynamicObject {
		public readonly Guid id;
		public readonly string name;

		public JSFuncCallback(Guid id, string name) {
			this.id = id;
			this.name = name;
		}

		public override bool TryInvoke(InvokeBinder binder, object[] args, out object result) {
			//TODO write callback to client
			result = "cool test bitches!";
			return true;
		}
	}
}
