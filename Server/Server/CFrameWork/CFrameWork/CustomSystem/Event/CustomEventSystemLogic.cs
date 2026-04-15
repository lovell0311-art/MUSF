using System.Linq;

namespace CustomFrameWork
{
    public partial class CustomEventSystem
    {
        private void Start()
        {
            while (mStartMethodQueue.Count > 0)
            {
                mStartMethodQueue.Dequeue().Start();
            }
        }
        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            Start();

            if (mIsDataChange)
            {
                mUpdateMethodArray = mUpdateMethodDic.Values.ToArray();
                mIsDataChange = false;
            }

            for (int i = 0, len = mUpdateMethodArray.Length; i < len; i++)
            {
                if (mUpdateMethodArray[i].IsRunUpdate)
                    mUpdateMethodArray[i].Update();
            }
        }
		public void LateUpdate()
		{
			 
		}
		public void FixedUpdate()
		{
			 
		}
    }
}
