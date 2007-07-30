/*
   Copyright (C) 2006  Matthias Braun <matze@braunis.de>
   Scott Ellington <scott.ellington@gmail.com>

   This library is free software; you can redistribute it and/or
   modify it under the terms of the GNU Lesser General Public
   License as published by the Free Software Foundation; either
   version 2 of the License, or (at your option) any later version.

   This library is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
   Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public
   License along with this library; if not, write to the
   Free Software Foundation, Inc., 59 Temple Place - Suite 330,
   Boston, MA 02111-1307, USA.
   */

using System;
using System.Collections;
using System.IO;
using System.Text;

using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Projects.Serialization;
using MonoDevelop.Deployment;

using Mono.Unix.Native;

namespace MonoDevelop.Autotools
{
	public class SolutionDeployer
	{
		string solution_dir;
		string solution_name;
		string solution_version;

		AutotoolsContext context;
		bool generateAutotools;

		public SolutionDeployer (bool generateAutotools)
		{
			this.generateAutotools = generateAutotools;
		}

		public bool HasGeneratedFiles ( Combine combine )
		{
			string dir = Path.GetDirectoryName(combine.FileName);
			if (generateAutotools) {
				return File.Exists (Path.Combine (dir, "configure.ac")) &&
						File.Exists (Path.Combine (dir, "autogen.sh"));
			} else {
				return File.Exists (Path.Combine (dir, "Makefile")) &&
						File.Exists (Path.Combine (dir, "configure")) &&
						File.Exists (Path.Combine (dir, "rules.make"));
			}
		}
		
		public bool CanDeploy (CombineEntry entry)
		{
			IMakefileHandler handler = AutotoolsContext.GetMakefileHandler (entry, generateAutotools);
			if ( handler == null || !handler.CanDeploy (entry) ) return false;
			return true;
		}

		public bool GenerateFiles (DeployContext ctx, Combine combine, string defaultConf, IProgressMonitor monitor )
		{
			string filesString = generateAutotools ? "Autotools files" : "Makefiles";
			monitor.BeginTask ( GettextCatalog.GetString ("Generating {0} for Solution {1}", filesString, combine.Name), 1 );

			try
			{
				solution_dir = Path.GetDirectoryName(combine.FileName);

				string[] configs = new string [ combine.Configurations.Count ];
				for (int ii=0; ii < configs.Length; ii++ )
					configs [ii] = combine.Configurations[ii].Name;
					
				context = new AutotoolsContext ( ctx, solution_dir, configs );
				
				IMakefileHandler handler = AutotoolsContext.GetMakefileHandler ( combine, generateAutotools );
				if ( handler == null || !handler.CanDeploy (combine) )
					throw new Exception ( GettextCatalog.GetString ("MonoDevelop does not currently support generating {0} for one (or more) child projects.", filesString) );

				solution_name = combine.Name;
				solution_version = AutotoolsContext.EscapeStringForAutoconf (combine.Version, true);

				Makefile makefile = handler.Deploy ( context, combine, monitor );
				string path = Path.Combine (solution_dir, "Makefile");
				if (generateAutotools) {
					context.AddAutoconfFile (path);
					CreateAutoGenDotSH (context, monitor);
					CreateConfigureDotAC (combine, defaultConf, monitor, context);
				} else {
					CreateConfigureScript (combine, defaultConf, context, monitor);

					monitor.Log.WriteLine ( GettextCatalog.GetString ("Creating rules.make"));
					string rules_make_path = Path.Combine (solution_dir, "rules.make");
					File.Copy (Path.Combine (context.TemplateDir, "rules.make"), rules_make_path, true);
					context.AddGeneratedFile (rules_make_path);
				}

				CreateMakefileInclude ( context, monitor );
				AddTopLevelMakefileVars ( makefile, monitor );

				if (generateAutotools)
					path = path + ".am";
				StreamWriter writer = new StreamWriter (path);
				makefile.Write ( writer );
				writer.Close ();

				context.AddGeneratedFile (path);

				monitor.ReportSuccess ( GettextCatalog.GetString ("{0} were successfully generated.", filesString ) );
				monitor.Step (1);
			}
			catch ( Exception e )
			{
				monitor.ReportError ( GettextCatalog.GetString ("{0} could not be generated: ", filesString ), e );
				DeleteGeneratedFiles ( context );
				return false;
			}
			finally
			{
				monitor.EndTask ();
			}
			return true;
		}

