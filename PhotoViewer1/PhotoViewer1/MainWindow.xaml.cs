using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace M12PhotoViewer
{
    public partial class MainWindow : Window
    {
        private string[] imageFiles;
        private int currentIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            string imageFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            if (Directory.Exists(imageFolderPath))
            {
                imageFiles = Directory.GetFiles(imageFolderPath, "*.*")
                    .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                   file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                   file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (imageFiles.Length > 0)
                {
                    ShowImage(currentIndex);
                }
                else
                {
                    MessageBox.Show("No images found in the Images folder.");
                }
            }
            else
            {
                MessageBox.Show("Images folder not found.");
            }
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (imageFiles == null || imageFiles.Length == 0) return;

            currentIndex = (currentIndex + 1) % imageFiles.Length;
            ShowImage(currentIndex);
        }

        private void ShowImage(int index)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageFiles[index], UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imageViewer.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }
    }
}
