// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;
using MonoDevelop.Core.Gui.Components;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Serialization;

namespace ILAsmBinding
{
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class ILAsmCompilerParameters: ICloneable
	{
		// Add options here
		
		public object Clone ()
		{
			return MemberwiseClone ();
		}
	}
}
