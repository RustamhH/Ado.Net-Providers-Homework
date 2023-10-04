using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ProviderFactory_Homework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbConnection? connection = null;
        IConfigurationRoot? configuration = null;
        DbProviderFactory? providerFactory = null;
        string currentDbName = "";











        public void StartConfig()
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", typeof(SqlClientFactory));
            DbProviderFactories.RegisterFactory("System.Data.OleDb", typeof(OleDbFactory));
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            DataTable table = DbProviderFactories.GetFactoryClasses();

            ProvidersComboBox.Items.Clear();
            foreach (DataRow row in table.Rows)
                ProvidersComboBox.Items.Add(row["InvariantName"].ToString());

        }

        

        public MainWindow()
        {
            InitializeComponent();
            StartConfig();
        }

        private void ProvidersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentDbName = ProvidersComboBox.SelectedItem.ToString();

            var connectionString = configuration.GetConnectionString(currentDbName);
            providerFactory = DbProviderFactories.GetFactory(currentDbName);

            connection = providerFactory.CreateConnection();
            connection.ConnectionString = connectionString;


        }


        public void NormalExecuteQuery()
        {
            using var command = connection.CreateCommand();
            command.CommandText = QueryTextBox.Text;

            var adapter = providerFactory.CreateDataAdapter();
            adapter.SelectCommand = command;

            DataSet table = new DataSet();
            adapter.Fill(table);


            for (int i = 0; i < table.Tables.Count; i++)
            {
                DataGrid dataGrid = new DataGrid();
                if (i == 0)
                {
                    dataGrid.ItemsSource = table.Tables["table"].DefaultView;

                    DataTableMapping dataTableMapping = new DataTableMapping();
                    dataTableMapping.SourceTable = "table";
                    dataTableMapping.DataSetTable = "first";


     


                    DataColumnMapping dataColumnMapping = new DataColumnMapping();
                    dataColumnMapping.SourceColumn = "Id";
                    dataColumnMapping.DataSetColumn = "sira No";

                    dataTableMapping.ColumnMappings.Add(dataColumnMapping);
                    adapter.TableMappings.Add(dataTableMapping);

                }
                else
                {
                    dataGrid.ItemsSource = table.Tables[$"table{i}"].DefaultView;
                    DataTableMapping dataTableMapping = new DataTableMapping();
                    dataTableMapping.SourceTable = $"table{i}";
                    dataTableMapping.DataSetTable = $"table no{i}";





                    DataColumnMapping dataColumnMapping = new DataColumnMapping();
                    dataColumnMapping.SourceColumn = "Id";
                    dataColumnMapping.DataSetColumn = "sira No";

                    dataTableMapping.ColumnMappings.Add(dataColumnMapping);
                    adapter.TableMappings.Add(dataTableMapping);
                }
                TabItem newTabItem = new TabItem();
                newTabItem.Header = "New Tab";
                newTabItem.Content = dataGrid;
                tabControl.Items.Add(newTabItem);
            }




        }





        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {

            if (ProvidersComboBox.SelectedItem is null || string.IsNullOrEmpty(QueryTextBox.Text)) return;
            NormalExecuteQuery();






        }
    }
}
