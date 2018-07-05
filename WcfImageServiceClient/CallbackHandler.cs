using System.Windows;
using WcfImageServiceClient.ServiceReference1;
using WcfImageServiceClient.ServiceReference2;

namespace WcfImageServiceClient
{
    class CallbackHandler : ICallbackCallback
    {
        public void createDescriptionCallback()
        {
            MessageBox.Show("Empty description was created.");
            MainWindow mainWindow = Application.Current.Windows[0] as MainWindow;
            ServiceClient client = new ServiceClient();
            mainWindow.ImagesList.ItemsSource = client.getImagesList();
            mainWindow.UploadButton.IsEnabled = true;
        }
    }
}