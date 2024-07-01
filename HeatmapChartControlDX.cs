using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Heatmap;
using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class HeatmapChartControlDX : DevExpress.XtraEditors.XtraUserControl, IMessageFilterReceiver
    {
        #region Constants
        int RECTANGULAR_ZOOM_CONTROL_MARGIN = 2;
        #endregion


        #region Fields
        // HeatmapControl of DevExpress.
        HeatmapControl heatmapControl;
        Chart heatmapControlChart;
        int[] xAxis;
        int[] yAxis;
        int[][] valuesToPlot;
        int xStartIdx;
        int xEndIdx;
        int yStartIdx;
        int yEndIdx;
        int xStartPxls;
        int xEndPxls;
        int yStartPxls;
        int yEndPxls;
        int currentXIdxCoord;
        int currentYIdxCoord;
        bool isMouseDown;
        bool isCtrlKeyPressed;
        bool dataHasBeenUpdated;
        bool isToPaint;
        bool isCopyPasteInAction;
        bool isRectangleCopyPasteSelectionActive;

        MessageFilter message;
        #endregion


        #region Constructor
        public HeatmapChartControlDX()
        {
            InitializeComponent();
            CreateHeatMapControl();
            // Create a MessageFilter Object.
            message = new MessageFilter(this);

            //Application.AddMessageFilter(message);
        }
        #endregion


        #region Properties 
        /// <summary>
        /// Set the X axis.
        /// </summary>
        public int[] XAxis
        {
            get { return this.xAxis; }
            set
            {
                if (this.xAxis != value)
                {

                    this.xAxis = value;

                    // Assign the new X axis ticks to the heatmap.
                    ((MatrixDataAdapterExt)heatmapControl.DataAdapter).XArguments = this.xAxis;
                }
            }
        }
        /// <summary>
        /// Set the Y axis.
        /// </summary>
        public int[] YAxis
        {
            get { return this.yAxis; }
            set
            {
                if (this.yAxis != value)
                {

                    this.yAxis = value;

                    // Assign the new Y axis ticks to the heatmap.
                    ((MatrixDataAdapterExt)heatmapControl.DataAdapter).YArguments = this.yAxis;
                }
            }
        }
        /// <summary>
        /// Set the Value to plot.
        /// </summary>
        public int[][] ValuesToPlot
        {
            get { return this.valuesToPlot; }
            set
            {
                this.valuesToPlot = value;

                //((HeatmapMatrixAdapter)heatmapControl.DataAdapter).Values = null;

                // ---- Assign the new X axis ticks to the heatmap.
                // Before convert the 0s and 1s of the jagged array into Colors.
                int[][] colorsToPlot = ConvertValuesToColors(this.valuesToPlot);
                // Only if the Y axis orientation of the matrix needs to be inverted.
                // int[][] colorsToPlotInverted = InvertYAxisOrientation(colorsToPlot);
                // Then convert the double jagged array into a 2D matrix.
                double[,] colorsToPlotMatrix = ReshapeIntoMatrix(colorsToPlot);

                if (colorsToPlotMatrix.GetLength(0) != this.yAxis.Length || colorsToPlotMatrix.GetLength(1) != this.xAxis.Length)
                    return;

                // Now assign the new values to the HeatMap.
                ((MatrixDataAdapterExt)heatmapControl.DataAdapter).Values = colorsToPlotMatrix;
            }
        }
        public bool DataHasBeenUpdated
        {
            get { return this.dataHasBeenUpdated; }
            set
            {
                if (this.dataHasBeenUpdated != value)
                {
                    this.dataHasBeenUpdated = value;
                }
            }
        }
        public bool IsCopyPasteInAction
        {
            get { return this.isCopyPasteInAction; }
            set { this.isCopyPasteInAction = value; }
        }
        #endregion


        #region Methods
        private void CreateHeatMapControl()
        {
            // Cerate an instance of the DevExpress Heatmap Control.
            heatmapControl = new HeatmapControl();
            heatmapControlChart = ((IChartContainer)heatmapControl).Chart;

            // Docking style of the heatmap control. 
            heatmapControl.Dock = DockStyle.Fill;

            // Set the Appearance of the HeatMap Chart.
            ChartAppearance();

            // Add the heatmap to the Controls list. 
            this.Controls.Add(heatmapControl);

            // Initialize the heatmap DataAdapter, which allows you to use collections of values to specify x-arguments and y-arguments, and pass a matrix of numeric values to define heatmap cell values. 
            heatmapControl.DataAdapter = new MatrixDataAdapterExt();

            heatmapControl.MouseDown -= HeatmapControl_MouseDown;
            heatmapControl.MouseDown += HeatmapControl_MouseDown;
            heatmapControl.MouseMove -= HeatmapControl_MouseMove;
            heatmapControl.MouseMove += HeatmapControl_MouseMove;
            heatmapControl.MouseUp -= HeatmapControl_MouseUp;
            heatmapControl.MouseUp += HeatmapControl_MouseUp;

            heatmapControl.Paint -= HeatmapControl_Paint;
            heatmapControl.Paint += HeatmapControl_Paint;

            heatmapControl.KeyPress -= HeatmapControl_KeyPress;
            heatmapControl.KeyPress += HeatmapControl_KeyPress;

            heatmapControl.MouseClick += HeatmapControl_MouseClick;
        }
        /// <summary>
        /// Define the appearance of the HeatMap Chart Control.
        /// </summary>
        private void ChartAppearance()
        {
            heatmapControlChart.Border.Visibility = DevExpress.Utils.DefaultBoolean.False;
            heatmapControlChart.Padding.All = 0;

            ((XYDiagram)heatmapControlChart.Diagram).Margins.All = 0;
            ((XYDiagram)heatmapControlChart.Diagram).DefaultPane.BorderVisible = true;

            heatmapControl.Diagram.BorderVisible = true;
            heatmapControl.Diagram.BackColor = Color.Green;

            heatmapControl.HighlightMode = HeatmapHighlightMode.Cell;

            // Axis Visibility.
            heatmapControl.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.False;
            heatmapControl.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.False;
            heatmapControl.AxisX.Visibility = DevExpress.Utils.DefaultBoolean.False;
            heatmapControl.AxisY.Visibility = DevExpress.Utils.DefaultBoolean.False;
            heatmapControl.AxisX.Label.Visible = false;
            heatmapControl.AxisY.Label.Visible = false;
            heatmapControl.AxisX.Tickmarks.Visible = false;
            heatmapControl.AxisY.Tickmarks.Visible = false;
        }
        #endregion


        #region Utility Methods
        /// <summary>
        /// Method for converting the numerical values of the valueToPlot matrix into Colors.
        /// </summary>
        /// <returns></returns>
        private int[][] ConvertValuesToColors(int[][] valuesToPlotNumerical)
        {
            int[][] valuesToPlotColors = new int[valuesToPlotNumerical.Length][];

            // Loop through all the values of the valuesToPlotNumerical and convert them to colors. For the moment Black and White.
            for (int row = 0; row < valuesToPlotNumerical.Length; row++)
            {
                valuesToPlotColors[row] = new int[valuesToPlotNumerical[row].Length];

                for (int col = 0; col < valuesToPlotNumerical[row].Length; col++)
                {
                    // If the value is 1, then convert it to BLACK;
                    if (valuesToPlotNumerical[row][col] == 1)
                    {
                        valuesToPlotColors[row][col] = Color.Blue.ToArgb();
                    }
                    else if (valuesToPlotNumerical[row][col] == 2)
                    {
                         Color newColor = Color.FromArgb(245, 216, 224);
                        valuesToPlotColors[row][col] = newColor.ToArgb();
                    }
                    // If the value is 0, then convert it to LIGHT GRAY;
                    else
                    {
                        valuesToPlotColors[row][col] = Color.LightGray.ToArgb();
                    }
                }
            }
            return valuesToPlotColors;
        }
        /// <summary>
        /// Method used only when there is the need to invert the Y axis of the matrix. 
        /// </summary>
        /// <param name="colorsToPlot"></param>
        /// <returns></returns>
        private int[][] InvertYAxisOrientation(int[][] colorsToPlot)
        {
            int[][] colorsToPlotInverted = new int[colorsToPlot.Length][];

            for (int row = 0; row < colorsToPlot.Length; row++)
            {
                // Compute the new inverted index on the Y axis.
                int newRowIdx = colorsToPlot.Length - 1 - row;

                colorsToPlotInverted[newRowIdx] = colorsToPlot[row];
            }
            return colorsToPlotInverted;
        }
        /// <summary>
        /// Method for reshaping the double jagged array into a 2D matrix. 
        /// </summary>
        /// <param name="colorsToPlotJagged"></param>
        /// <returns></returns>
        private double[,] ReshapeIntoMatrix(int[][] colorsToPlotJagged)
        {
            double[,] colorsToPlotMatrix = new double[colorsToPlotJagged.Length, colorsToPlotJagged[0].Length];

            for (int row = 0; row < colorsToPlotJagged.Length; row++)
            {
                for (int col = 0; col < colorsToPlotJagged[row].Length; col++)
                {
                    colorsToPlotMatrix[row, col] = (double)colorsToPlotJagged[row][col];
                }
            }
            return colorsToPlotMatrix;
        }
        public void ClearData()
        {
            // Re-Initialize the HeatMapControl Data Adapter.
            heatmapControl.DataAdapter = new HeatmapMatrixAdapter();
        }
        #endregion


        #region EventHandler
        protected void RaiseHeatMapCellSelection(object sender, int x, int y, bool isFromRightClick)
        {
            HeatMapCellSelection?.Invoke(sender, new SelectedCellsEventArgs(x, y, isFromRightClick));
        }
        protected void RaiseHeatMapCopyPasteSelection(object sender, int xStart, int yStart, int xEnd, int yEnd)
        {
            HeatMapCopyPasteSelection?.Invoke(sender, new CopyPasteSelectionEventArgs(xStart, yStart, xEnd, yEnd));
        }
        private void RaiseHeatMapCopyPasteCtrlC(object sender, KeyPressEventArgs e)
        {
            HeatMapCopyPasteCtrlC?.Invoke(sender, e);
        }
        private void RaiseHeatMapCopyPasteCtrlV(object sender, int xIdx, int yIdx)
        {
            HeatMapCopyPasteCtrlV?.Invoke(sender, new CopyPasteCtrlVEventArgs(xIdx, yIdx));
        }
        private void RaiseHeatMapCopyPasteCtrlX(object sender, KeyPressEventArgs e)
        {
            HeatMapCopyPasteCtrlX?.Invoke(sender, e);
        }
        private void HeatmapControl_MouseDown(object sender, MouseEventArgs e)
        {
            // Set the boolean isMouseDown to True.
            this.isMouseDown = true;
            // Set the isToPaint bool to false, so that the copy paste highlighting square will disappear.
            this.isToPaint = false;

            // Check if the Ctrl key is pressed. If it is, then it means that we are selecting the area for the copy paste action.
            if (ModifierKeys == Keys.Control)
            {
                this.isCtrlKeyPressed = true;

                // Store the X and Y coordinates indexes.
                HeatmapHitInfo mouseDownInfo = heatmapControl.CalcHitInfo(e.Location);
                if (mouseDownInfo.Cell != null)
                {
                    // Extract the X and Y indexes of the selected cell.
                    this.xStartIdx = (int)mouseDownInfo.Cell.XArgument;
                    this.yStartIdx = (int)mouseDownInfo.Cell.YArgument;
                }

                // Retrieve and also store the X and Y pixels coordinates of the starting point.
                this.xStartPxls = e.X;
                this.yStartPxls = e.Y;
                this.xEndPxls = e.X;
                this.yEndPxls = e.Y;

                this.isToPaint = true;
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Ways of listening the events that coming from buttons and actions. ///////////////////////////////////
            // 
            // 1) ---- ModifierKeys
            //
            // if (ModifierKeys
            // Keys.Control)
            // {
            //     // Check if the ModifierKeys (the pressed key) is the Ctrl key. 
            // }
            //
            //
            // 2) ---- Message Filter Class
            //
            // Application.AddMessageFilter(message);
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
        }
        private void HeatmapControl_MouseMove(object sender, MouseEventArgs e)
        {
            //this.isMouseMoving = true;

            if (this.isMouseDown)
            {
                HeatmapHitInfo mouseMoveInfo = heatmapControl.CalcHitInfo(e.Location);

                // First check if the Ctrl key is pressed.
                if (ModifierKeys == Keys.Control)
                {
                    // In this case the Ctrl key has been pressed while the mouse was down and moving (therefore the mouse was used for painting)
                    // Interrupt the paining and start the 
                    if (this.isCtrlKeyPressed == false)
                    {
                        this.isCtrlKeyPressed = true;
                        this.isToPaint = true;

                        // Store the X and Y index coordinates as the starting ones.
                        // Sometimes can happen (if you select the cell in a specific position) that the xStartIdx and yStartIdx don't take the value of the clicked cell.
                        if (mouseMoveInfo.Cell != null || ((this.xStartPxls == 0 || this.yStartIdx == 0) && mouseMoveInfo.Cell != null))
                        {
                            // Extract the X and Y indexes of the selected cell.
                            this.xStartIdx = (int)mouseMoveInfo.Cell.XArgument;
                            this.yStartIdx = (int)mouseMoveInfo.Cell.YArgument;
                        }
                        // Retrieve and also store the X and Y pixels coordinates of the starting point.
                        // Do things for copy paste. Paint for the square.
                        //heatmapControl.Paint -= HeatmapControl_Paint;
                        //heatmapControl.Paint += HeatmapControl_Paint;
                        this.xStartPxls = e.X;
                        this.yStartPxls = e.Y;
                        this.xEndPxls = e.X;
                        this.yEndPxls = e.Y;
                        heatmapControl.Refresh();
                        //heatmapControl.Paint -= HeatmapControl_Paint;
                    }
                    else
                    {
                        // Always store the current X and Y index coordinates as the end ones. Just in case, even if I then save the final one at mouse up.
                        if (mouseMoveInfo.Cell != null || ((this.xEndPxls == 0 || this.yEndIdx == 0) && mouseMoveInfo.Cell != null))
                        {
                            // Extract the X and Y indexes of the selected cell.
                            this.xEndIdx = (int)mouseMoveInfo.Cell.XArgument;
                            this.yEndIdx = (int)mouseMoveInfo.Cell.YArgument;
                        }
                        //// Do things for copy paste. Paint for the square.
                        //heatmapControl.Paint -= HeatmapControl_Paint;
                        //heatmapControl.Paint += HeatmapControl_Paint;
                        this.xEndPxls = e.X;
                        this.yEndPxls = e.Y;
                        heatmapControl.Refresh();
                        //heatmapControl.Paint -= HeatmapControl_Paint;

                    }
                }
                // If it is not, then do action for the painting of the heatmap cell.
                else
                {
                    // If it enters in this else, means that the isCtrlKeyPressed is not pressed or has been released.
                    this.isCtrlKeyPressed = false;

                    if (mouseMoveInfo.Cell != null)
                    {
                        // Extract the X and Y indexes of the selected cell.
                        int x = (int)mouseMoveInfo.Cell.XArgument;
                        int y = (int)mouseMoveInfo.Cell.YArgument;

                        // Check if the mouse click event is coming from the right or left click on the mouse. If it comes from the right, means that we are erasing.
                        if (e.Button == MouseButtons.Right)
                            RaiseHeatMapCellSelection(this, x, y, true);
                        else
                            RaiseHeatMapCellSelection(this, x, y, false);
                    }
                }
            }
        }
        private void HeatmapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.isMouseDown)
            {
                HeatmapHitInfo mouseUpInfo = heatmapControl.CalcHitInfo(e.Location);

                // If the Ctrl key is pressed, I'm selected the cells for the Copy Paste action.
                if (this.isCtrlKeyPressed == true)
                {
                    if (mouseUpInfo.Cell != null)
                    {
                        // Extract the X and Y indexes where the mouse click has been released.
                        this.xEndIdx = (int)mouseUpInfo.Cell.XArgument;
                        this.yEndIdx = (int)mouseUpInfo.Cell.YArgument;
                    }

                    heatmapControl.Refresh();

                    this.isCopyPasteInAction = true;
                                    
                    RaiseHeatMapCopyPasteSelection(this, this.xStartIdx, this.yStartIdx, this.xEndIdx, this.yEndIdx);
                }
                // Otherwise I'm painting the heatmap cells.
                else
                {
                    // If this command arrives when the rectangle for the copy paste selection is active, then just remove the rectangle.
                    if (this.isRectangleCopyPasteSelectionActive)
                    {
                        this.isRectangleCopyPasteSelectionActive = false;
                        //this.isMouseDown = false;
                        //this.isCtrlKeyPressed = false;
                        //return;
                    }
                    else
                    {
                        if (mouseUpInfo.Cell != null)
                        {
                            int x = (int)mouseUpInfo.Cell.XArgument;
                            int y = (int)mouseUpInfo.Cell.YArgument;

                            // Check if the mouse click event is coming from the right or left click on the mouse. If it comes from the right, means that we are erasing.
                            if (e.Button == MouseButtons.Right)
                                RaiseHeatMapCellSelection(this, x, y, true);
                            else
                                RaiseHeatMapCellSelection(this, x, y, false);
                        }
                    }
                }
            }
            this.isMouseDown = false;
            this.isCtrlKeyPressed = false;
            //this.isToPaint = true;
        }
        private void HeatmapControl_Paint(object sender, PaintEventArgs e)
        {
            // If you want the red border to not disappear, then comment this isToPaint check.
            if (this.isToPaint == false)
                return;

            this.isRectangleCopyPasteSelectionActive = true;

            // Rectangle width and height.
            float rectangleWidth = 0;
            float rectangleHeight = 0;
            // Rectangle X and Y starting point.
            float xStartN = 0;
            float yStartN = 0;

            rectangleWidth = Math.Abs(this.xEndPxls - this.xStartPxls);
            rectangleHeight = Math.Abs(this.yEndPxls - this.yStartPxls);
            if (this.xEndPxls >= this.xStartPxls && this.yEndPxls >= this.yStartPxls)
            {
                xStartN = this.xStartPxls;
                yStartN = this.yStartPxls;

                // Check whether the rectangle to draw exceeds the dimension of the control itself. Minus 5 pixels because of the DefaultMargins.
                if (xStartN + rectangleWidth > this.heatmapControl.Width - RECTANGULAR_ZOOM_CONTROL_MARGIN)
                {
                    rectangleWidth = this.heatmapControl.Width - xStartN - RECTANGULAR_ZOOM_CONTROL_MARGIN;
                }
                if (yStartN + rectangleHeight > this.heatmapControl.Height - RECTANGULAR_ZOOM_CONTROL_MARGIN)
                {
                    rectangleHeight = this.heatmapControl.Height - yStartN - RECTANGULAR_ZOOM_CONTROL_MARGIN;
                }
            }
            else if (this.xEndPxls >= this.xStartPxls && this.yEndPxls <= this.yStartPxls)
            {
                xStartN = this.xStartPxls;
                yStartN = this.yEndPxls;

                // Check whether the rectangle to draw exceeds the dimension of the control itself. Minus 5 pixels because of the DefaultMargins.
                if (yStartN < RECTANGULAR_ZOOM_CONTROL_MARGIN)
                {
                    yStartN = RECTANGULAR_ZOOM_CONTROL_MARGIN;
                    rectangleHeight = this.yStartPxls - yStartN;
                }
                if (xStartN + rectangleWidth > this.heatmapControl.Width - RECTANGULAR_ZOOM_CONTROL_MARGIN)
                {
                    rectangleWidth = this.heatmapControl.Width - xStartN - RECTANGULAR_ZOOM_CONTROL_MARGIN;
                }
            }
            else if (this.xEndPxls <= this.xStartPxls && this.yEndPxls <= this.yStartPxls)
            {
                xStartN = this.xEndPxls;
                yStartN = this.yEndPxls;

                // Check whether the rectangle to draw exceeds the dimension of the control itself. Minus 5 pixels because of the DefaultMargins.
                if (yStartN < RECTANGULAR_ZOOM_CONTROL_MARGIN)
                {
                    yStartN = RECTANGULAR_ZOOM_CONTROL_MARGIN;
                    rectangleHeight = this.yStartPxls - yStartN;
                }
                if (xStartN < RECTANGULAR_ZOOM_CONTROL_MARGIN)
                {
                    xStartN = RECTANGULAR_ZOOM_CONTROL_MARGIN;
                    rectangleWidth = this.xStartPxls - xStartN;
                }
            }
            else if (this.xEndPxls <= this.xStartPxls && this.yEndPxls >= this.yStartPxls)
            {
                xStartN = this.xEndPxls;
                yStartN = this.yStartPxls;

                // Check whether the rectangle to draw exceeds the dimension of the control itself. Minus 5 pixels because of the DefaultMargins.
                if (xStartN < RECTANGULAR_ZOOM_CONTROL_MARGIN)
                {
                    xStartN = RECTANGULAR_ZOOM_CONTROL_MARGIN;
                    rectangleWidth = this.xStartPxls - xStartN;
                }
                if (yStartN + rectangleHeight > this.heatmapControl.Height - RECTANGULAR_ZOOM_CONTROL_MARGIN)
                {
                    rectangleHeight = this.heatmapControl.Height - yStartN - RECTANGULAR_ZOOM_CONTROL_MARGIN;
                }
            }

            // If the compute rectangle Width or Height are 0, then assign them a value of 1.
            if (rectangleWidth == 0)
                rectangleWidth = 1;
            if (rectangleHeight == 0)
                rectangleHeight = 1;

            // ---- Draw Rectangle
            Pen p = new Pen(Color.Red, 2);
            Brush b = new SolidBrush(Color.FromArgb(130, Color.Red));
            e.Graphics.DrawRectangle(p, xStartN, yStartN, rectangleWidth, rectangleHeight);
            e.Graphics.FillRectangle(b, xStartN, yStartN, rectangleWidth, rectangleHeight);
        }
        private void HeatmapControl_MouseClick(object sender, MouseEventArgs e)
        {
            HeatmapHitInfo mouseClickInfo = heatmapControl.CalcHitInfo(e.Location);

            if (mouseClickInfo.Cell != null)
            {
                this.currentXIdxCoord = (int)mouseClickInfo.Cell.XArgument;
                this.currentYIdxCoord = (int)mouseClickInfo.Cell.YArgument;
            }            
        }
        private void HeatmapControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the combo of key pressed is Ctrl+C (Copy command).
            if (e.KeyChar == '\u0003')
            {
                this.isToPaint = false;
                heatmapControl.Refresh();
                // Raise the Copy (Ctrl+C) event delegate.
                RaiseHeatMapCopyPasteCtrlC(sender, e);
            }
            // Check if the combo of key pressed is Ctrl+X (Cut command).
            else if (e.KeyChar == '\u0018')
            {
                this.isToPaint = false;
                heatmapControl.Refresh();
                RaiseHeatMapCopyPasteCtrlX(sender, e);
            }
            // Check if the combo of key pressed is Ctrl+V (Paste command).
            else if (e.KeyChar == '\u0016')
            {
                RaiseHeatMapCopyPasteCtrlV(this, this.currentXIdxCoord, this.currentYIdxCoord);
            }
        }
        public bool OnMessageFilterMessageReceived(ref Message message)
        {
            //Mouse Left Button Up
            if (message.Msg == (int)0x0202)
            {
            }
            // Mouse Left Button Down
            else if (message.Msg == (int)0x0201)
            {
            }
            // Mouse Right Button Down
            else if (message.Msg == (int)0x0204)
            {
            }
            // Mouse Right Button Up
            else if (message.Msg == (int)0x0205)
            {
            }
            // Mouse Move
            else if (message.Msg == (int)0x0200)
            {
            }
            // Key Down
            else if (message.Msg == (int)0x0100)
            {
            }
            // Key Up
            else if (message.Msg == (int)0x0101)
            {
            }

            // Allow continue dispatching the message
            return false;
        }
        #endregion


        #region Event
        public event EventHandlerSelectedCell HeatMapCellSelection;
        public event EventHandlerCopyPasteSelection HeatMapCopyPasteSelection;
        public event EventHandlerCopyPasteCtrlC HeatMapCopyPasteCtrlC;
        public event EventHandlerCopyPasteCtrlV HeatMapCopyPasteCtrlV;
        public event EventHandlerCopyPasteCtrlX HeatMapCopyPasteCtrlX;
        #endregion
    }

    public class CopyPasteCtrlVEventArgs : EventArgs
    {
        #region Fields
        int xIdxCtrlV;
        int yIdxCtrlV;
        #endregion

        #region Properties
        public int XIdxCtrlV
        {
            get { return this.xIdxCtrlV; }
            set { this.xIdxCtrlV = value; }
        }
        public int YIdxCtrlV
        {
            get { return this.yIdxCtrlV; }
            set { this.yIdxCtrlV = value; }
        }
        #endregion

        #region Constructor
        public CopyPasteCtrlVEventArgs(int xIdxCtrlV, int yIdxCtrlV)
        {
            this.xIdxCtrlV = xIdxCtrlV;
            this.yIdxCtrlV = yIdxCtrlV;
        }
        #endregion
    }

    /// <summary>
    /// Arguments of the new EventHandlerClickOnSelectedCell for Cell Colorization Event Handler.
    /// </summary>
    public class SelectedCellsEventArgs : EventArgs
    {
        #region Fields
        int xIdx;
        int yIdx;
        bool isFromRightClick;
        #endregion

        #region Properties 
        public int XIdx
        {
            get { return xIdx; }
            set { this.xIdx = value; }
        }
        public int YIdx
        {
            get { return yIdx; }
            set { this.yIdx = value; }
        }
        public bool IsFromRightClick
        {
            get { return isFromRightClick; }
            set { this.isFromRightClick = value; }
        }
        #endregion

        #region Constructor
        public SelectedCellsEventArgs(int xIndex, int yIndex, bool isFromRightClick = false)
        {
            this.xIdx = xIndex;
            this.yIdx = yIndex;
            this.isFromRightClick = isFromRightClick;
        }
        #endregion
    }

    /// <summary>
    /// Arguments of the new CopyPasteSelectionEventArgs for the Copy Paste of the selected cells.
    /// </summary>
    public class CopyPasteSelectionEventArgs : EventArgs
    {
        #region Fields
        int xStartIdx;
        int xEndIdx;
        int yStartIdx;
        int yEndIdx;

        int cellLength;
        int cellWidth;
        #endregion

        #region Properties
        public int XStartIdx
        {
            get { return xStartIdx; }
            set { this.xStartIdx = value; }
        }
        public int XEndIdx
        {
            get { return xEndIdx; }
            set { this.xEndIdx = value; }
        }
        public int YStartIdx
        {
            get { return yStartIdx; }
            set { this.yStartIdx = value; }
        }
        public int YEndIdx
        {
            get { return yEndIdx; }
            set { this.yEndIdx = value; }
        }
        public int CellLength
        {
            get { return cellLength; }
            set { this.cellLength = value; }
        }
        public int CellWidth
        {
            get { return cellWidth; }
            set { this.cellWidth = value; }
        }
        #endregion

        #region Constructor
        public CopyPasteSelectionEventArgs(int xStartIdx, int yStartIdx, int xEndIdx, int yEndIdx)
        {
            this.xStartIdx = xStartIdx;
            this.yStartIdx = yStartIdx;
            this.xEndIdx = xEndIdx;
            this.yEndIdx = yEndIdx;

            ComputeWidhtAndLength();
        }

        private void ComputeWidhtAndLength()
        {
            this.cellLength = this.xEndIdx - this.xStartIdx;
            this.cellWidth = this.yEndIdx - this.yStartIdx;
        }
        #endregion
    }

    /// <summary>
    /// Delegate.
    /// </summary>
    public delegate void EventHandlerSelectedCell(object sender, SelectedCellsEventArgs e);
    public delegate void EventHandlerCopyPasteSelection(object sender, CopyPasteSelectionEventArgs e);
    public delegate void EventHandlerCopyPasteCtrlC(object sender, EventArgs e);
    public delegate void EventHandlerCopyPasteCtrlV(object sender, CopyPasteCtrlVEventArgs e);
    public delegate void EventHandlerCopyPasteCtrlX(object sender, EventArgs e);
}
