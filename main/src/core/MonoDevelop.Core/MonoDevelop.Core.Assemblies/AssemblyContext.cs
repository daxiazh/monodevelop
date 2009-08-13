// 
// AssemblyContext.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
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

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Mono.PkgConfig;

namespace MonoDevelop.Core.Assemblies
{
	public class AssemblyContext: IAssemblyContext
	{
		Dictionary<string, SystemPackage> assemblyPathToPackage = new Dictionary<string, SystemPackage> ();
		Dictionary<string, SystemAssembly> assemblyFullNameToAsm = new Dictionary<string, SystemAssembly> ();
		Dictionary<string, SystemPackage> packagesHash = new Dictionary<string, SystemPackage> ();
		List<SystemPackage> packages = new List<SystemPackage> ();
		
		public event EventHandler Changed;

		public ICollection<string> GetAssemblyFullNames ()
		{
			return assemblyFullNameToAsm.Keys;
		}
		
		internal SystemPackage RegisterPackage (SystemPackage p, SystemPackageInfo pinfo, bool isInternal, PackageAssemblyInfo[] assemblyFiles)
		{
			List<SystemAssembly> asms = new List<SystemAssembly> ();
			foreach (PackageAssemblyInfo asm in assemblyFiles)
				asms.Add (AddAssembly (asm.File, new AssemblyInfo (asm), p));
			p.Initialize (pinfo, asms, isInternal);
			packages.Add (p);
			packagesHash [pinfo.Name] = p;
			
			if (Changed != null)
				Changed (this, EventArgs.Empty);
			
			return p;
		}
		
		public void UnregisterPackage (string name, string version)
		{
			SystemPackage p = GetPackage (name, version);
			if (!p.IsInternalPackage)
				throw new InvalidOperationException ("Only internal packages can be unregistered");
			
			packages.Remove (p);
			packagesHash.Remove (name);

			if (Changed != null)
				Changed (this, EventArgs.Empty);
		}
		
		public IEnumerable<SystemPackage> GetPackages ()
		{
			return packages; 
		}
		
		public IEnumerable<SystemPackage> GetPackages (TargetFramework fx)
		{
			foreach (SystemPackage pkg in packages) {
				if (pkg.TargetFramework == fx.Id) {
					yield return pkg;
				}
				else if (pkg.IsBaseCorePackage) {
					if (pkg.TargetFramework == fx.BaseCoreFramework)
						yield return pkg;
				}
				else if (pkg.IsFrameworkPackage) {
					if (fx.IsCompatibleWithFramework (pkg.TargetFramework)) {
						TargetFramework packageFx = Runtime.SystemAssemblyService.GetTargetFramework (pkg.TargetFramework);
						if (packageFx.BaseCoreFramework == fx.BaseCoreFramework)
							yield return pkg;
					}
				}
				else if (fx.IsCompatibleWithFramework (pkg.TargetFramework))
					yield return pkg;
			}
		}

		public SystemAssembly[] GetAssembliesFromFullName (string fullname)
		{
			List<SystemAssembly> asms = new List<SystemAssembly> (GetAssembliesFromFullNameInternal (fullname));
			return asms.ToArray ();
		}
		
		IEnumerable<SystemAssembly> GetAssembliesFromFullNameInternal (string fullname)
		{
			Initialize ();
			fullname = NormalizeAsmName (fullname);
			SystemAssembly asm;
			if (!assemblyFullNameToAsm.TryGetValue (fullname, out asm))
				yield break;

			while (asm != null) {
				yield return asm;
				asm = asm.NextSameName;
			}
		}
		
		internal string GetCorlibFramework (string fullName)
		{
			// Called during initialization, so the Initialize() call is not required here
			fullName = NormalizeAsmName (fullName);
			SystemAssembly asm;
			if (assemblyFullNameToAsm.TryGetValue (fullName, out asm))
				return asm.Package.TargetFramework;
			else
				return null;
		}
		
		public IEnumerable<SystemAssembly> GetAssemblies ()
		{
			Initialize ();
			foreach (SystemPackage pkg in packages) {
				foreach (SystemAssembly asm in pkg.Assemblies)
					yield return asm;
			}
		}
		
		public IEnumerable<SystemAssembly> GetAssemblies (TargetFramework fx)
		{
			Initialize ();
			
			if (fx == null) {
				foreach (SystemPackage pkg in packages) {
					if (pkg.IsFrameworkPackage)
						continue;
					foreach (SystemAssembly asm in pkg.Assemblies)
						yield return asm;
				}
			} else {
				foreach (SystemPackage pkg in GetPackages (fx)) {
					foreach (SystemAssembly asm in pkg.Assemblies)
						yield return asm;
				}
			}
		}
		
