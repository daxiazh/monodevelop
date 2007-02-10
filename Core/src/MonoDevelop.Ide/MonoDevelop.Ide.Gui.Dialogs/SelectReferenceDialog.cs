// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

using MonoDevelop.Projects;
using MonoDevelop.Core;

using Gtk;
using Glade;

namespace MonoDevelop.Ide.Gui.Dialogs
{
	internal interface IReferencePanel
	{
	}
	
	internal class SelectReferenceDialog
	{
		ListStore refTreeStore;
		[Widget] Dialog    AddReferenceDialog;
		[Widget] TreeView  ReferencesTreeView;
//		[Widget] Button    okbutton;
//		[Widget] Button    cancelbutton;
		[Widget] Button    RemoveReferenceButton;
		[Widget] Notebook  mainBook;
		
		GacReferencePanel gacRefPanel;
		ProjectReferencePanel projectRefPanel;
		AssemblyReferencePanel assemblyRefPanel;
		Project configureProject;
		
		const int NameColumn = 0;
		const int TypeNameColumn = 1;
		const int LocationColumn = 2;
		const int ProjectReferenceColumn = 3;
		const int IconColumn = 4;
		
		public ProjectReferenceCollection ReferenceInformations {
			get {
				ProjectReferenceCollection referenceInformations = new ProjectReferenceCollection();
				Gtk.TreeIter looping_iter;
				if (!refTreeStore.GetIterFirst (out looping_iter)) {
					return referenceInformations;
				}
				do {
					referenceInformations.Add ((ProjectReference) refTreeStore.GetValue(looping_iter, ProjectReferenceColumn));
				} while (refTreeStore.IterNext (ref looping_iter));
				return referenceInformations;
			}
		}

		public int Run ()
		{
			return AddReferenceDialog.Run ();
		}

		public void Hide ()
		{
			AddReferenceDialog.Hide ();
		}
		
		public Gtk.Window Window {
			get { return AddReferenceDialog; }
		}

		public void SetProject (Project configureProject)
		{
			this.configureProject = configureProject;
			((ListStore) ReferencesTreeView.Model).Clear ();

			projectRefPanel.SetProject (configureProject);
			gacRefPanel.SetProject (configureProject);
			gacRefPanel.Reset ();
			assemblyRefPanel.SetBasePath (configureProject.BaseDirectory);

			foreach (ProjectReference refInfo in configureProject.ProjectReferences)
				AddReference (refInfo);

			OnChanged (null, null);
		}
		
		TreeIter AddReference (ProjectReference refInfo)
		{
			switch (refInfo.ReferenceType) {
				case ReferenceType.Assembly:
					return AddAssemplyReference (refInfo);
				case ReferenceType.Project:
					return AddProjectReference (refInfo);
				case ReferenceType.Gac:
					return AddGacReference (refInfo);
				default:
					return TreeIter.Zero;
			}
		}

		TreeIter AddAssemplyReference (ProjectReference refInfo)
		{
			return refTreeStore.AppendValues (System.IO.Path.GetFileName (refInfo.Reference), GetTypeText (refInfo), System.IO.Path.GetFullPath (refInfo.Reference), refInfo, "md-closed-folder");
		}

		TreeIter AddProjectReference (ProjectReference refInfo)
		{
			Combine c = configureProject.RootCombine;
			if (c == null) return TreeIter.Zero;
			
			Project p = c.FindProject (refInfo.Reference);
			if (p == null) return TreeIter.Zero;
			
			string iconName = Services.Icons.GetImageForProjectType (p.ProjectType);
			projectRefPanel.SignalRefChange (refInfo.Reference, true);
			return refTreeStore.AppendValues (System.IO.Path.GetFileName (refInfo.Reference), GetTypeText (refInfo), p.BaseDirectory, refInfo, iconName);
		}

		TreeIter AddGacReference (ProjectReference refInfo)
		{
			gacRefPanel.SignalRefChange (refInfo.Reference, true);
			return refTreeStore.AppendValues (System.IO.Path.GetFileNameWithoutExtension (refInfo.Reference), GetTypeText (refInfo), refInfo.Reference, refInfo, "md-package");
		}
		
