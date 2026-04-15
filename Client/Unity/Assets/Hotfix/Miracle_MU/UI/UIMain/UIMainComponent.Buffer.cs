using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// Buffer模块
    /// </summary>
    public partial class UIMainComponent
    {
        public Transform buffContent;
        public Transform buff;
        Dictionary<long, GameObject> buffDic = new Dictionary<long, GameObject>();
       public Dictionary<long, string> dicbuff = new Dictionary<long, string>();
        List<int> buffids = new List<int>();
        bool isInitBuffer = false;

        public void Init_Buffer()
        {

            buffContent = ReferenceCollector_Main.GetGameObject("Buffers").transform;
            //buff = buffContent.GetChild(0);
            //if (buff.gameObject.activeSelf==false)
            //{

            //    buff.gameObject.SetActive(true);
            //}
            //buff.gameObject.SetActive(false);
            isInitBuffer = true;
            UIMainComponent.Instance.dicbuff.Clear();
            Log.DebugBrown("初始化buff"+dicbuff.Count);
        }

        //public void ReshUi()
        //{
        //    if (buffContent.transform.childCount > 0)
        //    {
        //        for (int i = 0; i < buffContent.transform.childCount; i++)
        //        {
        //            if (i > 0)
        //            {
        //                buffContent.transform.GetChild(i).gameObject.SetActive(true);
        //            }
        //        }
        //    }
        //    Log.DebugBrown("buff孩子长度" + buffContent.transform.childCount);
        //}


        /// <summary>
        /// 显示buffer图标
        /// </summary>
        /// <param name="bufferId"></param>
        public void ShowBuff(long bufferId, string bufferIconName, string bufferName)
        {
          //  Log.Info("UIMainComponentBuffer --ShowBuff -- " + isInitBuffer + " bufferId =  " + bufferId + " bufferIconName= " + bufferIconName+"::数据量"+buffDic.Count+ ":::isInitBuffer"+ isInitBuffer);
            if (isInitBuffer == false) return;
            if (!UIMainComponent.Instance.dicbuff.ContainsKey(bufferId))
            {
                UIMainComponent.Instance.dicbuff.Add(bufferId, bufferIconName);
            }
            try
            {
             //   Log.Info("buff记录的数量"+ UIMainComponent.Instance.dicbuff.Count);
                //  if (SpriteUtility.Instance.GetAtlasSprite(AtalsType.UI_BufferIcons, bufferIconName) == null) return;
                UIMainComponent.Instance.buffContent.gameObject.SetActive(true);
                for (int i = 0; i < UIMainComponent.Instance.buffContent.childCount; i++)
                {
                    UIMainComponent.Instance.buffContent.GetChild(i).gameObject.SetActive(false);
                }
                for (int i = 0; i < UIMainComponent.Instance.dicbuff.Count; i++)
                {
                    UIMainComponent.Instance.buffContent.GetChild(i).gameObject.SetActive(true);
                }
                int count = 0;
                foreach (var items in UIMainComponent.Instance.dicbuff)
                {
                    UIMainComponent.Instance.buffContent.GetChild(count).GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.UI_BufferIcons, items.Value);
                    // buffContent.GetChild(count).GetComponent<Text>().text=
                    count++;
                }
                //   GameObject item = UnityEngine.GameObject.Instantiate(this.buff.gameObject, ReferenceCollector_Main.GetGameObject("Buffers").transform, false);
                //   Log.DebugBrown("000000000");
                //   if (item == null) return;
                //   item.transform.localScale = Vector3.one;
                //   Log.DebugBrown("11111111111");
                ////   Log.DebugBrown($"Buffer图标：{bufferIconName} {SpriteUtility.Instance.GetAtlasSprite(AtalsType.UI_BufferIcons, bufferIconName) == null}");
                //   if (SpriteUtility.Instance.GetAtlasSprite(AtalsType.UI_BufferIcons, bufferIconName) == null) return;
                //   Log.DebugBrown("2222222");
                //   item.GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.UI_BufferIcons, bufferIconName);
                //   Log.DebugBrown("3333333");
                //   item.transform.GetChild(0).GetComponent<Text>().text = bufferName;
                //   /* if (bufferRef.GetSprite(bufferIconName) == null) return;
                //    item.GetComponent<Image>().sprite = bufferRef.GetSprite(bufferIconName);*/
                //   Log.DebugBrown("4444444444");
                //   buffDic[bufferId] = item;
                //  item.SetActive(true);
                //if (buffDic.TryGetValue(bufferId, out GameObject buff))
                //{
                //    Log.Info("-0");
                //    buff.SetActive(true);
                //}
                //else
                //{

                //    Log.Info("-1");
                //    GameObject item = UnityEngine.GameObject.Instantiate(this.buff.gameObject, buffContent, false);
                //    if (item == null) return;
                //    item.transform.localScale = Vector3.one;
                //    Log.DebugBrown($"Buffer图标：{bufferIconName} {SpriteUtility.Instance.GetAtlasSprite(AtalsType.UI_BufferIcons, bufferIconName) == null}");
                //    if (SpriteUtility.Instance.GetAtlasSprite(AtalsType.UI_BufferIcons, bufferIconName) == null) return;
                //    item.GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.UI_BufferIcons, bufferIconName);

                //    item.transform.GetChild(0).GetComponent<Text>().text = bufferName;
                //    /* if (bufferRef.GetSprite(bufferIconName) == null) return;
                //     item.GetComponent<Image>().sprite = bufferRef.GetSprite(bufferIconName);*/

                //    buffDic[bufferId] = item;
                //    item.SetActive(true);
                //}
            }
            catch (System.Exception e)
            {


            }
        }
        /// <summary>
        /// 隐藏buffer图标
        /// </summary>
        /// <param name="buffId"></param>
        public void HideBuff(long buffId)
        {
            Log.Info("HideBuff--- " + buffId);
            if (buffDic.TryGetValue(buffId, out GameObject buff))
            {
                if (buff == null) return;
                buff.SetActive(false);
            }
        }

        public void ClearBuffer()
        {
            Log.Info("ClearBuffer");
            foreach (var item in buffDic.Values)
            {
                item.SetActive(false);
            }
        }
    }
}