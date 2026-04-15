using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETModel
{
    /// <summary>
    /// 数据库代理管理器
    /// </summary>
    public class DBProxyManagerComponent : TCustomComponent<MainFactory>
    {
        public Dictionary<DBType, Dictionary<int, DBProxyComponent>> DBCacheDic = new Dictionary<DBType, Dictionary<int, DBProxyComponent>>();

        public override void Awake()
        {

        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            // 清理缓存
            if (DBCacheDic != null)
            {
                if (DBCacheDic.Count > 0)
                {
                    var temps = DBCacheDic.Values.ToList();
                    for (int i = 0, len = temps.Count; i < len; i++)
                    {
                        var temp = temps[i];

                        var temp2s = temp.Values.ToList();
                        for (int j = 0, jlen = temp2s.Count; j < jlen; j++)
                        {
                            var temp2 = temp2s[j];

                            temp2.Dispose();
                        }
                    }
                    DBCacheDic.Clear();
                }
            }

            base.Dispose();
        }
    }
}