// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Ide.CodeTemplates {
    
    
    public partial class EditTemplateDialog {
        
        private Gtk.VBox vbox2;
        
        private Gtk.Table table2;
        
        private Gtk.HBox hbox1;
        
        private Gtk.Entry entryDescription;
        
        private Gtk.Label label2;
        
        private Gtk.ComboBoxEntry comboboxentryMime;
        
        private Gtk.HBox hbox2;
        
        private Gtk.Entry entryShortcut1;
        
        private Gtk.Label label5;
        
        private Gtk.ComboBoxEntry comboboxentryGroups;
        
        private Gtk.HBox hbox5;
        
        private Gtk.CheckButton checkbuttonExpansion;
        
        private Gtk.CheckButton checkbuttonSurroundWith;
        
        private Gtk.Label label1;
        
        private Gtk.Label label3;
        
        private Gtk.VPaned vpaned1;
        
        private Gtk.VBox vbox3;
        
        private Gtk.HBox hbox3;
        
        private Gtk.Label label6;
        
        private Gtk.Fixed fixed1;
        
        private Gtk.CheckButton checkbuttonWhiteSpaces;
        
        private Gtk.ScrolledWindow scrolledwindow1;
        
        private Gtk.VBox vbox4;
        
        private Gtk.HBox hbox4;
        
        private Gtk.Label label7;
        
        private Gtk.Fixed fixed2;
        
        private Gtk.ScrolledWindow GtkScrolledWindow;
        
        private Gtk.TreeView treeviewVariables;
        
        private Gtk.Button buttonCancel;
        
        private Gtk.Button buttonOk;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget MonoDevelop.Ide.CodeTemplates.EditTemplateDialog
            this.Name = "MonoDevelop.Ide.CodeTemplates.EditTemplateDialog";
            this.Title = "";
            this.WindowPosition = ((Gtk.WindowPosition)(4));
            this.BorderWidth = ((uint)(6));
            this.SkipPagerHint = true;
            this.SkipTaskbarHint = true;
            this.HasSeparator = false;
            // Internal child MonoDevelop.Ide.CodeTemplates.EditTemplateDialog.VBox
            Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog1_VBox";
            w1.Spacing = 6;
            w1.BorderWidth = ((uint)(2));
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            this.vbox2.BorderWidth = ((uint)(6));
            // Container child vbox2.Gtk.Box+BoxChild
            this.table2 = new Gtk.Table(((uint)(3)), ((uint)(2)), false);
            this.table2.Name = "table2";
            this.table2.RowSpacing = ((uint)(6));
            this.table2.ColumnSpacing = ((uint)(6));
            // Container child table2.Gtk.Table+TableChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.entryDescription = new Gtk.Entry();
            this.entryDescription.CanFocus = true;
            this.entryDescription.Name = "entryDescription";
            this.entryDescription.IsEditable = true;
            this.entryDescription.InvisibleChar = '●';
            this.hbox1.Add(this.entryDescription);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.hbox1[this.entryDescription]));
            w2.Position = 0;
            // Container child hbox1.Gtk.Box+BoxChild
            this.label2 = new Gtk.Label();
            this.label2.Name = "label2";
            this.label2.Xalign = 0F;
            this.label2.LabelProp = Mono.Unix.Catalog.GetString("_Mime:");
            this.label2.UseUnderline = true;
            this.hbox1.Add(this.label2);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.hbox1[this.label2]));
            w3.Position = 1;
            w3.Expand = false;
            w3.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.comboboxentryMime = Gtk.ComboBoxEntry.NewText();
            this.comboboxentryMime.Name = "comboboxentryMime";
            this.hbox1.Add(this.comboboxentryMime);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.hbox1[this.comboboxentryMime]));
            w4.Position = 2;
            w4.Expand = false;
            w4.Fill = false;
            this.table2.Add(this.hbox1);
            Gtk.Table.TableChild w5 = ((Gtk.Table.TableChild)(this.table2[this.hbox1]));
            w5.TopAttach = ((uint)(1));
            w5.BottomAttach = ((uint)(2));
            w5.LeftAttach = ((uint)(1));
            w5.RightAttach = ((uint)(2));
            w5.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.hbox2 = new Gtk.HBox();
            this.hbox2.Name = "hbox2";
            this.hbox2.Spacing = 6;
            // Container child hbox2.Gtk.Box+BoxChild
            this.entryShortcut1 = new Gtk.Entry();
            this.entryShortcut1.CanFocus = true;
            this.entryShortcut1.Name = "entryShortcut1";
            this.entryShortcut1.IsEditable = true;
            this.entryShortcut1.InvisibleChar = '●';
            this.hbox2.Add(this.entryShortcut1);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.hbox2[this.entryShortcut1]));
            w6.Position = 0;
            // Container child hbox2.Gtk.Box+BoxChild
            this.label5 = new Gtk.Label();
            this.label5.Name = "label5";
            this.label5.Xalign = 0F;
            this.label5.LabelProp = Mono.Unix.Catalog.GetString("_Group:");
            this.label5.UseUnderline = true;
            this.hbox2.Add(this.label5);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.hbox2[this.label5]));
            w7.Position = 1;
            w7.Expand = false;
            w7.Fill = false;
            // Container child hbox2.Gtk.Box+BoxChild
            this.comboboxentryGroups = Gtk.ComboBoxEntry.NewText();
            this.comboboxentryGroups.Name = "comboboxentryGroups";
            this.hbox2.Add(this.comboboxentryGroups);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.hbox2[this.comboboxentryGroups]));
            w8.Position = 2;
            w8.Expand = false;
            w8.Fill = false;
            this.table2.Add(this.hbox2);
            Gtk.Table.TableChild w9 = ((Gtk.Table.TableChild)(this.table2[this.hbox2]));
            w9.LeftAttach = ((uint)(1));
            w9.RightAttach = ((uint)(2));
            w9.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.hbox5 = new Gtk.HBox();
            this.hbox5.Name = "hbox5";
            this.hbox5.Spacing = 6;
            // Container child hbox5.Gtk.Box+BoxChild
            this.checkbuttonExpansion = new Gtk.CheckButton();
            this.checkbuttonExpansion.CanFocus = true;
            this.checkbuttonExpansion.Name = "checkbuttonExpansion";
            this.checkbuttonExpansion.Label = Mono.Unix.Catalog.GetString("Is _expandable template");
            this.checkbuttonExpansion.DrawIndicator = true;
            this.checkbuttonExpansion.UseUnderline = true;
            this.hbox5.Add(this.checkbuttonExpansion);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(this.hbox5[this.checkbuttonExpansion]));
            w10.Position = 0;
            w10.Expand = false;
            // Container child hbox5.Gtk.Box+BoxChild
            this.checkbuttonSurroundWith = new Gtk.CheckButton();
            this.checkbuttonSurroundWith.CanFocus = true;
            this.checkbuttonSurroundWith.Name = "checkbuttonSurroundWith";
            this.checkbuttonSurroundWith.Label = Mono.Unix.Catalog.GetString("Is _surround with template");
            this.checkbuttonSurroundWith.DrawIndicator = true;
            this.checkbuttonSurroundWith.UseUnderline = true;
            this.hbox5.Add(this.checkbuttonSurroundWith);
            Gtk.Box.BoxChild w11 = ((Gtk.Box.BoxChild)(this.hbox5[this.checkbuttonSurroundWith]));
            w11.Position = 1;
            this.table2.Add(this.hbox5);
            Gtk.Table.TableChild w12 = ((Gtk.Table.TableChild)(this.table2[this.hbox5]));
            w12.TopAttach = ((uint)(2));
            w12.BottomAttach = ((uint)(3));
            w12.LeftAttach = ((uint)(1));
            w12.RightAttach = ((uint)(2));
            w12.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.Xalign = 0F;
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("_Shortcut:");
            this.label1.UseUnderline = true;
            this.table2.Add(this.label1);
            Gtk.Table.TableChild w13 = ((Gtk.Table.TableChild)(this.table2[this.label1]));
            w13.XOptions = ((Gtk.AttachOptions)(4));
            w13.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.label3 = new Gtk.Label();
            this.label3.Name = "label3";
            this.label3.Xalign = 1F;
            this.label3.LabelProp = Mono.Unix.Catalog.GetString("_Description:");
            this.label3.UseUnderline = true;
            this.table2.Add(this.label3);
            Gtk.Table.TableChild w14 = ((Gtk.Table.TableChild)(this.table2[this.label3]));
            w14.TopAttach = ((uint)(1));
            w14.BottomAttach = ((uint)(2));
            w14.XOptions = ((Gtk.AttachOptions)(4));
            w14.YOptions = ((Gtk.AttachOptions)(4));
            this.vbox2.Add(this.table2);
            Gtk.Box.BoxChild w15 = ((Gtk.Box.BoxChild)(this.vbox2[this.table2]));
            w15.Position = 0;
            w15.Expand = false;
            w15.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.vpaned1 = new Gtk.VPaned();
            this.vpaned1.CanFocus = true;
            this.vpaned1.Name = "vpaned1";
            this.vpaned1.Position = 157;
            // Container child vpaned1.Gtk.Paned+PanedChild
            this.vbox3 = new Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            this.vbox3.BorderWidth = ((uint)(6));
            // Container child vbox3.Gtk.Box+BoxChild
            this.hbox3 = new Gtk.HBox();
            this.hbox3.Name = "hbox3";
            this.hbox3.Spacing = 6;
            // Container child hbox3.Gtk.Box+BoxChild
            this.label6 = new Gtk.Label();
            this.label6.Name = "label6";
            this.label6.Xalign = 0F;
            this.label6.LabelProp = Mono.Unix.Catalog.GetString("Template Text:");
            this.hbox3.Add(this.label6);
            Gtk.Box.BoxChild w16 = ((Gtk.Box.BoxChild)(this.hbox3[this.label6]));
            w16.Position = 0;
            w16.Expand = false;
            w16.Fill = false;
            // Container child hbox3.Gtk.Box+BoxChild
            this.fixed1 = new Gtk.Fixed();
            this.fixed1.Name = "fixed1";
            this.fixed1.HasWindow = false;
            this.hbox3.Add(this.fixed1);
            Gtk.Box.BoxChild w17 = ((Gtk.Box.BoxChild)(this.hbox3[this.fixed1]));
            w17.Position = 1;
            // Container child hbox3.Gtk.Box+BoxChild
            this.checkbuttonWhiteSpaces = new Gtk.CheckButton();
            this.checkbuttonWhiteSpaces.CanFocus = true;
            this.checkbuttonWhiteSpaces.Name = "checkbuttonWhiteSpaces";
            this.checkbuttonWhiteSpaces.Label = Mono.Unix.Catalog.GetString("S_how whitespaces");
            this.checkbuttonWhiteSpaces.DrawIndicator = true;
            this.checkbuttonWhiteSpaces.UseUnderline = true;
            this.hbox3.Add(this.checkbuttonWhiteSpaces);
            Gtk.Box.BoxChild w18 = ((Gtk.Box.BoxChild)(this.hbox3[this.checkbuttonWhiteSpaces]));
            w18.Position = 2;
            w18.Expand = false;
            this.vbox3.Add(this.hbox3);
            Gtk.Box.BoxChild w19 = ((Gtk.Box.BoxChild)(this.vbox3[this.hbox3]));
            w19.Position = 0;
            w19.Expand = false;
            w19.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.scrolledwindow1 = new Gtk.ScrolledWindow();
            this.scrolledwindow1.CanFocus = true;
            this.scrolledwindow1.Name = "scrolledwindow1";
            this.scrolledwindow1.ShadowType = ((Gtk.ShadowType)(1));
            this.vbox3.Add(this.scrolledwindow1);
            Gtk.Box.BoxChild w20 = ((Gtk.Box.BoxChild)(this.vbox3[this.scrolledwindow1]));
            w20.Position = 1;
            this.vpaned1.Add(this.vbox3);
            Gtk.Paned.PanedChild w21 = ((Gtk.Paned.PanedChild)(this.vpaned1[this.vbox3]));
            w21.Resize = false;
            // Container child vpaned1.Gtk.Paned+PanedChild
            this.vbox4 = new Gtk.VBox();
            this.vbox4.Name = "vbox4";
            this.vbox4.Spacing = 6;
            this.vbox4.BorderWidth = ((uint)(6));
            // Container child vbox4.Gtk.Box+BoxChild
            this.hbox4 = new Gtk.HBox();
            this.hbox4.Name = "hbox4";
            this.hbox4.Spacing = 6;
            // Container child hbox4.Gtk.Box+BoxChild
            this.label7 = new Gtk.Label();
            this.label7.Name = "label7";
            this.label7.Xalign = 0F;
            this.label7.LabelProp = Mono.Unix.Catalog.GetString("Variables:");
            this.hbox4.Add(this.label7);
            Gtk.Box.BoxChild w22 = ((Gtk.Box.BoxChild)(this.hbox4[this.label7]));
            w22.Position = 0;
            w22.Expand = false;
            w22.Fill = false;
            // Container child hbox4.Gtk.Box+BoxChild
            this.fixed2 = new Gtk.Fixed();
            this.fixed2.Name = "fixed2";
            this.fixed2.HasWindow = false;
            this.hbox4.Add(this.fixed2);
            Gtk.Box.BoxChild w23 = ((Gtk.Box.BoxChild)(this.hbox4[this.fixed2]));
            w23.Position = 1;
            this.vbox4.Add(this.hbox4);
            Gtk.Box.BoxChild w24 = ((Gtk.Box.BoxChild)(this.vbox4[this.hbox4]));
            w24.Position = 0;
            w24.Expand = false;
            w24.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.GtkScrolledWindow = new Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeviewVariables = new Gtk.TreeView();
            this.treeviewVariables.CanFocus = true;
            this.treeviewVariables.Name = "treeviewVariables";
            this.GtkScrolledWindow.Add(this.treeviewVariables);
            this.vbox4.Add(this.GtkScrolledWindow);
            Gtk.Box.BoxChild w26 = ((Gtk.Box.BoxChild)(this.vbox4[this.GtkScrolledWindow]));
            w26.Position = 1;
            this.vpaned1.Add(this.vbox4);
            this.vbox2.Add(this.vpaned1);
            Gtk.Box.BoxChild w28 = ((Gtk.Box.BoxChild)(this.vbox2[this.vpaned1]));
            w28.Position = 1;
            w1.Add(this.vbox2);
            Gtk.Box.BoxChild w29 = ((Gtk.Box.BoxChild)(w1[this.vbox2]));
            w29.Position = 0;
            // Internal child MonoDevelop.Ide.CodeTemplates.EditTemplateDialog.ActionArea
            Gtk.HButtonBox w30 = this.ActionArea;
            w30.Name = "dialog1_ActionArea";
            w30.Spacing = 6;
            w30.BorderWidth = ((uint)(5));
            w30.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonCancel = new Gtk.Button();
            this.buttonCancel.CanDefault = true;
            this.buttonCancel.CanFocus = true;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseStock = true;
            this.buttonCancel.UseUnderline = true;
            this.buttonCancel.Label = "gtk-cancel";
            this.AddActionWidget(this.buttonCancel, -6);
            Gtk.ButtonBox.ButtonBoxChild w31 = ((Gtk.ButtonBox.ButtonBoxChild)(w30[this.buttonCancel]));
            w31.Expand = false;
            w31.Fill = false;
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonOk = new Gtk.Button();
            this.buttonOk.CanDefault = true;
            this.buttonOk.CanFocus = true;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseStock = true;
            this.buttonOk.UseUnderline = true;
            this.buttonOk.Label = "gtk-ok";
            this.AddActionWidget(this.buttonOk, -5);
            Gtk.ButtonBox.ButtonBoxChild w32 = ((Gtk.ButtonBox.ButtonBoxChild)(w30[this.buttonOk]));
            w32.Position = 1;
            w32.Expand = false;
            w32.Fill = false;
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 655;
            this.DefaultHeight = 494;
            this.Show();
        }
    }
}
