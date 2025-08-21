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
        MainService _mainService;

        public MainWindow()
        {
            InitializeComponent();
            _mainService = new MainService();
           
        }
        
        private void ShowSalariesInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainService.TransformXml();
                _mainService.AddEmployeeTotalSalaryAttributes();
                _mainService.AddTotalPayAttributesToData1();
                var employees = _mainService.GetEmployeesSalaryData();
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
                    _mainService.AppendItemNodeToData1(dlg.ItemName, dlg.ItemSurname, dlg.ItemMonth, dlg.ItemAmount.ToString());

                    _mainService.TransformXml();
                    _mainService.AddEmployeeTotalSalaryAttributes();
                    _mainService.AddTotalPayAttributesToData1();
                    var employees = _mainService.GetEmployeesSalaryData();
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