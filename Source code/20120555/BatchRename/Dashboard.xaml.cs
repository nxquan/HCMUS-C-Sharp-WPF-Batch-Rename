using Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Xml;
using System.Diagnostics;
using MessageBox = System.Windows.MessageBox;
using RemoveSpecialChar;
using System.ComponentModel;
using DataObject = System.Windows.DataObject;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using ListViewItem = System.Windows.Controls.ListViewItem;
using System.Xml.Linq;
using System.Text.Json.Nodes;
using AddCounter;
using static System.Windows.Forms.Design.AxImporter;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading;

namespace BatchRename
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Method> _systemMethods = new ObservableCollection<Method>();
        private ObservableCollection<FileItem> _files = new ObservableCollection<FileItem>();
        private bool isRenamingFile = true;
        public event PropertyChangedEventHandler? PropertyChanged;
        private static readonly JsonSerializerOptions _options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        private int _start = 0;
        private int _step = 0;
        private int _numberOfDigits = 0;
        private PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void autoLoad()
        {
            //autoload rules
            var lines = File.ReadAllLines("./history/rules.txt");
            int order = 1;
            foreach (var line in lines)
            {
                var rule = RuleFactory.Instance().create(line);
                if (rule != null)
                {
                    for (int i = 0; i < _systemMethods.Count; i++)
                    {
                        if (rule.Name == _systemMethods[i].Name)
                        {
                            _systemMethods[i].changeStatus(true);
                            _systemMethods[i].rule = rule;
                            _systemMethods[i].Order = order;
                            order++;
                        }
                    }
                }
                if(rule!.Name == "AddCounter")
                {
                    var temp = (AddCounterRule)rule;
                    _start = temp.Start;
                    _step = temp.Step;
                    _numberOfDigits = temp.NumberOfDigit;
                }
            }

            foreach (var method in _systemMethods)
            {
                if (method.Order == 0)
                {
                    method.Order = order;
                    order++;
                }
            }
            _systemMethods = new ObservableCollection<Method>(_systemMethods.OrderBy(i => i.Order));

            //autoload files
            var linesFile = File.ReadAllLines("./history/files.txt");
            if (linesFile.Length >= 1)
            {
                foreach (var path in linesFile)
                {
                    var info = new FileInfo(path);
                    var selectedFile = new FileItem()
                    {
                        FileName = info.Name,
                        NewFileName = info.Name,
                        FilePath = path,
                        Error = "No"
                    };

                    if (_files.Contains(selectedFile))
                    {
                        MessageBox.Show($"File {info.Name} đã tồn tại trong chương trình", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        _files.Add(selectedFile);
                    }
                }
            }
        }

        private async Task autoSave()
        {
            while (await timer.WaitForNextTickAsync())
            {
                //Save files
                StreamWriter swFiles = new StreamWriter("./history/files.txt");
                foreach(var _file in _files)
                {
                    swFiles.WriteLine(_file.FilePath);
                }
                swFiles.Dispose();

                //Save rules
                StreamWriter swRules = new StreamWriter("./history/rules.txt");
                int count = 0;
                foreach (var method in _systemMethods)
                {
                    if (method.IsChecked)
                    {
                        count++;
                    }
                }
                int realCount = 1;
                foreach (var method in _systemMethods)
                {
                    if (method.IsChecked)
                    {
                        string line = method.Name + " " + method.rule.GetParam() + "\n";
                        if (realCount == count)
                        {
                            line = method.Name + " " + method.rule.GetParam();
                        }
                        realCount++;
                        swRules.Write(line);
                    }
                }
                swRules.Dispose();
            }
        }

        public object InsertedItem
        {
            get { return (object)GetValue(InsertedItemProperty); }
            set { SetValue(InsertedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InsertedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InsertedItemProperty =
            DependencyProperty.Register("InsertedItem", typeof(object), typeof(ListViewItem), new FrameworkPropertyMetadata(null));

        public object TargetItem
        {
            get { return (object)GetValue(TargetItemProperty); }
            set { SetValue(TargetItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetItemProperty =
            DependencyProperty.Register("TargetItem", typeof(object), typeof(ListViewItem), new FrameworkPropertyMetadata(null));

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadRuleFromDllFile();
            autoLoad();
            if (_files.Count > 0)
            {
                applyAllFilesByAllRules(_files);
            }
            lvMethods.ItemsSource = _systemMethods;
            lvFiles.ItemsSource = _files;
            _ = autoSave();
        }

        public void loadRuleFromDllFile()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var dirInfor = new DirectoryInfo(basePath);
            var dllFiles = dirInfor.GetFiles("*.dll");
            foreach (var file in dllFiles)
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && typeof(IRule).IsAssignableFrom(type))
                    {
                        IRule rule = (IRule)Activator.CreateInstance(type)!;
                        RuleFactory.register(rule);
                        //Tạo đối tượng tương ứng với class
                        _systemMethods.Add(new Method() { Name = rule.Name, IsChecked = false, rule = rule });
                    }
                }
            }
        }

        public void loadRuleFromText(string path)
        {
            var lines = File.ReadAllLines(path);
            int order = 1;
            foreach (var line in lines)
            {
                var rule = RuleFactory.Instance().create(line);
                if(rule.Name == "AddCounter")
                {
                    var temp = (AddCounterRule)rule;
                    _start = temp.Start;
                    _step = temp.Step;
                    _numberOfDigits = temp.NumberOfDigit;
                }

                if (rule != null)
                {
                    for (int i = 0; i < _systemMethods.Count; i++)
                    {
                        if (rule.Name == _systemMethods[i].Name)
                        {
                            _systemMethods[i].changeStatus(true);
                            _systemMethods[i].rule = rule;
                            _systemMethods[i].Order = order;
                            order++;
                        }
                    }
                }
            }

            foreach (var method in _systemMethods)
            {
                if (method.Order == 0)
                {
                    method.Order = order;
                    order++;
                }
            }

            _systemMethods = new ObservableCollection<Method>(_systemMethods.OrderBy(i => i.Order));
        }

        public void handleDuplication(FileItem file)
        {
            //Handle duplication
            int count = 0;
            while (true)
            {
                int oldCount = count;
                foreach (var _file in _files)
                {
                    if (_file == file)
                    {
                        continue;
                    }
                    else
                    {
                        string checkedName;
                        if (count == 0)
                        {
                            checkedName = file.NewFileName;
                        }
                        else
                        {
                            int lastOfDotIndex = file.NewFileName.IndexOf(".");
                            checkedName = file.NewFileName.Insert(lastOfDotIndex, $"({count})");
                        }

                        if (checkedName == _file.NewFileName)
                        {
                            count++;
                            break;
                        }
                    }
                }
                if (count == oldCount)
                {
                    break;
                }
            }

            if (count > 0)
            {
                int lastOfDotIndex = file.NewFileName.IndexOf(".");
                file.NewFileName = file.NewFileName.Insert(lastOfDotIndex, $"({count})");
            }
        }

        public void applyOneFileByAllRules(FileItem file)
        {
            string newName = file.FileName;
            foreach (var method in _systemMethods)
            {
                if (method.IsChecked)
                {
                    newName = method.rule.Rename(newName);
                }
            }
            file.NewFileName = newName;
            handleDuplication(file);
        }

        public void applyAllFilesByAllRules(ObservableCollection<FileItem> items)
        {
            foreach (var item in items)
            {
                applyOneFileByAllRules(item);
            }
        }

        public void applyRuleForAllFiles(Method method)
        {
            for(int i = 0; i < _files.Count; i++)
            {
                _files[i].NewFileName = method.rule.Rename(_files[i].NewFileName);
                handleDuplication(_files[i]);
            }
        }

        private void btn_add_files_Click(object sender, RoutedEventArgs e)
        {
            var addFileScreen = new OpenFileDialog();
            addFileScreen.Multiselect = true;
            if (addFileScreen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var path in addFileScreen.FileNames)
                {
                    var info = new FileInfo(path);
                    bool isExist = false;

                    //Checking if file exist in my list files
                    foreach(var _file in _files)
                    {
                        if(_file.FilePath == path)
                        {
                            isExist = true;
                            break;
                        }
                    }

                    if(isExist)
                    {
                        MessageBox.Show($"File {info.Name} đã tồn tại trong chương trình", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }
                    else
                    {
                        var addeddFile = new FileItem()
                        {
                            FileName = info.Name,
                            NewFileName = info.Name,
                            FilePath = path,
                            Error = "No"
                        };
                        applyOneFileByAllRules(addeddFile);
                        _files.Add(addeddFile);
                    }
                }
            }
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            var choice = System.Windows.MessageBox.Show("Bạn chắc chắn muốn làm mới lại chương trình không?", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (choice == MessageBoxResult.Yes)
            {
                //Refresh files or folders
                if (isRenamingFile)
                {
                    _files.Clear();
                }
                else
                {
                    //do for folder but later
                }
                //Refresh all _userRules
                foreach (var method in _systemMethods)
                {
                    method.IsChecked = false;
                    method.rule.RefreshParam();
                }
            }

        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            var helpScreen = new HelpScreen();
            helpScreen.Show();
        }

        private void btn_start_batch_Click(object sender, RoutedEventArgs e)
        {
            var choice = MessageBox.Show("Bạn muốn đổi trên file gốc hay tạo thư mục chứa các file đã đổi tên? \nYes(Đổi trên file gốc)\nNo(Tạo thư mục)\nCancel(Hủy việc đổi tên)",
                "Thông báo", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (choice == MessageBoxResult.Yes)
            {
                //Refresh files or folders
                if (isRenamingFile)
                {
                    foreach (var file in _files)
                    {
                        File.Move(file.FilePath, file.renameFile());

                        file.FileName = file.NewFileName;
                        file.FilePath = file.renameFile();

                        File.Delete(file.FilePath);
                    }
                    MessageBox.Show("Chúc mừng bạn đã đỏi tên thành công \n Hãy kiểm tra thư mục chứa các file được đổi tên", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }else if(choice == MessageBoxResult.No)
            {
                var saveFileScreen = new FolderBrowserDialog();
                if (saveFileScreen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var rootFolder = saveFileScreen.SelectedPath;
                    foreach (var file in _files)
                    {
                        File.Move(file.FilePath, $"{rootFolder}/{file.NewFileName}");
                    }
                    MessageBox.Show("Chúc mừng bạn đã đỏi tên thành công \n Hãy kiểm tra thư mục chứa các file được đổi tên", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var indexItem = lvMethods.SelectedIndex;
            if (indexItem != -1)
            {
                Method selectedMethed = _systemMethods[indexItem];
                selectedMethed.changeStatus(!selectedMethed.IsChecked);
                if(selectedMethed.IsChecked)
                {
                    applyRuleForAllFiles(_systemMethods[indexItem]);
                }
                else
                {
                    foreach(var _method in _systemMethods)
                    {
                        if(_method.Name == "AddCounter")
                        {
                            _method.rule.PassParam(_start, _step, _numberOfDigits);
                        }
                    }
                    applyAllFilesByAllRules(_files);
                }
                lvMethods.ItemsSource = new ObservableCollection<Method>(_systemMethods);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var indexItem = lvMethods.SelectedIndex;
            var selectedItem = _systemMethods[indexItem];
            string selectedItemName = selectedItem.Name;
            if (selectedItemName == "AddPrefix"
                || selectedItemName == "AddSuffix"
                || selectedItemName == "ChangeExtension"
                || selectedItemName == "RemoveSpecialChar"
                )
            {
                var oneParamSreen = new RuleAndOneParam(selectedItem);
                bool? result = oneParamSreen.ShowDialog();
                if (result == true)
                {
                    string newParam = oneParamSreen.Param;
                    if (selectedItemName == "RemoveSpecialChar")
                    {
                        var tempRule = (RemoveSpecialCharRule)selectedItem.rule;
                        foreach (var c in newParam.Split(','))
                        {
                            tempRule.SpecialChars.Add(c[0]);
                        }
                    } else
                    {
                        selectedItem.rule.PassParam(newParam);
                    }
                }
                if(selectedItem.IsChecked)
                {
                    applyRuleForAllFiles(selectedItem);
                }

            } else if (selectedItemName == "AddCounter")
            {
                var editCounterScreen = new RuleAndThreeParam(selectedItem);
                bool? result = editCounterScreen.ShowDialog();
                if (result == true)
                {
                    _start = editCounterScreen.Start;
                    _step = editCounterScreen.Step;
                    _numberOfDigits = editCounterScreen.NumberOfDigits;

                    selectedItem.rule.PassParam(_start, _step, _numberOfDigits);
                }
                if (selectedItem.IsChecked)
                {
                    applyRuleForAllFiles(selectedItem);
                }
            } else
            {
                MessageBox.Show("Luật này không cần phải chỉnh sửa", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ListViewItem_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                sender is DependencyObject dependencyObject)
            {
                DragDrop.DoDragDrop(dependencyObject,
                    new DataObject(DataFormats.Serializable,
                    dependencyObject), DragDropEffects.Move);
            }
        }

        private void ListViewItem_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                InsertedItem = e.Data.GetData(DataFormats.Serializable);
                TargetItem = element.DataContext!;

                //exchange position of 2 items
                var insertMethod = (FrameworkElement)InsertedItem;
                int indexOfInserted = _systemMethods.IndexOf((Method)insertMethod.DataContext);
                int indexOfTarget = _systemMethods.IndexOf((Method)TargetItem);

                if (indexOfInserted == indexOfTarget) return;
                else if (indexOfTarget != -1 && indexOfInserted != -1)
                {
                    //Exchange order of 2 method
                    int tempOrder = _systemMethods[indexOfInserted].Order;
                    _systemMethods[indexOfInserted].Order = _systemMethods[indexOfTarget].Order;
                    _systemMethods[indexOfTarget].Order = tempOrder;
                    _systemMethods.Move(indexOfInserted, indexOfTarget);
                    lvMethods.ItemsSource = _systemMethods;
                }
            }
        }

        private void loadRuleBtn_Click(object sender, RoutedEventArgs e)
        {
            var addFileScreen = new OpenFileDialog();
            addFileScreen.Filter = "txt files|*.txt";
            if (addFileScreen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = addFileScreen.FileName;
                loadRuleFromText(path);
                lvMethods.ItemsSource = _systemMethods;
                if (_files.Count > 0)
                {
                    applyAllFilesByAllRules(_files);
                }
            }
        }

        private void saveRuleBtn_Click(object sender, RoutedEventArgs e)
        {
            var saveFileScreen = new SaveFileDialog() {
                Title = "Save your rules",
                Filter = "Text files|*.txt|Json files|*.json"
            };

            if (saveFileScreen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var filename = saveFileScreen.FileName;
                if (filename.Contains(".txt"))
                {
                    StreamWriter sw = new StreamWriter(File.Create(saveFileScreen.FileName));
                    int count = 0;
                    foreach (var method in _systemMethods)
                    {
                        if (method.IsChecked)
                        {
                            count++;
                        }
                    }
                    int realCount = 1;
                    foreach (var method in _systemMethods)
                    {
                        if (method.IsChecked)
                        {
                            string line = method.Name + " " + method.rule.GetParam() + "\n";
                            if (realCount == count)
                            {
                                line = method.Name + " " + method.rule.GetParam();
                            }
                            realCount++;
                            sw.Write(line);
                        }
                    }
                    sw.Dispose();
                } else if (filename.Contains(".json"))
                {
                    var options = new JsonSerializerOptions(_options)
                    {
                        WriteIndented = true
                    };
                    ObservableCollection<RuleJson> temp = new ObservableCollection<RuleJson>();

                    foreach (var method in _systemMethods)
                    {
                        temp.Add(new RuleJson()
                        {
                            Name = method.rule.Name,
                            Param = method.rule.GetParam()
                        });
                    }

                    var jsonString = JsonSerializer.Serialize(temp, options);
                    File.WriteAllText(filename, jsonString);
                }

                MessageBox.Show($"Bạn đã lưu các luật thành công \n File được lưu tại {saveFileScreen.FileName}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            }

        }

        private void lvFiles_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] dropFileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var path in dropFileNames)
                {
                    var info = new FileInfo(path);
                    bool isExist = false;
                    foreach(var _file in _files)
                    {
                        if(_file.FilePath == path)
                        {
                            isExist = true;
                            break;
                        }
                    }

                    if(isExist)
                    {
                        MessageBox.Show($"File {info.Name} đã tồn tại trong chương trình", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }
                    else
                    {
                        var currentFile = new FileItem()
                        {
                            FileName = info.Name,
                            NewFileName = "",
                            FilePath = path,
                            Error = "No"
                        };
                        applyOneFileByAllRules(currentFile);
                        _files.Add(currentFile);
                    }
                }
            }
        }

        public class RuleJson {
            public string Name { get; set; } = "";
            public string Param { get; set; } = "";
        }

    }
}
