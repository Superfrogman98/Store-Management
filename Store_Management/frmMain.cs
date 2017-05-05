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
        BindingSource inventoryBindingSource = new BindingSource();
        BindingSource productBindingSource = new BindingSource();

        String defaultConnection = "";
        public frmMain()
        {
            InitializeComponent();
            defaultConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Directory.GetCurrentDirectory() + "\\Store.accdb"; //get the connection string from the current program directory
            if (Properties.Settings.Default.Database == "")
            {
                Properties.Settings.Default.Database = defaultConnection;
            }


            connection.ConnectionString = defaultConnection;

           fillAllTables("Inventory data could not be recieved", "Product data could not be recieved");
        }


        //closes the form
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //fills the inventory table
        public void fillInventoryTable(string error)
        {
            try
            {
                connection.Open();
                OleDbCommand getInventory = new OleDbCommand();
                getInventory.Connection = connection;
                getInventory.CommandText = "SELECT ProductName, UPC, Department, Instock FROM Products ";
                OleDbDataAdapter adap = new OleDbDataAdapter(getInventory);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                getInventory.Dispose();
                connection.Close();
                inventoryBindingSource.DataSource = ds.Tables[0].DefaultView;
                dgvInventory.DataSource = inventoryBindingSource;
                dgvInventory.Columns[0].HeaderText = "Product Name";//set column 1 
                                                                    //Keep column two header as UPC
                dgvInventory.Columns[2].HeaderText = "Department";//set column 3 
                dgvInventory.Columns[3].HeaderText = "Amount Instock";//set column 4
            }
            catch (Exception)
            {
                MessageBox.Show(error);
            }

        }

        //fills the product table
        public void fillProductTable(string error)
        {
            try
            {
                connection.Open();
                OleDbCommand getInventory = new OleDbCommand();
                getInventory.Connection = connection;
                getInventory.CommandText = "SELECT ProductName, UPC, Department, SellPrice, BuyCost FROM Products ";
                OleDbDataAdapter adap = new OleDbDataAdapter(getInventory);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                getInventory.Dispose();
                connection.Close();
                productBindingSource.DataSource = ds.Tables[0].DefaultView;

                dgvProducts.DataSource = productBindingSource;
                dgvProducts.Columns[0].HeaderText = "Product Name";//set column 1 
                                                                    //Keep column two header as UPC
                dgvProducts.Columns[2].HeaderText = "Department";//set column 3 
                dgvProducts.Columns[3].HeaderText = "Sell Price";//set column 4
                dgvProducts.Columns[4].HeaderText = "Buy Cost";//set column 5
                dgvProducts.Columns[3].DefaultCellStyle.Format = "C2";
                dgvProducts.Columns[4].DefaultCellStyle.Format = "C2";

            }
            catch (Exception)
            {
                MessageBox.Show(error);
            }

        }

        private void changeDatabaseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Title = "Select Database File";
                openFileDialog1.Filter = "MS-Access Files|*.accdb";
                openFileDialog1.FileName = "";
                openFileDialog1.InitialDirectory = "C:\\Temp";  //Suggested path for where the file could exist
                openFileDialog1.ShowDialog();


                //if no file is selected then message will be displayed
                if (openFileDialog1.FileName == "")
                {

                    MessageBox.Show("No File was selected");

                }
                else {

                    String dbNameWithPath = openFileDialog1.FileName;//openFileDialog1.FileName is where the selected file is stored

                    String connectionString = "Provider=Microsoft.ace.OLEDB.12.0;Data Source=" + dbNameWithPath;


                    //trys filling the table from the database the user selected, upon fail the catch will causes the connection string to switch to the default database

                    connection.ConnectionString = connectionString;
                    fillAllTables("New Database does not have the required fields for Inventory", "New Database does not have the required fields for Products");
                    Properties.Settings.Default.Database = connectionString;
                    MessageBox.Show("New Connection Successful");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("New database connection failed");
                connection.ConnectionString = defaultConnection;
                fillAllTables("Inventory data could not be recieved", "Product data could not be recieved");
            }
        }

        //function for when all tables need to be refreshed
        private void fillAllTables(string msg1, string msg2)
        {
            fillInventoryTable(msg1);
            fillProductTable(msg2);
        }

    }
}
