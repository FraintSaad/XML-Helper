using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Xsl;

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

            string inputDirPath = Path.GetFullPath(Path.Combine( "..", "..", "..", "..", "..", "input"));
            _data1Path = Path.Combine(inputDirPath, "data1.xml");
            _xsltFilePath = Path.Combine(inputDirPath, "convert_to_employees.xslt");
            _employeesOutPath = Path.Combine(inputDirPath, "employees.xml");
            TestXsltConversion();
        }

        private void TestXsltConversion()
        {
            try
            {
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(_xsltFilePath);
                xslt.Transform(_data1Path, _employeesOutPath);

                AddEmployeeSums();
                AddData1Total();
                MessageBox.Show("Преобразование и подсчёт сумм прошли успешно!\nФайл создан: " + _employeesOutPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка XSLT: " + ex.Message);
            }
        }

        private void AddEmployeeSums()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_employeesOutPath);

            foreach (XmlNode emp in doc.SelectNodes("//Employee"))
            {
                double total = 0;
                foreach (XmlNode salary in emp.SelectNodes("salary"))
                {
                    string amountStr = salary.Attributes["amount"].Value.Replace(',', '.');
                    total += double.Parse(amountStr, System.Globalization.CultureInfo.InvariantCulture);
                }
                XmlAttribute totalAttr = doc.CreateAttribute("total");
                totalAttr.Value = total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                emp.Attributes.Append(totalAttr);
            }

            doc.Save(_employeesOutPath);
        }

        private void AddData1Total()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_data1Path);
            XmlNode payNode = doc.SelectSingleNode("//Pay");
            double total = 0;
            foreach (XmlNode item in doc.SelectNodes("//Pay/item"))
            {
                string amountStr = item.Attributes["amount"].Value.Replace(',', '.');
                total += double.Parse(amountStr, System.Globalization.CultureInfo.InvariantCulture);
            }

            XmlAttribute totalAttr = doc.CreateAttribute("total");
            totalAttr.Value = total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            payNode.Attributes.Append(totalAttr);
            doc.Save(_data1Path);
        }

    }
}