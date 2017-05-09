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
        BindingSource departmentBindingSource = new BindingSource();
        int inventorySort = 0;
        String defaultConnection = "";//string for holding the default connenction string to the built in database;
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
            cbxDepartment.SelectedIndex = 0;
        }

        //closes the form
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //fills the inventory table
        public int fillInventoryTable(string error)
        {
            try
            {
                connection.Open(); // connects to the database
                OleDbCommand getInventory = new OleDbCommand();  // sets up a new command
                getInventory.Connection = connection;  // sets the connection of the command
                getInventory.CommandText = "SELECT Products.ProductName, Products.UPC, Departments.DepartmentName, Products.Instock FROM Products INNER JOIN Departments ON Products.Department = Departments.ID"; // the sql of the command, this one joins the products and department tables using the department id to get the name of the dapartment.
                OleDbDataAdapter adap = new OleDbDataAdapter(getInventory);
                DataSet ds = new DataSet();
                adap.Fill(ds); //fills the datasource using the oledbcommand
                getInventory.Dispose();
                connection.Close();
                inventoryBindingSource.DataSource = ds.Tables[0].DefaultView; // sets the datasource to a binding source so it can be filtered
                dgvInventory.DataSource = inventoryBindingSource; //connects the datagridview to the datasource
                dgvInventory.Columns[0].HeaderText = "Product Name";//set column 1 
                                                                    //Keep column two header as UPC
                dgvInventory.Columns[2].HeaderText = "Department";//set column 3 
                dgvInventory.Columns[3].HeaderText = "Amount Instock";//set column 4
            }
            catch (Exception)
            {
                MessageBox.Show(error);
                return 1;
            }
            return 0;
        }

        //fills the product table
        public int fillProductTable(string error)
        {
            try
            {
                connection.Open(); // connects to the database
                OleDbCommand getInventory = new OleDbCommand(); // sets up a new command
                getInventory.Connection = connection; // sets the connection of the command
                getInventory.CommandText = "SELECT Products.ProductName, Products.UPC, Departments.DepartmentName, Products.SellPrice, Products.BuyCost, Products.Instock FROM Products INNER JOIN Departments ON Products.Department = Departments.ID"; // the sql of the command, this one joins the products and department tables using the department id to get the name of the dapartment.
                OleDbDataAdapter adap = new OleDbDataAdapter(getInventory);
                DataSet ds = new DataSet();
                adap.Fill(ds); //fills the datasource using the oledbcommand
                getInventory.Dispose();
                connection.Close();
                productBindingSource.DataSource = ds.Tables[0].DefaultView;// sets the datasource to a binding source so it can be filtered
                dgvProducts.DataSource = productBindingSource; //connects the datagridview to the datasource
                dgvProducts.Columns[0].HeaderText = "Product Name";//set column 1 
                                                                    //Keep column two header as UPC
                dgvProducts.Columns[2].HeaderText = "Department";//set column 3 
                dgvProducts.Columns[3].HeaderText = "Sell Price";//set column 4
                dgvProducts.Columns[4].HeaderText = "Buy Cost";//set column 5
                dgvProducts.Columns[5].Visible = false;          
                dgvProducts.Columns[3].DefaultCellStyle.Format = "C2";
                dgvProducts.Columns[4].DefaultCellStyle.Format = "C2";

            }
            catch (Exception)
            {
                MessageBox.Show(error);
                return 1;
            }
            return 0;
        }

        //fills the dropdown box filter for departments from the database
        public void setupDepartmentFilter()
        {
            try
            {
                connection.Open(); // connects to the database
                OleDbCommand getDepartments = new OleDbCommand();
                getDepartments.Connection = connection;
                getDepartments.CommandText = "SELECT DepartmentName,ID FROM Departments";
                OleDbDataAdapter adap = new OleDbDataAdapter(getDepartments);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                getDepartments.Dispose();
                connection.Close();
                departmentBindingSource.DataSource = ds.Tables[0].DefaultView;
                cbxDepartment.DataSource = departmentBindingSource;    
                cbxDepartment.DisplayMember = "DepartmentName";
                cbxDepartment.ValueMember = "DepartmentName";
                DataRow newRow = ds.Tables[0].NewRow();
                newRow[0] = "<No Filter>";
                newRow[1] = 0;
                ds.Tables[0].Rows.InsertAt(newRow,0);
            }
            catch (Exception)
            {
                MessageBox.Show("Department data could not be found");
            }
        }


        //function that allows the user to change which database they are connected to
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
                    if(fillAllTables("New Database does not have the required fields for Inventory", "New Database does not have the required fields for Products") == 0)
                    {
                        Properties.Settings.Default.Database = connectionString;
                        MessageBox.Show("New Connection Successful");
                    }
                    else
                    {
                        connection.Close();
                        MessageBox.Show("New database connection failed");
                        connection.ConnectionString = defaultConnection;
                    }    
                }
            }
            catch (Exception)
            {
                connection.Close();
                MessageBox.Show("New database connection failed");
                connection.ConnectionString = defaultConnection;
                fillAllTables("Inventory data could not be recieved", "Product data could not be recieved");
            }
        }

        //function for when all tables need to be refreshed
        private int fillAllTables(string msg1, string msg2)
        {
            int success = 0;
            success = fillInventoryTable(msg1);
            success = fillProductTable(msg2);
            setupDepartmentFilter();
            return success;
        }


