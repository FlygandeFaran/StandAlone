namespace Standalone
{
    partial class StructureChart
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.structurePlot = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.structurePlot)).BeginInit();
            this.SuspendLayout();
            // 
            // structurePlot
            // 
            chartArea1.Name = "ChartArea1";
            this.structurePlot.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.structurePlot.Legends.Add(legend1);
            this.structurePlot.Location = new System.Drawing.Point(12, 12);
            this.structurePlot.Name = "structurePlot";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.structurePlot.Series.Add(series1);
            this.structurePlot.Size = new System.Drawing.Size(776, 426);
            this.structurePlot.TabIndex = 0;
            this.structurePlot.Text = "structurePlot";
            // 
            // StructureChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.structurePlot);
            this.Name = "StructureChart";
            this.Text = "StructureChart";
            ((System.ComponentModel.ISupportInitialize)(this.structurePlot)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart structurePlot;
    }
}