		public void Deploy ( DeployContext ctx, Combine combine, string targetDir, bool generateFiles, IProgressMonitor monitor  )
		{
			Deploy ( ctx, combine, combine.ActiveConfiguration.Name, targetDir, generateFiles, monitor  );
		}
		
		public void Deploy ( DeployContext ctx, Combine combine, string defaultConf, string targetDir, bool generateFiles, IProgressMonitor monitor  )
		{
			if (generateFiles) {
				if ( !GenerateFiles ( ctx, combine, defaultConf, monitor ) ) 
					return;
			}
			
			monitor.BeginTask ( GettextCatalog.GetString( "Deploying Solution to Tarball" ) , 3 );
			try
			{
				string baseDir = Path.GetDirectoryName ( combine.FileName);
	
				ProcessWrapper ag_process = Runtime.ProcessService.StartProcess ( "sh", 
						generateAutotools ? "autogen.sh" : "configure",
						baseDir, 
						monitor.Log, 
						monitor.Log, 
						null );
				ag_process.WaitForOutput ();
				
				if ( ag_process.ExitCode > 0 )
					throw new Exception ( GettextCatalog.GetString ("An unspecified error occurred while running '{0}'", generateAutotools ? "autogen.sh" : "configure") );
				
				monitor.Step ( 1 );

				StringWriter sw = new StringWriter ();
				LogTextWriter chainedOutput = new LogTextWriter ();
				chainedOutput.ChainWriter (monitor.Log);
				chainedOutput.ChainWriter (sw);

				ProcessWrapper process = Runtime.ProcessService.StartProcess ( "make", 
						"dist", 
						baseDir, 
						chainedOutput, 
						monitor.Log, 
						null );
				process.WaitForOutput ();

				if ( process.ExitCode > 0 )
					throw new Exception ( GettextCatalog.GetString ("An unspecified error occurred while running '{0}'", "make dist") );

				monitor.Step ( 1 );

				// FIXME: hackish way to get the created tarball's filename
				string output = sw.ToString();
				int targz = output.LastIndexOf  ( "tar.gz" );
				int begin = output.LastIndexOf ( '>', targz );

				string filename = output.Substring ( begin + 1, (targz - begin) + 5 ).Trim ();
				
				Runtime.FileService.CopyFile (Path.Combine (baseDir, filename), Path.Combine (targetDir, filename));
				monitor.Step ( 1 );
			}
			catch ( Exception e )
			{
				monitor.ReportError ( GettextCatalog.GetString ("Solution could not be deployed: "), e );
				return;
			}
			finally 
			{
				monitor.EndTask ();
			}
			monitor.ReportSuccess ( GettextCatalog.GetString ( "Solution was succesfully deployed" ) );
		}

		void DeleteGeneratedFiles ( AutotoolsContext context )
		{
			foreach (string file in context.GetGeneratedFiles ())
				if ( File.Exists ( file ) ) Runtime.FileService.DeleteFile ( file );
		}

		void AddTopLevelMakefileVars ( Makefile makefile, IProgressMonitor monitor)
		{
			monitor.Log.WriteLine ( GettextCatalog.GetString ("Adding variables to top-level Makefile") );

			StringBuilder sb = new StringBuilder ();
			if (!generateAutotools)
				sb.Append ("rules.make configure Makefile.include");

			foreach ( string file in context.GetGlobalReferencedFiles() )
				sb.Append (' ').Append (file);
			
			string vals = sb.ToString ();

//			makefile.AppendToVariable ( "DLL_REFERENCES", vals );
			makefile.AppendToVariable ( "EXTRA_DIST", vals );
//			makefile.AppendToVariable ( "pkglib_DATA", "$(DLL_REFERENCES)" );
		}

		void CreateAutoGenDotSH (AutotoolsContext context, IProgressMonitor monitor)
		{
			monitor.Log.WriteLine ( GettextCatalog.GetString ("Creating autogen.sh") );

			TemplateEngine templateEngine = new TemplateEngine();			

			templateEngine.Variables["NAME"] = solution_name;

			string fileName = Path.Combine (solution_dir, "autogen.sh");

			StreamWriter writer = new StreamWriter( fileName );

			Stream stream = context.GetTemplateStream ("autogen.sh.template");
			StreamReader reader = new StreamReader(stream);

			templateEngine.Process(reader, writer);

			reader.Close();
			writer.Close();

			context.AddGeneratedFile (fileName);

			// make autogen.sh executable
			if (PlatformID.Unix == Environment.OSVersion.Platform)
				Syscall.chmod ( fileName , FilePermissions.S_IXOTH | FilePermissions.S_IROTH | FilePermissions.S_IRWXU | FilePermissions.S_IRWXG );
		}

