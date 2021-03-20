using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PredictFXCharts
{
    public partial class frmParameters : Form
    {
        consoledb2DataDataContext dataContext = new consoledb2DataDataContext();

        public frmParameters()
        {
            InitializeComponent();

            tbxQuikFldr.Text = cfg.u.QuikFolder;
            tbxDDEServer.Text = cfg.u.DdeServerName;
            tbxAccountID.Text = cfg.u.QuikAccount;

            tbxSecCode.Text = cfg.u.SecCode;
            tbxClassCode.Text = cfg.u.ClassCode;
            tbxClassName.Text = cfg.u.ClassName;
            tbxPriceStep.Text = cfg.u.PriceStep.ToString();
        }

        private void btnSelectQuikFldr_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = cfg.u.QuikFolder;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string foldername = folderBrowserDialog1.SelectedPath;
                tbxQuikFldr.Text = foldername;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            cfg.u.QuikFolder = tbxQuikFldr.Text;
            cfg.u.DdeServerName = tbxDDEServer.Text;

            cfg.u.QuikAccount = tbxAccountID.Text;
            cfg.u.SecCode = tbxSecCode.Text;
            cfg.u.ClassCode = tbxClassCode.Text;
            cfg.u.ClassName = tbxClassName.Text;

            double priceStp = 100;
            if (Double.TryParse(tbxPriceStep.Text, out priceStp) == false)
                priceStp = 100;
            cfg.u.PriceStep = priceStp;

            cfg.SaveUserConfig(cfg.UserCfgFile);

            cfg.LoadUserConfig(cfg.UserCfgFile);

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnSaveStocks_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmParameters_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "cONSOLEDB2DataSet.Stocks". При необходимости она может быть перемещена или удалена.
            this.stocksTableAdapter.Fill(this.cONSOLEDB2DataSet.stocks);
            this.stocksTableAdapter.FillByActive(this.cONSOLEDB2DataSet.stocks);
        }

        // **********************************************************************

    }
}
