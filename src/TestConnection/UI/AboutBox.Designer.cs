namespace TestConnection {
    partial class AboutBox {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label productNameLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Button okButton;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            this.productNameLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // productNameLabel
            this.productNameLabel.AutoSize = true;
            this.productNameLabel.Location = new System.Drawing.Point(16, 18);
            this.productNameLabel.Name = "productNameLabel";
            this.productNameLabel.Size = new System.Drawing.Size(44, 12);
            this.productNameLabel.TabIndex = 0;
            this.productNameLabel.Text = "製品名";
            // versionLabel
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(16, 45);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(50, 12);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "バージョン";
            // descriptionTextBox
            this.descriptionTextBox.Location = new System.Drawing.Point(18, 72);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ReadOnly = true;
            this.descriptionTextBox.Size = new System.Drawing.Size(360, 86);
            this.descriptionTextBox.TabIndex = 2;
            this.descriptionTextBox.TabStop = false;
            // okButton
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(303, 174);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // AboutBox
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 213);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.descriptionTextBox);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.productNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TestConnection";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
