using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using static System.Windows.Forms.AxHost;

namespace WindowsApp1
{
    public partial class Salaries : Form
    {

        private MongoDBHelper dBHelper;
        private const string EmployeeCollection = "employees";
        private const string DepartmentCollection = "department";
        private const string SalaryCollection = "salaries";
        private string Key;

        //Functions Con;



        public Salaries()
        {
            InitializeComponent();
            dBHelper = new MongoDBHelper("projekat1");

            //Con = new Functions();

            SalaryList.DataBindingComplete += EmpList_DataBindingComplete;
            ShowSalary();
            GetEmployees();
            DaysTb.TextChanged += DaysTb_TextChanged;

        }


        private void EmpList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (SalaryList.Columns.Count > 0)
            {
                SalaryList.Columns[0].Width = 57;
                SalaryList.Columns[1].Width = 57;
            }
        }



        private void GetEmployees()
        {
            try
            {
                var employees = dBHelper.GetData(EmployeeCollection);

                var employeeDict = new Dictionary<string, ObjectId>();

                foreach (var dep in employees)
                {
                    employeeDict.Add(dep.GetValue("EmpName").AsString, dep.GetValue("_id").AsObjectId);
                }

                EmpCb.DataSource = new BindingSource(employeeDict, null);
                EmpCb.DisplayMember = "Key";
                EmpCb.ValueMember = "Value";


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }


            /*
            string Query = "Select * from EmployeeTbl";
            EmpCb.DisplayMember = Con.GetData(Query).Columns["EmpName"].ToString();
            EmpCb.ValueMember = Con.GetData(Query).Columns["EmpId"].ToString();
            EmpCb.DataSource = Con.GetData(Query);
            */
        }






        int DSal = 0;
        String Period = "";
         
        private void GetSalary()
        {

            try
            {
                var salaries = dBHelper.GetData(SalaryCollection);
                

                var empId = EmpCb.SelectedValue.ToString();
                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(empId));
                var projection = Builders<BsonDocument>.Projection.Include("EmpSal");

                var employee = dBHelper.GetDataS(EmployeeCollection, filter, projection).FirstOrDefault();

                if (employee != null)
                {
                    DSal = employee.GetValue("EmpSal").AsInt32;

                    if (string.IsNullOrEmpty(DaysTb.Text))
                    {
                        AmountTb.Text = (d * DSal) + " RSD";
                    }
                    else
                    {
                        d = Convert.ToInt32(DaysTb.Text);
                        AmountTb.Text = (d * DSal) + " RSD";
                    }
                }
                else
                {
                    MessageBox.Show("Employee not found or salary data missing!");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}");
            }









            /*
            string Query = "Select EmpSal from EmployeeTbl where EmpId = {0}";
            Query = string.Format(Query,EmpCb.SelectedValue.ToString());

            foreach(DataRow dr in Con.GetData(Query).Rows)
            {
                DSal = Convert.ToInt32(dr["EmpSal"].ToString());
            }


            if(DaysTb.Text == "")
            {
                AmountTb.Text = (d * DSal) + " RSD";
            }
            else
            {
                d = Convert.ToInt32(DaysTb.Text);
                AmountTb.Text = (d * DSal) + " RSD";
            }

            */

