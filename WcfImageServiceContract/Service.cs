using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace WcfImageServiceContract
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Service : IService
    {
        public List<String> getImagesList()
        {
            List<String> imagesList = new List<String>();
            foreach (string image in Directory.GetFiles(System.Environment.CurrentDirectory + "\\images").Select(Path.GetFileName))
            {
                imagesList.Add(image);
            }
            return imagesList;
        }

        public ImageMessage downloadImage(ImageNameMessage imageName)
        {
            ImageMessage result = new ImageMessage();
            result.name = imageName.name;
            string imagePath = Path.Combine(System.Environment.CurrentDirectory, ".\\images\\" + imageName.name);
            FileStream image;
            try
            {
                image = File.OpenRead(imagePath);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
            result.image = image;
            result.size = image.Length;
            return result;
        }

        public DescriptionMessage downloadDescription(ImageNameMessage imageName)
        {
            DescriptionMessage result = new DescriptionMessage();
            result.name = imageName.name;
            string descriptionPath = Path.Combine(System.Environment.CurrentDirectory, ".\\descriptions\\" + imageName.name + ".txt");
            FileStream description;
            try
            {
                description = File.OpenRead(descriptionPath);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
            result.description = description;
            result.size = description.Length;
            return result;
        }

        public void uploadImage(ImageMessage imageMessage)
        {
            String imageName = imageMessage.name;
            Stream imageStream = imageMessage.image;
            saveFile(imageStream, System.Environment.CurrentDirectory + "\\images\\" + imageName);
        }

        public void uploadDescription(DescriptionMessage descriptionMessage)
        {
            String descriptionName = descriptionMessage.name;
            Stream descriptionStream = descriptionMessage.description;
            saveFile(descriptionStream, System.Environment.CurrentDirectory + "\\descriptions\\" + descriptionName);
        }

        private void saveFile(System.IO.Stream inStream, string path)
        {
            const int bufferLength = 8192;
            int counter = 0;
            byte[] buffer = new byte[bufferLength];
            FileStream outStream = File.Open(path, FileMode.Create, FileAccess.Write);
            while ((counter = inStream.Read(buffer, 0, bufferLength)) > 0)
            {
                outStream.Write(buffer, 0, counter);
            }
            outStream.Close();
            inStream.Close();
        }
    }
}