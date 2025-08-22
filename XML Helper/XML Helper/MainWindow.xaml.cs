using System.Windows;
using System.Windows.Media;

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
                RunMainWorkflow();
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
                    RunMainWorkflow();
                    MessageBox.Show("Данные добавлены и пересчитаны.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void RunMainWorkflow()
        {
            _mainService.TransformXml();
            _mainService.AddEmployeeTotalSalaryAttributes();
            _mainService.AddTotalPayAttributesToData1();
            var employees = _mainService.GetEmployeesSalaryData();
            dgEmployees.ItemsSource = employees;
        }

        private void UpdateButtonsState()
        {
            AddDataBtn.IsEnabled = !string.IsNullOrEmpty(_mainService.DataFileName);
            ShowSalariesInfoBtn.IsEnabled = !string.IsNullOrEmpty(_mainService.DataFileName) &&
                                           !string.IsNullOrEmpty(_mainService.XsltFileName);

            if (!string.IsNullOrEmpty(_mainService.DataFileName))
                ChooseDataFileBtn.Background = Brushes.LightGreen;

            if (!string.IsNullOrEmpty(_mainService.XsltFileName))
                ChooseXsltFileBtn.Background = Brushes.LightGreen;
        }

        private void ChooseDataFileBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileName = _mainService.ChooseDataFile();
            if (!string.IsNullOrEmpty(fileName))
            {
                DataFilePathText.Text = fileName;
                UpdateButtonsState();
            }
        }

        private void ChooseXsltFileBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileName = _mainService.ChooseXSLTFile();
            if (!string.IsNullOrEmpty(fileName))
            {
                XsltFilePathText.Text = fileName;
                UpdateButtonsState();
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}