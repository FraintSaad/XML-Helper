using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Xsl;
using XML_Helper.Models;
using System.Linq;
using System.Globalization;
using Microsoft.Win32;

namespace XML_Helper
{

    public partial class MainWindow : Window
    {
        MainWindowService _mainWindowService;

        public MainWindow()
        {
            InitializeComponent();
            _mainWindowService = new MainWindowService();
           
        }
        
        private void ShowSalariesInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainWindowService.TransformXml();
                _mainWindowService.AddEmployeeTotalSalaryAttributes();
                _mainWindowService.AddTotalPayAttributesToData1();
                _mainWindowService.ShowEmployeesSalaryData();
                var employees = _mainWindowService.ShowEmployeesSalaryData();
                dgEmployees.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddDataBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddItemWindow { Owner = this };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    _mainWindowService.AppendItemNodeToData1(dlg.ItemName, dlg.ItemSurname, dlg.ItemMonth, dlg.ItemAmount.ToString());

                    _mainWindowService.TransformXml();
                    _mainWindowService.AddEmployeeTotalSalaryAttributes();
                    _mainWindowService.AddTotalPayAttributesToData1();
                    var employees = _mainWindowService.ShowEmployeesSalaryData();
                    dgEmployees.ItemsSource = employees;

                    MessageBox.Show("Данные добавлены и пересчитаны.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
    }
}