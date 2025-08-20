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
        private readonly string _data1Path;
        private readonly string _xsltFilePath;
        private readonly string _employeesOutPath;

        public MainWindow()
        {
            InitializeComponent();

            string inputDirPath = Path.GetFullPath(Path.Combine("..", "..", "..", "..", "..", "input"));
            _data1Path = Path.Combine(inputDirPath, "data1.xml");
            _xsltFilePath = Path.Combine(inputDirPath, "convert_to_employees.xslt");
            _employeesOutPath = Path.Combine(inputDirPath, "employees.xml");
        }

        private void TransformXml()
        {
            try
            {
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(_xsltFilePath);
                xslt.Transform(_data1Path, _employeesOutPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при выполнении трансформации XSLT", ex);
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            LoadEmployeesToGrid();

        }

        private void AddEmployeeSums()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_employeesOutPath);
            var employeesNode = doc.SelectNodes("//Employee");
            if (employeesNode == null)
            {
                throw new NullReferenceException(nameof(employeesNode));
            }

            foreach (XmlNode emp in employeesNode)
            {
                double total = 0;
                var salaries = emp.SelectNodes("salary");
                if (salaries == null)
                {
                    throw new NullReferenceException();
                }

                foreach (XmlNode salary in salaries)
                {
                    var salaryFromAttribute = salary.Attributes?["amount"];
                    if (salaryFromAttribute == null)
                    {
                        throw new NullReferenceException();
                    }
                    string amountStr = salaryFromAttribute.Value.Replace(',', '.');
                    total += double.Parse(amountStr, CultureInfo.InvariantCulture);
                }
                XmlAttribute totalAttr = doc.CreateAttribute("total");
                totalAttr.Value = total.ToString("F2", CultureInfo.InvariantCulture);
                emp.Attributes?.Append(totalAttr);
            }

            doc.Save(_employeesOutPath);
        }

        private void AddData1Total()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_data1Path);
            var payNode = doc.SelectSingleNode("//Pay");
            var itemsNodeList = doc.SelectNodes("//Pay/item");
            if (payNode == null || itemsNodeList == null)
            {
                throw new NullReferenceException();
            }

            double total = 0;
            foreach (XmlNode item in itemsNodeList)
            {
                string? amountStr = item.Attributes?["amount"]?.Value.Replace(',', '.');
                if (amountStr == null)
                {
                    throw new NullReferenceException();
                }
                total += double.Parse(amountStr, CultureInfo.InvariantCulture);
            }

            XmlAttribute totalAttr = doc.CreateAttribute("total");
            totalAttr.Value = total.ToString("F2", CultureInfo.InvariantCulture);
            payNode.Attributes?.Append(totalAttr);
            doc.Save(_data1Path);
        }

        private Dictionary<string, double> GetEmployeeSalaries(XmlNode employee)
        {
            var employeeSalaries = new Dictionary<string, double>();

            var employeeSalariesRaw = employee.SelectNodes("salary");
            if (employeeSalariesRaw == null)
            {
                return employeeSalaries;
            }

            foreach (XmlNode currentSalary in employeeSalariesRaw)
            {
                string? currentSalaryMonthRaw = currentSalary.Attributes!["mount"]?.Value?.ToLower();
                string? currentSalaryAmountRaw = currentSalary.Attributes!["amount"]?.Value?.Replace(',', '.');

                if (currentSalaryMonthRaw == null || currentSalaryAmountRaw == null)
                {
                    continue;
                }
                employeeSalaries.Add(currentSalaryMonthRaw, Convert.ToDouble(currentSalaryAmountRaw, CultureInfo.InvariantCulture));
            }

            return employeeSalaries;
        }

        private void LoadEmployeesToGrid()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_employeesOutPath);

            List<EmployeeView> list = new List<EmployeeView>();
            var employees = doc.SelectNodes("//Employee");
            if (employees == null)
            {
                throw new NullReferenceException();
            }
            foreach (XmlNode employee in employees)
            {
                var salaries = employee.SelectNodes("salary");
                var employeeSalaries = GetEmployeeSalaries(employee);

                var name = employee.Attributes?["name"]?.Value;
                var surname = employee.Attributes?["surname"]?.Value;
                var total = employee.Attributes?["total"];
                if (name == null || surname == null || total == null )
                {
                    throw new Exception(); 
                }

                EmployeeView ev = new EmployeeView
                {
                    Name = name,
                    Surname = surname,
                    Total = Convert.ToDouble(total.Value.Replace(',', '.'), CultureInfo.InvariantCulture),
                    January = employeeSalaries.TryGetValue("january", out double januaryAmount) == true ? januaryAmount : 0,
                    February = employeeSalaries.TryGetValue("february", out double februaryAmount) == true ? februaryAmount : 0,
                    March = employeeSalaries.TryGetValue("january", out double marchAmount) == true ? marchAmount : 0,
                    April = employeeSalaries.TryGetValue("january", out double aprilAmount) == true ? aprilAmount : 0,
                    May = employeeSalaries.TryGetValue("january", out double mayAmount) == true ? mayAmount : 0,
                    June = employeeSalaries.TryGetValue("january", out double juneAmount) == true ? juneAmount : 0,
                    July = employeeSalaries.TryGetValue("january", out double julyAmount) == true ? julyAmount : 0,
                    August = employeeSalaries.TryGetValue("january", out double augustAmount) == true ? augustAmount : 0,
                    September = employeeSalaries.TryGetValue("january", out double septemberAmount) == true ? septemberAmount : 0,
                    October = employeeSalaries.TryGetValue("january", out double octoberAmount) == true ? octoberAmount : 0,
                    November = employeeSalaries.TryGetValue("january", out double novemberAmount) == true ? novemberAmount : 0,
                    December = employeeSalaries.TryGetValue("january", out double decemberAmount) == true ? decemberAmount : 0,
                
                };

                list.Add(ev);
            }

            dgEmployees.ItemsSource = list;
        }
        private void ConvertBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TransformXml();
                AddEmployeeSums();
                AddData1Total();
                LoadEmployeesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}