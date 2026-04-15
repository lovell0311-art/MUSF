using System.Collections.Generic;
using System.Net;

namespace ETModel
{
	public class LocationProxyComponent : Component
	{
		public IPEndPoint LocationAddress;

		public Dictionary<long, long> KeyValuePairs = new Dictionary<long, long>();


		public override void Dispose()
		{
			if (IsDisposed) return;
			KeyValuePairs.Clear();

			base.Dispose();
		}
	}
}