using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace PlainTextEditor1
{
    public partial class MainWindow : Window
    {
        private TextDocument document = new TextDocument();

        public MainWindow()
        {
            InitializeComponent();
            UpdateSaveMenuState();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (CheckUnsavedChanges())
            {
                document = new TextDocument();
                TextEditor.Text = string.Empty;
            }
            UpdateSaveMenuState();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUnsavedChanges()) return;

            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Text Documents (*.txt)|*.txt",
                Title = "Open Text File"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    document.Open(dlg.FileName);
                    TextEditor.Text = document.Content;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening file: " + ex.Message);
                }
            }
            UpdateSaveMenuState();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(document.FilePath))
                SaveAs_Click(sender, e);
            else
                try { document.Save(); }
                catch (Exception ex) { MessageBox.Show("Error saving file: " + ex.Message); }
            UpdateSaveMenuState();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Text Documents (*.txt)|*.txt",
                FileName = System.IO.Path.GetFileName(document.FilePath),
                InitialDirectory = string.IsNullOrEmpty(document.FilePath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : System.IO.Path.GetDirectoryName(document.FilePath)
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    document.SaveAs(dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file: " + ex.Message);
                }
            }
            UpdateSaveMenuState();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (CheckUnsavedChanges())
                Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Plain Text Editor\nCreated by Teesh\nMay 2025", "About");
        }

        private void TextEditor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (document != null)
            {
                document.Content = TextEditor.Text;
                document.IsDirty = true;
            }
            UpdateSaveMenuState();
        }

        private void UpdateSaveMenuState()
        {
            SaveMenu.IsEnabled = document != null && document.IsDirty;
        }

        private bool CheckUnsavedChanges()
        {
            if (document != null && document.IsDirty)
            {
                var result = MessageBox.Show("Do you want to save changes to your document?", "Unsaved Changes",
                    MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel) return false;
                if (result == MessageBoxResult.Yes) Save_Click(null, null);
            }
            return true;
        }
    }
}