		public SystemAssembly GetAssemblyFromFullName (string fullname, string package, TargetFramework fx)
		{
			if (package == null) {
				SystemAssembly found = null;
				SystemAssembly gacFound = null;
				foreach (SystemAssembly asm in GetAssembliesFromFullNameInternal (fullname)) {
					found = asm;
					if (asm.Package.IsFrameworkPackage && fx != null && asm.Package.TargetFramework == fx.Id)
						return asm;
					if (asm.Package.IsGacPackage)
						gacFound = asm;
				}
				return gacFound ?? found;
			}
			
			foreach (SystemAssembly asm in GetAssembliesFromFullNameInternal (fullname)) {
				if (package == asm.Package.Name)
					return asm;
			}
			return null;
		}
		
		public SystemPackage[] GetPackagesFromFullName (string fullname)
		{
			List<SystemPackage> packs = new List<SystemPackage> ();
			foreach (SystemAssembly asm in GetAssembliesFromFullNameInternal (fullname))
				packs.Add (asm.Package);
			return packs.ToArray ();
		}

		public SystemPackage GetPackage (string name)
		{
			Initialize ();
			return GetPackageInternal (name);
		}
		
		//safe to be called from the background initialization thread
		internal protected SystemPackage GetPackageInternal (string name)
		{
			SystemPackage res;
			packagesHash.TryGetValue (name, out res);
			return res;
		}

		public SystemPackage GetPackage (string name, string version)
		{
			Initialize ();
			return GetPackageInternal (name, version);
		}

		internal SystemPackage GetPackageInternal (string name, string version)
		{
			foreach (SystemPackage p in packages)
				if (p.Name == name && p.Version == version)
					return p;
			return null;
		}

		public SystemPackage GetPackageFromPath (string path)
		{
			Initialize ();
			return assemblyPathToPackage.ContainsKey (path) ? assemblyPathToPackage [path] : null;
		}
		
		public static string NormalizeAsmName (string name)
		{
			int i = name.ToLower ().IndexOf (", publickeytoken=null");
			if (i != -1)
				name = name.Substring (0, i).Trim ();
			i = name.ToLower ().IndexOf (", processorarchitecture=");
			if (i != -1)
				name = name.Substring (0, i).Trim ();
			return name;
		}
	
		// Returns the installed version of the given assembly name
		// (it returns the full name of the installed assembly).
		public string FindInstalledAssembly (string fullname, string package, TargetFramework fx)
		{
			Initialize ();
			fullname = NormalizeAsmName (fullname);
			
			SystemAssembly fasm = GetAssemblyFromFullName (fullname, package, fx);
			if (fasm != null)
				return fullname;
			
			// Try to find a newer version of the same assembly
			
			if (fx == null) {
				foreach (SystemAssembly asm in FindNewerAssembliesSameName (fullname))
					if (package == null || asm.Package.Name == package)
						return asm.FullName;
				return null;
			}
			
			string bestMatch = null;
			foreach (SystemAssembly asm in FindNewerAssembliesSameName (fullname)) {
				if (fx.Id == asm.Package.TargetFramework) {
					if (package == null || asm.Package.Name == package)
						return asm.FullName;
				}
				if (fx.IsCompatibleWithFramework (asm.Package.TargetFramework)) {
					if (package != null && asm.Package.Name == package)
						return asm.FullName;
					bestMatch = asm.FullName;
				}
			}
			return bestMatch;
		}
		
		IEnumerable<SystemAssembly> FindNewerAssembliesSameName (string fullname)
		{
			AssemblyName reqName = ParseAssemblyName (fullname);
			foreach (KeyValuePair<string,SystemAssembly> pair in assemblyFullNameToAsm) {
				AssemblyName foundName = ParseAssemblyName (pair.Key);
				if (reqName.Name == foundName.Name && (reqName.Version == null || reqName.Version.CompareTo (foundName.Version) < 0)) {
					SystemAssembly asm = pair.Value;
					while (asm != null) {
						yield return asm;
						asm = asm.NextSameName;
					}
				}
			}
		}
	
		public string GetAssemblyLocation (string assemblyName, TargetFramework fx)
		{
			return GetAssemblyLocation (assemblyName, null, fx);
		}
		
		public virtual string GetAssemblyLocation (string assemblyName, string package, TargetFramework fx)
		{
			Initialize ();
			
			assemblyName = NormalizeAsmName (assemblyName); 
			
			SystemAssembly asm = GetAssemblyFromFullName (assemblyName, package, fx);
			if (asm != null)
				return asm.Location;
			
			if (assemblyName == "mscorlib" || assemblyName.StartsWith ("mscorlib,"))
				return typeof(object).Assembly.Location;
			
			return null;
		}
		
		public virtual bool AssemblyIsInGac (string aname)
		{
			return false;
		}
		
