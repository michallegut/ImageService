using System.ServiceModel;

namespace WcfImageServiceContract
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICallbackHandler))]
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void createDescription(ImageNameMessage imageName);
    }

    public interface ICallbackHandler
    {
        [OperationContract(IsOneWay = true)]
        void createDescriptionCallback();
    }
}