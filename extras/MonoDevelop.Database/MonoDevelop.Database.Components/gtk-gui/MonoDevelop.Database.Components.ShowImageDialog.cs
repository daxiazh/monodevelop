// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Database.Components {
    
    
    public partial class ShowImageDialog {
        
        private Gtk.ScrolledWindow scrolledwindow;
        
        private Gtk.Image image1;
        
        private Gtk.HBox hboxError;
        
        private Gtk.Image image;
        
        private Gtk.Label label;
        
        private Gtk.Button buttonClose;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget MonoDevelop.Database.Components.ShowImageDialog
            this.Name = "MonoDevelop.Database.Components.ShowImageDialog";
            this.Title = AddinCatalog.GetString("Image");
            this.TypeHint = ((Gdk.WindowTypeHint)(1));
            this.WindowPosition = ((Gtk.WindowPosition)(1));
            this.SkipTaskbarHint = true;
            this.HasSeparator = false;
            // Internal child MonoDevelop.Database.Components.ShowImageDialog.VBox
            Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog1_VBox";
            w1.BorderWidth = ((uint)(2));
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.scrolledwindow = new Gtk.ScrolledWindow();
            this.scrolledwindow.CanFocus = true;
            this.scrolledwindow.Name = "scrolledwindow";
            this.scrolledwindow.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow.Gtk.Container+ContainerChild
            Gtk.Viewport w2 = new Gtk.Viewport();
            w2.ShadowType = ((Gtk.ShadowType)(0));
            // Container child GtkViewport.Gtk.Container+ContainerChild
            this.image1 = new Gtk.Image();
            this.image1.Name = "image1";
            w2.Add(this.image1);
            this.scrolledwindow.Add(w2);
            w1.Add(this.scrolledwindow);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(w1[this.scrolledwindow]));
            w5.Position = 0;
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.hboxError = new Gtk.HBox();
            this.hboxError.Name = "hboxError";
            this.hboxError.Spacing = 6;
            // Container child hboxError.Gtk.Box+BoxChild
            this.image = new Gtk.Image();
            this.image.Name = "image";
            this.image.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-dialog-error", Gtk.IconSize.Menu, 16);
            this.hboxError.Add(this.image);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.hboxError[this.image]));
            w6.Position = 0;
            w6.Expand = false;
            w6.Fill = false;
            // Container child hboxError.Gtk.Box+BoxChild
            this.label = new Gtk.Label();
            this.label.Name = "label";
            this.label.Xalign = 0F;
            this.label.LabelProp = AddinCatalog.GetString("Unable to load object as Image");
            this.hboxError.Add(this.label);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.hboxError[this.label]));
            w7.Position = 1;
            w1.Add(this.hboxError);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(w1[this.hboxError]));
            w8.Position = 1;
            w8.Expand = false;
            w8.Fill = false;
            // Internal child MonoDevelop.Database.Components.ShowImageDialog.ActionArea
            Gtk.HButtonBox w9 = this.ActionArea;
            w9.Name = "dialog1_ActionArea";
            w9.Spacing = 6;
            w9.BorderWidth = ((uint)(5));
            w9.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonClose = new Gtk.Button();
            this.buttonClose.CanDefault = true;
            this.buttonClose.CanFocus = true;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseStock = true;
            this.buttonClose.UseUnderline = true;
            this.buttonClose.Label = "gtk-close";
            this.AddActionWidget(this.buttonClose, -7);
            Gtk.ButtonBox.ButtonBoxChild w10 = ((Gtk.ButtonBox.ButtonBoxChild)(w9[this.buttonClose]));
            w10.Expand = false;
            w10.Fill = false;
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 400;
            this.DefaultHeight = 300;
            this.hboxError.Hide();
            this.Show();
            this.buttonClose.Clicked += new System.EventHandler(this.CloseClicked);
        }
    }
}