		protected virtual IEnumerable<string> GetAssemblyDirectories ()
		{
			yield break;
		}

		
		public static void ParseAssemblyName (string assemblyName, out string name, out string version, out string culture, out string token)
		{
			name = version = culture = token = null;
			string[] parts = assemblyName.Split (',');
			if (parts.Length < 1)
				return;
			name = parts[0].Trim ();
			
			if (parts.Length < 2)
				return;
			int i = parts[1].IndexOf ('=');
			version = i != -1 ? parts[1].Substring (i+1).Trim () : parts[1].Trim ();
			
			if (parts.Length < 3)
				return;
			i = parts[2].IndexOf ('=');
			culture = i != -1 ? parts[2].Substring (i+1).Trim () : parts[2].Trim ();
			if (culture == "neutral") culture = "";
			
			if (parts.Length < 4)
				return;
			i = parts[3].IndexOf ('=');
			token = i != -1 ? parts[3].Substring (i+1).Trim () : parts[3].Trim ();
		}
		
		// Given the full name of an assembly, returns the corresponding full assembly name
		// in the specified target CLR version, or null if it doesn't exist in that version.
		public string GetAssemblyNameForVersion (string fullName, TargetFramework fx)
		{
			return GetAssemblyNameForVersion (fullName, null, fx);
		}
		
		public string GetAssemblyNameForVersion (string fullName, string packageName, TargetFramework fx)
		{
			SystemAssembly asm = GetAssemblyForVersion (fullName, packageName, fx);
			if (asm != null)
				return asm.FullName;
			else
				return null;
		}
		
		// Given the full name of an assembly, returns the corresponding full assembly name
		// in the specified target CLR version, or null if it doesn't exist in that version.
		public SystemAssembly GetAssemblyForVersion (string fullName, string packageName, TargetFramework fx)
		{
			Initialize ();

			fullName = NormalizeAsmName (fullName);
			SystemAssembly asm = GetAssemblyFromFullName (fullName, packageName, fx);

			if (asm == null)
				return null;
			
			if (!asm.Package.IsFrameworkPackage) {
				// Return null if the package is not compatible with the requested version
				if (fx.IsCompatibleWithFramework (asm.Package.TargetFramework))
					return asm;
				else
					return null;
			}
			if (fx.IsExtensionOfFramework (asm.Package.TargetFramework))
				return asm;

			// We have to find a core package which contains whits assembly
			string fname = Path.GetFileName ((string) asm.Location);
			
			foreach (KeyValuePair<string, SystemAssembly> pair in assemblyFullNameToAsm) {
				SystemPackage rpack = pair.Value.Package;
				if (rpack.IsFrameworkPackage && fx.IsExtensionOfFramework (rpack.TargetFramework) && Path.GetFileName (pair.Value.Location) == fname)
					return pair.Value;
			}
			return null;
		}
		
		public string GetAssemblyFullName (string assemblyName, TargetFramework fx)
		{
			Initialize ();
			
			assemblyName = NormalizeAsmName (assemblyName);
			
			// Fast path for known assemblies.
			if (assemblyFullNameToAsm.ContainsKey (assemblyName))
				return assemblyName;

			// Look in assemblies of the framework. Done here since later steps look in the gac
			// without taking into account the framework.
			foreach (SystemAssembly sa in GetAssemblies (fx)) {
				if (sa.Package.IsGacPackage && sa.Name == assemblyName)
					return sa.FullName;
			}

			if (File.Exists (assemblyName))
				return SystemAssemblyService.GetAssemblyName (assemblyName);

			string file = GetAssemblyLocation (assemblyName, fx);
			if (file != null)
				return SystemAssemblyService.GetAssemblyName (file);
			else
				return null;
		}
		
		protected virtual void Initialize ()
		{
		}
		
		internal SystemAssembly AddAssembly (string assemblyfile, AssemblyInfo ainfo, SystemPackage package)
		{
			if (!File.Exists (assemblyfile))
				return null;

			try {
				SystemAssembly asm = SystemAssembly.FromFile (assemblyfile, ainfo);
				SystemAssembly prevAsm;
				if (assemblyFullNameToAsm.TryGetValue (asm.FullName, out prevAsm)) {
					asm.NextSameName = prevAsm.NextSameName;
					prevAsm.NextSameName = asm;
				} else
					assemblyFullNameToAsm [asm.FullName] = asm;
				assemblyPathToPackage [assemblyfile] = package;
				return asm;
			} catch {
				return null;
			}
		}
		
		internal void InternalAddPackage (SystemPackage package)
		{
			packages.Add (package);
		}

		public AssemblyName ParseAssemblyName (string fullname)
		{
			AssemblyName aname = new AssemblyName ();
			int i = fullname.IndexOf (',');
			if (i == -1) {
				aname.Name = fullname.Trim ();
				return aname;
			}
			
			aname.Name = fullname.Substring (0, i).Trim ();
			i = fullname.IndexOf ("Version", i+1);
			if (i == -1)
				return aname;
			i = fullname.IndexOf ('=', i);
			if (i == -1) 
				return aname;
			int j = fullname.IndexOf (',', i);
			if (j == -1)
				aname.Version = new Version (fullname.Substring (i+1).Trim ());
			else
				aname.Version = new Version (fullname.Substring (i+1, j - i - 1).Trim ());
			return aname;
		}
	}
}
