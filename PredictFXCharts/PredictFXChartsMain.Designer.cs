namespace PredictFXCharts
{
    partial class PredictFXChartsMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PredictFXChartsMain));
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.действиеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.параметрыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.помощьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.помощьToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.оПрограммеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView = new System.Windows.Forms.TreeView();
            this.treeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.rightPanel = new System.Windows.Forms.Panel();
            this.pnlChartParams = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.priceTypeCombo = new System.Windows.Forms.ComboBox();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.trbSize = new System.Windows.Forms.TrackBar();
            this.tickerSymbol = new System.Windows.Forms.ComboBox();
            this.stocksBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cONSOLEDB2DataSet = new PredictFXCharts.CONSOLEDB2DataSet1();
            this.btnUpdateChart = new System.Windows.Forms.Button();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.startDateLabel = new System.Windows.Forms.Label();
            this.timeFramePeriod = new System.Windows.Forms.ComboBox();
            this.timeFramesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.timePeriodLabel = new System.Windows.Forms.Label();
            this.tickerSymbolLabel = new System.Windows.Forms.Label();
            this.title = new System.Windows.Forms.Label();
            this.chartViewer1 = new ChartDirector.WinChartViewer();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.timeFramesTableAdapter = new PredictFXCharts.CONSOLEDB2DataSet1TableAdapters.timeframesTableAdapter();
            this.updateChartTimer = new System.Windows.Forms.Timer(this.components);
            this.stocksTableAdapter = new PredictFXCharts.CONSOLEDB2DataSet1TableAdapters.stocksTableAdapter();
            this.statusBar.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.pnlChartParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stocksBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cONSOLEDB2DataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeFramesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartViewer1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel1});
            this.statusBar.Location = new System.Drawing.Point(0, 640);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1084, 22);
            this.statusBar.TabIndex = 2;
            // 
            // statusLabel1
            // 
            this.statusLabel1.Name = "statusLabel1";
            this.statusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.действиеToolStripMenuItem,
            this.параметрыToolStripMenuItem,
            this.помощьToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1084, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // действиеToolStripMenuItem
            // 
            this.действиеToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.выходToolStripMenuItem});
            this.действиеToolStripMenuItem.Name = "действиеToolStripMenuItem";
            this.действиеToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.действиеToolStripMenuItem.Text = "Файл";
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // параметрыToolStripMenuItem
            // 
            this.параметрыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настройкиToolStripMenuItem});
            this.параметрыToolStripMenuItem.Name = "параметрыToolStripMenuItem";
            this.параметрыToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.параметрыToolStripMenuItem.Text = "Параметры";
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            this.настройкиToolStripMenuItem.Click += new System.EventHandler(this.настройкиToolStripMenuItem_Click);
            // 
            // помощьToolStripMenuItem
            // 
            this.помощьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.помощьToolStripMenuItem1,
            this.оПрограммеToolStripMenuItem});
            this.помощьToolStripMenuItem.Name = "помощьToolStripMenuItem";
            this.помощьToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.помощьToolStripMenuItem.Text = "Помощь";
            // 
            // помощьToolStripMenuItem1
            // 
            this.помощьToolStripMenuItem1.Name = "помощьToolStripMenuItem1";
            this.помощьToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.помощьToolStripMenuItem1.Text = "Помощь";
            this.помощьToolStripMenuItem1.Click += new System.EventHandler(this.помощьToolStripMenuItem1_Click);
            // 
            // оПрограммеToolStripMenuItem
            // 
            this.оПрограммеToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("оПрограммеToolStripMenuItem.Image")));
            this.оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            this.оПрограммеToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.оПрограммеToolStripMenuItem.Text = "О программе";
            this.оПрограммеToolStripMenuItem.Click += new System.EventHandler(this.оПрограммеToolStripMenuItem_Click);
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView.HotTracking = true;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.treeViewImageList;
            this.treeView.Location = new System.Drawing.Point(0, 24);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(200, 616);
            this.treeView.TabIndex = 1;
            this.treeView.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeCollapse);
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeExpand);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // treeViewImageList
            // 
            this.treeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeViewImageList.ImageStream")));
            this.treeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeViewImageList.Images.SetKeyName(0, "folderclose.gif");
            this.treeViewImageList.Images.SetKeyName(1, "folderopen.gif");
            this.treeViewImageList.Images.SetKeyName(2, "pieicon.png");
            this.treeViewImageList.Images.SetKeyName(3, "baricon.png");
            this.treeViewImageList.Images.SetKeyName(4, "lineicon.png");
            this.treeViewImageList.Images.SetKeyName(5, "trendicon.png");
            this.treeViewImageList.Images.SetKeyName(6, "bubbleicon.png");
            this.treeViewImageList.Images.SetKeyName(7, "areaicon.png");
            this.treeViewImageList.Images.SetKeyName(8, "boxicon.png");
            this.treeViewImageList.Images.SetKeyName(9, "gantt.gif");
            this.treeViewImageList.Images.SetKeyName(10, "contouricon.png");
            this.treeViewImageList.Images.SetKeyName(11, "financeicon.png");
            this.treeViewImageList.Images.SetKeyName(12, "xyicon.png");
            this.treeViewImageList.Images.SetKeyName(13, "surfaceicon.png");
            this.treeViewImageList.Images.SetKeyName(14, "polaricon.png");
            this.treeViewImageList.Images.SetKeyName(15, "pyramidicon.png");
            this.treeViewImageList.Images.SetKeyName(16, "meter.gif");
            this.treeViewImageList.Images.SetKeyName(17, "zoomInIcon.gif");
            this.treeViewImageList.Images.SetKeyName(18, "zoomOutIcon.gif");
            // 
            // rightPanel
            // 
            this.rightPanel.AutoScroll = true;
            this.rightPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rightPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.rightPanel.Controls.Add(this.pnlChartParams);
            this.rightPanel.Controls.Add(this.chartViewer1);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(200, 24);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(884, 616);
            this.rightPanel.TabIndex = 6;
            this.rightPanel.Layout += new System.Windows.Forms.LayoutEventHandler(this.rightPanel_Layout);
            // 
            // pnlChartParams
            // 
            this.pnlChartParams.BackColor = System.Drawing.SystemColors.Control;
            this.pnlChartParams.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlChartParams.Controls.Add(this.label2);
            this.pnlChartParams.Controls.Add(this.priceTypeCombo);
            this.pnlChartParams.Controls.Add(this.sizeLabel);
            this.pnlChartParams.Controls.Add(this.trbSize);
            this.pnlChartParams.Controls.Add(this.tickerSymbol);
            this.pnlChartParams.Controls.Add(this.btnUpdateChart);
            this.pnlChartParams.Controls.Add(this.dtpFromDate);
            this.pnlChartParams.Controls.Add(this.label1);
            this.pnlChartParams.Controls.Add(this.dtpToDate);
            this.pnlChartParams.Controls.Add(this.startDateLabel);
            this.pnlChartParams.Controls.Add(this.timeFramePeriod);
            this.pnlChartParams.Controls.Add(this.timePeriodLabel);
            this.pnlChartParams.Controls.Add(this.tickerSymbolLabel);
            this.pnlChartParams.Controls.Add(this.title);
            this.pnlChartParams.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlChartParams.Location = new System.Drawing.Point(0, 0);
            this.pnlChartParams.Name = "pnlChartParams";
            this.pnlChartParams.Size = new System.Drawing.Size(880, 100);
            this.pnlChartParams.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(277, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "Цена";
            // 
            // priceTypeCombo
            // 
            this.priceTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.priceTypeCombo.FormattingEnabled = true;
            this.priceTypeCombo.Items.AddRange(new object[] {
            "По Close",
            "По средневзвешенной"});
            this.priceTypeCombo.Location = new System.Drawing.Point(277, 48);
            this.priceTypeCombo.Name = "priceTypeCombo";
            this.priceTypeCombo.Size = new System.Drawing.Size(121, 21);
            this.priceTypeCombo.TabIndex = 12;
            // 
            // sizeLabel
            // 
            this.sizeLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sizeLabel.Location = new System.Drawing.Point(647, 33);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(72, 16);
            this.sizeLabel.TabIndex = 11;
            this.sizeLabel.Text = "Размер";
            // 
            // trbSize
            // 
            this.trbSize.Location = new System.Drawing.Point(647, 48);
            this.trbSize.Maximum = 15;
            this.trbSize.Minimum = 5;
            this.trbSize.Name = "trbSize";
            this.trbSize.Size = new System.Drawing.Size(104, 42);
            this.trbSize.TabIndex = 10;
            this.trbSize.Value = 9;
            // 
            // tickerSymbol
            // 
            this.tickerSymbol.DataSource = this.stocksBindingSource;
            this.tickerSymbol.DisplayMember = "ticker";
            this.tickerSymbol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tickerSymbol.FormattingEnabled = true;
            this.tickerSymbol.Location = new System.Drawing.Point(10, 47);
            this.tickerSymbol.Name = "tickerSymbol";
            this.tickerSymbol.Size = new System.Drawing.Size(121, 21);
            this.tickerSymbol.TabIndex = 0;
            this.tickerSymbol.ValueMember = "id";
            // 
            // stocksBindingSource
            // 
            this.stocksBindingSource.DataMember = "Stocks";
            this.stocksBindingSource.DataSource = this.cONSOLEDB2DataSet;
            // 
            // cONSOLEDB2DataSet
            // 
            this.cONSOLEDB2DataSet.DataSetName = "CONSOLEDB2DataSet";
            this.cONSOLEDB2DataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnUpdateChart
            // 
            this.btnUpdateChart.Location = new System.Drawing.Point(757, 45);
            this.btnUpdateChart.Name = "btnUpdateChart";
            this.btnUpdateChart.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateChart.TabIndex = 4;
            this.btnUpdateChart.Text = "Обновить";
            this.btnUpdateChart.UseVisualStyleBackColor = true;
            this.btnUpdateChart.Click += new System.EventHandler(this.btnUpdateChart_Click);
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(408, 49);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(112, 20);
            this.dtpFromDate.TabIndex = 2;
            this.dtpFromDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(408, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Дата с";
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(529, 49);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(112, 20);
            this.dtpToDate.TabIndex = 3;
            this.dtpToDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // startDateLabel
            // 
            this.startDateLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startDateLabel.Location = new System.Drawing.Point(526, 33);
            this.startDateLabel.Name = "startDateLabel";
            this.startDateLabel.Size = new System.Drawing.Size(72, 16);
            this.startDateLabel.TabIndex = 9;
            this.startDateLabel.Text = "Дата по";
            // 
            // timeFramePeriod
            // 
            this.timeFramePeriod.DataSource = this.timeFramesBindingSource;
            this.timeFramePeriod.DisplayMember = "timeframename";
            this.timeFramePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.timeFramePeriod.Location = new System.Drawing.Point(141, 48);
            this.timeFramePeriod.MaxDropDownItems = 20;
            this.timeFramePeriod.Name = "timeFramePeriod";
            this.timeFramePeriod.Size = new System.Drawing.Size(130, 21);
            this.timeFramePeriod.TabIndex = 1;
            this.timeFramePeriod.ValueMember = "id";
            // 
            // timeFramesBindingSource
            // 
            this.timeFramesBindingSource.DataMember = "TimeFrames";
            this.timeFramesBindingSource.DataSource = this.cONSOLEDB2DataSet;
            // 
            // timePeriodLabel
            // 
            this.timePeriodLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timePeriodLabel.Location = new System.Drawing.Point(141, 32);
            this.timePeriodLabel.Name = "timePeriodLabel";
            this.timePeriodLabel.Size = new System.Drawing.Size(100, 16);
            this.timePeriodLabel.TabIndex = 7;
            this.timePeriodLabel.Text = "Период";
            // 
            // tickerSymbolLabel
            // 
            this.tickerSymbolLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tickerSymbolLabel.Location = new System.Drawing.Point(7, 32);
            this.tickerSymbolLabel.Name = "tickerSymbolLabel";
            this.tickerSymbolLabel.Size = new System.Drawing.Size(100, 16);
            this.tickerSymbolLabel.TabIndex = 6;
            this.tickerSymbolLabel.Text = "Тикер";
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.title.Location = new System.Drawing.Point(7, 9);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(139, 14);
            this.title.TabIndex = 5;
            this.title.Text = "Наименование графика";
            // 
            // chartViewer1
            // 
            this.chartViewer1.HotSpotCursor = System.Windows.Forms.Cursors.Hand;
            this.chartViewer1.Location = new System.Drawing.Point(3, 106);
            this.chartViewer1.Name = "chartViewer1";
            this.chartViewer1.Size = new System.Drawing.Size(112, 98);
            this.chartViewer1.TabIndex = 0;
            this.chartViewer1.TabStop = false;
            this.chartViewer1.ClickHotSpot += new ChartDirector.WinHotSpotEventHandler(this.chartViewer1_ClickHotSpot);
            this.chartViewer1.ViewPortChanged += new ChartDirector.WinViewPortEventHandler(this.chartViewer1_ViewPortChanged);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(200, 24);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 616);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // timeFramesTableAdapter
            // 
            this.timeFramesTableAdapter.ClearBeforeFill = true;
            // 
            // updateChartTimer
            // 
            this.updateChartTimer.Tick += new System.EventHandler(this.updateChartTimer_Tick);
            // 
            // stocksTableAdapter
            // 
            this.stocksTableAdapter.ClearBeforeFill = true;
            // 
            // PredictFXChartsMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 662);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PredictFXChartsMain";
            this.Text = "PredictFX Charts";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PredictFXChartsMain_FormClosing);
            this.Load += new System.EventHandler(this.PredictFXChartsMain_Load);
            this.Shown += new System.EventHandler(this.PredictFXChartsMain_Shown);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.rightPanel.ResumeLayout(false);
            this.pnlChartParams.ResumeLayout(false);
            this.pnlChartParams.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stocksBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cONSOLEDB2DataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeFramesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartViewer1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem параметрыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem помощьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem помощьToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem оПрограммеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem действиеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList treeViewImageList;
        private System.Windows.Forms.Panel rightPanel;
        private ChartDirector.WinChartViewer chartViewer1;
        private System.Windows.Forms.Panel pnlChartParams;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Label tickerSymbolLabel;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label startDateLabel;
        private System.Windows.Forms.ComboBox timeFramePeriod;
        private System.Windows.Forms.Label timePeriodLabel;
        private System.Windows.Forms.Button btnUpdateChart;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ComboBox tickerSymbol;
        private System.Windows.Forms.BindingSource stocksBindingSource;
        private System.Windows.Forms.BindingSource timeFramesBindingSource;
        private System.Windows.Forms.Timer updateChartTimer;
        private System.Windows.Forms.TrackBar trbSize;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.ComboBox priceTypeCombo;
        private System.Windows.Forms.Label label2;
        private CONSOLEDB2DataSet1 cONSOLEDB2DataSet;
        private CONSOLEDB2DataSet1TableAdapters.stocksTableAdapter stocksTableAdapter;
        private CONSOLEDB2DataSet1TableAdapters.timeframesTableAdapter timeFramesTableAdapter;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel1;
    }
}

