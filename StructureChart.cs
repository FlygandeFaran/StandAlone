using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using DVHextractor;

namespace Standalone
{
    public partial class StructureChart : Form
    {
        public StructureChart(VVector[][] oneSlice)
        {
            InitializeComponent();
            structurePlot.Series.First().ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            foreach (var vector in oneSlice)
                foreach (var point in vector)
                    structurePlot.Series.First().Points.AddXY(point.x, point.y);
        }
    }
}
