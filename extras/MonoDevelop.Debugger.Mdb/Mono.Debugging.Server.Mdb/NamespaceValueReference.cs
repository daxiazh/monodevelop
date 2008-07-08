// NamespaceValueReference.cs
//
// Author:
//   Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

using System;
using System.Collections.Generic;
using Mono.Debugger;
using Mono.Debugger.Languages;
using MD = Mono.Debugger;
using Mono.Debugging.Client;

namespace DebuggerServer
{
	public class NamespaceValueReference: ValueReference
	{
		MD.StackFrame frame;
		string name;
		string namspace;
		string[] namespaces;
		
		public NamespaceValueReference (MD.StackFrame frame, string name): base (frame.Thread)
		{
			this.frame = frame;
			this.namspace = name;
			int i = namspace.LastIndexOf ('.');
			if (i != -1)
				this.name = namspace.Substring (i+1);
			else
				this.name = namspace;
		}

		public override Mono.Debugger.Languages.TargetObject Value {
			get {
				throw new NotSupportedException();
			}
			set {
				throw new NotSupportedException();
			}
		}

		
		public override Mono.Debugger.Languages.TargetType Type {
			get {
				throw new NotSupportedException();
			}
		}

		
		public override object ObjectValue {
			get {
				throw new NotSupportedException ();
			}
		}

		
		public override string Name {
			get {
				return name;
			}
		}

		
		public override ObjectValueFlags Flags {
			get {
				return ObjectValueFlags.Literal;
			}
		}

		public override ValueReference GetChild (string name)
		{
			string newNs = namspace + "." + name;
			
			TargetType t = frame.Language.LookupType (newNs);
			if (t != null)
				return new TypeValueReference (frame.Thread, t);
				
			if (namespaces == null)
				namespaces = frame.Method.GetNamespaces ();
			
			foreach (string ns in namespaces) {
				if (ns == newNs || ns.StartsWith (newNs + "."))
					return new NamespaceValueReference (frame, newNs);
			}
			return null;
		}

		public override ObjectValue[] GetChildren (Mono.Debugging.Client.ObjectPath path, int index, int count)
		{
			List<ObjectValue> obs = new List<ObjectValue> ();
			foreach (ValueReference val in GetChildReferences ()) {
				obs.Add (val.CreateObjectValue ());
			}
			return obs.ToArray ();
		}
		
		public override IEnumerable<ValueReference> GetChildReferences ()
		{
			List<string> list = new List<string> ();
			
			if (namespaces == null)
				namespaces = frame.Method.GetNamespaces ();
			
			// Child types
			
			List<string> types = new List<string> ();
			Module[] modules = frame.Thread.Process.Modules;
			foreach (Module mod in modules) {
				try {
					foreach (SourceFile sf in mod.Sources) {
						foreach (MethodSource met in sf.Methods) {
							string tn = met.DeclaringType.Name;
							int i = tn.LastIndexOf ('.');
							if (i != -1 && tn.Substring (0,i) == namspace && !types.Contains (tn))
								types.Add (tn);
						}
					}
				} catch (Exception ex) {
					Console.WriteLine ("pp: " + ex);
				}
			}
			
			foreach (string typeName in types) {
				TargetType tt = frame.Language.LookupType (typeName);
				if (tt != null)
					yield return new TypeValueReference (frame.Thread, tt);
			}
			
			// Child namespaces
			
			string basens = namspace + ".";
			foreach (string ns in namespaces) {
				if (ns.StartsWith (basens)) {
					string subns = ns;
					int i = subns.IndexOf ('.', basens.Length);
					if (i != -1)
						subns = subns.Substring (0, i);
					if (!list.Contains (subns))
						list.Add (subns);
				}
			}
				
			foreach (string ns in list)
				yield return new NamespaceValueReference (frame, ns);
		}

		public override Mono.Debugging.Client.ObjectValue CreateObjectValue ()
		{
			Connect ();
			return Mono.Debugging.Client.ObjectValue.CreateObject (this, new ObjectPath (Name), namspace, namspace, Flags, null);
		}

		public override string CallToString ()
		{
			return namspace;
		}
	}
}
