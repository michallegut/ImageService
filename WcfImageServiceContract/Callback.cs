using System;
using System.IO;
using System.ServiceModel;

namespace WcfImageServiceContract
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Callback : ICallback
    {
        ICallbackHandler callbackHandler = null;

        public Callback()
        {
            callbackHandler = OperationContext.Current.GetCallbackChannel<ICallbackHandler>();
        }

        public void createDescription(ImageNameMessage imageName)
        {
            String descriptionName = imageName.name + ".txt";
            System.Threading.Thread.Sleep(3000);
            StreamWriter description = File.CreateText(System.Environment.CurrentDirectory + "\\descriptions\\" + descriptionName);
            description.Close();
            callbackHandler.createDescriptionCallback();
        }
    }
}