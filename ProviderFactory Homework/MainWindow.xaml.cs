using Microsoft.Extensions.Configuration;
using System;
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


        public ListView NormalExecuteQuery()
        {
            using var command = connection.CreateCommand();
            command.CommandText = QueryTextBox.Text;

            var adapter = providerFactory.CreateDataAdapter();
            adapter.SelectCommand = command;

            DataTable table = new DataTable();
            adapter.Fill(table);
            
            ListView list = new ListView();
            GridView gridView = new GridView();

            foreach (DataColumn column in table.Columns)
            {
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = column.ColumnName,
                    DisplayMemberBinding = new Binding(column.ColumnName)
                });
            }

            list.View = gridView;
            list.ItemsSource = table.DefaultView;

            return list;
        }
 







        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {

            if (ProvidersComboBox.SelectedItem is null || string.IsNullOrEmpty(QueryTextBox.Text)) return;

            TabItem newTabItem = new TabItem();
            newTabItem.Header = "New Tab"; 
            newTabItem.Content = NormalExecuteQuery();
            tabControl.Items.Add(newTabItem);
            tabControl.SelectedItem = newTabItem;






        }
    }
}
