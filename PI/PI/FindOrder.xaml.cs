using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace PI
{
    /// <summary>
    /// Клас FindOrder, створений для взаємодії з FindOrder.xaml
    /// </summary>
    public partial class FindOrder : Page
    {
        public FindOrder()
        {
            InitializeComponent();
        }
        public FindOrder(string Login)
        {
            InitializeComponent();
            this.Login = Login;
        }
        public string Login { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CountAdult.SelectedIndex = 0;
            CountChild.SelectedIndex = 0;
            CountInfant.SelectedIndex = 0;
            DepartTaxi.Items.Clear();
            ArrivalTaxi.Items.Clear();
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DataBaseTaxiOperator;Integrated Security=True";
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT DISTINCT City FROM AddOrder Order by City";
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        DepartTaxi.Items.Add(dr["City"]);
                        ArrivalTaxi.Items.Add(dr["City"]);
                    }
                    dr.Close();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// SearchTaxi_Click витягує шукані поїздки з БД.
        /// </summary>
        /// <param name="sender">Об'єкт</param>
        /// <param name="e">Маршрутизована подія</param>

        private void SearchTaxi_Click(object sender, RoutedEventArgs e)
        {
            if (DepartTaxi.Text != "" && ArrivalTaxi.Text != "" && DatePicker.Text != "")
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;
                    string query = $"SELECT Id,DepartCity,ArriveCity,convert(varchar(10),DepartDate,104) as 'DepartDate'," +
                        $"convert(varchar(10),ArriveDate,104) as 'ArrivalDate',CAST(DepartTime AS CHAR(5)) as 'DepartTime',CAST(ArriveTime AS CHAR(5)) as 'ArriveTime',TAXIXID as 'TaxiID',DriverName as 'Phone number' FROM InfoOrder " +
                        $"where DepartCity = '{DepartTaxi.Text}' AND ArriveCity = '{ArrivalTaxi.Text}' and convert(varchar(10),DepartDate,104) = '{Convert.ToDateTime(DatePicker.Text).ToString("dd.MM.yyyy")}'";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        dataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
            else
            {
                MessageBox.Show("Заповніть всі поля");
            }
        }

        
    }
}
