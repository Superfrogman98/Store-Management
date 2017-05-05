//Programmer: Max Buckel
//Date: 2017-5-1
//Function: Manage the Inventory of the store, customer and store purchases for an individual store

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace Store_Management
{
    public partial class frmMain : Form
    {
        private OleDbConnection connection = new OleDbConnection();
        BindingSource storeBindingSource = new BindingSource();
        public frmMain()
        {
            InitializeComponent();
            connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+Directory.GetCurrentDirectory()+"\\Store.accdb";
            connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=H:\2016 school year\Store_Management\Store_Management\bin\Release\Store.accdb";
            fillInventoryTable();
        }


        //closes the form
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void fillInventoryTable()
        {
            try
            {
                connection.Open();
                OleDbCommand getInventory = new OleDbCommand();
                getInventory.Connection = connection;
                getInventory.CommandText = "SELECT ProductName, UPC, Instock, SellPrice, BuyCost FROM Products ";
                OleDbDataAdapter adap = new OleDbDataAdapter(getInventory);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                getInventory.Dispose();
                connection.Close();
                storeBindingSource.DataSource = ds.Tables[0].DefaultView;
                dgvInventory.DataSource = storeBindingSource;
                dgvInventory.Columns[0].HeaderText = "Product Name";//set column 1 
                //Keep column two header as UPC
                dgvInventory.Columns[2].HeaderText = "Amount Instock";//set column 3
                dgvInventory.Columns[3].HeaderText = "Sell Price";//set column 4
                dgvInventory.Columns[4].HeaderText = "Buy Cost";//set column 5

                dgvInventory.Columns[3].DefaultCellStyle.Format = "C2";
                dgvInventory.Columns[4].DefaultCellStyle.Format = "C2";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Inventory Table Could Not Be Filled" + ex);
            }
            
        }

    }
}
