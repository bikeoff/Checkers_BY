namespace Checkers_BY
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.начатьНовуюИгруToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.завершитьИгруToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.начатьНовуюИгруToolStripMenuItem,
            this.завершитьИгруToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(624, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // начатьНовуюИгруToolStripMenuItem
            // 
            this.начатьНовуюИгруToolStripMenuItem.Name = "начатьНовуюИгруToolStripMenuItem";
            this.начатьНовуюИгруToolStripMenuItem.Size = new System.Drawing.Size(125, 20);
            this.начатьНовуюИгруToolStripMenuItem.Text = "Начать новую игру";
            this.начатьНовуюИгруToolStripMenuItem.Click += new System.EventHandler(this.начатьНовуюИгруToolStripMenuItem_Click);
            // 
            // завершитьИгруToolStripMenuItem
            // 
            this.завершитьИгруToolStripMenuItem.Name = "завершитьИгруToolStripMenuItem";
            this.завершитьИгруToolStripMenuItem.Size = new System.Drawing.Size(108, 20);
            this.завершитьИгруToolStripMenuItem.Text = "Завершить игру";
            this.завершитьИгруToolStripMenuItem.Click += new System.EventHandler(this.завершитьИгруToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Белорусские шашки";
            this.ClientSizeChanged += new System.EventHandler(this.Form1_ClientSizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem начатьНовуюИгруToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem завершитьИгруToolStripMenuItem;

    }
}

