using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace WcfImageServiceContract
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        List<String> getImagesList();

        [OperationContract]
        ImageMessage downloadImage(ImageNameMessage imageName);

        [OperationContract]
        DescriptionMessage downloadDescription(ImageNameMessage imageName);

        [OperationContract]
        void uploadImage(ImageMessage image);

        [OperationContract]
        void uploadDescription(DescriptionMessage descriptionMessage);
    }

    [MessageContract]
    public class ImageNameMessage
    {
        [MessageHeader] public string name;
    }

    [MessageContract]
    public class ImageMessage
    {
        [MessageHeader] public string name;
        [MessageHeader] public long size;
        [MessageBodyMember] public Stream image;
    }

    [MessageContract]
    public class DescriptionMessage
    {
        [MessageHeader] public string name;
        [MessageHeader] public long size;
        [MessageBodyMember] public Stream description;
    }
}