            //MessageBox.Show("" + DSal);
            //EmpCb.DataSource = Con.GetData(Query);

        }


        private void ShowSalary()
        {
            try
            {
                var salaries = dBHelper.GetData(SalaryCollection);
                DataTable dt = new DataTable();

                dt.Columns.Add("SalId", typeof(string));
                dt.Columns.Add("EmpId", typeof(string));
                dt.Columns.Add("EmpName", typeof(string));
                dt.Columns.Add("Days", typeof(int));
                dt.Columns.Add("Period", typeof(string));
                dt.Columns.Add("Amount", typeof(int));
                dt.Columns.Add("Date", typeof(DateTime));

                foreach (var sal in salaries)
                {
                    dt.Rows.Add(
                        sal.GetValue("_id").AsObjectId,
                        sal.GetValue("EmpId").AsObjectId,
                        sal.GetValue("EmpName").AsString,
                        sal.GetValue("Days").ToInt32(),
                        sal.GetValue("Period").AsString,
                        sal.GetValue("Amount").ToInt32(),
                        sal.GetValue("Date").ToUniversalTime()
                    );
                }

                SalaryList.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching salaries: {ex.Message}");
            }




            /*
            try
            {
                String Query = "Select * from SalaryTbl";
                SalaryList.DataSource = Con.GetData(Query);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            */
        }

        int d = 1;
        private void AddBtn_Click(object sender, EventArgs e)
        {

            try
            {
                if (EmpCb.SelectedIndex == -1 || DaysTb.Text == "" || PeriodTb.Text == "")
                {
                    MessageBox.Show("Missing Data!!!");
                }
                else if (Convert.ToInt32(DaysTb.Text) > 31)
                {
                    MessageBox.Show("Days cannot be greater than 31!!!");
                }
                else
                {
                    Period = PeriodTb.Value.Date.Month.ToString() + "-" + PeriodTb.Value.Date.Year.ToString();
                    int Amount = DSal * Convert.ToInt32(DaysTb.Text);
                    int Days = Convert.ToInt32(DaysTb.Text);

                    var document = new BsonDocument
                    {
                        { "EmpId", (ObjectId)EmpCb.SelectedValue },
                        { "EmpName", EmpCb.Text },
                        { "Days", Days },
                        { "Period", Period },
                        { "Amount", Amount },
                        { "Date", DateTime.Now }
                    };

                    dBHelper.InsertData(SalaryCollection, document);
                    ShowSalary();

                    MessageBox.Show("Salary Paid!!!");
                    DaysTb.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }






            /*
            try
            {
                if (EmpCb.SelectedIndex == -1 || DaysTb.Text == "" || PeriodTb.Text == "")
                {
                    MessageBox.Show("Missing Data!!!");
                }
                else if (Convert.ToInt32(DaysTb.Text) > 31)
                {
                    MessageBox.Show("Days can not be greater then 31 !!!");
                }
                else
                {
                    Period = PeriodTb.Value.Date.Month.ToString() + "-" + PeriodTb.Value.Date.Year.ToString();
                    int Amount = DSal * Convert.ToInt32(DaysTb.Text);


                    int Days = Convert.ToInt32(DaysTb.Text);
                    string Query = "insert into SalaryTbl values ({0},{1},'{2}',{3},'{4}')";
                    Query = string.Format(Query, EmpCb.SelectedValue.ToString(), Days, Period, Amount, DateTime.Today.Date);
                    Con.SetData(Query);
                    ShowSalary();

                    MessageBox.Show("Salary Paid!!!");
                    DaysTb.Text = "";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */



        }



        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Key == null)
                {
                    MessageBox.Show("Select a record to delete!");
                }
                else
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(Key));
                    dBHelper.DeleteData(SalaryCollection, filter);
                    ShowSalary();

                    MessageBox.Show("Salary Deleted!!!");
                    DaysTb.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void EmpCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            GetSalary();
        }





        private void SalaryList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = SalaryList.Rows[e.RowIndex];

                EmpCb.SelectedValue = ObjectId.Parse(selectedRow.Cells["EmpId"].Value.ToString());
                DaysTb.Text = selectedRow.Cells["Days"].Value.ToString();
                PeriodTb.Text = selectedRow.Cells["Period"].Value.ToString();
                AmountTb.Text = selectedRow.Cells["Amount"].Value.ToString();

                Key = selectedRow.Cells["SalId"].Value.ToString();
            }
        }



        private void DaysTb_TextChanged(object sender, EventArgs e)
        {
            GetSalary();
        }



















        private void label9_Click(object sender, EventArgs e)
        {
            Departments Obj = new Departments();
            Obj.Show();
            this.Hide();
        }

        private void label10_Click(object sender, EventArgs e)
        {
            Employees Ojb = new Employees();
            Ojb.Show();
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
