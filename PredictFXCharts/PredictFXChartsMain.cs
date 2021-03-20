using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PredictFXCharts.Market;
using System.Globalization;

namespace PredictFXCharts
{
    public partial class PredictFXChartsMain : Form
    {

        DataManager dm;
        TermManager tmgr;
        consoledb2DataDataContext dataContext = new consoledb2DataDataContext();
        int lastSettingsCount;
        int lastVolCount;

        //
        // Array to hold all Windows.ChartViewers in the form to allow processing using loops
        //
        private ChartDirector.WinChartViewer[] chartViewers;

        //
        // Data structure to handle the Back / Forward buttons. We support up to
        // 100 histories. We store histories as the nodes begin selected.
        //
        private TreeNode[] history = new TreeNode[100];

        // The array index of the currently selected node in the history array.
        private int currentHistoryIndex = -1;

        // The array index of the last valid entry in the history array so that we
        // know if we can use the Forward button.
        private int lastHistoryIndex = -1;


        public PredictFXChartsMain()
        {
            InitializeComponent();

            //
            // Array to hold all Windows.ChartViewers in the form to allow processing 
            // using loops
            //
            chartViewers = new ChartDirector.WinChartViewer[] 
			{ 
				chartViewer1
			};

            ////
            //// Build the tree view on the left to represent available demo modules
            ////

            TreeNode pieNode1 = new TreeNode("Открытый интерес");
            treeView.Nodes.Add(pieNode1);

            pieNode1.Nodes.Add(MakeNode(new Open_Positions(), 11));



            //---
            TreeNode pieNode7 = new TreeNode("Графики");
            treeView.Nodes.Add(pieNode7);
            pieNode7.Nodes.Add(MakeNode(new PriceChart(), 4));

            pieNode7.Nodes.Add(MakeNode(new ValueAtRisk_Level(), 11));


            treeView.SelectedNode = getNextChartNode(treeView.Nodes[0]);


            // Default parameters

            dtpFromDate.Text = DateTime.Today.ToString();
            dtpToDate.Text = DateTime.Today.ToString();

            priceTypeCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// Help method to add a demo module to the tree
        /// </summary>
        private TreeNode MakeNode(DemoModule module, int icon)
        {
            TreeNode node = new TreeNode(module.getName(), icon, icon);
            node.Tag = module;	// The demo module is attached to the node as the Tag property.
            return node;
        }

        // **********************************************************************

        public static void ShowMessage(string text)
        {
            MessageBox.Show(text, cfg.FullProgName);
        }

        private void PredictFXChartsMain_Shown(object sender, EventArgs e)
        {
            if (!checkTrialPeriod())
            {
                ShowMessage("Пробный период использования программы закончен. Скачайте пожалуйста обновление на сайте PreditGroup.com.");
                this.Close();
            }

            dm = new DataManager();
            tmgr = new TermManager(dm);
            MarketProvider.SetReceiver(dm);
            ChartsManager.SetDataManager(dm);
            ChartsManager.SetTermManager(tmgr);

            MarketProvider.Activate();
            ChartsManager.Activate();

            tmgr.Connect();
        }

        private void PredictFXChartsMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmgr.ExecAction(new OwnAction(
                                            TradeOp.Cancel,
                                            BaseQuote.None,
                                            0,
                                            0)); // отменяем ордер

            MarketProvider.Deactivate();
            ChartsManager.Deactivate();

            tmgr.Disconnect();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.Show();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmParameters param = new frmParameters();
            param.Show();
        }

        // **********************************************************************

        /// <summary>
        /// Handler for the TreeView BeforeExpand event
        /// </summary>
        private void treeView_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            // Change the node to use the open folder icon when the node expands
            e.Node.SelectedImageIndex = e.Node.ImageIndex = 1;
        }

        /// <summary>
        /// Handler for the TreeView BeforeCollapse event
        /// </summary>
        private void treeView_BeforeCollapse(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            // Change the node to use the clode folder icon when the node collapse
            e.Node.SelectedImageIndex = e.Node.ImageIndex = 0;
        }

