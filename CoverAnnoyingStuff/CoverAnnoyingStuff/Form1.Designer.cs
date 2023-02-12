
namespace BlackboxApp
{
    partial class Form1
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
            this.samplePic = new SizeablePictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.samplePic)).BeginInit();
            this.SuspendLayout();
            // 
            // samplePic
            // 
            this.samplePic.BackColor = System.Drawing.Color.Black;
            this.samplePic.Location = new System.Drawing.Point(350, 200);
            this.samplePic.Name = "samplePic";
            this.samplePic.Size = new System.Drawing.Size(100, 50);
            this.samplePic.TabIndex = 2;
            this.samplePic.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.samplePic);
            this.Name = "Form1";
            this.Text = "Form1";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.White;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.samplePic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private SizeablePictureBox samplePic;
    }
}