//functions that are used for filtering the inventory table

        //handles filtering on the inventory tab dropdown filter
        private void cbxDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            handleInventoryFiltering();
        }

        //handles the in stock checkbox filter
        private void chkStock_CheckedChanged(object sender, EventArgs e)
        {
            handleInventoryFiltering();
        }

        //search box for product name
        private void txtSearchName_TextChanged(object sender, EventArgs e)
        {
            handleInventoryFiltering();
        }
        //search box for upc
        private void txtSearchUPC_TextChanged(object sender, EventArgs e)
        {
            handleInventoryFiltering();
        }

        //adds to an existing filter so that compound filters work;
        private static String addToFilter(String newParam, String oldFilter)
        {
            if (String.IsNullOrEmpty(oldFilter))
            {
                return newParam;
            }

            return "("+ oldFilter + ") AND (" + newParam + ")";
        }


        //function to handle all the filters on the inventory, allows for combo filters
        private void handleInventoryFiltering()
        {
            // removes existing filter, then rebuilds the filter 
            inventoryBindingSource.RemoveFilter();
            dgvInventory.ClearSelection();
            if (cbxDepartment.SelectedIndex > 0)
            {
                inventoryBindingSource.Filter = addToFilter("DepartmentName = '" + cbxDepartment.SelectedValue.ToString() + "'", inventoryBindingSource.Filter);
            }
            if(chkStock.Checked == true)
            {
                inventoryBindingSource.Filter = addToFilter("Instock = 0", inventoryBindingSource.Filter);
            }
            else
            {
                inventoryBindingSource.Filter = addToFilter("Instock >= 0", inventoryBindingSource.Filter);
            }
            if(txtSearchName.Text.Trim() != "")
            {
                inventoryBindingSource.Filter = addToFilter("ProductName LIKE '" + txtSearchName.Text.Trim() + "*'", inventoryBindingSource.Filter);
            }
            if (txtSearchUPC.Text.Trim() != "")
            {
                inventoryBindingSource.Filter = addToFilter("UPC LIKE '" + txtSearchUPC.Text.Trim() + "*'", inventoryBindingSource.Filter);
            }
            dgvInventory.Refresh();
        }

        //function to handle all the filters on the inventory, allows for combo filters
        private void handleProductFiltering()
        {
            // removes existing filter, then rebuilds the filter 
            productBindingSource.RemoveFilter();
            dgvProducts.ClearSelection();
            if (cbxDepartment.SelectedIndex > 0)
            {
                productBindingSource.Filter = addToFilter("DepartmentName = '" + cbxProductDepartment.SelectedValue.ToString() + "'", productBindingSource.Filter);
            }
            if (txtSearchName.Text.Trim() != "")
            {
                productBindingSource.Filter = addToFilter("ProductName LIKE '" + txtSearchProductName.Text.Trim() + "*'", productBindingSource.Filter);
            }
            if (txtSearchUPC.Text.Trim() != "")
            {
                productBindingSource.Filter = addToFilter("UPC LIKE '" + txtSearchProductUPC.Text.Trim() + "*'", productBindingSource.Filter);
            }
            dgvProducts.Refresh();
        }


        //displays the data from the datagridview on the right and sets up data for it to be added to the order form
        private void dgvInventory_SelectionChanged(object sender, EventArgs e)
        {
           
                try
                {
           
                    lblName.Text = dgvInventory.SelectedRows[0].Cells[0].Value.ToString();
                    lblUPC.Text = dgvInventory.SelectedRows[0].Cells[1].Value.ToString();
                    lblDepartment.Text = dgvInventory.SelectedRows[0].Cells[2].Value.ToString();
                    lblInstock.Text = dgvInventory.SelectedRows[0].Cells[3].Value.ToString();
                }
                catch (Exception)
                {

                    lblName.Text = " ";
                    lblUPC.Text = " ";
                    lblDepartment.Text = " ";
                    lblInstock.Text = " ";
                }
        }

        //adds the selected inventory item to the order form
        private void btnOrder_Click(object sender, EventArgs e)
        {

        }

        //fills the product labels on the right when the selected cell changes
        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            try
            {

                lblProductName.Text = dgvProducts.SelectedRows[0].Cells[0].Value.ToString();
                lblProductUPC.Text = dgvProducts.SelectedRows[0].Cells[1].Value.ToString();
                lblProductDepartment.Text = dgvProducts.SelectedRows[0].Cells[2].Value.ToString();
                lblSell.Text = dgvProducts.SelectedRows[0].Cells[3].Value.ToString();
                lblBuy.Text = dgvProducts.SelectedRows[0].Cells[3].Value.ToString();
            }
            catch (Exception)
            {

                lblProductName.Text = " ";
                lblProductUPC.Text = " ";
                lblProductDepartment.Text = " ";
                lblSell.Text = " ";
                lblBuy.Text = " ";
            }
        }
        //makes the product filtering function be called when the filter fields are changed
        private void txtSearchProductName_TextChanged(object sender, EventArgs e)
        {
            handleProductFiltering();
        }

        private void txtSearchProductUPC_TextChanged(object sender, EventArgs e)
        {
            handleProductFiltering();
        }

        private void cbxProductDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            handleProductFiltering();
        }
    }
}
