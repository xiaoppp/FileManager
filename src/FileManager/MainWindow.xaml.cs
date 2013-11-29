using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using System.IO;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Windows.Forms;

namespace DatumManage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ExpandRecursively(ItemsControl itemsControl, bool expand)
        {
            ItemContainerGenerator itemContainerGenerator = itemsControl.ItemContainerGenerator;

            for (int i = itemsControl.Items.Count - 1; i >= 0; --i)
            {
                ItemsControl childControl = itemContainerGenerator.ContainerFromIndex(i) as ItemsControl;
                if (childControl != null)
                    ExpandRecursively(childControl, expand);
            }

            TreeViewItem treeViewItem = itemsControl as TreeViewItem;

            if (treeViewItem != null)
                treeViewItem.IsExpanded = expand;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var list = tvFolder.ItemsSource as List<TreeNodeModel>;

            XDocument document = new XDocument(new XProcessingInstruction("xml-stylesheet", "type='text/xsl' href='htmlstyle.xslt'")); 
            XElement element = new XElement("Root");
            
            BuildXML(list, element);
            document.Add(element);

            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = "xml";
            if (dialog.ShowDialog().Value)
            {
                var path = dialog.FileName;
                document.Save(path);

                messageLabel.Content = "save successful";
            }
        }

        private void BuildXML(List<TreeNodeModel> list, XElement ele)
        {
            foreach (TreeNodeModel model in list)
            {
                if (model.IsSelected)
                {
                    XElement pathElement = new XElement("Path",
                        new XAttribute("Name", model.Name),
                        //new XAttribute("IsSelected", model.IsSelected),
                        new XAttribute("FullPath", model.FullPath));
                    ele.Add(pathElement);

                    if (model.NodeList.Count == 0)
                    {
                        var files = Directory.GetFiles(model.FullPath);

                        if (files != null)
                        {
                            foreach (var file in files)
                            {
                                var filename = Path.GetFileName(file);
                                XElement fileElement = new XElement("File",
                                    new XAttribute("Name", filename),
                                    new XAttribute("Path", file));

                                pathElement.Add(fileElement);
                            }
                        }
                    }

                    BuildXML(model.NodeList, pathElement);
                }
            }
        }

        private void btnBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            messageLabel.Content = "browse folder...";
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPath.Text = dialog.SelectedPath;
            }

            var path = txtPath.Text;

            if (string.IsNullOrEmpty(path))
                return;

            var model = new TreeNodeModel();
            model.IsRoot = true;

            BuildFolderNodes(path, model);
            this.tvFolder.ItemsSource = model.NodeList;

            ExpandRecursively(tvFolder, true);
        }

        private void BuildFolderNodes(string path, TreeNodeModel model)
        {
            var dirs = Directory.GetDirectories(path);
            var name = Path.GetFileName(path);

            var childModel = new TreeNodeModel { FullPath = path, Name = name };

            model.NodeList.Add(childModel);

            foreach (string dir in dirs)
            {
                BuildFolderNodes(dir, childModel);
            }
        }
    }
}

public class TreeNodeModel {

    public TreeNodeModel()
    {
        NodeList = new List<TreeNodeModel>();
        IsSelected = true;
        IsRoot = false;
    }

    public string Name { get; set; }
    public bool IsSelected { get; set; }
    public string FullPath { get; set; }

    public List<TreeNodeModel> NodeList { get; set; }

    public bool IsRoot { get; set; }
}
