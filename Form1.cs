using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// The universe of the Game of Life is an infinite, two-dimensional orthogonal grid of square cells, each of which is in one of two possible states, live or dead (or populated and unpopulated, respectively).
        /// Every cell interacts with its eight neighbors, which are the cells that horizontally, vertically, or diagonally adjacent. At each step in time, the following transition occur:
        /// 
        /// 1) Any live cell with fewer than two live neighbors dies, as if by underpopulation. 
        /// 2) Any live cell with two or three live neighbors lives on to the next generation. 
        /// 3) Any live cell with more than three live neighbors dies, as if by overpopulation. 
        /// 4) Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction. 
        /// 
        /// These rules, which compare the behavior of the automaton to real life, can be condensed into the following:
        /// 
        /// 1) Any live cell with two or three live neighbors survives. 
        /// 2) Any dead cell with three live neighbors become a live cell.
        /// 3) all other live cells die in the next generation. similarly all other dead cells stay dead.
        /// 
        /// The initial pattern constitutes the seed of the system. The first generation is created by applying the above rules simultaneously to every cell in the seed, live or dead.
        /// Births and deaths occur simultaneously, and the discrete moment at which this happens is sometime called a tick. 
        /// Each generation is a pure function of the preceding one. The rules continue to be applied repeatedly to create further generations. 
        /// </summary>


        #region Constant
        int TOP_PANEL_HEIGHT = 20;
        int BOTTOM_PANEL_HEIGHT = 30;
        int TIMER_SPEED_INCREASE_DECREASE = 20;
        #endregion


        #region Fields
        HeatmapChartControlDX heatmapChartControl;
        SimpleButton startStopGenerationButton;
        PanelControl topLeftPanelControl;
        PanelControl topRightPanelControl;
        PanelControl bottomLeftPanelControl;
        PanelControl bottomRightPanelControl;
        Label speedTextEdit;
        Label cellSizeTextEdit;
        Label generationCounter;
        Label instructionText;
        SimpleButton clearButton;
        SimpleButton startFromBeginningButton;
        SimpleButton openRulesDialogButton;
        SimpleButton openFileButton;
        SimpleButton exportToFile;
        SimpleButton plusSpeedButton;
        SimpleButton minusSpeedButton;
        SimpleButton plusCellSizeButton;
        SimpleButton minusCellSizeButton;
        PanelControl rulesPanel;
        Thread gameOfLifeThr;
        int[][] startingValuesToPlot;
        int[][] currGenValuesToPlot;
        int[][] copyPasteStartingValuesToPlot;
        int[][] copyPasteCurrGenValuesToPlot;
        int nRowHeatmap;
        int nColHeatmap;
        int generationCount;
        int cellSizePixels;
        int timerSpeedMilliSecond;
        int maximumCellSizePossible;
        int minimumCellSizePossible;
        int maximumSpeedPossible;
        int minimumSpeedPossible;
        int xStartIdxCP;
        int yStartIdxCP;
        int xEndIdxCP;
        int yEndIdxCP;
        bool keepIteratig;
        bool hasMatrixValuesToPlot;
        bool isRulesPanelVisualized;
        #endregion


        #region Constructor
        public Form1()
        {
            // Initialize Components.
            InitializeComponent();
            // Set the name of the Form.
            this.Text = "Game Of Life";
            // Loading the Default variable Values.
            LoadDefaultValues();
            // Create the HeatmapControl and add it to the control list.
            InitializeHeatMapAndControls();
            // Initialize the dimensions and the number of the Heatmap rows and cols of the Grid based on the dimension of the form.
            InitializeDimensionsOfHeatmapGrid();
            // Initialize the empty starting heatmap matrix with the values to plot.
            InitializeHetamapMatrix();
            // Only at first opening of the Game Of Life Form.
            LoadDefaultPatternAtFirstStart();
            
        }
        #endregion


        #region Methods
        private void LoadDefaultPatternAtFirstStart()
        {
            //StreamReader reader = System.IO.File.OpenText(@"InfiniteSpaceshipGeneratorPattern.txt");
            StreamReader reader = null;

            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameOfLifePattern");
            // Check if the directory exists.
            if (Directory.Exists(folderPath))
            {
                reader = System.IO.File.OpenText(folderPath + "/" + "InfiniteSpaceshipGeneratorPattern.txt");
            }

            string line;

            List<char[]> parsedFile = new List<char[]>();

            if (reader != null)
            {
                // Loop through all the lines in the file.
                while ((line = reader.ReadLine()) != null)
                {
                    // REmove the empty spaces at the end of the line if there are. 
                    line = line.TrimEnd();
                    line = line.TrimStart();

                    // Split each character and store it in an array.
                    List<char> items = new List<char>();
                    // Loop through all the characters and store them in an array.
                    foreach (char c in line)
                    {
                        items.Add(c);
                    }
                    parsedFile.Add(items.ToArray());
                }

                if (parsedFile.Count == 0 || parsedFile[0].Length == 0)
                    return;

                // Draw and Add this starting pattern in the heatmap matrix.
                // First, in order to fo that, check that the uploaded matrix doesn't exceeds the dimension of the heatmap matrix in the control.
                while (parsedFile.Count + 10 > this.startingValuesToPlot.Length || parsedFile[0].Length + 10 > this.startingValuesToPlot[0].Length)
                {
                    if (this.cellSizePixels - 1 > this.minimumCellSizePossible)
                    {
                        this.cellSizePixels -= 1;
                        // I can modify the dimension of the heatmap matrix.
                        DecreaseCellSizeMatrixDimension();
                    }
                }

                // Then just add the uploaded matrix in the center of the heatmap control.
                // Find the index of the heatmap startingValuesToPlot where to start to copy the uploaded matrix.
                int rowStartIdx = (int)((this.startingValuesToPlot.Length - parsedFile.Count) / 2.0);
                int colStartIdx = (int)((this.startingValuesToPlot[0].Length - parsedFile[0].Length) / 2.0);

                // Copy (while rotating its orientation on the Y axis) the uploaded matrix in the startingValuesToPlot.
                for (int row = 0; row < parsedFile.Count; row++)
                {
                    for (int col = 0; col < parsedFile[0].Length; col++)
                    {
                        if (parsedFile[row][col] != '.')
                            this.startingValuesToPlot[this.startingValuesToPlot.Length - 1 - (rowStartIdx + row)][colStartIdx + col] = 1;
                    }
                }
                this.hasMatrixValuesToPlot = true;

                AssignValuesToPlot(this.startingValuesToPlot);
            }
        }
        /// <summary>
        /// Initialize the Default values of the variables.
        /// </summary>
        private void LoadDefaultValues()
        {
            // Initialize some variables Default Values.
            this.cellSizePixels = 10;
            this.timerSpeedMilliSecond = 200;
            this.keepIteratig = false;
            this.generationCount = 0;
            // Initialize the minimum and maximum values for the Cell Size and the Speed of the Refresh.
            // Minimum possible cell size is 2 pixels.
            this.minimumCellSizePossible = 2;
            // Maximum possible cell size is the initial with size of the panel divided by two.
            this.maximumCellSizePossible = (int)(this.Size.Width / 2.0);
            // Refresh is done every 1 second.
            this.minimumSpeedPossible = 1000;
            // Refresh is done every 20 milliSeconds.
            this.maximumSpeedPossible = 20;
        }
        /// <summary>
        /// Initialize all the controls that will be part of the Form.
        /// </summary>
        private void InitializeHeatMapAndControls()
        {
            // There will be two panels on the top and on the bottom of the window form.
            //
            // Form Name.

            // --- TOP PANELS which will contain some instruction and the start bottom.
            // LEFT PANEL
            this.topLeftPanelControl = new PanelControl();
            this.topRightPanelControl = new PanelControl();
            this.exportToFile = new SimpleButton();
            this.exportToFile.Text = "Export To File";
            this.exportToFile.Dock = DockStyle.Left;
            this.topLeftPanelControl.Controls.Add(this.exportToFile);
            this.openFileButton = new SimpleButton();
            this.openFileButton.Text = "Open From File";
            this.openFileButton.Dock = DockStyle.Left;
            this.topLeftPanelControl.Controls.Add(this.openFileButton);
            this.instructionText = new Label();
            this.instructionText.Text = "Draw your seed pattern and click start. Or: ";
            this.instructionText.Dock = DockStyle.Left;
            this.topLeftPanelControl.Controls.Add(this.instructionText);
            // RIGHT PANEL
            this.startStopGenerationButton = new SimpleButton();
            this.startStopGenerationButton.Text = "Start";
            this.startStopGenerationButton.Font = new Font("Arial", 10, FontStyle.Bold);
            this.startStopGenerationButton.Dock = DockStyle.Fill;
            this.topRightPanelControl.Controls.Add(this.startStopGenerationButton);
            // Adding the instruction Button which will show an instruction dialog.
            this.openRulesDialogButton = new SimpleButton();
            this.openRulesDialogButton.Text = "Game Rules";
            this.openRulesDialogButton.Font = new Font("Arial", 8, FontStyle.Bold);
            this.openRulesDialogButton.Dock = DockStyle.Left;
            //this.openRulesDialogButton.ForeColor = Color.LightBlue;
            this.topRightPanelControl.Controls.Add(this.openRulesDialogButton);
            // -- Add the two panels on top of the form.
            this.topLeftPanelControl.Dock = DockStyle.None;
            this.topRightPanelControl.Dock = DockStyle.None;
            this.Controls.Add(topRightPanelControl);
            this.Controls.Add(topLeftPanelControl);

            // Initialize the panel which will contain the GameOfLife instructions.
            rulesPanel = new PanelControl();
            InitializeRulesMessagePanel(); rulesPanel.Dock = DockStyle.Fill;
            this.Controls.Add(rulesPanel);
            rulesPanel.SendToBack();

            // --- HEATMAP CONTROL
            // Create and add the HeatMapChartControl.
            this.heatmapChartControl = new HeatmapChartControlDX();
            this.heatmapChartControl.Dock = DockStyle.None;
            //this.heatmapChartControl.DataHasBeenUpdated = true;
            this.Controls.Add(heatmapChartControl);
            heatmapChartControl.BringToFront();

            // --- BOTTOM PANELS which will contain the Generation counter, some spin edit about Velocity and 
            // LEFT PANEL
            this.bottomLeftPanelControl = new PanelControl();
            // Size Controls
            this.cellSizeTextEdit = new Label();
            this.cellSizeTextEdit.Text = "Cell Size: ";
            this.plusCellSizeButton = new SimpleButton();
            this.minusCellSizeButton = new SimpleButton();
            this.plusCellSizeButton.Text = "+";
            this.minusCellSizeButton.Text = "-";
            this.plusCellSizeButton.Font = new Font("Arial", 12, FontStyle.Bold);
            this.minusCellSizeButton.Font = new Font("Arial", 12, FontStyle.Bold);
            this.cellSizeTextEdit.Dock = DockStyle.Left;
            this.plusCellSizeButton.Dock = DockStyle.Left;
            this.minusCellSizeButton.Dock = DockStyle.Left;
            this.bottomLeftPanelControl.Controls.Add(this.plusCellSizeButton);
            this.bottomLeftPanelControl.Controls.Add(this.minusCellSizeButton);
            this.bottomLeftPanelControl.Controls.Add(this.cellSizeTextEdit);
            // Speed Controls
            this.speedTextEdit = new Label();
            this.speedTextEdit.Text = "Speed: ";
            this.plusSpeedButton = new SimpleButton();
            this.minusSpeedButton = new SimpleButton();
            this.plusSpeedButton.Text = "Speed Up";
            this.minusSpeedButton.Text = "Slow Down";
            this.plusSpeedButton.Font = new Font("Arial", 9, FontStyle.Bold);
            this.minusSpeedButton.Font = new Font("Arial", 9, FontStyle.Bold);
            this.speedTextEdit.Dock = DockStyle.Left;
            this.plusSpeedButton.Dock = DockStyle.Left;
            this.minusSpeedButton.Dock = DockStyle.Left;
            this.bottomLeftPanelControl.Controls.Add(this.plusSpeedButton);
            this.bottomLeftPanelControl.Controls.Add(this.minusSpeedButton);
            this.bottomLeftPanelControl.Controls.Add(this.speedTextEdit);
            // RIGHT PANEL
            this.bottomRightPanelControl = new PanelControl();
            // Start From the Beginning Button
            this.startFromBeginningButton = new SimpleButton();
            this.startFromBeginningButton.Text = "Reset to Initial Pattern";
            this.startFromBeginningButton.Dock = DockStyle.Right;
            this.bottomRightPanelControl.Controls.Add(this.startFromBeginningButton);
            // Clear Button
            this.clearButton = new SimpleButton();
            this.clearButton.Text = "Clear";
            this.clearButton.Dock = DockStyle.Right;
            this.clearButton.Font = new Font("Arial", 10, FontStyle.Bold);
            this.bottomRightPanelControl.Controls.Add(this.clearButton);
            // Generation Label
            this.generationCounter = new Label();
            this.generationCounter.Text = "Generation: 0";
            this.generationCounter.Dock = DockStyle.Left;
            this.bottomRightPanelControl.Controls.Add(this.generationCounter);
            // Add the two panels on the bottom of the form (after the HeatMap Controls)
            this.bottomLeftPanelControl.Dock = DockStyle.None;
            this.bottomRightPanelControl.Dock = DockStyle.None;
            this.Controls.Add(this.bottomLeftPanelControl);
            this.Controls.Add(this.bottomRightPanelControl);

            // Set the Bounds in the correct Position all the Panels and Controls.
            SetBoundsToControls();

            // Add the subscription to the events. 
            this.openFileButton.Click -= OpenFileButton_Click;
            this.openFileButton.Click += OpenFileButton_Click;
            this.exportToFile.Click -= ExportToFile_Click;
            this.exportToFile.Click += ExportToFile_Click;
            this.startFromBeginningButton.Click -= StartFromBeginningButton_Click;
            this.startFromBeginningButton.Click += StartFromBeginningButton_Click;
            this.startStopGenerationButton.Click -= StartStopGenerationButton_Click;
            this.startStopGenerationButton.Click += StartStopGenerationButton_Click;
            this.openRulesDialogButton.Click -= OpenRulesDialogButton_Click;
            this.openRulesDialogButton.Click += OpenRulesDialogButton_Click;
            this.clearButton.Click -= ClearButton_Click;
            this.clearButton.Click += ClearButton_Click;
            this.plusSpeedButton.Click += PlusSpeedButton_Click;
            this.minusSpeedButton.Click += MinusSpeedButton_Click;
            this.plusCellSizeButton.Click += IncreaseCellSizeButton_Click;
            this.minusCellSizeButton.Click += DecreaseCellSizeButton_Click;
            this.heatmapChartControl.HeatMapCellSelection -= HeatmapChartControl_HeatMapCellSelection;
            this.heatmapChartControl.HeatMapCellSelection += HeatmapChartControl_HeatMapCellSelection;
            this.heatmapChartControl.HeatMapCopyPasteSelection -= HeatmapChartControl_HeatMapCopyPasteSelection;
            this.heatmapChartControl.HeatMapCopyPasteSelection += HeatmapChartControl_HeatMapCopyPasteSelection;
            this.heatmapChartControl.HeatMapCopyPasteCtrlC -= HeatmapChartControl_HeatMapCopyPasteCtrlC;
            this.heatmapChartControl.HeatMapCopyPasteCtrlC += HeatmapChartControl_HeatMapCopyPasteCtrlC;
            this.heatmapChartControl.HeatMapCopyPasteCtrlX -= HeatmapChartControl_HeatMapCopyPasteCtrlX;
            this.heatmapChartControl.HeatMapCopyPasteCtrlX += HeatmapChartControl_HeatMapCopyPasteCtrlX;
            this.heatmapChartControl.HeatMapCopyPasteCtrlV -= HeatmapChartControl_HeatMapCopyPasteCtrlV;
            this.heatmapChartControl.HeatMapCopyPasteCtrlV += HeatmapChartControl_HeatMapCopyPasteCtrlV;
        }
        /// <summary>
        /// Initialize the Text that is displayed in the Rules and Information of the Game Of Life inside the "custom" panel.
        /// </summary>
        private void InitializeRulesMessagePanel()
        {
            string gameOfLifeRulesAndInformations =
                "The Universe of the Game of Life is a two-dimensional orthogonal grid of square cells, each of which is in one of two possible states, \"live\" or \"dead\".\r\n" +
                "Every cell interacts with its eight neighbots, which are the cells horizonally, vertically, or diagonally adjacent. At each step in time, the following transition occur:\r\n" +
                "\r\n1. Any live cell with fewer than two live neighbors dies, as if by underpopulation.\r\n" +
                "2. Any live cell with two or three live neighbors lives on to the next generation.\r\n" +
                "3. Any live cell with more than three live neighbors dies, as if by overpopulation.\r\n" +
                "4. Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction." +
                "\r\n\r\n" +
                "Command for the usage:\r\n" +
                "   - Left mouse click -->      Select / Make Cell Alive\r\n" +
                "   - Right mouse click -->     Erease / Kill Cell\r\n" +
                "   - Ctrl + Right mouse click + Drag -->       Select Multiple Cells / Make Multiple Cells Alive\r\n" +
                "   - Ctrl + C or X (After Multiple Cell Selection) -->     Copy / Cut Selected Cells\r\n" +
                "   - Ctrl + V -->      Paste Selected Cells" +
                "\r\n\r\n\r\n" +
                "Starting Pattern: \r\n" +
                "https://github.com/AlessandroCaula/ProgrammingForFun/tree/main/GameOfLife/GameOfLifeStartingPatterns";

            TextBox rulesText = new TextBox();
            rulesText.Font = new Font(rulesText.Font.ToString(), 12);
            rulesText.Dock = DockStyle.Fill;
            rulesText.Multiline = true;
            rulesText.Text = gameOfLifeRulesAndInformations;
            rulesPanel.Controls.Add(rulesText);

            SimpleButton closeRulesPanelButton = new SimpleButton();
            closeRulesPanelButton.Text = "Close";
            closeRulesPanelButton.Font = new Font("Arial", 15, FontStyle.Bold);            
            closeRulesPanelButton.Dock = DockStyle.Bottom;
            rulesPanel.Controls.Add(closeRulesPanelButton);
            closeRulesPanelButton.BackColor = Color.LightBlue;

            // Add the event, which is fired when the Close button is clicked in the Rules and Information panel. 
            closeRulesPanelButton.Click -= CloseRulesPanelButton_Click;
            closeRulesPanelButton.Click += CloseRulesPanelButton_Click;
        }
        /// <summary>
        /// Set the Dock Bounds of the controls added to the Form.
        /// </summary>
        private void SetBoundsToControls()
        {
            if (this.topLeftPanelControl == null)
                return;

            // TOP PANELS position.
            int leftPanelWidth = (int)(this.Size.Width / 2);
            this.topLeftPanelControl.SetBounds(0, 0, leftPanelWidth, TOP_PANEL_HEIGHT);
            this.instructionText.Width = TextRenderer.MeasureText(this.instructionText.Text, this.cellSizeTextEdit.Font).Width + 10;
            this.openFileButton.Width = (leftPanelWidth - this.instructionText.Width) / 2;
            this.exportToFile.Width = (leftPanelWidth - this.instructionText.Width) / 2;
            this.topRightPanelControl.SetBounds(leftPanelWidth, 0, this.Size.Width - leftPanelWidth, TOP_PANEL_HEIGHT);
            this.topLeftPanelControl.BringToFront();
            this.topRightPanelControl.BringToFront();
            // HEATMAP position
            int heatmapHeight = this.Size.Height - TOP_PANEL_HEIGHT - BOTTOM_PANEL_HEIGHT - 40;
            this.heatmapChartControl.SetBounds(0, TOP_PANEL_HEIGHT, this.Size.Width - 16, heatmapHeight);
            // BOTTOM PANELS position
            this.bottomLeftPanelControl.SetBounds(0, heatmapHeight + 20, leftPanelWidth, BOTTOM_PANEL_HEIGHT);
            this.bottomRightPanelControl.SetBounds(leftPanelWidth, heatmapHeight + 20, this.Size.Width - leftPanelWidth, BOTTOM_PANEL_HEIGHT);
            this.bottomLeftPanelControl.BringToFront();
            this.bottomRightPanelControl.BringToFront();
            // Set the width of the speed and cell labels.
            this.speedTextEdit.Width = TextRenderer.MeasureText("Speed: ", this.speedTextEdit.Font).Width + 5;
            this.cellSizeTextEdit.Width = TextRenderer.MeasureText("Cell Size: ", this.cellSizeTextEdit.Font).Width + 5;
            // Set the length of the TrackBar
            int leftPanelSpaceForPlusMinusButtons = (int)((leftPanelWidth - (this.speedTextEdit.Width + this.cellSizeTextEdit.Width) - 5) / 2.0);
            int leftPanelSpaceForSingleButton = (int)(leftPanelSpaceForPlusMinusButtons / 2.0);
            this.plusCellSizeButton.Width = leftPanelSpaceForSingleButton;
            this.minusCellSizeButton.Width = leftPanelSpaceForSingleButton;
            this.plusSpeedButton.Width = leftPanelSpaceForSingleButton;
            this.minusSpeedButton.Width = leftPanelSpaceForSingleButton;
            // Set the dimensions of the text and buttons in the bottom right panel.
            int rightPanelControlsWidth = (int)(leftPanelWidth / 3.0);
            this.generationCounter.Width = rightPanelControlsWidth;
            this.clearButton.Width = rightPanelControlsWidth;
            this.startFromBeginningButton.Width = rightPanelControlsWidth;
        }
        /// <summary>
        /// Initialize the first dimensions of the Heatmap Grid.
        /// </summary>
        private void InitializeDimensionsOfHeatmapGrid()
        {
            // Retrieve the dimensions of the current form. 
            double formHighPixels = this.Size.Height - TOP_PANEL_HEIGHT - BOTTOM_PANEL_HEIGHT - 18;
            double formWidthPixels = this.Size.Width;
            // Compute the number of rows and columns of the heatmap grid.
            this.nRowHeatmap = (int)(formHighPixels / this.cellSizePixels);
            this.nColHeatmap = (int)(formWidthPixels / this.cellSizePixels);

            // Check if the number of rows and/or columns is not an even number. In that case change it, and make it even. 
            if (this.nRowHeatmap % 2 != 0)
                this.nRowHeatmap += 1;
            if (this.nColHeatmap % 2 != 0)
                this.nColHeatmap += 1;
        }
        /// <summary>
        /// Thread iteration of the GameOfLife.
        /// </summary>
        private void GameOfLifeIterations()
        {
            if (this.hasMatrixValuesToPlot)
            {
                // Thread function
                this.gameOfLifeThr = new Thread(GameOfLifeThreadIteration);
                gameOfLifeThr.Start();
            }
        }
        /// <summary>
        /// Thread of the Game of Life Iteration.
        /// </summary>
        private void GameOfLifeThreadIteration()   
        {
            int[][] prevGenValues = null;
            if (this.currGenValuesToPlot != null)
                prevGenValues = this.currGenValuesToPlot;
            else
                prevGenValues = this.startingValuesToPlot;
            // Iterate the Game Of Life rules 5 times

            while (this.keepIteratig)
            {
                // Sleep the Thread for 300 ms.
                Thread.Sleep(this.timerSpeedMilliSecond);
                // Call the method with the instruction for the Game Of Life
                this.currGenValuesToPlot = GameOfLifeGenerationComputation(prevGenValues);
                // Plot the current Generation Values
                //AssignValuesToPlot(this.currGenValuesToPlot);
                // Update the previous generation.
                prevGenValues = this.currGenValuesToPlot;
                // Increase the generation Count.
                this.generationCount++;

                this.BeginInvoke((MethodInvoker)(() =>
                {
                    // Plot the current Generation Values
                    AssignValuesToPlot(this.currGenValuesToPlot);
                    // Update the Text of the new generation.
                    this.generationCounter.Text = "Generation: " + this.generationCount.ToString();
                }));
            }
        }
        #endregion


        #region UtilityMethods
        /// <summary>
        /// Method for the dimension initialization of the empty Heatmap, that will be the two-dimensional space in which the Game Of Life will be created.
        /// </summary>
        private void InitializeHetamapMatrix()
        {   
            this.keepIteratig = false;
            this.hasMatrixValuesToPlot = false;

            // Initialize an empty starting values to plot matrix.
            this.startingValuesToPlot = new int[this.nRowHeatmap][];

            for (int row = 0; row < this.startingValuesToPlot.Length; row++)
            {
                this.startingValuesToPlot[row] = new int[this.nColHeatmap];

                for (int col = 0; col < this.startingValuesToPlot[row].Length; col++)
                {
                    // Initialize the entire matrix values to plot with Zeros.
                    this.startingValuesToPlot[row][col] = 0;
                }
            }
            AssignValuesToPlot(this.startingValuesToPlot);
        }
        /// <summary>
        /// Updating or creating the living or death cell in the heatmap.
        /// </summary>
        /// <param name="xCoords"></param>
        /// <param name="yCoords"></param>
        /// <param name="isFromRightClick"></param>
        private void GenerateOrUpdateInitialHetmapMatrixSingleCoord(int xCoords, int yCoords, bool isFromRightClick)
        {
            this.hasMatrixValuesToPlot = true;

            // You can only update the Chart patter if the Game Of Life iteration is in Pause
            if (this.keepIteratig == false)
            {
                if (this.currGenValuesToPlot != null)
                {
                    // If the event comes from a single mouse click, then check if the current cell was already selected (has already a value of 1). If yes, set the cell to 0 (like a deselection).
                    if (isFromRightClick)
                    {
                        // Check if the current cell was already selected (has already a value of 1). If yes, set the cell to 0 (like a deselection).
                        if (this.currGenValuesToPlot[yCoords][xCoords] == 1)
                            this.currGenValuesToPlot[yCoords][xCoords] = 0;
                    }
                    // Otherwise, just color the selected cell.
                    else
                        this.currGenValuesToPlot[yCoords][xCoords] = 1;

                    AssignValuesToPlot(this.currGenValuesToPlot);
                }
                else
                {
                    // If the event comes from a single mouse click, then check if the current cell was already selected (has already a value of 1). If yes, set the cell to 0 (like a deselection).
                    if (isFromRightClick)
                    {
                        // Check if the current cell was already selected (has already a value of 1). If yes, set the cell to 0 (like a deselection).
                        if (this.startingValuesToPlot[yCoords][xCoords] == 1)
                            this.startingValuesToPlot[yCoords][xCoords] = 0;
                    }
                    // Otherwise, just color the selected cell.
                    else
                        this.startingValuesToPlot[yCoords][xCoords] = 1;

                    AssignValuesToPlot(this.startingValuesToPlot);
                }
            }
        }
        /// <summary>
        /// Assign the value to the selected cell. The heatmap will then convert the value to color cells.
        /// </summary>
        /// <param name="valueToPlot"></param>
        private void AssignValuesToPlot(int[][] valueToPlot)
        {
            if (valueToPlot != null)
            {
                // Generate X and Y Axis arrays.
                int[] yAxisSteps = new int[valueToPlot.Length];
                int[] xAxisSteps = new int[valueToPlot[0].Length];
                // Fill the Y axis steps with values.
                for (int rowIdx = 0; rowIdx < valueToPlot.Length; rowIdx++)
                {
                    yAxisSteps[rowIdx] = rowIdx + 1;
                }
                // Fill the X axis steps with values.
                for (int colIdx = 0; colIdx < valueToPlot[0].Length; colIdx++)
                {
                    xAxisSteps[colIdx] = colIdx + 1;
                }
                // Assign these values to the heatmapControl.
                heatmapChartControl.XAxis = xAxisSteps;
                heatmapChartControl.YAxis = yAxisSteps;
                heatmapChartControl.ValuesToPlot = valueToPlot;
            }
        }
        /// <summary>
        /// Main loop which will update the heatmap cells status.
        /// </summary>
        /// <param name="prevGenValues"></param>
        /// <returns></returns>
        private int[][] GameOfLifeGenerationComputation(int[][] prevGenValues)
        {
            // ---- GAME OF LIFE RULES:
            // The universe of the Game of Life is an infinite, two-dimensional orthogonal grid of square cells, each of which is in one of two possible states, live or dead (or populated and unpopulated, respectively).
            // Every cell interacts with its eight neighbors, which are the cells that horizontally, vertically, or diagonally adjacent. At each step in time, the following transition occur:
            // 1) Any live cell with fewer than two live neighbors dies, as if by underpopulation. 
            // 2) Any live cell with two or three live neighbors lives on to the next generation. 
            // 3) Any live cell with more than three live neighbors dies, as if by overpopulation. 
            // 4) Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction. 

            // Initialize the new matrix with the new Current Generation of the values to plot Matrix.
            int[][] currGenValues = new int[prevGenValues.Length][];
            for (int row = 0; row < currGenValues.Length; row++)
            {
                currGenValues[row] = new int[prevGenValues[row].Length];
            }

            // Loop through all the cells of the matrix and check the aforementioned conditions.
            for (int currRow = 0; currRow < prevGenValues.Length; currRow++)
            {
                for (int currCol = 0; currCol < prevGenValues[currRow].Length; currCol++)
                {
                    // 1 => Alive
                    // 0 => Dead
                    int currCellStatus = prevGenValues[currRow][currCol];

                    // Now check the status of the Neighbor Cells.
                    int livingNeighborCells = 0;
                    // Loop the neighborhood of the current cell, which is composed by 8 cells.
                    for (int nearRow = currRow - 1; nearRow <= currRow + 1; nearRow++)
                    {
                        for (int nearCol = currCol - 1; nearCol <= currCol + 1; nearCol++)
                        {
                            // Check if the index of the near Row or Col (neighbor cells) is going outside the original matrix. If it is, skip the cell.
                            if (nearRow < 0 || nearCol < 0 || nearRow >= prevGenValues.Length || nearCol >= prevGenValues[currRow].Length)
                                continue;
                            // Check as well if we are in the current cell/row/col already.
                            if (nearRow == currRow && nearCol == currCol)
                                continue;

                            // If the cell value is 1 means that is Alive.
                            if (prevGenValues[nearRow][nearCol] == 1)
                                livingNeighborCells += 1;
                        }
                    }
                    // Now check the Game Of Life Conditions.
                    // 1) Any live cell with fewer than two live neighbors dies, as if by underpopulation. 
                    // 2) Any live cell with two or three live neighbors lives on to the next generation. 
                    // 3) Any live cell with more than three live neighbors dies, as if by overpopulation. 
                    if (currCellStatus == 1)
                    {
                        // Case 1.
                        if (livingNeighborCells < 2)
                            currGenValues[currRow][currCol] = 0;
                        // Case 2.
                        else if (livingNeighborCells == 2 || livingNeighborCells == 3)
                            currGenValues[currRow][currCol] = 1;
                        // Case 3.
                        else if (livingNeighborCells > 3)
                            currGenValues[currRow][currCol] = 0;
                    }
                    // 4) Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
                    else
                    {
                        if (livingNeighborCells == 3)
                            currGenValues[currRow][currCol] = 1;
                    }
                }
            }
            return currGenValues;
        }
        /// <summary>
        /// Action performed when the increase (+) CellSize dimension button is clicked.
        /// </summary>
        private void IncreaseCellSizeMatrixDimension()
        {
            InitializeDimensionsOfHeatmapGrid();

            // Compute the difference between the previous number of rows and columns of the existing matrix and the new number of rows and cols computed with the new dimension of the cells.
            int prevNumberOfRow = this.startingValuesToPlot.Length;
            int prevNumberOfCol = this.startingValuesToPlot[0].Length;
            int rowsToRemove = prevNumberOfRow - this.nRowHeatmap;
            int colsToRemove = prevNumberOfCol - this.nColHeatmap;

            // Initialize the new matrix that will have the new dimensions (nRows, nCols) and that will be substituted to the startingValuesToPlot matrix.
            int[][] newIncreasedStartingValuesToPlot = new int[this.nRowHeatmap][];
            for (int row = 0; row < newIncreasedStartingValuesToPlot.Length; row++)
                newIncreasedStartingValuesToPlot[row] = new int[this.nColHeatmap];
            // Now we have to "copy" the previous startValuesToPlot in this new increased-in-dimension matrix.
            // First compute the number of the cells that will be added from both the top/bottom and left/right margins. Therefore the indexes from and to start copying the old matrix.
            int rowIdxWhereToStartCopying = (int)Math.Floor(rowsToRemove / 2.0);
            int colIdxWhereToStartCopying = (int)Math.Floor(colsToRemove / 2.0);
            //int rowIdxWhereToStopCopying = this.startingValuesToPlot.Length - 1 - ((int)Math.Floor(rowsToRemove / 2.0));
            //int colIdxWhereToStopCopying = this.startingValuesToPlot[0].Length - 1 - ((int)Math.Floor(colsToRemove / 2.0));
            int rowIdxWhereToStopCopying = this.startingValuesToPlot.Length - 1 - (rowsToRemove - (int)Math.Floor(rowsToRemove / 2.0));
            int colIdxWhereToStopCopying = this.startingValuesToPlot[0].Length - 1 - (colsToRemove - (int)Math.Floor(colsToRemove / 2.0));

            // Loop through the old matrix, and start copying the values in the new matrix when are included between the "zoomed" new indexes.
            int newRow = 0;
            for (int oldRow = 0; oldRow < this.startingValuesToPlot.Length; oldRow++)
            {
                int newCol = 0;

                if (oldRow >= rowIdxWhereToStartCopying && oldRow <= rowIdxWhereToStopCopying)
                {
                    for (int oldCol = 0; oldCol < this.startingValuesToPlot[oldRow].Length; oldCol++)
                    {
                        if (oldCol >= colIdxWhereToStartCopying && oldCol <= colIdxWhereToStopCopying)
                        {
                            newIncreasedStartingValuesToPlot[newRow][newCol] = this.startingValuesToPlot[oldRow][oldCol];

                            newCol++;
                        }
                    }
                    newRow++;
                }
            }
            // Assign the new increased-in-dimension matrix to the original one.
            this.startingValuesToPlot = newIncreasedStartingValuesToPlot;

            // If the currGenValuesToPlot is not null, I have to change its dimensions as well.
            if (this.currGenValuesToPlot != null)
            {
                // Initialize the dimension of the newDecreasedCurrGenValuesToPlot matrix.
                int[][] newIncreasedCurrGenValuesToPlot = new int[this.nRowHeatmap][];
                for (int row = 0; row < newIncreasedCurrGenValuesToPlot.Length; row++)
                    newIncreasedCurrGenValuesToPlot[row] = new int[this.nColHeatmap];

                int newRow1 = 0;
                for (int oldRow = 0; oldRow < this.currGenValuesToPlot.Length; oldRow++)
                {
                    int newCol1 = 0;

                    if (oldRow >= rowIdxWhereToStartCopying && oldRow <= rowIdxWhereToStopCopying)
                    {
                        for (int oldCol = 0; oldCol < this.currGenValuesToPlot[oldRow].Length; oldCol++)
                        {
                            if (oldCol >= colIdxWhereToStartCopying && oldCol <= colIdxWhereToStopCopying)
                            {
                                newIncreasedCurrGenValuesToPlot[newRow1][newCol1] = this.currGenValuesToPlot[oldRow][oldCol];

                                newCol1++;
                            }
                        }
                        newRow1++;
                    }
                }
                this.currGenValuesToPlot = newIncreasedCurrGenValuesToPlot;
            }
        }
        /// <summary>
        /// Action performed when the Decrease (-) CellSize dimension button is clicked.
        /// </summary>
        private void DecreaseCellSizeMatrixDimension()
        {
            InitializeDimensionsOfHeatmapGrid();

            if (this.nColHeatmap < 0 || this.nRowHeatmap < 0)
                return;

            // Compute the difference between the previous number of rows and columns of the existing matrix and the new number of rows and cols computed with the new dimension of the cells.
            int prevNumberOfRow = this.startingValuesToPlot.Length;
            int prevNumberOfCol = this.startingValuesToPlot[0].Length;
            int rowsToAdd = this.nRowHeatmap - prevNumberOfRow;
            int colsToAdd = this.nColHeatmap - prevNumberOfCol;

            // Initialize the new matrix that will have the new dimensions (nRows, nCols) and that will be substituted to the startingValuesToPlot matrix.
            int[][] newDecreasedStartingValuesToPlot = new int[this.nRowHeatmap][];
            for (int row = 0; row < newDecreasedStartingValuesToPlot.Length; row++)
                newDecreasedStartingValuesToPlot[row] = new int[this.nColHeatmap];
            // Now we have to "copy" the previous startValuesToPlot in this new increased-in-dimension matrix.
            // First compute the number of the cells that will be added from both the top/bottom and left/right margins. Therefore the indexes from and to start copying the old matrix.
            int rowIdxWhereToStartCopying = (int)Math.Floor(rowsToAdd / 2.0);
            int colIdxWhereToStartCopying = (int)Math.Floor(colsToAdd / 2.0);
            //int rowIdxWhereToStopCopying = newDecreasedStartingValuesToPlot.Length - 1 - ((int)Math.Floor(rowsToAdd / 2.0));
            //int colIdxWhereToStopCopying = newDecreasedStartingValuesToPlot[0].Length - 1 - ((int)Math.Floor(colsToAdd / 2.0));
            int rowIdxWhereToStopCopying = newDecreasedStartingValuesToPlot.Length - 1 - (rowsToAdd - (int)Math.Floor(rowsToAdd / 2.0));
            int colIdxWhereToStopCopying = newDecreasedStartingValuesToPlot[0].Length - 1 - (colsToAdd - (int)Math.Floor(colsToAdd / 2.0));

            // Loop through the new matrix, and start copying the values from the old matrix when correct.
            for (int newRow = 0; newRow < newDecreasedStartingValuesToPlot.Length; newRow++)
            {
                for (int newCol = 0; newCol < newDecreasedStartingValuesToPlot[newRow].Length; newCol++)
                {
                    // Check if the index of the new increased matrix correspond to the old matrix, so that we can copy the old matrix in the new increased one. 
                    if ((newRow >= rowIdxWhereToStartCopying && newRow <= rowIdxWhereToStopCopying) && (newCol >= colIdxWhereToStartCopying && newCol <= colIdxWhereToStopCopying))
                    {
                        newDecreasedStartingValuesToPlot[newRow][newCol] = this.startingValuesToPlot[newRow - rowIdxWhereToStartCopying][newCol - colIdxWhereToStartCopying];
                    }
                }
            }
            // Assign the new increased-in-dimension matrix to the original one.
            this.startingValuesToPlot = newDecreasedStartingValuesToPlot;

            // For this, I need to change both the startingValuesToPlot and the currGenValuesToPlot, if it is not null. 
            if (this.currGenValuesToPlot != null)
            {
                int[][] newDecreasedCurrGenValuesToPlot = new int[this.nRowHeatmap][];
                for (int row = 0; row < newDecreasedStartingValuesToPlot.Length; row++)
                    newDecreasedCurrGenValuesToPlot[row] = new int[this.nColHeatmap];
                // Now we have to copy the previous CurrentGeneration ValuesToPlot in this new increased-in-dimension matrix.
                // Loop through the new matrix, and start copying the values from the old matrix when correct.
                for (int newRow = 0; newRow < newDecreasedCurrGenValuesToPlot.Length; newRow++)
                {
                    for (int newCol = 0; newCol < newDecreasedCurrGenValuesToPlot[newRow].Length; newCol++)
                    {
                        // Check if the index of the new increased matrix correspond to the old matrix, so that we can copy the old matrix values in the new increased one.
                        if ((newRow >= rowIdxWhereToStartCopying && newRow < rowIdxWhereToStopCopying) && (newCol >= colIdxWhereToStartCopying && newCol <= colIdxWhereToStopCopying))
                        {
                            newDecreasedCurrGenValuesToPlot[newRow][newCol] = this.currGenValuesToPlot[newRow - rowIdxWhereToStartCopying][newCol - colIdxWhereToStartCopying];
                        }
                    }
                }
                // Assign the new increased-in-dimension matrix to the original one.
                this.currGenValuesToPlot = newDecreasedCurrGenValuesToPlot;
            }
        }
        /// <summary>
        /// Method used for extracting the matrix selected cells with the Ctrl action used for the Copy/Paste operation. 
        /// </summary>
        private void CopyPasteSelectionHeatmapCellCtrlC(int xStartIdx, int yStartIdx, int xEndIdx, int yEndIdx)
        {
            // Compute the number of row and number of columns of the new copyPasteCurrGenValuesToPlot.
            int newNRow = 1;
            if (yStartIdx > yEndIdx)
                newNRow = yStartIdx - yEndIdx + 1;
            else if (yEndIdx > yStartIdx)
                newNRow = yEndIdx - yStartIdx + 1;
            int newNCol = 1;
            if (xStartIdx > xEndIdx)
                newNCol = xStartIdx - xEndIdx + 1;
            else if (xEndIdx > xStartIdx)
                newNCol = xEndIdx - xStartIdx + 1;

            // Check if the currentValuesToPlot is null. If not, select the cells.
            if (this.currGenValuesToPlot != null)
            {
                // Initialize the Copy Paste Rows matrix, that will contain the selected cells.
                this.copyPasteCurrGenValuesToPlot = new int[newNRow][];
                // Counter for the new copyPasteCurrGenValuesToPlot rows.
                int newRowIdx = 0;
                // Loop through all the selected cells. 
                // In order to make them to look like selected, change the values of the cells, in which the value is equal to 0, to a value of 2. That will be then colored by the red.
                // If the cell has already a value of 1, then don't change anything.
                for (int row = 0; row < this.currGenValuesToPlot.Length; row++)
                {
                    // Counter for the new copyPasteCurrGenValuesToPlot rows.
                    int newColIdx = 0;

                    // Check if the current row is in between the xStart and xEnd Idx coordinates of the copy paste selection.
                    if ((row <= yStartIdx && row >= yEndIdx) || (row >= yStartIdx && row <= yEndIdx))
                    {
                        // Initialize the Copy Paste Cols matrix, that will contain the selected cells.
                        this.copyPasteCurrGenValuesToPlot[newRowIdx] = new int[newNCol];

                        // Loop through all the columns of the matrix.
                        for (int col = 0; col < this.currGenValuesToPlot[row].Length; col++)
                        {
                            // Check if the current col is included in between yStart and yEnd coordinates of the copy paste selection.
                            // Inverted < and > for the col comparison with the Start and End indexes. (probably because the Y axis is upside-down).
                            if ((col >= xStartIdx && col <= xEndIdx) || (col <= xStartIdx && col >= xEndIdx))
                            {
                                // Copy the values of the selected rectangle.
                                this.copyPasteCurrGenValuesToPlot[newRowIdx][newColIdx] = this.currGenValuesToPlot[row][col];
                                newColIdx++;

                                //// If the current cell has a value of 0. Then substitute it with a value of 2.
                                //if (this.currGenValuesToPlot[row][col] == 0)
                            }
                        }
                        newRowIdx++;
                    }
                }

                //AssignValuesToPlot(this.copyPasteCurrGenValuesToPlot);
            }
            // Otherwise change the values of the startingValuesToPlot
            else
            {
                // Initialize the Copy Paste Rows matrix, that will contain the selected cells.
                this.copyPasteStartingValuesToPlot = new int[newNRow][];
                // Counter for the new copyPasteCurrGenValuesToPlot rows.
                int newRowIdx = 0;

                // Loop through all the selected cells. 
                // In order to make them to look like selected, change the values of the cells, in which the value is equal to 0, to a value of 2. That will be then colored by the red.
                // If the cell has already a value of 1, then don't change anything.
                for (int row = 0; row < this.startingValuesToPlot.Length; row++)
                {
                    // Counter for the new copyPasteCurrGenValuesToPlot rows.
                    int newColIdx = 0;

                    // Check if the current row is in between the xStart and xEnd Idx coordinates of the copy paste selection.
                    if ((row <= yStartIdx && row >= yEndIdx) || (row >= yStartIdx && row <= yEndIdx))
                    {
                        // Initialize the Copy Paste Cols matrix, that will contain the selected cells.
                        this.copyPasteStartingValuesToPlot[newRowIdx] = new int[newNCol];

                        // Loop through all the columns of the matrix.
                        for (int col = 0; col < this.startingValuesToPlot[row].Length; col++)
                        {
                            // Check if the current col is included in between yStart and yEnd coordinates of the copy paste selection.
                            // Inverted < and > for the col comparison with the Start and End indexes. (probably because the Y axis is upside-down).
                            if ((col >= xStartIdx && col <= xEndIdx) || (col <= xStartIdx && col >= xEndIdx))
                            {
                                // Copy the values of the selected rectangle.
                                this.copyPasteStartingValuesToPlot[newRowIdx][newColIdx] = this.startingValuesToPlot[row][col];
                                newColIdx++;
                            }
                        }
                        newRowIdx++;
                    }
                }

                //AssignValuesToPlot(this.copyPasteStartingValuesToPlot);
            }
        }
        /// <summary>
        /// Method used for the extraction of the matrix selected cells with the Ctrl action used for the Cut/Paste operation.
        /// </summary>
        /// <param name="xStartIdx"></param>
        /// <param name="yStartIdx"></param>
        /// <param name="xEndIdx"></param>
        /// <param name="yEndIdx"></param>
        private void CopyPasteSelectionHeatmapCellCtrlX(int xStartIdx, int yStartIdx, int xEndIdx, int yEndIdx)
        {
            // Compute the number of row and number of columns of the new copyPasteCurrGenValuesToPlot.
            int newNRow = 1;
            if (yStartIdx > yEndIdx)
                newNRow = yStartIdx - yEndIdx + 1;
            else if (yEndIdx > yStartIdx)
                newNRow = yEndIdx - yStartIdx + 1;
            int newNCol = 1;
            if (xStartIdx > xEndIdx)
                newNCol = xStartIdx - xEndIdx + 1;
            else if (xEndIdx > xStartIdx)
                newNCol = xEndIdx - xStartIdx + 1;

            // Check if the currentValuesToPlot is null. If not, select the cells.
            if (this.currGenValuesToPlot != null)
            {
                // Initialize the Copy Paste Rows matrix, that will contain the selected cells.
                this.copyPasteCurrGenValuesToPlot = new int[newNRow][];
                // Counter for the new copyPasteCurrGenValuesToPlot rows.
                int newRowIdx = 0;
                // Loop through all the selected cells. 
                // In order to make them to look like selected, change the values of the cells, in which the value is equal to 0, to a value of 2. That will be then colored by the red.
                // If the cell has already a value of 1, then don't change anything.
                for (int row = 0; row < this.currGenValuesToPlot.Length; row++)
                {
                    // Counter for the new copyPasteCurrGenValuesToPlot rows.
                    int newColIdx = 0;

                    // Check if the current row is in between the xStart and xEnd Idx coordinates of the copy paste selection.
                    if ((row <= yStartIdx && row >= yEndIdx) || (row >= yStartIdx && row <= yEndIdx))
                    {
                        // Initialize the Copy Paste Cols matrix, that will contain the selected cells.
                        this.copyPasteCurrGenValuesToPlot[newRowIdx] = new int[newNCol];

                        // Loop through all the columns of the matrix.
                        for (int col = 0; col < this.currGenValuesToPlot[row].Length; col++)
                        {
                            // Check if the current col is included in between yStart and yEnd coordinates of the copy paste selection.
                            // Inverted < and > for the col comparison with the Start and End indexes. (probably because the Y axis is upside-down).
                            if ((col >= xStartIdx && col <= xEndIdx) || (col <= xStartIdx && col >= xEndIdx))
                            {
                                // Copy the values of the selected rectangle.
                                this.copyPasteCurrGenValuesToPlot[newRowIdx][newColIdx] = this.currGenValuesToPlot[row][col];

                                // Remove the elements selected from the currGenValuesToPlot.
                                this.currGenValuesToPlot[row][col] = 0;

                                newColIdx++;
                            }
                        }
                        newRowIdx++;
                    }
                }

                AssignValuesToPlot(this.currGenValuesToPlot);
            }
            // Otherwise change the values of the startingValuesToPlot
            else
            {
                // Initialize the Copy Paste Rows matrix, that will contain the selected cells.
                this.copyPasteStartingValuesToPlot = new int[newNRow][];
                // Counter for the new copyPasteCurrGenValuesToPlot rows.
                int newRowIdx = 0;

                // Loop through all the selected cells. 
                // In order to make them to look like selected, change the values of the cells, in which the value is equal to 0, to a value of 2. That will be then colored by the red.
                // If the cell has already a value of 1, then don't change anything.
                for (int row = 0; row < this.startingValuesToPlot.Length; row++)
                {
                    // Counter for the new copyPasteCurrGenValuesToPlot rows.
                    int newColIdx = 0;

                    // Check if the current row is in between the xStart and xEnd Idx coordinates of the copy paste selection.
                    if ((row <= yStartIdx && row >= yEndIdx) || (row >= yStartIdx && row <= yEndIdx))
                    {
                        // Initialize the Copy Paste Cols matrix, that will contain the selected cells.
                        this.copyPasteStartingValuesToPlot[newRowIdx] = new int[newNCol];

                        // Loop through all the columns of the matrix.
                        for (int col = 0; col < this.startingValuesToPlot[row].Length; col++)
                        {
                            // Check if the current col is included in between yStart and yEnd coordinates of the copy paste selection.
                            // Inverted < and > for the col comparison with the Start and End indexes. (probably because the Y axis is upside-down).
                            if ((col >= xStartIdx && col <= xEndIdx) || (col <= xStartIdx && col >= xEndIdx))
                            {
                                // Copy the values of the selected rectangle.
                                this.copyPasteStartingValuesToPlot[newRowIdx][newColIdx] = this.startingValuesToPlot[row][col];

                                this.startingValuesToPlot[row][col] = 0;

                                newColIdx++;
                            }
                        }
                        newRowIdx++;
                    }
                }

                AssignValuesToPlot(this.startingValuesToPlot);
            }
        }
        /// <summary>
        /// Method that will paste the previously Copied or Cut selected cells.
        /// </summary>
        /// <param name="xIdxCtrlV"></param>
        /// <param name="yIdxCtrlV"></param>
        private void CopyPasteSelectionHeatmapCellCtrlV(int xIdxCtrlV, int yIdxCtrlV)
        {
            // Check if exist a currentGenValueToPlot matrix.
            if (this.currGenValuesToPlot != null)
            {
                this.currGenValuesToPlot[yIdxCtrlV][xIdxCtrlV] = 0;

                if (this.copyPasteCurrGenValuesToPlot != null)
                {
                    // Since the xIdxCtrlV and yIdxCtrlV indexes will be the center of the submatrix to plot. Find the indexes in the currGenValuesToPlot matrix in which to start pasting the submatrix.
                    // Find the number of rows and columns of the copyPasteCurrGenValuesToPlot.
                    int nRowsSubmatrix = this.copyPasteCurrGenValuesToPlot.Length;
                    int nColsSubmatrix = this.copyPasteCurrGenValuesToPlot[0].Length;

                    // Find the starting X and Y coordinates in the original startingValuesToPlot Matrix which to start Pasting this new submatrix.
                    int yCenterSubmatrix = (int)Math.Floor(nRowsSubmatrix / 2.0);
                    int xCenterSubmatrix = (int)Math.Floor(nColsSubmatrix / 2.0);

                    // --- Starting Point of the Paste action.
                    int xStartIdxForPaste = xIdxCtrlV - xCenterSubmatrix;
                    int yStartIdxForPaste = yIdxCtrlV - yCenterSubmatrix;
                    // Check if these points are before the 0 or exceeds the length of the matrix.
                    if (xStartIdxForPaste < 0)
                        xStartIdxForPaste = 0;
                    if (yStartIdxForPaste < 0)
                        yStartIdxForPaste = 0;
                    if (xStartIdxForPaste > this.currGenValuesToPlot[0].Length - 1)
                        xStartIdxForPaste = this.currGenValuesToPlot[0].Length - 1;
                    if (yStartIdxForPaste > this.currGenValuesToPlot.Length - 1)
                        yStartIdxForPaste = this.currGenValuesToPlot.Length - 1;

                    // --- Ending Point of the Paste action.
                    int xEndPointForPaste = xStartIdxForPaste + this.copyPasteCurrGenValuesToPlot[0].Length - 1;
                    int yEndPointForPaste = yStartIdxForPaste + this.copyPasteCurrGenValuesToPlot.Length - 1;
                    // Check if these point don't exceeds the length of the startingValuesToPlot matrix.
                    if (xEndPointForPaste > this.currGenValuesToPlot[0].Length - 1)
                        xEndPointForPaste = this.currGenValuesToPlot[0].Length - 1;
                    if (yEndPointForPaste > this.currGenValuesToPlot.Length - 1)
                        yEndPointForPaste = this.currGenValuesToPlot.Length - 1;

                    // Loop through all the rows and column of the startingValuesToPlot matrix, and paste the copied sub-matrix.
                    int rowInCopiedMtx = 0;
                    for (int row = 0; row < this.currGenValuesToPlot.Length; row++)
                    {
                        int colInCopiedMtx = 0;

                        if (row >= yStartIdxForPaste && row <= yEndPointForPaste)
                        {
                            for (int col = 0; col < this.currGenValuesToPlot[0].Length; col++)
                            {
                                if (col >= xStartIdxForPaste && col <= xEndPointForPaste)
                                {
                                    // Copy the copiedMatrix into the startingValueToPlot matrix.
                                    if (this.copyPasteCurrGenValuesToPlot[rowInCopiedMtx][colInCopiedMtx] != 0)
                                    {
                                        this.currGenValuesToPlot[row][col] = this.copyPasteCurrGenValuesToPlot[rowInCopiedMtx][colInCopiedMtx];
                                    }
                                    colInCopiedMtx++;
                                }
                            }
                            rowInCopiedMtx++;
                        }
                    }
                    AssignValuesToPlot(this.currGenValuesToPlot);
                }
            }
            // Paste the copied matrix in the StartingValuesToPlot Matrix.
            else
            {
                this.startingValuesToPlot[yIdxCtrlV][xIdxCtrlV] = 0;

                // Check if exist a copied startingValuesToPlot submatrix that can be pasted.
                if (this.copyPasteStartingValuesToPlot != null)
                {
                    // Since xIdxCtrlV and yIdxCtrlV indexes will be the center of the sub-matrix to plot. Find the indexes in the startingValuesToPlot matrix in which to start pasting the submatrix.
                    // Find the number of rows and column of the copyPasteStartingValuesToPlot.
                    int nRowsSubmatrix = this.copyPasteStartingValuesToPlot.Length;
                    int nColsSubmatrix = this.copyPasteStartingValuesToPlot[0].Length;

                    // Find the starting X and Y coordinates in the original startingValuesToPlot Matrix which to start Pasting this new submatrix.
                    int yCenterSubmatrix = (int)Math.Floor(nRowsSubmatrix / 2.0);
                    int xCenterSubmatrix = (int)Math.Floor(nColsSubmatrix / 2.0);
                    // --- Starting Point of the Paste action.
                    int xStartIdxForPaste = xIdxCtrlV - xCenterSubmatrix;
                    int yStartIdxForPaste = yIdxCtrlV - yCenterSubmatrix;
                    // Check if these points are before the 0 or exceeds the length of the matrix.
                    if (xStartIdxForPaste < 0)
                        xStartIdxForPaste = 0;
                    if (yStartIdxForPaste < 0)
                        yStartIdxForPaste = 0;
                    if (xStartIdxForPaste > this.startingValuesToPlot[0].Length - 1)
                        xStartIdxForPaste = this.startingValuesToPlot[0].Length - 1;
                    if (yStartIdxForPaste > this.startingValuesToPlot.Length - 1)
                        yStartIdxForPaste = this.startingValuesToPlot.Length - 1;

                    // --- Ending Point of the Paste action.
                    int xEndPointForPaste = xStartIdxForPaste + this.copyPasteStartingValuesToPlot[0].Length - 1;
                    int yEndPointForPaste = yStartIdxForPaste + this.copyPasteStartingValuesToPlot.Length - 1;
                    // Check if these point don't exceeds the length of the startingValuesToPlot matrix.
                    if (xEndPointForPaste > this.startingValuesToPlot[0].Length - 1)
                        xEndPointForPaste = this.startingValuesToPlot[0].Length - 1;
                    if (yEndPointForPaste > this.startingValuesToPlot.Length - 1)
                        yEndPointForPaste = this.startingValuesToPlot.Length - 1;


                    // Loop through all the rows and column of the startingValuesToPlot matrix, and paste the copied sub-matrix.
                    bool isTheClickedCellIncluded = false;
                    int rowInCopiedMtx = 0;
                    for (int row = 0; row < this.startingValuesToPlot.Length; row++)
                    {
                        int colInCopiedMtx = 0;

                        if (row >= yStartIdxForPaste && row <= yEndPointForPaste)
                        {
                            for (int col = 0; col < this.startingValuesToPlot[0].Length; col++)
                            {
                                if (col >= xStartIdxForPaste && col <= xEndPointForPaste)
                                {
                                    // Check if we are passing through the initial xIdxCtrlV and yIdxCtrlV. If not, then we will remove this from the final startingValuesToPlot.
                                    if (row == yIdxCtrlV && col == xIdxCtrlV)
                                        isTheClickedCellIncluded = true;

                                    // Copy the copiedMatrix into the startingValueToPlot matrix.
                                    if (this.copyPasteStartingValuesToPlot[rowInCopiedMtx][colInCopiedMtx] != 0)
                                    {
                                        this.startingValuesToPlot[row][col] = this.copyPasteStartingValuesToPlot[rowInCopiedMtx][colInCopiedMtx];
                                    }
                                    colInCopiedMtx++;
                                }
                            }
                            rowInCopiedMtx++;
                        }
                    }
                    AssignValuesToPlot(this.startingValuesToPlot);
                }
            }
        }
        #endregion


        #region Event Handler
        /// <summary>
        /// Action performed when the size of the form is changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetBoundsToControls();

            // If the panel with the rules is visualized, then keep it to the front.
            if (this.isRulesPanelVisualized)
                this.rulesPanel.BringToFront();

            if (this.startingValuesToPlot != null || this.currGenValuesToPlot != null)
            {
                // The action of reducing the cell size can be only done when the Game of Life Thread is in pause.
                bool isTheGameOfLifeRunning = false;
                // Stop the Thread momentaneously. 
                if (this.keepIteratig && this.gameOfLifeThr != null)
                {
                    isTheGameOfLifeRunning = true;
                    this.keepIteratig = false;
                    this.gameOfLifeThr.Abort();
                }

                DecreaseCellSizeMatrixDimension();

                // Check if the GameOfLifeIteration was running. If it was, then re-run the GameOfLife Method/Task.
                if (isTheGameOfLifeRunning)
                {
                    this.keepIteratig = true;
                    GameOfLifeIterations();
                }
                else
                {
                    // Check if there is any currGenValuesToPlot to plot.
                    if (this.currGenValuesToPlot != null)
                        AssignValuesToPlot(this.currGenValuesToPlot);
                    else
                        AssignValuesToPlot(this.startingValuesToPlot);
                }
            }
        }
        /// <summary>
        /// Action performed when the OpenFile button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            // The action of opening a new file can be only done when the Game of Life Thread is in pause.
            // Stop the Thread. 
            if (this.keepIteratig && this.gameOfLifeThr != null)
            {
                this.keepIteratig = false;
                this.gameOfLifeThr.Abort();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.AddExtension = true;
            openFileDialog.Multiselect = false;
            string initialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameOfLifePattern");
            openFileDialog.InitialDirectory = initialDirectory;
            openFileDialog.Filter = "TXT files (*.txt)|*.txt";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Retrieve the entire path of the file:
                string filePath = openFileDialog.FileName;

                // Retrieve the name of the opened .txt file.
                string fileName = filePath.Split('\\')[filePath.Split('\\').Length - 1].Split('.')[0];

                // Add the name of the file to the text of the form. 
                this.Text = this.Text + " - " + fileName;

                // Parse the file.
                StreamReader reader = File.OpenText(filePath);

                string line;
                List<char[]> parsedFile = new List<char[]>();
                // Loop through all the lines in the file.
                while ((line = reader.ReadLine()) != null)
                {
                    // Remove the empty spaces at the end of the line if there are.
                    line = line.TrimEnd();
                    line = line.TrimStart();

                    // Split each character and store it in an array.
                    List<char> items = new List<char>();
                    // Loop through all the character in the line and add it to the list
                    foreach (char c in line)
                    {
                        items.Add(c);
                    }

                    parsedFile.Add(items.ToArray());

                }

                // Re-Initialize everything.
                if (this.gameOfLifeThr != null)
                    this.gameOfLifeThr.Abort();
                this.keepIteratig = false;
                this.startStopGenerationButton.Text = "Start";
                this.generationCounter.Text = "Generation: 0";
                this.generationCount = 0;
                this.currGenValuesToPlot = null;

                // Re-initialize the startingValuesToPlot
                InitializeHetamapMatrix();

                if (parsedFile.Count == 0 || parsedFile[0].Length == 0)
                    return;

                // Drawn and Add the opened pattern in the heatmap matrix.
                // First in order to do that, check that the uploaded matrix doesn't exceeds the dimension of the heatmap matrix in the control.
                while (parsedFile.Count + 10 > this.startingValuesToPlot.Length || parsedFile[0].Length + 10 > this.startingValuesToPlot[0].Length)
                {
                    if (this.cellSizePixels - 1 > this.minimumCellSizePossible)
                    {
                        this.cellSizePixels -= 1;
                        // Only now that the thread is stopped. I can modify the dimension of the heatmap matrix.
                        DecreaseCellSizeMatrixDimension();
                    }
                }
                // Then just add the uploaded matrix in the center of the heatmap control.
                // Find the index of the heatmap startingValuesToPlot where to start to copy the uploaded matrix.
                int rowStartIdx = (int)((this.startingValuesToPlot.Length - parsedFile.Count) / 2.0);
                int colStartIdx = (int)((this.startingValuesToPlot[0].Length - parsedFile[0].Length) / 2.0);

                // Copy (while rotating its orientation on the Y axis) the uploaded matrix in the startingValuesToPlot.
                for (int row = 0; row < parsedFile.Count; row++)
                {
                    for (int col = 0; col < parsedFile[0].Length; col++)
                    {
                        if (parsedFile[row][col] != '.')
                            this.startingValuesToPlot[this.startingValuesToPlot.Length - 1 - (rowStartIdx + row)][colStartIdx + col] = 1;
                    }
                }

                this.hasMatrixValuesToPlot = true;

                AssignValuesToPlot(this.startingValuesToPlot);
            }
        }
        /// <summary>
        /// Action performed when the ExportFile button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportToFile_Click(object sender, EventArgs e)
        {
            // The action of opening a new file can be only done when the Game of Life Thread is in pause.
            // Stop the Thread. 
            if (this.keepIteratig && this.gameOfLifeThr != null)
            {
                this.keepIteratig = false;
                this.gameOfLifeThr.Abort();
            }

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "Text file(*.txt)|*.txt";
            sfd.FilterIndex = 1;

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Get the Directory Location for saving the data.
                string dirLocation = sfd.FileName;

                // Exporting the file in Txt format.
                using (StreamWriter writer = new StreamWriter(dirLocation))
                {
                    // If exist a currGenValuesToPlot Matrix, then export its values.
                    if (this.currGenValuesToPlot != null)
                    {
                        for (int row = this.currGenValuesToPlot.Length - 1; row >= 0; row--)
                        {
                            string lineToExport = "";

                            for (int col = 0; col < this.currGenValuesToPlot[row].Length; col++)
                            {
                                // If the value in the matrix is a 0 => then export it as a "." character.
                                if (this.currGenValuesToPlot[row][col] == 0)
                                    lineToExport += ".";
                                // If the value in the matrix is a 1 => then export it as a "*" character.
                                else if (this.currGenValuesToPlot[row][col] == 1)
                                    lineToExport += "*";
                            }

                            // Go in a new line only if it is not the last row.
                            if (row != 0)
                                lineToExport += "\t";
                            // Write the line to the exported file.
                            writer.WriteLine(lineToExport);
                        }
                    }
                    // Otherwise export the values of the startingValueToPlot.
                    else if (this.startingValuesToPlot != null)
                    {
                        for (int row = this.startingValuesToPlot.Length - 1; row >= 0; row--)
                        {
                            string lineToExport = "";

                            for (int col = 0; col < this.startingValuesToPlot[row].Length; col++)
                            {
                                // If the value in the matrix is a 0 => then export it as a "." character.
                                if (this.startingValuesToPlot[row][col] == 0)
                                    lineToExport += ".";
                                // If the value in the matrix is a 1 => then export it as a "*" character.
                                else if (this.startingValuesToPlot[row][col] == 1)
                                    lineToExport += "*";
                            }

                            // Go in a new line only if it is not the last row.
                            if (row != 0)
                                lineToExport += "\t";
                            // Write the line to the exported file.
                            writer.WriteLine(lineToExport);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Action performed when the Start/Pause the generation of the cells is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartStopGenerationButton_Click(object sender, EventArgs e)
        {
            // If the Game of Life is iterating, means that a start action has already been performed. Therefore Just Stop the iteration for this instruction.
            if (this.keepIteratig == true)
            {
                this.keepIteratig = false;
                this.startStopGenerationButton.Text = "Start";
            }
            // Start the execution of the Game of Life
            else
            {
                if (this.hasMatrixValuesToPlot)
                {
                    this.keepIteratig = true;
                    this.startStopGenerationButton.Text = "Pause";
                    GameOfLifeIterations();
                }
            }
        }
        /// <summary>
        /// Action performed when the button that allows you to start from the first pattern is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartFromBeginningButton_Click(object sender, EventArgs e)
        {
            if (this.gameOfLifeThr != null)
                this.gameOfLifeThr.Abort();
            this.keepIteratig = false;
            this.startStopGenerationButton.Text = "Start";
            this.generationCounter.Text = "Generation: 0";
            this.generationCount = 0;
            this.currGenValuesToPlot = null;
            AssignValuesToPlot(this.startingValuesToPlot);
        }
        /// <summary>
        /// Action performed when the button that clears all the cells is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (this.gameOfLifeThr != null)
                this.gameOfLifeThr.Abort();
            this.keepIteratig = false;
            this.startStopGenerationButton.Text = "Start";
            this.generationCounter.Text = "Generation: 0";
            this.generationCount = 0;
            this.currGenValuesToPlot = null;
            // Reset the name of Form.
            this.Text = "Game Of Life";
            InitializeHetamapMatrix();
        }
        /// <summary>
        /// Action performed when the button that allows to visualize the rules of the game is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenRulesDialogButton_Click(object sender, EventArgs e)
        {
            if (this.keepIteratig == true)
            {
                this.keepIteratig = false;
                this.startStopGenerationButton.Text = "Start";
            }

            this.isRulesPanelVisualized = true;

            // Bring to Front the Rules Panel already created.
            rulesPanel.BringToFront();
        }
        /// <summary>
        /// Action performed when the Close button on the Panel with the Game rules is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseRulesPanelButton_Click(object sender, EventArgs e)
        {
            rulesPanel.SendToBack();
            this.isRulesPanelVisualized = false;
        }
        /// <summary>
        /// Extracting the X and Y coordinates of the cell selection for the copy paste function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeatmapChartControl_HeatMapCellSelection(object sender, SelectedCellsEventArgs e)
        {
            // Extract the X and Y Idx Coordinates of the selected cell that will be colored
            int xIdx = e.XIdx - 1;
            int yIdx = e.YIdx - 1;
            bool isFromRightClick = e.IsFromRightClick;

            GenerateOrUpdateInitialHetmapMatrixSingleCoord(xIdx, yIdx, isFromRightClick);
        }
        /// <summary>
        /// Storing the coordinates of the CopyPaste Start and End cells.
        /// </summary>
        private void HeatmapChartControl_HeatMapCopyPasteSelection(object sender, CopyPasteSelectionEventArgs e)
        {
            // Extract the coordinates of the Copy Paste selection. Save them in order to then have them when the copy paste action is performed. 
            this.xStartIdxCP = e.XStartIdx - 1;
            this.yStartIdxCP = e.YStartIdx - 1;
            this.xEndIdxCP = e.XEndIdx - 1;
            this.yEndIdxCP = e.YEndIdx - 1;

            //CopyPasteSelectionHeatmapCellHighlights(xStartIdx, yStartIdx, xEndIdx, yEndIdx);
        }
        /// <summary>
        /// Copy action (Ctrl + C).
        /// </summary>
        private void HeatmapChartControl_HeatMapCopyPasteCtrlC(object sender, EventArgs e)
        {
            // The action of selecting the cells for the copy paste can be done only when the Game Of Life iteration is in Pause.
            if (this.keepIteratig == false)
            {
                // Call the method that will copy (Ctrl + C) (store) the selected matrix cells.
                CopyPasteSelectionHeatmapCellCtrlC(this.xStartIdxCP, this.yStartIdxCP, this.xEndIdxCP, this.yEndIdxCP);
            }
        }
        /// <summary>
        /// Cut action (Ctrl + C).
        /// </summary>
        private void HeatmapChartControl_HeatMapCopyPasteCtrlX(object sender, EventArgs e)
        {
            // The action of selecting the cells for the cut paste can be done only when the Game Of Life iteration is in Pause.
            if (this.keepIteratig == false)
            {
                // Call the method that will copy (Ctrl + X) (store) the selected matrix cells.
                CopyPasteSelectionHeatmapCellCtrlX(this.xStartIdxCP, this.yStartIdxCP, this.xEndIdxCP, this.yEndIdxCP);
            }
        }
        /// <summary>
        /// Paste action (Ctrl + V).
        /// </summary>
        private void HeatmapChartControl_HeatMapCopyPasteCtrlV(object sender, CopyPasteCtrlVEventArgs e)
        {
            // Retrieve the X and Y coordinates in which to paste the copied matrix.
            int xIdxCtrlV = e.XIdxCtrlV - 1;
            int yIdxCtrlV = e.YIdxCtrlV - 1;

            // Call the method that paste the selected and copied matrix.
            CopyPasteSelectionHeatmapCellCtrlV(xIdxCtrlV, yIdxCtrlV);
        }
        /// <summary>
        /// Decrease Cell Size. Increase the number of the cell in the Heatmap.
        /// </summary>
        private void DecreaseCellSizeButton_Click(object sender, EventArgs e)
        {
            bool isTheGameOfLifeRunning = false;

            // The action of reducing the cell size can be only done when the Game of Life Thread is in pause.
            // Stop the Thread momentaneously 
            if (this.keepIteratig && this.gameOfLifeThr != null)
            {
                isTheGameOfLifeRunning = true;
                this.keepIteratig = false;
                this.gameOfLifeThr.Abort();
            }
            // Decrease the pixel dimension of the cell by 1. Only if it does not go under the minimumCellSizePossible.
            if (this.cellSizePixels - 1 > this.minimumCellSizePossible)
            {
                this.cellSizePixels -= 1;
                // Only now that the thread is stopped. I can modify the dimension of the heatmap matrix.
                DecreaseCellSizeMatrixDimension();
            }
            // Check if the GameOfLifeIteration was running. If it was, then re-run the GameOfLife Method/Task.
            if (isTheGameOfLifeRunning)
            {
                this.keepIteratig = true;
                GameOfLifeIterations();
            }
            // Otherwise just assign the new increased in dimension matrix to the heatmap.
            else
            {
                // Check if there is any currGenValuesToPlot to plot.
                if (this.currGenValuesToPlot != null)
                    AssignValuesToPlot(this.currGenValuesToPlot);
                else
                    AssignValuesToPlot(this.startingValuesToPlot);
            }
        }
        /// <summary>
        /// Increase Cell Size. Decrease the number of the cell in the Heatmap.
        /// </summary>
        private void IncreaseCellSizeButton_Click(object sender, EventArgs e)
        {
            bool isTheGameOfLifeRunning = false;

            // The action of reducing the cell size can be only done when the Game of Life Thread is in pause.
            // Stop the Thread momentaneously 
            if (this.keepIteratig && this.gameOfLifeThr != null)
            {
                isTheGameOfLifeRunning = true;
                this.keepIteratig = false;
                this.gameOfLifeThr.Abort();
            }
            // Decrease the pixel dimension of the cell by 1. Only if it does not go under the minimumCellSizePossible.
            if (this.cellSizePixels <= this.maximumCellSizePossible)
            {
                this.cellSizePixels += 1;
                // Only now that the thread is stopped. I can modify the dimension of the heatmap matrix.
                IncreaseCellSizeMatrixDimension();
            }
            // Check if the GameOfLifeIteration was running. If it was, then re-run the GameOfLife Method/Task.
            if (isTheGameOfLifeRunning)
            {
                this.keepIteratig = true;
                GameOfLifeIterations();
            }
            // Otherwise just assign the new increased in dimension matrix to the heatmap.
            else
            {
                // Check if there is any currGenValuesToPlot to plot.
                if (this.currGenValuesToPlot != null)
                    AssignValuesToPlot(this.currGenValuesToPlot);
                else
                    AssignValuesToPlot(this.startingValuesToPlot);
            }
        }
        /// <summary>
        /// Slow down the speed of the GameOfLife.
        /// </summary>
        private void MinusSpeedButton_Click(object sender, EventArgs e)
        {
            bool isTheGameOfLifeRunning = false;
            // The action of reducing the cell size can be only done when the Game of Life Thread is in pause.
            // Stop the Thread momentaneously 
            if (this.keepIteratig && this.gameOfLifeThr != null)
            {
                isTheGameOfLifeRunning = true;
                this.keepIteratig = false;
                this.gameOfLifeThr.Abort();
            }
            // Decrease the pixel dimension of the cell by 1. Only if it does not go under the minimumCellSizePossible.
            if (this.timerSpeedMilliSecond + TIMER_SPEED_INCREASE_DECREASE < this.minimumSpeedPossible)
            {
                this.timerSpeedMilliSecond += TIMER_SPEED_INCREASE_DECREASE;
            }
            // Check if the GameOfLifeIteration was running. If it was, then re-run the GameOfLife Method/Task.
            if (isTheGameOfLifeRunning)
            {
                this.keepIteratig = true;
                GameOfLifeIterations();
            }
        }
        /// <summary>
        /// Speed up the speed of the GameOfLife.
        /// </summary>
        private void PlusSpeedButton_Click(object sender, EventArgs e)
        {
            bool isTheGameOfLifeRunning = false;
            // The action of reducing the cell size can be only done when the Game of Life Thread is in pause.
            // Stop the Thread momentaneously 
            if (this.keepIteratig && this.gameOfLifeThr != null)
            {
                isTheGameOfLifeRunning = true;
                this.keepIteratig = false;
                this.gameOfLifeThr.Abort();
            }
            // Decrease the pixel dimension of the cell by 1. Only if it does not go under the minimumCellSizePossible.
            if (this.timerSpeedMilliSecond - TIMER_SPEED_INCREASE_DECREASE > this.maximumSpeedPossible)
            {
                this.timerSpeedMilliSecond -= TIMER_SPEED_INCREASE_DECREASE;
            }
            // Check if the GameOfLifeIteration was running. If it was, then re-run the GameOfLife Method/Task.
            if (isTheGameOfLifeRunning)
            {
                this.keepIteratig = true;
                GameOfLifeIterations();
            }
        }
        #endregion
    }
}
