using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using WcfImageServiceContract;

namespace WcfImageServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri basicHttpBindingAddress = new Uri("http://localhost:9999/");
            ServiceHost host = new ServiceHost(typeof(Service), basicHttpBindingAddress);
            Uri wSDualHttpBindingAddress = new Uri("http://localhost:8888/");
            ServiceHost callbackHost = new ServiceHost(typeof(Callback), wSDualHttpBindingAddress);
            try
            {
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                basicHttpBinding.TransferMode = TransferMode.Streamed;
                basicHttpBinding.MaxReceivedMessageSize = Int32.MaxValue;
                basicHttpBinding.MaxBufferSize = 8192;
                ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IService), basicHttpBinding, basicHttpBindingAddress);
                ServiceMetadataBehavior smbasicHttpBinding = new ServiceMetadataBehavior();
                smbasicHttpBinding.HttpGetEnabled = true;
                host.Description.Behaviors.Add(smbasicHttpBinding);
                host.Open();
                WSDualHttpBinding wSDualHttpBinding = new WSDualHttpBinding();
                ServiceEndpoint endpointCallback = callbackHost.AddServiceEndpoint(typeof(ICallback), wSDualHttpBinding, wSDualHttpBindingAddress);
                ServiceMetadataBehavior smwSDualHttpBinding = new ServiceMetadataBehavior();
                smwSDualHttpBinding.HttpGetEnabled = true;
                callbackHost.Description.Behaviors.Add(smwSDualHttpBinding);
                callbackHost.Open();
                Console.WriteLine("Service has started");
                Console.WriteLine("Press <Enter> to stop");
                Console.ReadLine();
                host.Close();
                Console.WriteLine("Service has stopped");
            }
            catch (CommunicationException e)
            {
                Console.WriteLine(e.Message);
                host.Abort();
            }
        }
    }
}