		void CreateConfigureDotAC ( Combine combine, string defaultConf, IProgressMonitor monitor, AutotoolsContext context )
		{
			monitor.Log.WriteLine ( GettextCatalog.GetString ("Creating configure.ac") );
			TemplateEngine templateEngine = new TemplateEngine();			
			templateEngine.Variables["WARNING"] = "Warning: This is an automatically generated file, do not edit!";		
			
			// add solution configuration options
			StringBuilder config_options = new StringBuilder ();
			foreach ( IConfiguration config in combine.Configurations )
			{
				string name = context.EscapeAndUpperConfigName (config.Name).ToLower();
				string def = config.Name == defaultConf ? "YES" : "NO";
				string ac_var = "enable_" + name;

				// test to see if a configuration was enabled
				config_options.AppendFormat ( "AC_ARG_ENABLE({0},\n", name );
				config_options.AppendFormat ("	AC_HELP_STRING([--enable-{0}],\n", name );
				config_options.AppendFormat ("		[Use '{0}' Configuration [default={1}]]),\n", context.EscapeAndUpperConfigName (config.Name), def );
				config_options.AppendFormat ( "		{0}=yes, {0}=no)\n", ac_var );
				config_options.AppendFormat ( "AM_CONDITIONAL({0}, test x${1} = xyes)\n", ac_var.ToUpper(), ac_var );

				// if yes, populate some vars
				config_options.AppendFormat ( "if test \"x${0}\" = \"xyes\" ; then\n", ac_var );
//				AppendConfigVariables ( combine, config.Name, config_options );
				config_options.Append ( "	CONFIG_REQUESTED=\"yes\"\nfi\n" );
			}

			// if no configuration was specified, set to default (if there is a default)
			if (defaultConf != null)
			{
				config_options.Append ( "if test -z \"$CONFIG_REQUESTED\" ; then\n" );
//				AppendConfigVariables ( combine, defaultConf, config_options );
				config_options.AppendFormat ( "	AM_CONDITIONAL({0}, true)\nfi\n", "ENABLE_"
						+ context.EscapeAndUpperConfigName (defaultConf));
			}

			templateEngine.Variables ["CONFIG_OPTIONS"] = config_options.ToString();

			// build compiler checks
			StringBuilder compiler_checks = new StringBuilder();
			foreach (string compiler in context.GetCommandChecks () ) 
			{
				compiler_checks.AppendFormat ("AC_PATH_PROG({0}, {1}, no)\n", compiler.ToUpper(), compiler);
				compiler_checks.AppendFormat ("if test \"x${0}\" = \"xno\"; then\n", compiler.ToUpper() );
				compiler_checks.AppendFormat ("        AC_MSG_ERROR([{0} Not found])\n", compiler );
				compiler_checks.Append("fi\n");
			}
			templateEngine.Variables["COMPILER_CHECKS"] = compiler_checks.ToString();

			// build list of *.in files
			StringBuilder configFiles = new StringBuilder();
			string tmpmf = null;
			foreach (string makefile in context.GetAutoConfFiles () ) 
			{
				tmpmf = Runtime.FileService.AbsoluteToRelativePath ( solution_dir, makefile );
				if (PlatformID.Unix != Environment.OSVersion.Platform)
					tmpmf = tmpmf.Replace("\\","/");

				configFiles.Append(Runtime.FileService.NormalizeRelativePath (tmpmf));
				configFiles.Append("\n");
			}
			templateEngine.Variables["CONFIG_FILES"] = configFiles.ToString();

			// build list of pkgconfig checks we must make
			StringWriter packageChecks = new StringWriter();
			foreach (SystemPackage pkg in context.GetRequiredPackages ())
			{
				string pkgvar = AutotoolsContext.GetPkgConfigVariable (pkg.Name);
				packageChecks.Write("PKG_CHECK_MODULES([");
				packageChecks.Write(pkgvar);
				packageChecks.Write("], [");
				packageChecks.Write(pkg.Name);
				packageChecks.WriteLine("])");
			}
			templateEngine.Variables["PACKAGE_CHECKS"] = packageChecks.ToString();
			templateEngine.Variables["SOLUTION_NAME"] = solution_name;
			templateEngine.Variables["VERSION"] = solution_version;

			string configureFileName = Path.Combine (solution_dir, "configure.ac");

			StreamWriter writer = new StreamWriter(configureFileName);
			Stream stream = context.GetTemplateStream ("configure.ac.template");
			StreamReader reader = new StreamReader(stream);

			templateEngine.Process(reader, writer);

			reader.Close();
			writer.Close();
			context.AddGeneratedFile (configureFileName);
		}

