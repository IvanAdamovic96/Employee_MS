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
    public partial class Employees : Form
    {
        private MongoDBHelper dBHelper;
        private const string EmployeeCollection = "employees";
        private const string DepartmentCollection = "department";

        Functions Con;


        public Employees()
        {
            InitializeComponent();

            dBHelper = new MongoDBHelper("projekat1");

            //Con = new Functions();
            EmpList.DataBindingComplete += EmpList_DataBindingComplete;
            ShowEmp();
            GetDepartment();
            searchTb.TextChanged += SearchTb_TextChanged;
        }


        //width of the first column
        private void EmpList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (EmpList.Columns.Count > 0)
            {
                EmpList.Columns[0].Width = 57; 
            }
        }


        //Get Data
        private void ShowEmp()
        {
            try
            {
                var employees = dBHelper.GetData(EmployeeCollection);
                /*var bindingList = new BindingList<BsonDocument>(employees);
                var source = new BindingSource(bindingList, null);
                EmpList.DataSource = source;*/

                DataTable dt = new DataTable();


                dt.Columns.Add("EmpId", typeof(string)); 
                dt.Columns.Add("EmpName", typeof(string));
                dt.Columns.Add("EmpGen", typeof(string));
                dt.Columns.Add("EmpDep", typeof(string)); 
                dt.Columns.Add("EmpDOB", typeof(string)); 
                dt.Columns.Add("EmpDate", typeof(string)); 
                dt.Columns.Add("EmpSal", typeof(int)); 

                // Populate the DataTable with MongoDB data
                foreach (var emp in employees)
                {
                    dt.Rows.Add(
                        emp.GetValue("_id").AsObjectId.ToString(), //  AsObjectId.GetHashCode()
                        emp.GetValue("EmpName").AsString,
                        emp.GetValue("EmpGen").AsString,
                        emp.GetValue("EmpDep").AsString,
                        emp.GetValue("EmpDOB").AsString, 
                        emp.GetValue("EmpDate").AsString, 
                        emp.GetValue("EmpSal").AsInt32
                    );
                }

                // Bind DataTable to DataGridView
                EmpList.DataSource = dt;


            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}");
            }




            /*
            String Query = "Select * from EmployeeTbl";
            EmpList.DataSource = Con.GetData(Query);
            */




        }



        //Get Departments
        private void GetDepartment()
        {

            try
            {
                var departments = dBHelper.GetData(DepartmentCollection);

                var departmentDict = new Dictionary<string, ObjectId>();

                foreach(var dep in departments)
                {
                    departmentDict.Add(dep.GetValue("DepName").AsString, dep.GetValue("_id").AsObjectId);
                }

                DepCb.DataSource = new BindingSource(departmentDict, null);
                DepCb.DisplayMember = "Key";
                DepCb.ValueMember = "Value";



                
            }catch(Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }




            /*
            string Query = "Select * from DepartmentTbl";
            DepCb.DisplayMember = Con.GetData(Query).Columns["DepName"].ToString();
            DepCb.ValueMember = Con.GetData(Query).Columns["DepId"].ToString();
            DepCb.DataSource = Con.GetData(Query);
            */
        }



        //Insert Data
        private void AddBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmpNameTb.Text == "" || GenCb.SelectedIndex == -1 || DepCb.SelectedIndex == -1 || DailySalTb.Text == "")
                {
                    MessageBox.Show("Missing Data !!!");
                }
                else
                {
                    var document = new BsonDocument
                    {
                        {"EmpName", EmpNameTb.Text },
                        {"EmpGen", GenCb.SelectedItem.ToString() },
                        {"EmpDep", DepCb.Text },
                        {"EmpDOB", DOBTb.Value.ToString("yyyy-MM-dd") },
                        {"EmpDate", JDateTb.Value.ToString("yyyy-MM-dd") },
                        {"EmpSal", Convert.ToInt32(DailySalTb.Text) },
                    };

                    dBHelper.InsertData(EmployeeCollection, document);
                    MessageBox.Show("Employee Added!!!");
                    // Refresh the DataGridView
                    ShowEmp();


                    /*
                    string Name = EmpNameTb.Text;
                    string Gender = GenCb.SelectedItem.ToString();
                    int Dep = Convert.ToInt32(DepCb.SelectedValue.ToString());
                    string DOB = DOBTb.Value.ToString();
                    string JDate = JDateTb.Value.ToString();
                    int Salary = Convert.ToInt32(DailySalTb.Text);

                    
                    string Query = "insert into EmployeeTbl values ('{0}','{1}',{2},'{3}','{4}',{5})";
                    Query = string.Format(Query,Name,Gender,Dep,DOB,JDate,Salary);
                    Con.SetData(Query);
                    MessageBox.Show("Employee Added!!!");
                    ShowEmp();
                    */



                    // Clear form fields
                    EmpNameTb.Text = "";
                    GenCb.SelectedIndex = -1;
                    DepCb.SelectedIndex = -1;
                    DailySalTb.Text = "";

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        //Update Data
        private void EditBtn_Click(object sender, EventArgs e)
        {

            try
            {
                if (EmpNameTb.Text == "" || GenCb.SelectedIndex == -1 || DepCb.SelectedIndex == -1 || DailySalTb.Text == "")
                {
                    MessageBox.Show("Missing Data !!!");
                }
                else
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(Key));

                    // Prepare update with new values
                    var update = Builders<BsonDocument>.Update
                        .Set("EmpName", EmpNameTb.Text)
                        .Set("EmpGen", GenCb.SelectedItem.ToString())
                        .Set("EmpDep", DepCb.Text)
                        //.Set("EmpDep", Convert.ToInt32(DepCb.SelectedValue))
                        .Set("EmpDOB", DOBTb.Value.ToString("yyyy-MM-dd"))
                        .Set("EmpDate", JDateTb.Value.ToString("yyyy-MM-dd"))
                        .Set("EmpSal", Convert.ToInt32(DailySalTb.Text));

                    // Call MongoDBHelper to update the document
                    dBHelper.UpdateData(EmployeeCollection, filter, update);

                    MessageBox.Show("Employee Updated!!!");

                    // Refresh the DataGridView with updated data
                    ShowEmp();




                    // Clear form fields
                    EmpNameTb.Text = "";
                    GenCb.SelectedIndex = -1;
                    DepCb.SelectedIndex = -1;
                    DailySalTb.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing employee: {ex.Message}");
            }







            /*
            try
            {
                if (EmpNameTb.Text == "" || GenCb.SelectedIndex == -1 || DepCb.SelectedIndex == -1 || DailySalTb.Text == "")
                {
                    MessageBox.Show("Missing Data !!!");
                }
                else
                {
                    string Name = EmpNameTb.Text;
                    string Gender = GenCb.SelectedItem.ToString();
                    int Dep = Convert.ToInt32(DepCb.SelectedValue.ToString());
                    string DOB = DOBTb.Value.ToString();
                    string JDate = JDateTb.Value.ToString();
                    int Salary = Convert.ToInt32(DailySalTb.Text);


                    string Query = "update EmployeeTbl set EmpName = '{0}', EmpGen = '{1}', EmpDep = {2}, EmpDOB = '{3}', EmpDate = '{4}', EmpSal = {5} where EmpId = {6}";
                    Query = string.Format(Query, Name, Gender, Dep, DOB, JDate, Salary,Key);
                    Con.SetData(Query);
                    MessageBox.Show("Employee Updated!!!");
                    ShowEmp();
                    EmpNameTb.Text = "";
                    GenCb.SelectedIndex = -1;
                    DepCb.SelectedIndex = -1;
                    DailySalTb.Text = "";

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */
        }




        string Key = "";
        private void EmpList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = EmpList.Rows[e.RowIndex];

                EmpNameTb.Text = selectedRow.Cells["EmpName"].Value.ToString();
                GenCb.Text = selectedRow.Cells["EmpGen"].Value.ToString();
                DepCb.Text = selectedRow.Cells["EmpDep"].Value.ToString();




                if (DateTime.TryParseExact(selectedRow.Cells["EmpDOB"].Value?.ToString(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dob))
                {
                    DOBTb.Value = dob;
                }
                else
                {
                    // Handle if parsing fails or format is incorrect
                    MessageBox.Show("Invalid date format for DOB.");
                }

                if (DateTime.TryParseExact(selectedRow.Cells["EmpDate"].Value?.ToString(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime jDate))
                {
                    JDateTb.Value = jDate;
                }
                else
                {
                    // Handle if parsing fails or format is incorrect
                    MessageBox.Show("Invalid date format for DOB.");
                }

                
                DailySalTb.Text = selectedRow.Cells["EmpSal"].Value.ToString();


                /*
                if (DateTime.TryParse(selectedRow.Cells["EmpDOB"].Value.ToString(), out DateTime dob))
                {
                    DOBTb.Value = dob;
                }
                else
                {
                    // Handle if parsing fails (optional)
                    MessageBox.Show("Invalid date format for DOB.");
                }

                if (DateTime.TryParse(selectedRow.Cells["EmpDate"].Value.ToString(), out DateTime jDate))
                {
                    JDateTb.Value = jDate;
                }
                else
                {
                    // Handle if parsing fails (optional)
                    MessageBox.Show("Invalid date format for Joining Date.");
                }

                */




                /*
                 DOBTb.Value = DateTime.ParseExact(selectedRow.Cells["EmpDOB"].Value?.ToString(), "yyyy-MM-dd", null);
                JDateTb.Value = DateTime.ParseExact(selectedRow.Cells["EmpDate"].Value?.ToString(), "yyyy-MM-dd", null);
                 */

                //DOBTb.Text = selectedRow.Cells[4].Value.ToString();
                //JDateTb.Text = selectedRow.Cells[5].Value.ToString();

                //string Salary = Convert.ToString(selectedRow.Cells[6].Value);
                //DailySalTb.Text = Salary;



                Key = selectedRow.Cells["EmpId"].Value.ToString();
                //Key = Convert.ToInt32(selectedRow.Cells["EmpId"].Value.ToString());
                //MessageBox.Show(Key.ToString());
            }
        }



        //Delete Data
        private void DeleteBtn_Click(object sender, EventArgs e)
        {


            try
            {

                if(Key == "")
                {
                    MessageBox.Show("Missing data!!!");
                }
                else
                {
                    var confirmation = MessageBox.Show("Are you sure you want to delete this employee?", "Delete Confirmation", MessageBoxButtons.YesNo);

                    if (confirmation == DialogResult.Yes)
                    {
                        var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(Key));
                        //var filter = Builders<BsonDocument>.Filter.Eq("_id", Key);

                        dBHelper.DeleteData(EmployeeCollection, filter);

                        MessageBox.Show("Employee Deleted!!!");

                        // Refresh the DataGridView with updated data
                        ShowEmp();

                        // Clear form fields
                        EmpNameTb.Text = "";
                        GenCb.SelectedIndex = -1;
                        DepCb.SelectedIndex = -1;
                        DailySalTb.Text = "";

                    }


                }


            } catch (Exception ex)
            {
                MessageBox.Show($"Error deleting employee: {ex.Message}");
            }









            /*
            try
            {
                if (Key == "")
                {
                    MessageBox.Show("Missing Data !!!");
                }
                else
                {

                    string Query = "delete from EmployeeTbl where EmpId = {0}";
                    Query = string.Format(Query,Key);
                    Con.SetData(Query);
                    MessageBox.Show("Employee Deleted!!!");
                    ShowEmp();
                    EmpNameTb.Text = "";
                    GenCb.SelectedIndex = -1;
                    DepCb.SelectedIndex = -1;
                    DailySalTb.Text = "";

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */




        }



        //Search 
        private void SearchEmployee(string searchTerm = "")
        {
            try
            {
                var filter = string.IsNullOrEmpty(searchTerm)
                    ? new BsonDocument()
                    : Builders<BsonDocument>.Filter.Regex("EmpName", new BsonRegularExpression(searchTerm, "i"));

                var employees = dBHelper.SearchData(EmployeeCollection, filter);

                DataTable dt = new DataTable();
                dt.Columns.Add("EmpId", typeof(string));
                dt.Columns.Add("EmpName", typeof(string));
                dt.Columns.Add("EmpGen", typeof(string));
                dt.Columns.Add("EmpDep", typeof(string));
                dt.Columns.Add("EmpDOB", typeof(string));
                dt.Columns.Add("EmpDate", typeof(string));
                dt.Columns.Add("EmpSal", typeof(int));

                foreach (var emp in employees)
                {
                    dt.Rows.Add(
                        emp.GetValue("_id").AsObjectId.ToString(),
                        emp.GetValue("EmpName").AsString,
                        emp.GetValue("EmpGen").AsString,
                        emp.GetValue("EmpDep").AsString,
                        emp.GetValue("EmpDOB").AsString,
                        emp.GetValue("EmpDate").AsString,
                        emp.GetValue("EmpSal").AsInt32
                    );
                }

                EmpList.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching employees: {ex.Message}");
            }
        }
        private void SearchTb_TextChanged(object sender, EventArgs e)
        {
            SearchEmployee(searchTb.Text);
        }











        private void label11_Click(object sender, EventArgs e)
        {
            Departments Obj = new Departments();
            Obj.Show();
            this.Hide();
        }

        private void label10_Click(object sender, EventArgs e)
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
            if(WindowState == FormWindowState.Normal)
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
