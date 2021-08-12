using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CurrencyConverterDatabase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SqlConnection sqlConnection = new SqlConnection();

        SqlCommand sqlCommand = new SqlCommand();

        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();

        private int CurrencyId = 0;

        private double FromAmount = 0;

        private double ToAmount = 0;


        public MainWindow()
        {
            InitializeComponent();

            BindCurrencyData();




        }


        public void Connect()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }







        private void BindCurrencyData()
        {
            Connect();

            DataTable dataTableCurrency = new DataTable();

            sqlCommand = new SqlCommand("select Id, CurrencyName from CurrencyMaster", sqlConnection);

            sqlCommand.CommandType = CommandType.Text;

            sqlDataAdapter = new SqlDataAdapter(sqlCommand);

            sqlDataAdapter.Fill(dataTableCurrency);



            DataRow newRow = dataTableCurrency.NewRow();

            newRow["Id"] = 0;
            newRow["CurrencyName"] = "--SELECT--";

            dataTableCurrency.Rows.InsertAt(newRow, 0);



            if (dataTableCurrency != null && dataTableCurrency.Rows.Count > 0)
            {
                comboboxFromCurrency.ItemsSource = dataTableCurrency.DefaultView;
                comboboxToCurrency.ItemsSource = dataTableCurrency.DefaultView;
            }

            sqlConnection.Close();

            comboboxFromCurrency.DisplayMemberPath = "CurrencyName";
            comboboxFromCurrency.SelectedValuePath = "Id";
            comboboxFromCurrency.SelectedValue = 0;

            comboboxToCurrency.DisplayMemberPath = "CurrencyName";
            comboboxToCurrency.SelectedValuePath = "Id";
            comboboxToCurrency.SelectedValue = 0;

        }



        //Allow only the integer value in TextBox
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            if (textCurrency.Text == null || textCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                textCurrency.Focus();
                return;
            }

            else if (comboboxFromCurrency.SelectedValue == null || comboboxFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select From Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                comboboxFromCurrency.Focus();
                return;
            }

            else if (comboboxToCurrency.SelectedValue == null || comboboxToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select To Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                comboboxToCurrency.Focus();
                return;
            }



            if (comboboxFromCurrency.Text == comboboxToCurrency.Text)
            {
                ConvertedValue = double.Parse(textCurrency.Text);

                labelCurrency.Content = comboboxToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                ConvertedValue = (double.Parse(comboboxFromCurrency.SelectedValue.ToString()) * double.Parse(textCurrency.Text)) / double.Parse(comboboxToCurrency.SelectedValue.ToString());

                labelCurrency.Content = comboboxToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }

        }



        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                textCurrency.Text = string.Empty;

                if (comboboxFromCurrency.Items.Count > 0)
                    comboboxFromCurrency.SelectedIndex = 0;

                if (comboboxToCurrency.Items.Count > 0)
                    comboboxToCurrency.SelectedIndex = 0;

                labelCurrency.Content = "";
                textCurrency.Focus();

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }


        private void ClearMaster()
        {
            try
            {
                textAmount.Text = string.Empty;
                textCurrencyName.Text = string.Empty;
                buttonSave.Content = "Save";
                GetMasterData();
                CurrencyId = 0;
                BindCurrencyData();
                textAmount.Focus();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }









        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {

            if (textAmount.Text == null || textAmount.Text.Trim() == "")
            {
                MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                textAmount.Focus();
                return;
            }
            else if (textCurrencyName.Text == null || textCurrencyName.Text.Trim() == "")
            {
                MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                textCurrencyName.Focus();
                return;
            }
            else
            {
                if (CurrencyId > 0)
                {

                    if (MessageBox.Show("Are you sure you want to update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Connect();
                        DataTable dt = new DataTable();

                        sqlCommand = new SqlCommand("UPDATE CurrencyMaster SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", sqlConnection);
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@Id", CurrencyId);
                        sqlCommand.Parameters.AddWithValue("@Amount", textAmount.Text);
                        sqlCommand.Parameters.AddWithValue("@CurrencyName", textCurrencyName.Text);
                        sqlCommand.ExecuteNonQuery();
                        sqlConnection.Close();

                        MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {

                    if (MessageBox.Show("Are you sure you want to Save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Connect();
                        DataTable dt = new DataTable();
                        sqlCommand = new SqlCommand("INSERT INTO CurrencyMaster(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", sqlConnection);
                        sqlCommand.CommandType = CommandType.Text;

                        sqlCommand.Parameters.AddWithValue("@Amount", textAmount.Text);
                        sqlCommand.Parameters.AddWithValue("@CurrencyName", textCurrencyName.Text);

                        sqlCommand.ExecuteNonQuery();
                        sqlConnection.Close();

                        MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }
                ClearMaster();

            }




        }



        public void GetMasterData()
        {
            Connect();

            DataTable dataTable = new DataTable();

            sqlCommand = new SqlCommand("SELECT * FROM CurrencyMaster", sqlConnection);

            sqlCommand.CommandType = CommandType.Text;

            sqlDataAdapter = new SqlDataAdapter(sqlCommand);

            sqlDataAdapter.Fill(dataTable);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                datagridCurrency.ItemsSource = dataTable.DefaultView;
            }
            else
            {
                datagridCurrency.ItemsSource = null;
            }

            sqlConnection.Close();
        }




        private void DatagridCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;

                DataRowView row_selected = grd.CurrentItem as DataRowView;

                if (row_selected != null)
                {

                    if (datagridCurrency.Items.Count > 0)
                    {
                        if (grd.SelectedCells.Count > 0)
                        {

                            CurrencyId = Int32.Parse(row_selected["Id"].ToString());

                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)
                            {

                                textAmount.Text = row_selected["Amount"].ToString();
                                textCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                buttonSave.Content = "Update";
                            }

                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                if (MessageBox.Show("Are you sure you want to delete ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    Connect();
                                    DataTable dt = new DataTable();

                                    sqlCommand = new SqlCommand("DELETE FROM CurrencyMaster WHERE Id = @Id", sqlConnection);
                                    sqlCommand.CommandType = CommandType.Text;

                                    sqlCommand.Parameters.AddWithValue("@Id", CurrencyId);
                                    sqlCommand.ExecuteNonQuery();
                                    sqlConnection.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }








        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void comboboxFromCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboboxFromCurrency.SelectedValue != null && int.Parse(comboboxFromCurrency.SelectedValue.ToString()) != 0 && comboboxFromCurrency.SelectedIndex != 0)
            {
                int CurrencyFromId = int.Parse(comboboxFromCurrency.SelectedValue.ToString());

                Connect();
                DataTable dataTable = new DataTable();

                sqlCommand = new SqlCommand("SELECT Amount FROM CurrencyMaster WHERE Id = @CurrencyFromId", sqlConnection);
                sqlCommand.CommandType = CommandType.Text;

                if (CurrencyFromId != null && CurrencyFromId != 0)
                    sqlCommand.Parameters.AddWithValue("@CurrencyFromId", CurrencyFromId);

                sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                sqlDataAdapter.Fill(dataTable);

                if (dataTable != null && dataTable.Rows.Count > 0)
                    FromAmount = double.Parse(dataTable.Rows[0]["Amount"].ToString());

                sqlConnection.Close();
            }
        }



        private void comboboxToCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboboxToCurrency.SelectedValue != null && int.Parse(comboboxToCurrency.SelectedValue.ToString()) != 0 && comboboxToCurrency.SelectedIndex != 0)
            {
                int CurrencyToId = int.Parse(comboboxToCurrency.SelectedValue.ToString());

                Connect();

                DataTable dt = new DataTable();
                sqlCommand = new SqlCommand("SELECT Amount FROM CurrencyMaster WHERE Id = @CurrencyToId", sqlConnection);
                sqlCommand.CommandType = CommandType.Text;

                if (CurrencyToId != null && CurrencyToId != 0)
                    sqlCommand.Parameters.AddWithValue("@CurrencyToId", CurrencyToId);

                sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                sqlDataAdapter.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                    ToAmount = double.Parse(dt.Rows[0]["Amount"].ToString());
                sqlConnection.Close();
            }
        }



        private void comboboxFromCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.SystemKey == Key.Enter)
            {
                comboboxFromCurrency_SelectionChanged(sender, null);
            }
        }

        private void comboboxToCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.SystemKey == Key.Enter)
            {
                comboboxToCurrency_SelectionChanged(sender, null);
            }
        }













    }

}