		void CreateConfigureScript (Combine combine, string defaultConf, AutotoolsContext ctx, IProgressMonitor monitor)
		{
			monitor.Log.WriteLine ( GettextCatalog.GetString ("Creating configure script") );

			TemplateEngine templateEngine = new TemplateEngine ();

			// Build list of configurations
			StringBuilder sbConfig = new StringBuilder ();
			sbConfig.Append ("\"");
			foreach (string config in ctx.GetConfigurations ())
				sbConfig.AppendFormat (" {0}", config);
			sbConfig.Append ("\"");

			// Build list of required packages
			StringBuilder sbPackages = new StringBuilder ();
			sbPackages.Append ("\"");
			foreach (SystemPackage pkg in ctx.GetRequiredPackages ())
				sbPackages.AppendFormat (" {0};{1}", pkg.Name, pkg.Version);
			sbPackages.Append ("\"");

			templateEngine.Variables ["VERSION"] = solution_version;
			templateEngine.Variables ["PACKAGE"] = AutotoolsContext.EscapeStringForAutoconf (combine.Name).ToLower ();
			templateEngine.Variables ["DEFAULT_CONFIG"] = ctx.EscapeAndUpperConfigName (defaultConf);
			templateEngine.Variables ["CONFIGURATIONS"] = sbConfig.ToString ();
			templateEngine.Variables ["REQUIRED_PACKAGES"] = sbPackages.ToString ();


			Stream stream = context.GetTemplateStream ("configure.template");

			string filename = Path.Combine (solution_dir, "configure");
			StreamReader reader = new StreamReader (stream);
			StreamWriter writer = new StreamWriter (filename);
			templateEngine.Process(reader, writer);
			reader.Close ();
			writer.Close ();

			ctx.AddGeneratedFile (filename);

			if (PlatformID.Unix == Environment.OSVersion.Platform)
				Syscall.chmod ( filename , FilePermissions.S_IXOTH | FilePermissions.S_IROTH | FilePermissions.S_IRWXU | FilePermissions.S_IRWXG );
		}


/*		void AppendConfigVariables ( Combine combine, string config, StringBuilder options )
		{
			string name = config.ToLower();

			StringBuilder refb = new StringBuilder ();
			StringBuilder libb = new StringBuilder ();
			foreach ( CombineEntry ce in combine.GetAllBuildableEntries ( config ) )
			{
				Project p = ce as Project;
				if ( p == null ) continue;

				DotNetProjectConfiguration dnpc = p.Configurations [config] as DotNetProjectConfiguration;
				if ( dnpc == null ) continue;

				if ( dnpc.CompileTarget != CompileTarget.Library ) continue;
				
				string filename = Path.GetFileName ( dnpc.CompiledOutputName );
				string projname = AutotoolsContext.EscapeStringForAutoconf (p.Name.ToUpper());
				
				// provide these for per-library pc files
				options.AppendFormat ( "	{0}_{1}_LIB='{2}'\n", projname , config.ToUpper(), filename);
				options.AppendFormat ( "	AC_SUBST({0}_{1}_LIB)\n", projname, config.ToUpper() );
				
				libb.Append ( " ${pkglibdir}/" + filename  );
				refb.Append ( " -r:${pkglibdir}/" +  filename );
			}

			// must also add referenced dlls
			foreach ( string dll in context.GetReferencedDlls () )
			{
				libb.Append ( " ${pkglibdir}/" + Path.GetFileName (dll) );
				refb.Append ( " -r:${pkglibdir}/" + Path.GetFileName (dll) );
			}
			
			options.AppendFormat ( "	{0}_CONFIG_LIBRARIES='{1}'\n", name.ToUpper(), libb.ToString ());
			options.AppendFormat ( "	{0}_CONFIG_LIBS='{1}'\n", name.ToUpper(), refb.ToString ());
			options.AppendFormat ( "	AC_SUBST({0}_CONFIG_LIBRARIES)\n", name.ToUpper() );
			options.AppendFormat ( "	AC_SUBST({0}_CONFIG_LIBS)\n", name.ToUpper() );
		}
*/
		void CreateMakefileInclude (AutotoolsContext context, IProgressMonitor monitor)
		{
			monitor.Log.WriteLine ( GettextCatalog.GetString ("Creating Makefile.include") );
			
			TemplateEngine templateEngine = new TemplateEngine();
			
			StringBuilder deployDirs = new StringBuilder ();
			IDictionary dirs = context.GetReferencedTargetDirectories ();
			string deployDirVars = "";
			
			foreach (DictionaryEntry e in dirs) {
				// It may be a sub-path
				string dir = (string) e.Key;
				int i = dir.IndexOf ('/');
				if (i != -1)
					dir = dir.Substring (0, i);
				string resolved = context.DeployContext.GetDirectory (dir);
				if (resolved == null)
					throw new InvalidOperationException ("Unknown directory: " + e.Key);
				
				if (i != -1)
					resolved += ((string)e.Key).Substring (i);
				
				string var = (string) e.Value;
				string dname = var.ToLower ().Replace ("_","");
				deployDirs.AppendFormat ("{0}dir = {1}\n", dname, resolved);
				deployDirs.AppendFormat ("{0}_DATA = $({1})\n", dname, var);
				deployDirVars += "$(" + var + ") ";
			}
			
			templateEngine.Variables["DEPLOY_DIRS"] = deployDirs.ToString();
			templateEngine.Variables["DEPLOY_FILES_CLEAN"] = deployDirVars;

			string fileName = Path.Combine (solution_dir, "Makefile.include");

			Stream stream = context.GetTemplateStream ("Makefile.include");

			StreamReader reader = new StreamReader(stream);
			StreamWriter writer = new StreamWriter(fileName);

			templateEngine.Process(reader, writer);

			reader.Close();
			writer.Close();

			context.AddGeneratedFile (fileName);
		}

/*
		void CreatePkgConfigFile ( Combine combine, IProgressMonitor monitor, Makefile mfile, AutotoolsContext context )
		{
			if ( !pkgconfig ) return;

			monitor.Log.WriteLine ( GettextCatalog.GetString ("Creating pkg-config file") );

			TemplateEngine templateEngine = new TemplateEngine();			

			templateEngine.Variables["NAME"] = solution_name;
			templateEngine.Variables["DESCRIPTION"] = combine.Description;
			templateEngine.Variables["VERSION"] = solution_version;
			
			StringBuilder pkgs = new StringBuilder ();
			foreach ( string pkg in context.GetRequiredPackages () )
				pkgs.AppendFormat ( "{0} ", pkg );
			templateEngine.Variables ["REQUIRES_PKGS"] = pkgs.ToString ();

			StringBuilder libs = new StringBuilder ();
			StringBuilder libraries = new StringBuilder ();
			foreach ( IConfiguration config in combine.Configurations )
			{
				string configName = context.EscapeAndUpperConfigName (config.Name);
				libs.AppendFormat ( "@{0}_CONFIG_LIBS@ ", configName);
				libraries.AppendFormat ( "@{0}_CONFIG_LIBRARIES@ ", configName);
			}
			templateEngine.Variables ["LIBS"] = libs.ToString ();
			templateEngine.Variables ["LIBRARIES"] = libraries.ToString ();
			
			string fileName = solution_name.ToLower() + ".pc";
			string path = String.Format ( "{0}/{1}.in", solution_dir, fileName );

			StreamWriter writer = new StreamWriter( path );

			Stream stream = context.GetTemplateStream ("package.pc.template");
			StreamReader reader = new StreamReader(stream);

			templateEngine.Process(reader, writer);

			reader.Close();
			writer.Close();
			
			context.AddAutoconfFile ( fileName );
			mfile.AppendToVariable ( "EXTRA_DIST", fileName + ".in" );
			mfile.SetVariable ( "pkgconfigdir", "$(libdir)/pkgconfig" );
			mfile.AppendToVariable ( "pkgconfig_DATA", fileName );
			mfile.AppendToVariable ( "DISTCLEANFILES", fileName );
		}
*/
	}
}

