namespace Talent.RemoteCarMeasure
{
    partial class VideosForm
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.videoBig = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.videoBig)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panel2.Location = new System.Drawing.Point(0, 268);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(476, 111);
            this.panel2.TabIndex = 1;
            // 
            // videoBig
            // 
            this.videoBig.Location = new System.Drawing.Point(4, 3);
            this.videoBig.Name = "videoBig";
            this.videoBig.Size = new System.Drawing.Size(469, 259);
            this.videoBig.TabIndex = 2;
            this.videoBig.TabStop = false;
            // 
            // VideosForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.videoBig);
            this.Controls.Add(this.panel2);
            this.Name = "VideosForm";
            this.Size = new System.Drawing.Size(476, 379);
            this.Load += new System.EventHandler(this.VideosForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.videoBig)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel panel2;
        private System.Windows.Forms.PictureBox videoBig;
    }
}
