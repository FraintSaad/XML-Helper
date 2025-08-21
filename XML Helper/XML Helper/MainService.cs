using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using XML_Helper.Models;

namespace XML_Helper
{
    internal class MainService
    {
        private readonly string _data1Path;
        private readonly string _xsltFilePath;
        private readonly string _employeesOutPath;

        internal MainService()
        {
            string inputDirPath = Path.GetFullPath(Path.Combine("..", "..", "..", "..", "..", "input"));
            _data1Path = Path.Combine(inputDirPath, "data1.xml");
            _xsltFilePath = Path.Combine(inputDirPath, "convert_to_employees.xslt");
            _employeesOutPath = Path.Combine(inputDirPath, "employees.xml");
        }
        internal void TransformXml()
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
                double total = 0;
                var salaries = emp.SelectNodes("salary");
                if (salaries == null)
                {
                    throw new NullReferenceException(nameof(salaries));
                }

                foreach (XmlNode salary in salaries)
                {
                    var salaryFromAttribute = salary.Attributes!["amount"];
                    if (salaryFromAttribute == null)
                    {
                        throw new NullReferenceException(nameof(salaryFromAttribute));
                    }
                    string amountStr = salaryFromAttribute.Value.Replace(',', '.');
                    total += double.Parse(amountStr, CultureInfo.InvariantCulture);
                }
                XmlAttribute totalAttr = doc.CreateAttribute("total");
                totalAttr.Value = total.ToString("F2", CultureInfo.InvariantCulture);
                emp.Attributes!.Append(totalAttr);
            }

            doc.Save(_employeesOutPath);
        }

        internal List<EmployeeView> ShowEmployeesSalaryData()
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

                var name = employee.Attributes!["name"]?.Value;
                var surname = employee.Attributes!["surname"]?.Value;
                var total = employee.Attributes!["total"];
                if (name == null || surname == null || total == null)
                {
                    throw new NullReferenceException("Отсутствует одно или несколько необходимых значений аттрибутов");
                }

                EmployeeView ev = new EmployeeView
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
                string? amountStr = item.Attributes!["amount"]?.Value.Replace(',', '.');
                if (amountStr == null)
                {
                    throw new NullReferenceException(nameof(amountStr));
                }
                total += double.Parse(amountStr, CultureInfo.InvariantCulture);
            }

            XmlAttribute totalAttr = doc.CreateAttribute("total");
            totalAttr.Value = total.ToString("F2", CultureInfo.InvariantCulture);
            payNode.Attributes!.Append(totalAttr);
            doc.Save(_data1Path);
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
            doc.Load(_data1Path);

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
            doc.Save(_data1Path);
        }
    }
}