		public SelectReferenceDialog(Project configureProject)
		{
			Glade.XML refXML = new Glade.XML (null, "Base.glade", "AddReferenceDialog", null);
			refXML.Autoconnect (this);
			
			refTreeStore = new ListStore (typeof (string), typeof(string), typeof(string), typeof(ProjectReference), typeof(string));
			ReferencesTreeView.Model = refTreeStore;

			TreeViewColumn col = new TreeViewColumn ();
			col.Title = GettextCatalog.GetString("Reference");
			CellRendererPixbuf crp = new CellRendererPixbuf ();
			col.PackStart (crp, false);
			col.AddAttribute (crp, "stock-id", IconColumn);
			CellRendererText text_render = new CellRendererText ();
			col.PackStart (text_render, true);
			col.AddAttribute (text_render, "text", NameColumn);
			
			ReferencesTreeView.AppendColumn (col);
			ReferencesTreeView.AppendColumn (GettextCatalog.GetString ("Type"), new CellRendererText (), "text", TypeNameColumn);
			ReferencesTreeView.AppendColumn (GettextCatalog.GetString ("Location"), new CellRendererText (), "text", LocationColumn);
			
			projectRefPanel = new ProjectReferencePanel (this);
			gacRefPanel = new GacReferencePanel (this);
			assemblyRefPanel = new AssemblyReferencePanel (this);
			SetProject (configureProject);
			
			mainBook.RemovePage (mainBook.CurrentPage);
			mainBook.AppendPage (gacRefPanel, new Label (GettextCatalog.GetString ("Packages")));
			mainBook.AppendPage (projectRefPanel, new Label (GettextCatalog.GetString ("Projects")));
			mainBook.AppendPage (assemblyRefPanel, new Label (GettextCatalog.GetString (".Net Assembly")));
			mainBook.Page = 0;
			ReferencesTreeView.Selection.Changed += new EventHandler (OnChanged);
			AddReferenceDialog.ShowAll ();
			OnChanged (null, null);
		}

		void OnChanged (object o, EventArgs e)
		{
			if (ReferencesTreeView.Selection.CountSelectedRows () > 0)
				RemoveReferenceButton.Sensitive = true;
			else
				RemoveReferenceButton.Sensitive = false;
		}
		
		string GetTypeText (ProjectReference pref)
		{
			switch (pref.ReferenceType) {
				case ReferenceType.Gac: return GettextCatalog.GetString ("Package");
				case ReferenceType.Assembly: return GettextCatalog.GetString ("Assembly");
				case ReferenceType.Project: return GettextCatalog.GetString ("Project");
				default: return "";
			}
		}

		public void RemoveReference (ReferenceType referenceType, string reference)
		{
			TreeIter iter = FindReference (referenceType, reference);
			if (iter.Equals (TreeIter.Zero))
				return;
			refTreeStore.Remove (ref iter);
		}
		
		public void AddReference (ReferenceType referenceType, string reference)
		{
			TreeIter iter = FindReference (referenceType, reference);
			if (!iter.Equals (TreeIter.Zero))
				return;
			
			ProjectReference tag = new ProjectReference (referenceType, reference);
			TreeIter ni = AddReference (tag);
			if (!ni.Equals (TreeIter.Zero))
				ReferencesTreeView.ScrollToCell (refTreeStore.GetPath (ni), null, false, 0, 0);
		}
		
		TreeIter FindReference (ReferenceType referenceType, string reference)
		{
			TreeIter looping_iter;
			if (refTreeStore.GetIterFirst (out looping_iter)) {
				do {
					ProjectReference pref = (ProjectReference) refTreeStore.GetValue (looping_iter, ProjectReferenceColumn);
					if (pref.Reference == reference && pref.ReferenceType == referenceType) {
						return looping_iter;
					}
				} while (refTreeStore.IterNext (ref looping_iter));
			}
			return TreeIter.Zero;
		}
		
		protected void RemoveReference (object sender, EventArgs e)
		{
			TreeIter iter;
			TreeModel mdl;
			if (ReferencesTreeView.Selection.GetSelected (out mdl, out iter)) {
				switch (((ProjectReference)refTreeStore.GetValue (iter, ProjectReferenceColumn)).ReferenceType) {
					case ReferenceType.Gac:
						gacRefPanel.SignalRefChange ((string)refTreeStore.GetValue (iter, LocationColumn), false);
						break;
					case ReferenceType.Project:
						projectRefPanel.SignalRefChange ((string)refTreeStore.GetValue (iter, NameColumn), false);
						break;
				}
				TreeIter newIter = iter;
				if (refTreeStore.IterNext (ref newIter)) {
					ReferencesTreeView.Selection.SelectIter (newIter);
					refTreeStore.Remove (ref iter);
				} else {
					TreePath path = refTreeStore.GetPath (iter);
					if (path.Prev ()) {
						ReferencesTreeView.Selection.SelectPath (path);
						refTreeStore.Remove (ref iter);
					} else {
						refTreeStore.Remove (ref iter);
					}
				}
			}
		}
	}
}

