using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WcfImageServiceClient.ServiceReference1;
using WcfImageServiceClient.ServiceReference2;

namespace WcfImageServiceClient
{
    public partial class MainWindow : Window
    {
        private ServiceClient client;
        private CallbackClient callbackClient;

        public MainWindow()
        {
            InitializeComponent();
            client = new ServiceClient();
            callbackClient = new CallbackClient(new InstanceContext(new CallbackHandler()));
            ImagesList.ItemsSource = client.getImagesList();
            ImagesList.SelectionChanged += imagesListSelectionChanged;
            DownloadButton.IsEnabled = false;
            DownloadButton.Click += downloadButtonClick;
            DeleteButton.IsEnabled = false;
            DeleteButton.Click += deleteButtonClick;
            UploadButton.Click += uploadButtonClick;
        }

        private void imagesListSelectionChanged(object sender, RoutedEventArgs e)
        {
            ImageProgressBar.Value = 0;
            DescriptionProgressBar.Value = 0;
            String imageName = ImagesList.SelectedItem.ToString();
            String filePath = System.Environment.CurrentDirectory + "\\images\\" + imageName;
            if (File.Exists(filePath))
            {
                DownloadButton.IsEnabled = false;
                DeleteButton.IsEnabled = true;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(filePath);
                image.EndInit();
                ImageDisplay.Source = image;
                String descriptionPath = System.Environment.CurrentDirectory + "\\descriptions\\" + imageName + ".txt";
                ImageDescription.Text = File.ReadAllText(descriptionPath);
            }
            else
            {
                DownloadButton.IsEnabled = true;
                DeleteButton.IsEnabled = false;
                ImageDisplay.Source = null;
                ImageDescription.Text = null;
            }
        }

        private void downloadButtonClick(object sender, RoutedEventArgs e)
        {
            DownloadButton.IsEnabled = false;
            DeleteButton.IsEnabled = true;
            String imageName = ImagesList.SelectedItem.ToString();
            Stream imageStream = null;
            ImageProgressBar.Maximum = client.downloadImage(ref imageName, out imageStream) + 1;
            Stream descriptionStream = null;
            DescriptionProgressBar.Maximum = client.downloadDescription(ref imageName, out descriptionStream) + 1;
            saveFile(imageStream, System.Environment.CurrentDirectory + "\\images\\" + imageName, ImageProgressBar);
            saveFile(descriptionStream, System.Environment.CurrentDirectory + "\\descriptions\\" + imageName + ".txt", DescriptionProgressBar);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(System.Environment.CurrentDirectory + "\\images\\" + imageName);
            image.EndInit();
            ImageDisplay.Source = image;
            ImageDescription.Text = File.ReadAllText(System.Environment.CurrentDirectory + "\\descriptions\\" + imageName + ".txt");
        }

        private void saveFile(System.IO.Stream inStream, string path, ProgressBar progressBar)
        {
            const int bufferLength = 8192;
            int counter = 0;
            byte[] buffer = new byte[bufferLength];
            FileStream outStream = File.Open(path, FileMode.Create, FileAccess.Write);
            while ((counter = inStream.Read(buffer, 0, bufferLength)) > 0)
            {
                outStream.Write(buffer, 0, counter);
                progressBar.Dispatcher.Invoke(() => progressBar.Value += counter, DispatcherPriority.Background);
            }
            progressBar.Dispatcher.Invoke(() => progressBar.Value = progressBar.Maximum, DispatcherPriority.Background);
            outStream.Close();
            inStream.Close();
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            DownloadButton.IsEnabled = true;
            DeleteButton.IsEnabled = false;
            DescriptionProgressBar.Value = 0;
            ImageProgressBar.Value = 0;
            String imageName = ImagesList.SelectedItem.ToString();
            ImageDisplay.Source = null;
            File.Delete(System.Environment.CurrentDirectory + "\\images\\" + imageName);
            ImageDescription.Text = null;
            File.Delete(System.Environment.CurrentDirectory + "\\descriptions\\" + imageName + ".txt");
        }

        private void uploadButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Choose image file.");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            Nullable<bool> imageDialogResult = openFileDialog.ShowDialog();
            if (imageDialogResult == true)
            {
                string imagePath = openFileDialog.FileName;
                String imageName = Path.GetFileName(imagePath);
                if (!client.getImagesList().Contains(imageName))
                {
                    FileStream image;
                    try
                    {
                        image = File.OpenRead(imagePath);
                    }
                    catch (IOException exception)
                    {
                        Console.WriteLine(exception.ToString());
                        throw exception;
                    }
                    Stream imageStream = image;
                    long imageSize = image.Length;
                    client.uploadImage(imageName, imageSize, imageStream);
                    MessageBox.Show("Choose description file or cancel to upload image without a description.");
                    Nullable<bool> descriptionDialogResult = openFileDialog.ShowDialog();
                    if (descriptionDialogResult == true)
                    {
                        string descriptionPath = openFileDialog.FileName;
                        String descriptionName = imageName + ".txt";
                        FileStream description;
                        try
                        {
                            description = File.OpenRead(descriptionPath);
                        }
                        catch (IOException exception)
                        {
                            Console.WriteLine(exception.ToString());
                            throw exception;
                        }
                        Stream descriptionStream = description;
                        long descriptionSize = description.Length;
                        client.uploadDescription(descriptionName, descriptionSize, descriptionStream);
                        ImagesList.ItemsSource = client.getImagesList();
                    }
                    else
                    {
                        UploadButton.IsEnabled = false;
                        callbackClient.createDescription(imageName);
                    }
                }
                else
                {
                    MessageBox.Show("Image with this name is already on the server.");
                }
            }
        }
    }
}