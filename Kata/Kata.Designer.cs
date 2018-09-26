namespace Kata {
    partial class Kata {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.button3 = new System.Windows.Forms.Button();
            this.buttonRandomSelect = new System.Windows.Forms.Button();
            this.labelResult = new System.Windows.Forms.Label();
            this.comboBoxLesson = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(75, 115);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 49);
            this.button3.TabIndex = 0;
            this.button3.Text = "Randomly Select";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // buttonRandomSelect
            // 
            this.buttonRandomSelect.Location = new System.Drawing.Point(75, 115);
            this.buttonRandomSelect.Name = "buttonRandomSelect";
            this.buttonRandomSelect.Size = new System.Drawing.Size(75, 49);
            this.buttonRandomSelect.TabIndex = 0;
            this.buttonRandomSelect.Text = "Randomly Select";
            this.buttonRandomSelect.UseVisualStyleBackColor = true;
            this.buttonRandomSelect.Click += new System.EventHandler(this.buttonRandomSelect_Click);
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Location = new System.Drawing.Point(72, 203);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(47, 13);
            this.labelResult.TabIndex = 2;
            this.labelResult.Text = "Exercise";
            // 
            // comboBoxLesson
            // 
            this.comboBoxLesson.FormattingEnabled = true;
            this.comboBoxLesson.Location = new System.Drawing.Point(75, 75);
            this.comboBoxLesson.Name = "comboBoxLesson";
            this.comboBoxLesson.Size = new System.Drawing.Size(387, 21);
            this.comboBoxLesson.TabIndex = 3;
            this.comboBoxLesson.SelectedIndexChanged += new System.EventHandler(this.comboBoxLesson_SelectedIndexChanged);
            // 
            // Kata
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 450);
            this.Controls.Add(this.comboBoxLesson);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.buttonRandomSelect);
            this.Controls.Add(this.button3);
            this.Name = "Kata";
            this.Text = "Kata";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button buttonRandomSelect;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.ComboBox comboBoxLesson;
    }
}

