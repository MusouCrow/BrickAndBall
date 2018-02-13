using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

namespace Game.Component.Network {
    public class Connection : LoadBalancingClient {
        public delegate void EventCallback(byte eventCode, object content, int senderId);

        public event EventCallback OnEventCall;

        private MemoryStream stream = new MemoryStream();
        private BinaryFormatter formatter = new BinaryFormatter();
        private RaiseEventOptions allOptions = new RaiseEventOptions() {
            Receivers = ReceiverGroup.All   
        };
        private RaiseEventOptions serverOptions = new RaiseEventOptions() {
            Receivers = ReceiverGroup.MasterClient   
        };

        public bool CreateRoom() {
            return this.OpCreateRoom(null, new RoomOptions {MaxPlayers = 2}, null);
        }

        public void Send(byte code, object content, bool toServer=false) {
            var op = new RaiseEventOptions();
            op.Receivers = toServer ? ReceiverGroup.MasterClient : ReceiverGroup.All;
            //var op = toServer ? this.serverOptions : this.allOptions; 

            if (content == null) {
                this.OpRaiseEvent(code, content, true, op);
                return;
            }

            this.stream.Position = 0;
            this.stream.SetLength(0);
            this.formatter.Serialize(this.stream, content);
            this.OpRaiseEvent(code, this.stream.ToArray(), true, op);
        }

        public T Receive<T>(object content) {
            var bytes = (byte[])content;
            this.stream.SetLength(bytes.Length);
            this.stream.Write(bytes, 0, bytes.Length);
            this.stream.Position = 0;

            return (T)this.formatter.Deserialize(this.stream);
        }

        public override void OnEvent(EventData photonEvent) {
            base.OnEvent(photonEvent);
            
            if (photonEvent.Code < 200) {
                int actorNr = (int)photonEvent[ParameterCode.ActorNr];
                object content = photonEvent[ParameterCode.Data];

                if (this.OnEventCall != null) {
                    this.OnEventCall(photonEvent.Code, content, actorNr);
                }
            }
        }
    }
}

