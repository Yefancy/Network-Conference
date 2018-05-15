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
            multimediaManager.OutputVideo = false;
            multimediaManager.OutputAudio = false;
            chatContainer = new ChatContainer(multimediaManager);
            BindingEvent();
        }

        public void createRoom(string roomID,string teacherID)
        {
            ExitRoom();
            multimediaManager.OutputVideo = true;
            multimediaManager.OutputAudio = true;
            chatContainer.JoinChatGroup(roomID, teacherID);
            chatContainer.IsWhiteBoardWatchingOnly = false;
        }
    }
}
