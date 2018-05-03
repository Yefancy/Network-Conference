using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.OMCS
{
    public class TeacherCallOMCS : CallOMCS
    {
        public TeacherCallOMCS() : base()
        {          
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Initialize(string id, string password, string IP, int port)
        {
            multimediaManager.CameraVideoSize = new System.Drawing.Size(320, 240);
            multimediaManager.AutoAdjustCameraEncodeQuality = true;
            multimediaManager.CameraDeviceIndex = 0;
            multimediaManager.Initialize(id, password, IP, port);
            chatContainer = new ChatContainer(multimediaManager);
            BindingEvent();
        }

        public void createRoom(string roomID)
        {
            if (chatContainer == null)
                throw new Exception("未初始化CallOMCS类实例");
            if (CameraConnector.Connected)
                CameraConnector.Disconnect();
            if (WhiteBoardControl.Connected)
                WhiteBoardControl.Disconnect();
            multimediaManager.OutputVideo = true;
            multimediaManager.OutputAudio = true;
            chatContainer.JoinChatGroup(roomID);
            WhiteBoardControl.BeginConnect(roomID);
        }
    }
}
