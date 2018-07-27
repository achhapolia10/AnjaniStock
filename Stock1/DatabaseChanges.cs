using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  System.Data;
using System.Data.SqlClient;


namespace Stock1
{
    class DatabaseChanges
    {
        private SqlConnection connection;
        public DatabaseChanges()
        {
            this.connection =new SqlConnection(Properties.Settings.Default.Database1ConnectionString1);
        }
        public DataSet getEntriesByDay(String date)
        {
            connection.Open();
            SqlDataAdapter adapter;
            DataSet dataSet = new DataSet() ;
            string command = "Select * from dbo.DAILYENTRYTABLE where Date='" + date + "';";
            adapter = new SqlDataAdapter(command, connection);
            
            adapter.Fill(dataSet);
            connection.Close();
            return dataSet;
        }
        public string[] getProductList()
        {
            connection.Open(); 
            List<string> products = new List<String>();
            SqlCommand cm = new SqlCommand("Select * from dbo.Product",connection);
            try
            {
                SqlDataReader dataReader = cm.ExecuteReader();
                while (dataReader.Read())
                {
                    products.Add(dataReader.GetString(1));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                connection.Close();
            }

            return products.ToArray();
        }
    }
}
