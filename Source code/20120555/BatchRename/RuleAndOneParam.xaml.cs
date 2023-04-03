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
    public partial class RuleAndOneParam : Window, INotifyPropertyChanged
    {
        public Method EditedMethod { get; set; }
        public string NameRule { get; set; }
        public string Param { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public RuleAndOneParam(Method method)
        {
            InitializeComponent();
            EditedMethod = method.Clone()! as Method;
            NameRule = EditedMethod!.rule.Name;
            Param = EditedMethod.rule.GetParam();
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
