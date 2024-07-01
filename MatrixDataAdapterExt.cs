using DevExpress.Charts.Heatmap.Native;
using DevExpress.XtraCharts.Heatmap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class MatrixDataAdapterExt : HeatmapMatrixAdapter
    {
        public MatrixDataAdapterExt() : base()
        {

        }

        public HeatmapData HeatmapDataTest 
        {
            get { return base.HeatmapData; }

            set { base.HeatmapData = value; }
        }

        protected override bool IsCellEmpty(int x, int y)
        {
            try
            {
                return base.IsCellEmpty(x, y);

            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
