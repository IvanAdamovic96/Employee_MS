using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WindowsApp1
{
    public partial class Departments : Form
    {
        private MongoDBHelper dBHelper;
        private const string DepartmentCollection = "department";
        //Functions Con;
        private string Key;
        
        



        public Departments()
        {
            InitializeComponent();
            dBHelper = new MongoDBHelper("projekat1");

            //Con = new Functions();


            DepList.DataBindingComplete += DepList_DataBindingComplete;
            ShowDepartments();
        }




        //Get Data
        private void ShowDepartments()
        {

            try
            {


                var departments = dBHelper.GetData(DepartmentCollection);
                

                DataTable dt = new DataTable();


                dt.Columns.Add("DepId", typeof(string));
                dt.Columns.Add("DepName", typeof(string));
                

                // Populate the DataTable with MongoDB data
                foreach (var dep in departments)
                {
                    dt.Rows.Add(
                        dep.GetValue("_id").AsObjectId.ToString(), 
                        dep.GetValue("DepName").AsString
                    );
                }

                
                DepList.DataSource = dt;


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}");
            }
            



            /*
            String Query = "Select * from DepartmentTbl";
            DepList.DataSource = Con.GetData(Query);
            */

        }


        //Width of the first column
        private void DepList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (DepList.Columns.Count > 0)
            {
                DepList.Columns[0].Width = 100;
            }
        }



        //Insert Data
        private void AddBtn_Click(object sender, EventArgs e)
        {

            try
            {
                if(DepNameTb.Text == "")
                {
                    MessageBox.Show("Missing data for Department Name !");
                }
                else
                {
                    


                    var document = new BsonDocument
                    {
                        {"DepName", DepNameTb.Text }
                    };

                    dBHelper.InsertData(DepartmentCollection, document);
                    MessageBox.Show("Department Added!");
                    // Refresh the DataGridView
                    ShowDepartments();

                    //Clear form field
                    DepNameTb.Text = "";


                    
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            





            /*
            try
            {
                if(DepNameTb.Text == "")
                {
                    MessageBox.Show("Missing Data !!!");
                }
                else
                {
                    string Dep = DepNameTb.Text;
                    string Query = "insert into DepartmentTbl values ('{0}')";
                    Query = string.Format(Query,DepNameTb.Text);
                    Con.SetData(Query);
                    MessageBox.Show("Department Added!!!");
                    ShowDepartments();
                    DepNameTb.Text = "";
                }

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */
        }   



        //Update Data
        private void EditBtn_Click(object sender, EventArgs e)
        {

            try
            {
                if (DepNameTb.Text == "")
                {
                    MessageBox.Show("Missing Data !!!");
                }
                else
                {
                    if(ObjectId.TryParse(Key, out ObjectId objectId))
                    {
                        var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

                        // Prepare update with new values
                        var update = Builders<BsonDocument>.Update
                            .Set("DepName", DepNameTb.Text);


                        // Call MongoDBHelper to update the document
                        dBHelper.UpdateData(DepartmentCollection, filter, update);
                        
                        MessageBox.Show("Employee Updated!!!");

                        // Refresh the DataGridView with updated data
                        ShowDepartments();




                        // Clear form fields
                        DepNameTb.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Invalid ObjectId format!!!");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing employee: {ex.Message}");
            }








            /*
            try
            {
                if (DepNameTb.Text == "")
                {
                    MessageBox.Show("Missing Data !!!");
                }
                else
                {
                    string Dep = DepNameTb.Text;
                    string Query = "Update DepartmentTbl set DepName = '{0}' where DepId = {1}";
                    Query = string.Format(Query,DepNameTb.Text,Key);
                    Con.SetData(Query);
                    MessageBox.Show("Department Updated!!!");
                    ShowDepartments();
                    DepNameTb.Text = "";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */
        }

        //int Key = 0;

        private void DepList_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

            if(e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = DepList.Rows[e.RowIndex];

                DepNameTb.Text = selectedRow.Cells[1].Value.ToString();


                Key = selectedRow.Cells["DepId"].Value.ToString() ;

                //Key = ObjectId.Parse(selectedRow.Cells["DepId"].Value.ToString());
                //Key = Convert.ToInt32(selectedRow.Cells[0].Value.ToString());
            }



            //if (DepList.SelectedRows.Count > 0)
            //{
            //    DataGridViewCell cell = DepList.SelectedRows[0].Cells[1];
            //    if (cell.Value != null)
            //    {
            //        DepNameTb.Text = cell.Value.ToString();
            //    }
            //}
            //DataGridViewRow selectedRow = DepList.Rows[e.RowIndex];

            //DepNameTb.Text = selectedRow.Cells[1].Value.ToString();

            //if (DepNameTb.Text == "")
            //{
            //    Key = 0;
            //}
            //else
            //{
            //    Key = Convert.ToInt32(DepList.SelectedRows[0].Cells[1].Value.ToString());
            //}
        }



        //Delete Data
        private void DeleteBtn_Click(object sender, EventArgs e)
        {

            try
            {
                if(DepNameTb.Text == "")
                {
                    MessageBox.Show("Missing data!!!");
                }
                else
                {

                    if(ObjectId.TryParse(Key, out ObjectId objectId))
                    {

                        var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

                        dBHelper.DeleteData(DepartmentCollection, filter);
                        MessageBox.Show("Department deleted!");
                        ShowDepartments();
                        DepNameTb.Text = "";


                    }
                    else
                    {
                        MessageBox.Show("Invalid ObjectId format!!!");
                    }



                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }








            /*
            try
            {
                if (DepNameTb.Text == "")
                {
                    MessageBox.Show("Missing Data !!!");
                }
                else
                {
                    string Dep = DepNameTb.Text;
                    string Query = "Delete from DepartmentTbl where DepId = {0}";
                    Query = string.Format(Query,Key);
                    Con.SetData(Query);
                    MessageBox.Show("Department Deleted!!!");
                    ShowDepartments();
                    DepNameTb.Text = "";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */

        }















        private void EmpLbl_Click(object sender, EventArgs e)
        {
            Employees Obj = new Employees();
            Obj.Show();
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Salaries Obj = new Salaries();
            Obj.Show();
            this.Hide();
        }

        private void LogoutLbl_Click(object sender, EventArgs e)
        {
            Login Obj = new Login();
            Obj.Show();
            this.Hide();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;

            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        
    }
}
