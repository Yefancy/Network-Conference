using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.OMCS
{
    public class StudentCallOMCS : CallOMCS
    {
        public StudentCallOMCS() : base()
        {            
        }

        public override void Dispose()
        {
            base.Dispose();            
        }

        public override void Initialize(string id, string password, string IP, int port)
        {
            multimediaManager.AutoAdjustCameraEncodeQuality = false;
            multimediaManager.CameraDeviceIndex = -1;            
            multimediaManager.Initialize(id, password, IP, port);
            chatContainer = new ChatContainer(multimediaManager);
            multimediaManager.OutputVideo = false;
            multimediaManager.OutputAudio = false;
            BindingEvent();
        }

    }
}