        /// <summary>
        /// Handler for the TreeView AfterSelect event
        /// </summary>
        private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.ByMouse || e.Action == TreeViewAction.ByKeyboard)
            {
                DateTime fromDate = dtpFromDate.Value;
                DateTime toDate = dtpToDate.Value;

                // Check if a demo module node is selected. Demo module is attached to the node
                // as the Tag property
                DemoModule demo = (DemoModule)e.Node.Tag;
                if (demo != null)
                {
                    // Display the title
                    title.Text = demo.getName();

                    // Clear all ChartViewers
                    for (int i = 0; i < chartViewers.Length; ++i)
                        chartViewers[i].Visible = false;

                    // Each demo module can display a number of charts
                    int noOfCharts = demo.getNoOfCharts();
                    for (int i = 0; i < noOfCharts; ++i)
                    {
                        int chartWidth = rightPanel.Width - 50;
                        int chartHeighth = rightPanel.Height - 110;
                        demo.createChart(chartViewers[i], "" + i, Int32.Parse(tickerSymbol.SelectedValue.ToString()), Int32.Parse(timeFramePeriod.SelectedValue.ToString()), fromDate, toDate, (int)trbSize.Value, chartWidth, chartHeighth, priceTypeCombo.SelectedIndex);
                        chartViewers[i].Visible = true;
                    }

                    // Now perform flow layout of the charts (viewers) 
                    layoutCharts();

                    // Add current node to the history array to support Back/Forward browsing
                    addHistory(e.Node);
                }

                // Update the state of the buttons, status bar, etc.
                updateControls();
            }
        }

        /// <summary>
        /// Helper method to perform a flow layout (left to right, top to bottom) of
        /// the chart.
        /// </summary>
        private void layoutCharts()
        {
            // Margin between the charts
            int margin = 5;

            // The first chart is always at the position as seen on the visual designer
            ChartDirector.WinChartViewer viewer = chartViewers[0];
            viewer.Top = pnlChartParams.Bottom + margin;

            // The next chart is at the left of the first chart
            int currentX = viewer.Left + viewer.Width + margin;
            int currentY = viewer.Top;

            // The line height of a line of charts is that of the tallest chart in the line
            int lineHeight = viewer.Height;

            // Now layout subsequent charts (other than the first chart)
            for (int i = 1; i < chartViewers.Length; ++i)
            {
                viewer = chartViewers[i];

                // Layout only visible charts
                if (!viewer.Visible)
                    continue;

                // Check for enough space on the left before it hits the panel border
                if (currentX + viewer.Width > rightPanel.Width)
                {
                    // Not enough space, so move to the next line

                    // Start of line is to align with the left of the first chart
                    currentX = chartViewers[0].Left;

                    // Adjust vertical by lineHeight plus a margin
                    currentY += lineHeight + margin;

                    // Reset the lineHeight
                    lineHeight = 0;
                }

                // Put the chart at the current position
                viewer.Left = currentX;
                viewer.Top = currentY;

                // Advance the current position to the left prepare for the next chart
                currentX += viewer.Width + margin;

                // Update the lineHeight to reflect the tallest chart so far encountered
                // in the same line
                if (lineHeight < viewer.Height)
                    lineHeight = viewer.Height;
            }
        }

        /// <summary>
        /// Add a selected node to the history array
        /// </summary>
        private void addHistory(TreeNode node)
        {
            // Don't add if selected node is current node to avoid duplication.
            if ((currentHistoryIndex >= 0) && (node == history[currentHistoryIndex]))
                return;

            // Check if the history array is full
            if (currentHistoryIndex + 1 >= history.Length)
            {
                // History array is full. Remove oldest 25% from the history array.
                // We add 1 to make sure at least 1 item is removed.
                int itemsToDiscard = history.Length / 4 + 1;

                // Remove the oldest items by shifting the array. 
                for (int i = itemsToDiscard; i < history.Length; ++i)
                    history[i - itemsToDiscard] = history[i];

                // Adjust index because array is shifted.
                currentHistoryIndex = history.Length - itemsToDiscard;
            }

            // Add node to history array
            history[++currentHistoryIndex] = node;

            // After adding a new node, the forward button is always disabled. (This
            // is consistent with normal browser behaviour.) That means the last 
            // history node is always assumed to be the current node. 
            lastHistoryIndex = currentHistoryIndex;
        }


        // **********************************************************************


        /// <summary>
        /// Handler for the Back button
        /// </summary>
        private void backHistory()
        {
            // Select the previous node in the history array
            if (currentHistoryIndex > 0)
                treeView.SelectedNode = history[--currentHistoryIndex];
        }

        /// <summary>
        /// Handler for the Forward button
        /// </summary>
        private void forwardHistory()
        {
            // Select the next node in the history array
            if (lastHistoryIndex > currentHistoryIndex)
                treeView.SelectedNode = history[++currentHistoryIndex];
        }

        /// <summary>
        /// Handler for the Next button
        /// </summary>
        private void nextNode()
        {
            // Getnext chart node of the current selected node by going down the tree
            TreeNode node = getNextChartNode(treeView.SelectedNode);

            // Display the node if available
            if (node != null)
                treeView.SelectedNode = node;
        }

        /// <summary>
        /// Helper method to go to the next chart node down the tree
        /// </summary>
        private TreeNode getNextChartNode(TreeNode node)
        {
            // Get the next node of by going down the tree
            node = getNextNode(node);

            // Skip nodes that are not associated with charts (e.g the folder nodes)
            while ((node != null) && (node.Tag == null))
                node = getNextNode(node);

            return node;
        }

        /// <summary>
        /// Helper method to go to the next node down the tree
        /// </summary>
        private TreeNode getNextNode(TreeNode node)
        {
            if (node == null)
                return null;

            // If the current node is a folder, the next node is the first child.
            if (node.FirstNode != null)
                return node.FirstNode;

            while (node != null)
            {
                // If there is a next sibling node, it is the next node.
                if (node.NextNode != null)
                    return node.NextNode;

                // If there is no sibling node, the next node is the next sibling 
                // of the parent node. So we go back to the parent and loop again.
                node = node.Parent;
            }

            // No next node - must be already the last node.
            return null;
        }

        /// <summary>
        /// Handler for the Previous button
        /// </summary>
        private void prevNode()
        {
            // Get previous chart node of the current selected node by going up the tree
            TreeNode node = getPrevChartNode(treeView.SelectedNode);

            // Display the node if available
            if (node != null)
                treeView.SelectedNode = node;
        }

        /// <summary>
        /// Helper method to go to the previous chart node down the tree
        /// </summary>
        private TreeNode getPrevChartNode(TreeNode node)
        {
            // Get the prev node of by going up the tree
            node = getPrevNode(node);

            // Skip nodes that are not associated with charts (e.g the folder nodes)
            while ((node != null) && (node.Tag == null))
                node = getPrevNode(node);

            return node;
        }

        /// <summary>
        /// Helper method to go to the previous node up the tree
        /// </summary>
        private TreeNode getPrevNode(TreeNode node)
        {
            if (node == null)
                return null;

            // If there is no previous sibling node, the previous node must be its
            // parent. 
            if (node.PrevNode == null)
                return node.Parent;

            // If there is a previous sibling node, the previous node is the last
            // child of the previous sibling (if it has any child at all).
            node = node.PrevNode;
            while (node.LastNode != null)
                node = node.LastNode;

            return node;
        }

        // **********************************************************************

        /// <summary>
        /// Helper method to update the various controls
        /// </summary>
        private void updateControls()
        {
            //
            // Enable the various buttons there is really something they can do.
            //

            // The status bar always reflects the selected demo module
            if ((null != treeView.SelectedNode) && (null != treeView.SelectedNode.Tag))
            {
                DemoModule m = (DemoModule)treeView.SelectedNode.Tag;
                statusLabel1.Text = " Module " + m.GetType().Name + ": " + m.getName();
            }
            else
                statusLabel1.Text = "";
        }

        /// <summary>
        /// Handler for the panel layout event
        /// </summary>
        private void rightPanel_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            // Perform flow layout of the charts 
            if (chartViewers != null)
                layoutCharts();
        }

        /// <summary>
        /// Handler for the ClickHotSpot event, which occurs when the mouse clicks on 
        /// a hot spot on the chart
        /// </summary>
        private void chartViewer1_ClickHotSpot(object sender, ChartDirector.WinHotSpotEventArgs e)
        {
            // In this demo, just list out the information provided by ChartDirector about hot spot

        }

        // **********************************************************************

        private void btnUpdateChart_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null)
                return;

            //+
            Int64 tickerID = (Int64)tickerSymbol.SelectedValue;
            int timeFrameID = Int32.Parse(timeFramePeriod.SelectedValue.ToString());

            this.timeFramesTableAdapter.FillByActive(this.cONSOLEDB2DataSet.timeframes);

            this.stocksTableAdapter.FillByActive(this.cONSOLEDB2DataSet.stocks);

            if (tickerID >= 0)
            {
                tickerSymbol.SelectedValue = tickerID;
            }
            if (timeFrameID >= 0)
            {
                timeFramePeriod.SelectedValue = timeFrameID;

            }
            //-

            DateTime fromDate = dtpFromDate.Value;
            DateTime toDate = dtpToDate.Value;

            // Check if a demo module node is selected. Demo module is attached to the node
            // as the Tag property
            DemoModule demo = (DemoModule)treeView.SelectedNode.Tag;
            if (demo != null)
            {
                // Display the title
                title.Text = demo.getName();

                // Clear all ChartViewers
                for (int i = 0; i < chartViewers.Length; ++i)
                    chartViewers[i].Visible = false;

                // Each demo module can display a number of charts
                int noOfCharts = demo.getNoOfCharts();
                for (int i = 0; i < noOfCharts; ++i)
                {
                    int chartWidth = rightPanel.Width - 50;
                    int chartHeighth = rightPanel.Height - 110;
                    demo.createChart(chartViewers[i], "" + i, Int32.Parse(tickerSymbol.SelectedValue.ToString()), Int32.Parse(timeFramePeriod.SelectedValue.ToString()), fromDate, toDate, (int)trbSize.Value, chartWidth, chartHeighth, priceTypeCombo.SelectedIndex);
                    chartViewers[i].Visible = true;
                }

                // Now perform flow layout of the charts (viewers) 
                layoutCharts();

                // Add current node to the history array to support Back/Forward browsing
                addHistory(treeView.SelectedNode);
            }

            // Update the state of the buttons, status bar, etc.
            updateControls();
        }

        private void PredictFXChartsMain_Load(object sender, EventArgs e)
        {

            this.timeFramesTableAdapter.FillByActive(this.cONSOLEDB2DataSet.timeframes);

            this.stocksTableAdapter.FillByActive(this.cONSOLEDB2DataSet.stocks);

            lastSettingsCount = 0;
            lastVolCount = 0;

            updateChartTimer.Interval = 5000;
            updateChartTimer.Start();

            var stk_id = (from s in dataContext.stocks
                          where s.ticker == cfg.u.SecCode
                          select s).FirstOrDefault();
            if (stk_id != null)
            {
                tickerSymbol.SelectedValue = stk_id.id;
            }
        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.stocksTableAdapter.FillByActive(this.cONSOLEDB2DataSet.stocks);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void помощьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("");
        }

        private void updateChartTimer_Tick(object sender, EventArgs e)
        {
            chartViewer1.updateViewPort(true, false);
        }

        private void chartViewer1_ViewPortChanged(object sender, ChartDirector.WinViewPortEventArgs e)
        {
            DemoModule demo = (DemoModule)treeView.SelectedNode.Tag;
            if (demo != null)
            {
                bool updated = false;


                DateTime fromDate = dtpFromDate.Value;
                DateTime toDate = dtpToDate.Value;

                if (demo.getChartGroup() == 1)
                {
                    var volQuotes = (from q in dataContext.volquotes
                                     where q.stock == Int32.Parse(tickerSymbol.SelectedValue.ToString())
                                     && q.datetime >= fromDate
                                     && q.datetime <= toDate.AddDays(1)
                                         //&& q.TimeFrame == Int32.Parse(timeFramePeriod.SelectedValue.ToString())
                                     && q.timeframe == 1 // поменяли в связи с тем, что в графиках последняя свеча суммируется по таймфрейму 1, т.е. каждую минуту даже если график таймфреймом более 1 минуты.
                                     select q).Count();

                    if (lastVolCount != volQuotes)
                    {
                        updated = true;
                        lastVolCount = volQuotes;
                    }

                }
                if (demo.getChartGroup() == 2)
                {
                    var settingQuotes = (from sq in dataContext.settingquotes
                                         where sq.stock == Int32.Parse(tickerSymbol.SelectedValue.ToString())
                                         && sq.datetime >= fromDate
                                         && sq.datetime <= toDate.AddDays(1)
                                             //&& sq.TimeFrame == Int32.Parse(timeFramePeriod.SelectedValue.ToString())
                                         && sq.timeframe == 1 // поменяли в связи с тем, что в графиках последняя свеча суммируется по таймфрейму 1, т.е. каждую минуту даже если график таймфреймом более 1 минуты.
                                         select sq).Count();
                    if (lastSettingsCount != settingQuotes)
                    {
                        updated = true;
                        lastSettingsCount = settingQuotes;
                    }
                }

                if (updated)
                {
                    // Check if a demo module node is selected. Demo module is attached to the node
                    // as the Tag property

                    // Clear all ChartViewers
                    for (int i = 0; i < chartViewers.Length; ++i)
                        chartViewers[i].Visible = false;

                    // Each demo module can display a number of charts
                    int noOfCharts = demo.getNoOfCharts();
                    for (int i = 0; i < noOfCharts; ++i)
                    {
                        int chartWidth = rightPanel.Width - 50;
                        int chartHeighth = rightPanel.Height - 110;
                        demo.createChart(chartViewers[i], "" + i, Int32.Parse(tickerSymbol.SelectedValue.ToString()), Int32.Parse(timeFramePeriod.SelectedValue.ToString()), fromDate, toDate, (int)trbSize.Value, chartWidth, chartHeighth, priceTypeCombo.SelectedIndex);
                        chartViewers[i].Visible = true;
                    }
                }

            }
        }

        // **********************************************************************

        private bool checkTrialPeriod()
        {
            bool retBool = true;

            DateTime trialDate = DateTime.Parse("2025-02-04 14:00:00.000");

            //var lastDate = (from d in dataContext.VolQuotes
            //                orderby d.Date descending
            //                select d.Date.Value).FirstOrDefault();

            //if (lastDate == null)
            //{
            //    retBool = false;
            //}
            //else
            //{
            //    if(lastDate >= trialDate)
            //        retBool = false;
            //}

            return retBool;
        }

    }
}
