using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ETModel
{
    /// <summary>
    /// 文本超链接
    /// </summary>
    [AddComponentMenu("UI/UIHyperlinkText")]
    public class UIHyperlinkText : Text, IPointerClickHandler
    {
        /// <summary>
        /// 超链接信息类
        /// 超链接写法： <a href=event:testUrl><color=#fce644>超链接1</color></a> 
        /// </summary>
        private class HyperlinkInfo
        {
            //起始Index
            public int StartIndex;
            //结束Index
            public int EndIndex;
            //内容
            public string RefValue;
            public string InnerValue;
            //颜色
            public Color Color;
            //包围框
            public List<Rect> BoxList = new List<Rect>();
        }
        #region 私有变量
        //超链接正则
        private static Regex hrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);
        //颜色正则
        private static Regex colorRegex = new Regex(@"<color=([^>\n\s]+)>(.*?)(</color>)", RegexOptions.Singleline);
        //超链接信息列表
        private List<HyperlinkInfo> hyperlinkInfoList = new List<HyperlinkInfo>();

        private static Action<string, string> clickCallback = null;
        private string fixedText = string.Empty;
        private static Color innerTextColor = Color.blue;
        #endregion

        #region 公有变量
      
        #endregion

        #region 生命周期
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);

            InitHyperlinkInfo();
            InitHyperlinkBox(toFill);
            //DrawUnderLine(toFill);
            
        }

        #endregion

        #region 公有方法

        #endregion

        #region 动作
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);

            foreach (HyperlinkInfo hyperlinkInfo in hyperlinkInfoList)
            {
                var boxeList = hyperlinkInfo.BoxList;
                for (var i = 0; i < boxeList.Count; ++i)
                {
                    if (boxeList[i].Contains(localPoint))
                    {
                        if (clickCallback != null)
                        {
                            clickCallback(hyperlinkInfo.RefValue, hyperlinkInfo.InnerValue);
                        }
                        return;
                    }
                }
            }
        }

        public void RegisterClickCallback(Action<string, string> callback)
        {
            clickCallback = callback;
        }

        public void SetLinkColor(string innerColor)
        {
            if (!innerColor.StartsWith("#"))
            {
                innerColor = "#" + innerColor;
            }

            Color nowColor;
            ColorUtility.TryParseHtmlString(innerColor, out nowColor);
            innerTextColor = nowColor;
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// HEX转换成RGB，并返回color实例
        /// </summary>
        private Color HexToRgb(string sColor)
        {
            float r = (float)Convert.ToInt32("0x" + sColor.Substring(1, 2), 16) / 255;
            float g = (float)Convert.ToInt32("0x" + sColor.Substring(3, 2), 16) / 255;
            float b = (float)Convert.ToInt32("0x" + sColor.Substring(5, 2), 16) / 255;
            return new Color(r, g, b, 1);
        }

        /// <summary>
        /// 初始化连接信息
        /// </summary>
        private void InitHyperlinkInfo()
        {
            fixedText = GetOutputText(text);
        }

        /// <summary>
        /// 初始化连接包围框
        /// </summary>
        /// <param name="toFill"></param>
        private void InitHyperlinkBox(VertexHelper toFill)
        {
            // var orignText = m_Text;
            // m_Text = fixedText;
            // base.OnPopulateMesh(toFill);
            // m_Text = orignText;

            UIVertex vert = new UIVertex();

            // 处理超链接包围框
            foreach (var hrefInfo in hyperlinkInfoList)
            {
                hrefInfo.BoxList.Clear();

                //一个字符是四个顶点，所以Index要乘以4
                int startVertex = hrefInfo.StartIndex * 4;
                int endVertex = hrefInfo.EndIndex * 4;

                if (startVertex >= toFill.currentVertCount)
                {
                    continue;
                }

                // 将超链接里面的文本顶点索引坐标加入到包围框
                toFill.PopulateUIVertex(ref vert, startVertex);

                var pos = vert.position;
                var bounds = new Bounds(pos, Vector3.zero);

                for (int i = startVertex; i < endVertex; i++)
                {
                    if (i >= toFill.currentVertCount)
                    {
                        break;
                    }

                    toFill.PopulateUIVertex(ref vert, i);

                    vert.color = hrefInfo.Color;   // 在这里修改超链接颜色
                    toFill.SetUIVertex(vert, i);

                    pos = vert.position;

                    bool needEncapsulate = true;

                    if ((i - startVertex) != 0 && (i - startVertex) % 4 == 0)
                    {
                        UIVertex lastV = new UIVertex();
                        toFill.PopulateUIVertex(ref lastV, i - 4);
                        var lastPos = lastV.position;

                        if (pos.x < lastPos.x && pos.y < lastPos.y) // 换行重新添加包围框
                        {
                            hrefInfo.BoxList.Add(new Rect(bounds.min, bounds.size));
                            bounds = new Bounds(pos, Vector3.zero);
                            needEncapsulate = false;
                        }
                    }

                    if (needEncapsulate)
                    {
                        bounds.Encapsulate(pos); // 扩展包围框
                    }
                }

                hrefInfo.BoxList.Add(new Rect(bounds.min, bounds.size));
            }
        }

        private void DrawUnderLine(VertexHelper vh)
        {
            foreach (var link in hyperlinkInfoList)
            {
                foreach (var rect in link.BoxList)
                {
                    float height = rect.height;
                    // 左下
                    var pos1 = new Vector3(rect.min.x, rect.min.y, 0);
                    // 右下
                    var pos2 = new Vector3(rect.max.x, rect.max.y, 0) - new Vector3(0, height, 0);

                   
                    MeshUnderLine(vh, pos1, pos2, link.Color);
                }
            }
        }

        private void MeshUnderLine(VertexHelper vh, Vector2 startPos, Vector2 endPos, Color linkColor)
        {
            Vector2 extents = rectTransform.rect.size;
            var setting = GetGenerationSettings(extents);

            TextGenerator underlineText = new TextGenerator();
            underlineText.Populate("—", setting);

            IList<UIVertex> lineVer = underlineText.verts;/*new UIVertex[4];*///"_"的的顶点数组

            Vector3[] pos = new Vector3[4];
            pos[0] = startPos + new Vector2(-8, 0);
            pos[3] = startPos + new Vector2(-8, -4f);
            pos[2] = endPos + new Vector2(8, -4f);
            pos[1] = endPos + new Vector2(8, 0);


            UIVertex[] tempVerts = new UIVertex[4];
            for (int i = 0; i < 4; i++)
            {
                tempVerts[i] = lineVer[i];
                tempVerts[i].color = linkColor;
                tempVerts[i].position = pos[i];
                tempVerts[i].uv0 = lineVer[i].uv0;
                tempVerts[i].uv1 = lineVer[i].uv1;
                tempVerts[i].uv2 = lineVer[i].uv2;
                tempVerts[i].uv3 = lineVer[i].uv3;
            }

            vh.AddUIVertexQuad(tempVerts);
        }

        /// <summary>
        /// 获取超链接解析后的最后输出文本
        /// </summary>
        /// <returns></returns>
        private string GetOutputText(string outputText)
        {
            StringBuilder stringBuilder = new StringBuilder();

            hyperlinkInfoList.Clear();

            int strIndex = 0;
            // 普通带颜色的文本替换成不带颜色的
            MatchCollection colorMatches = colorRegex.Matches(outputText);
            MatchCollection hrefMatches = hrefRegex.Matches(outputText);
            bool isMatch = false;
            foreach (Match cMatch in colorMatches)
            {
                isMatch = false;
                foreach (Match hMatch in hrefMatches)
                {
                    if (hMatch.Groups[2].Value.Contains(cMatch.Groups[2].Value))
                    {
                        isMatch = true;
                        break;
                    }
                }

                if (isMatch == false)
                {
                    // 暴力替换
                    outputText = outputText.Replace("<color=" + cMatch.Groups[1].Value + ">" + cMatch.Groups[2].Value + "</color>",
                        cMatch.Groups[2].Value);
                }
            }

            foreach (Match match in hrefRegex.Matches(outputText))
            {
                string appendStr = outputText.Substring(strIndex, match.Index - strIndex);
                stringBuilder.Append(appendStr);

                //空格和回车没有顶点渲染，所以要去掉
                stringBuilder = stringBuilder.Replace(" ", "");
                stringBuilder = stringBuilder.Replace("\n", "");

                int startIndex = stringBuilder.Length;

                //第一个是连接url,第二个是连接文本，跳转用url，计算index用文本
                Group urlGroup = match.Groups[1];
                Group titleGroup = match.Groups[2];

                //如果有Color语法嵌套，则还要继续扒，直到把最终文本扒出来
                Match colorMatch = colorRegex.Match(titleGroup.Value);

                if (colorMatch.Groups.Count > 3)
                {
                    titleGroup = colorMatch.Groups[2];
                }
                Group colorGroup = colorMatch.Groups[1];

                //stringBuilder.Append($"<color={GetHyperlinkColor()}>");
                stringBuilder.Append(titleGroup.Value);
                //stringBuilder.Append("</color>");

                HyperlinkInfo hyperlinkInfo = new HyperlinkInfo
                {
                    StartIndex = startIndex,
                    EndIndex = (startIndex + titleGroup.Length),
                    RefValue = urlGroup.Value,
                    InnerValue = titleGroup.Value,
                    Color = colorGroup.Value == "" ? innerTextColor : HexToRgb(colorGroup.Value)
                };

                strIndex = match.Index + match.Length;
                hyperlinkInfoList.Add(hyperlinkInfo);
            }
            stringBuilder.Append(outputText.Substring(strIndex, outputText.Length - strIndex));
            return stringBuilder.ToString();
        }
        #endregion

      
    }
}