using System;
using System.Globalization;
using System.Windows;
using System.Xml.Linq;

namespace XML_Helper
{
    public partial class AddItemWindow : Window
    {
        public string ItemName => NameTextBox.Text.Trim();
        public string ItemSurname => SurnameTextBox.Text.Trim();
        public string ItemMonth
        {
            get
            {
                string selectedRus = MonthComboBox.SelectedItem?.ToString() ?? "";
                return _months.FirstOrDefault(x => x.Value == selectedRus).Key ?? "";
            }
        }
        public double ItemAmount { get; private set; }

        private readonly Dictionary<string, string> _months = new Dictionary<string, string>
        {
            { "january", "Январь" },
            { "february", "Февраль" },
            { "march", "Март" },
            { "april", "Апрель" },
            { "may", "Май" },
            { "june", "Июнь" },
            { "july", "Июль" },
            { "august", "Август" },
            { "september", "Сентябрь" },
            { "october", "Октябрь" },
            { "november", "Ноябрь" },
            { "december", "Декабрь" }
        };

        public AddItemWindow()
        {
            InitializeComponent();

            foreach (var month in _months.Values)
            {
                MonthComboBox.Items.Add(month);
            }
            MonthComboBox.SelectedIndex = 0;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ItemName) ||
                string.IsNullOrWhiteSpace(ItemSurname) ||
                string.IsNullOrWhiteSpace(ItemMonth) ||
                string.IsNullOrWhiteSpace(AmountTextBox.Text))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            var normalized = AmountTextBox.Text.Trim().Replace(',', '.');
            if (!double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedAmount))
            {
                MessageBox.Show("Сумма должна быть числом.");
                return;
            }

            ItemAmount = parsedAmount;

            DialogResult = true;
        }
    }
}
