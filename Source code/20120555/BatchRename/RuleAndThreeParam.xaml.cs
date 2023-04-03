using AddCounter;
using AddPrefix;
using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace BatchRename
{
    /// <summary>
    /// Interaction logic for RuleAndOneParam.xaml
    /// </summary>
    public partial class RuleAndThreeParam : Window, INotifyPropertyChanged
    {
        public Method OldMethod { get; set; }
        public AddCounter.AddCounterRule _addCounterRule { get; set; }
        public string NameRule { get; set; }
        public int Start { get; set; } 
        public int Step { get; set; } 
        public int NumberOfDigits { get; set; } 
        public event PropertyChangedEventHandler? PropertyChanged;

        public RuleAndThreeParam(Method method)
        {
            InitializeComponent();
            OldMethod = (Method)method.Clone()!;
            NameRule = OldMethod.rule.Name;
            _addCounterRule = (AddCounter.AddCounterRule)OldMethod.rule;
            Start = _addCounterRule.Start;
            Step = _addCounterRule.Step;
            NumberOfDigits = _addCounterRule.NumberOfDigit;

            DataContext = this;
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
