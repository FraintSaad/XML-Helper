using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using XML_Helper.Models;

namespace XML_Helper
{
    internal class MainService
    {
        private string _dataFilePath;
        private string _converterFilePath;
        private string _employeesOutPath;
        internal string DataFileName => string.IsNullOrEmpty(_dataFilePath) ? string.Empty : Path.GetFileName(_dataFilePath);
        internal string XsltFileName => string.IsNullOrEmpty(_converterFilePath) ? string.Empty : Path.GetFileName(_converterFilePath);

        internal void TransformXml()
        {
            try
            {
                if (string.IsNullOrEmpty(_employeesOutPath))
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Title = "Сохранить employees.xml",
                        Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                        FileName = "employees.xml"
                    };

                    if (saveFileDialog.ShowDialog() != true)
                    {
                        throw new Exception("Пожалуйста выберите место сохранения для выходного файла");
                    }
                    _employeesOutPath = saveFileDialog.FileName;
                }
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(_converterFilePath);
                xslt.Transform(_dataFilePath, _employeesOutPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при выполнении трансформации XSLT", ex);
            }
        }

        internal void AddEmployeeTotalSalaryAttributes()
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
                var salaryNodes = emp.SelectNodes("salary");
                if (salaryNodes == null)
                {
                    throw new NullReferenceException(nameof(salaryNodes));
                }
                var total = GetTotal(salaryNodes);
                AddAttribute(doc, emp, "total", total.ToString("F2", CultureInfo.InvariantCulture));
            }
            doc.Save(_employeesOutPath);
        }

        private void AddAttribute(XmlDocument doc, XmlNode node, string attributeName, string attributeValue)
        {
            XmlAttribute totalAttr = doc.CreateAttribute(attributeName);
            totalAttr.Value = attributeValue;
            node.Attributes!.Append(totalAttr);
        }

        internal List<EmployeeViewModel> GetEmployeesSalaryData()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_employeesOutPath);

            List<EmployeeViewModel> list = new List<EmployeeViewModel>();
            var employees = doc.SelectNodes("//Employee");
            if (employees == null)
            {
                throw new NullReferenceException();
            }
            foreach (XmlNode employee in employees)
            {
                var salaries = employee.SelectNodes("salary");
                var employeeSalaries = GetEmployeeSalaries(employee);

                var name = employee.Attributes!["name"]?.Value;
                var surname = employee.Attributes!["surname"]?.Value;
                var total = employee.Attributes!["total"];
                if (name == null || surname == null || total == null)
                {
                    throw new NullReferenceException("Отсутствует одно или несколько необходимых значений аттрибутов");
                }

                EmployeeViewModel ev = new EmployeeViewModel
                {
                    Name = name,
                    Surname = surname,
                    Total = Convert.ToDouble(total.Value.Replace(',', '.'), CultureInfo.InvariantCulture),
                    January = employeeSalaries.TryGetValue("january", out double januaryAmount) == true ? januaryAmount : 0,
                    February = employeeSalaries.TryGetValue("february", out double februaryAmount) == true ? februaryAmount : 0,
                    March = employeeSalaries.TryGetValue("march", out double marchAmount) == true ? marchAmount : 0,
                    April = employeeSalaries.TryGetValue("april", out double aprilAmount) == true ? aprilAmount : 0,
                    May = employeeSalaries.TryGetValue("may", out double mayAmount) == true ? mayAmount : 0,
                    June = employeeSalaries.TryGetValue("june", out double juneAmount) == true ? juneAmount : 0,
                    July = employeeSalaries.TryGetValue("july", out double julyAmount) == true ? julyAmount : 0,
                    August = employeeSalaries.TryGetValue("august", out double augustAmount) == true ? augustAmount : 0,
                    September = employeeSalaries.TryGetValue("september", out double septemberAmount) == true ? septemberAmount : 0,
                    October = employeeSalaries.TryGetValue("october", out double octoberAmount) == true ? octoberAmount : 0,
                    November = employeeSalaries.TryGetValue("november", out double novemberAmount) == true ? novemberAmount : 0,
                    December = employeeSalaries.TryGetValue("december", out double decemberAmount) == true ? decemberAmount : 0,
                };

                list.Add(ev);
            }

            return list;
        }

        internal void AddTotalPayAttributesToData1()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_dataFilePath);
            var payNode = doc.SelectSingleNode("//Pay");
            var salaryNodes = doc.SelectNodes("//Pay/item");
            if (payNode == null || salaryNodes == null)
            {
                throw new NullReferenceException();
            }
            var total = GetTotal(salaryNodes);
            AddAttribute(doc, payNode, "total", total.ToString("F2", CultureInfo.InvariantCulture));
            doc.Save(_dataFilePath);
        }

        internal Dictionary<string, double> GetEmployeeSalaries(XmlNode employee)
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

        internal void AppendItemNodeToData1(string name, string surname, string month, string amountRaw)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_dataFilePath);

            var pay = doc.SelectSingleNode("/Pay");
            if (pay == null)
            {
                throw new Exception("Не найден элемент <Pay> в data1.xml");
            }

            XmlElement item = doc.CreateElement("item");

            XmlAttribute addingSurname = doc.CreateAttribute("surname");
            addingSurname.Value = surname;
            item.Attributes.Append(addingSurname);

            XmlAttribute addingName = doc.CreateAttribute("name");
            addingName.Value = name;
            item.Attributes.Append(addingName);


            XmlAttribute addingAmount = doc.CreateAttribute("amount");
            addingAmount.Value = amountRaw;
            item.Attributes.Append(addingAmount);

            XmlAttribute addingMount = doc.CreateAttribute("mount");
            addingMount.Value = month;
            item.Attributes.Append(addingMount);

            pay.AppendChild(item);
            doc.Save(_dataFilePath);
        }

        internal double GetTotal(XmlNodeList salaryNodes)
        {
            double total = 0;
            foreach (XmlNode salaryNode in salaryNodes)
            {
                var salaryFromAttribute = salaryNode.Attributes!["amount"];
                if (salaryFromAttribute == null)
                {
                    throw new NullReferenceException(nameof(salaryFromAttribute));
                }
                string amountStr = salaryFromAttribute.Value.Replace(',', '.');
                total += double.Parse(amountStr, CultureInfo.InvariantCulture);
            }
            return total;
        }

        internal string ChooseXSLTFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XSLT files (*.xslt;*.xsl)|*.xslt;*.xsl|All files (*.*)|*.*",
                Title = "Выберите XSLT-файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _converterFilePath = openFileDialog.FileName;
                return Path.GetFileName(_converterFilePath);
            }
            else
            {
                return string.Empty;
            }
        }

        internal string ChooseDataFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "Выберите XML data-файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _dataFilePath = openFileDialog.FileName;
                return Path.GetFileName(_dataFilePath);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
