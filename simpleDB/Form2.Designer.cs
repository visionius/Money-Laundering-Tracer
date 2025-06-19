namespace simpleDB
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gViewer = new Microsoft.Glee.GraphViewerGdi.GViewer();
            this.SuspendLayout();
            // 
            // gViewer
            // 
            this.gViewer.AsyncLayout = false;
            this.gViewer.AutoScroll = true;
            this.gViewer.BackwardEnabled = false;
            this.gViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gViewer.ForwardEnabled = false;
            this.gViewer.Graph = null;
            this.gViewer.Location = new System.Drawing.Point(0, 0);
            this.gViewer.Margin = new System.Windows.Forms.Padding(4);
            this.gViewer.MouseHitDistance = 0.05D;
            this.gViewer.Name = "gViewer";
            this.gViewer.NavigationVisible = true;
            this.gViewer.PanButtonPressed = false;
            this.gViewer.SaveButtonVisible = true;
            this.gViewer.Size = new System.Drawing.Size(880, 373);
            this.gViewer.TabIndex = 10;
            this.gViewer.ZoomF = 1D;
            this.gViewer.ZoomFraction = 0.5D;
            this.gViewer.ZoomWindowThreshold = 0.05D;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 373);
            this.Controls.Add(this.gViewer);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form2_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Glee.GraphViewerGdi.GViewer gViewer;
    }
}