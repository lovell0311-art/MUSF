using System.Net;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class InnerConfig : AConfigComponent
    {
        [BsonIgnore]
        public IPEndPoint IPEndPoint { get; private set; }
        [BsonIgnore]
        public IPEndPoint IPEndPoint2 { get; private set; }

        public string Address { get; set; }
        [BsonIgnore]
        public string Address2 { get; set; } = "";
        public override void EndInit()
        {
            this.IPEndPoint = NetworkHelper.ToIPEndPoint(this.Address);

            if (Address2 != "")
            {
                this.IPEndPoint2 = NetworkHelper.ToIPEndPoint(this.Address2);
            }
            else
            {
                this.IPEndPoint2 = null;
            }
        }

        public override void Dispose()
        {
            if (IsDisposed) return;

            Address = "";
            Address2 = "";
            IPEndPoint = null;
            IPEndPoint2 = null;

            base.Dispose();
        }
    }
}