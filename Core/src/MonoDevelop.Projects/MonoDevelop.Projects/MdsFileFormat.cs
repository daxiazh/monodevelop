//
// CmbxFileFormat.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.IO;
using System.Xml;
using MonoDevelop.Core;
using MonoDevelop.Projects.Serialization;

namespace MonoDevelop.Projects
{
	internal class MdsFileFormat: IFileFormat
	{
		public string Name {
			get { return "MonoDevelop Solution"; }
		}
		
		public string GetValidFormatName (string fileName)
		{
			return Path.ChangeExtension (fileName, ".mds");
		}
		
		public bool CanReadFile (string file)
		{
			return String.Compare (Path.GetExtension (file), ".mds", true) == 0;
		}
		
		public bool CanWriteFile (object obj)
		{
			return obj is Combine;
		}
		
		public void WriteFile (string file, object node, IProgressMonitor monitor)
		{
			Combine combine = node as Combine;
			if (combine == null)
				throw new InvalidOperationException ("The provided object is not a Combine");

			string tmpfilename = String.Empty;
			try {
				try {
					if (File.Exists (file))
						tmpfilename = Path.GetTempFileName ();
				} catch (IOException) {
				}

				if (tmpfilename == String.Empty) {
					WriteFileInternal (file, file, combine, monitor);
				} else {
					WriteFileInternal (file, tmpfilename, combine, monitor);
					File.Delete (file);
					File.Move (tmpfilename, file);
				}
			} catch {
				if (tmpfilename != String.Empty)
					File.Delete (tmpfilename);
				throw;
			}
		}

		void WriteFileInternal (string actualFile, string outFile, Combine combine, IProgressMonitor monitor)
		{
			StreamWriter sw = new StreamWriter (outFile);
			try {
				monitor.BeginTask (GettextCatalog.GetString ("Saving solution: {0}", actualFile), 1);
				XmlTextWriter tw = new XmlTextWriter (sw);
				tw.Formatting = Formatting.Indented;
				DataSerializer serializer = new DataSerializer (Services.ProjectService.DataContext, actualFile);
				CombineWriterV2 combineWriter = new CombineWriterV2 (serializer, monitor);
				combineWriter.WriteCombine (tw, combine);
			} catch (Exception ex) {
				monitor.ReportError (GettextCatalog.GetString ("Could not save solution: {0}", actualFile), ex);
				throw;
			} finally {
				monitor.EndTask ();
				sw.Close ();
			}
		}
	
		public object ReadFile (string file, IProgressMonitor monitor)
		{
			XmlTextReader reader = new XmlTextReader (new StreamReader (file));
			reader.MoveToContent ();
			
			string version = reader.GetAttribute ("version");
			if (version == null) version = reader.GetAttribute ("fileversion");
			
			DataSerializer serializer = new DataSerializer (Services.ProjectService.DataContext, file);
			ICombineReader combineReader = null;
			
			if (version == "1.0" || version == "1") {
				combineReader = new CombineReaderV1 (serializer, monitor);
				monitor.ReportWarning (GettextCatalog.GetString ("The file '{0}' is using an old solution file format. It will be automatically converted to the current format.", file));
			}
			else if (version == "2.0")
				combineReader = new CombineReaderV2 (serializer, monitor);
			
			try {
				if (combineReader != null)
					return combineReader.ReadCombine (reader);
				else
					throw new UnknownProjectVersionException (file, version);
			} catch (Exception ex) {
				monitor.ReportError (GettextCatalog.GetString ("Could not load solution: {0}", file), ex);
				throw;
			} finally {
				reader.Close ();
			}
		}
	}
}
