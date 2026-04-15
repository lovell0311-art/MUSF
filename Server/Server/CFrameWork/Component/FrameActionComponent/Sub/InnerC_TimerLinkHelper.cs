
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomFrameWork.Baseic;

namespace CustomFrameWork.Component
{
    public partial class FrameActionComponent
    {
        private enum InnerE_TwoLinkDirection
        {
            Last,
            Next
        }
        private enum InnerE_TwoLinkSearchLevel
        {
            IndexSearch,
            ValueSearch
        }

        public class InnerC_TimerLinkHelper : ADataContext
        {
            public InnerC_TimerLink NodeFirst { get; set; }
            public InnerC_TimerLink NodeEnd { get; set; }

            public int Count { get; set; }

            public int IndexLength { get; set; } = 1000;


            public void WaitAsync(Inner_FrameSyncTimer b_FrameSyncTimer)
            {
                if (NodeFirst == null)
                {
                    var mNewNode = Root.CreateBuilder.GetInstance<InnerC_TimerLink, InnerC_TimerLinkHelper>(this);
                    mNewNode.Time = b_FrameSyncTimer.NextWaitTimeTick;
                    mNewNode.FrameSyncTimerlist.Add(b_FrameSyncTimer);
                    // 以第一个为标点 间隔单位长度设置索引
                    mNewNode.SetIndexNode();

                    NodeFirst = mNewNode;
                    return;
                }
                if (b_FrameSyncTimer.NextWaitTimeTick < NodeFirst.Time)
                {
                    var mNewNode = Root.CreateBuilder.GetInstance<InnerC_TimerLink, InnerC_TimerLinkHelper>(this);
                    mNewNode.Time = b_FrameSyncTimer.NextWaitTimeTick;
                    mNewNode.FrameSyncTimerlist.Add(b_FrameSyncTimer);
                    // 首节点和尾节点默认为索引 真伪不确定
                    mNewNode.SetIndexNode();
                    AppendNodeIndex(mNewNode, NodeFirst);

                    mNewNode.NodeNext = NodeFirst;
                    NodeFirst.NodeLast = mNewNode;

                    NodeFirst = mNewNode;
                }
                else if (NodeFirst.Time == b_FrameSyncTimer.NextWaitTimeTick)
                {
                    NodeFirst.FrameSyncTimerlist.Add(b_FrameSyncTimer);
                }
                else if (NodeEnd == null)
                {// 这里要重新检查生成 不晓得第一个点下面有多少东西
                    this.Add(NodeFirst, InnerE_TwoLinkDirection.Next, b_FrameSyncTimer, InnerE_TwoLinkSearchLevel.IndexSearch, NodeFirst);
                    return;
                }
                else if (NodeEnd.Time < b_FrameSyncTimer.NextWaitTimeTick)
                {
                    var mNewNode = Root.CreateBuilder.GetInstance<InnerC_TimerLink, InnerC_TimerLinkHelper>(this);
                    mNewNode.Time = b_FrameSyncTimer.NextWaitTimeTick;
                    mNewNode.FrameSyncTimerlist.Add(b_FrameSyncTimer);
                    // 首节点和尾节点默认为索引 真伪不确定
                    mNewNode.SetIndexNode();
                    // 续上索引链
                    NodeEnd.IndexNodeNext = mNewNode;
                    mNewNode.IndexNodeLast = NodeEnd;
                    AppendNodeIndex(NodeEnd.IndexNodeLast, NodeEnd);

                    NodeEnd.NodeNext = mNewNode;
                    mNewNode.NodeLast = NodeEnd;

                    NodeEnd = mNewNode;
                }
                else if (NodeEnd.Time == b_FrameSyncTimer.NextWaitTimeTick)
                {
                    NodeEnd.FrameSyncTimerlist.Add(b_FrameSyncTimer);
                }
                else
                {
                    var mFirstTime = Math.Abs(NodeFirst.Time - b_FrameSyncTimer.NextWaitTimeTick);
                    var mEndTime = Math.Abs(NodeEnd.Time - b_FrameSyncTimer.NextWaitTimeTick);

                    if (mFirstTime <= mEndTime)
                        this.Add(NodeFirst, InnerE_TwoLinkDirection.Next, b_FrameSyncTimer, InnerE_TwoLinkSearchLevel.IndexSearch, NodeFirst);
                    else
                        this.Add(NodeEnd, InnerE_TwoLinkDirection.Last, b_FrameSyncTimer, InnerE_TwoLinkSearchLevel.IndexSearch, NodeEnd);
                }
            }

            private void AppendNodeIndex(InnerC_TimerLink b_Node, InnerC_TimerLink b_IndexNodeNext)
            {
                if (b_Node == null || b_IndexNodeNext == null) return;
                // 首节点和尾节点默认为索引 真伪不确定
                // 设置后需要检查当前首节点是否为真索引 是否需要重建索引
                // 只要当前节点和后一个节点小于单位值即可
                if (b_IndexNodeNext.Time - b_Node.Time < this.IndexLength)
                {// 取消索引判断  时间不满足 取消索引
                    b_IndexNodeNext.CancelIndex();

                    var mNodeTemp = b_IndexNodeNext.IndexNodeNext;
                    if (mNodeTemp != null)
                    {
                        b_Node.IndexNodeNext = mNodeTemp;
                        mNodeTemp.IndexNodeLast = b_Node;

                        b_IndexNodeNext.IndexNodeNext = null;
                    }
                    b_IndexNodeNext.IndexNodeLast = null;
                }
                else
                {
                    b_Node.IndexNodeNext = b_IndexNodeNext;
                    b_IndexNodeNext.IndexNodeLast = b_Node;
                }
            }

            /// <summary>
            /// 递归添加
            /// </summary>
            /// <param name="b_Node"></param>
            /// <param name="b_NodeLinkDirection"></param>
            /// <param name="b_FrameSyncTimer"></param>
            /// <param name="b_SearchLevel">搜索等级:索引搜索还是值搜索</param>
            /// <param name="b_IndexNode"> b_Node 节点所在的索引节点</param>
            private void Add(InnerC_TimerLink b_Node, InnerE_TwoLinkDirection b_NodeLinkDirection, Inner_FrameSyncTimer b_FrameSyncTimer, InnerE_TwoLinkSearchLevel b_SearchLevel, InnerC_TimerLink b_IndexNode)
            {
                // 反向只支持索引搜索 索引到位置转为正向值搜索
                switch (b_NodeLinkDirection)
                {
                    case InnerE_TwoLinkDirection.Last:
                        {
                            if (b_Node.NodeLast == null)
                            {
                                // 倒序时前置节点不应该没有 检查这里的问题 首节点已在前置逻辑检查
                                LogToolComponent.Error(" 链前后关系异常,必须检查该逻辑 !!!!!");
                                throw new Exception(" 链前后关系异常,必须检查该逻辑 !!!!!");
                            }
                            else
                            {
                                var mNodeTemp = b_Node.IndexNodeLast;
                                if (mNodeTemp == null)
                                {
                                    // 倒序时前置索引不应该没有 检查这里的问题 首节点已在前置逻辑检查
                                    //this.Add(b_Node, InnerE_TwoLinkDirection.Next, b_FrameSyncTimer, InnerE_TwoLinkSearchLevel.ValueSearch);
                                    LogToolComponent.Error(" 链前后关系异常,必须检查该逻辑 !!!!!");
                                    throw new Exception(" 链前后关系异常,必须检查该逻辑 !!!!!");
                                }
                                else if (mNodeTemp.Time == b_FrameSyncTimer.NextWaitTimeTick)
                                {
                                    mNodeTemp.FrameSyncTimerlist.Add(b_FrameSyncTimer);
                                }
                                else if (b_FrameSyncTimer.NextWaitTimeTick > mNodeTemp.Time)
                                {
                                    this.Add(mNodeTemp, InnerE_TwoLinkDirection.Next, b_FrameSyncTimer, InnerE_TwoLinkSearchLevel.ValueSearch, mNodeTemp);
                                }
                                else
                                {
                                    this.Add(mNodeTemp, b_NodeLinkDirection, b_FrameSyncTimer, InnerE_TwoLinkSearchLevel.IndexSearch, mNodeTemp);
                                }
                            }
                        }
                        break;
                    case InnerE_TwoLinkDirection.Next:
                        {
                            if (b_Node.NodeNext == null)
                            {
                                var mNewNode = Root.CreateBuilder.GetInstance<InnerC_TimerLink, InnerC_TimerLinkHelper>(this);
                                mNewNode.Time = b_FrameSyncTimer.NextWaitTimeTick;
                                mNewNode.FrameSyncTimerlist.Add(b_FrameSyncTimer);
                                // 首节点和尾节点默认为索引 真伪不确定
                                mNewNode.SetIndexNode();
                                // 续上索引链
                                b_Node.IndexNodeNext = mNewNode;
                                mNewNode.IndexNodeLast = b_Node;
                                AppendNodeIndex(b_Node.IndexNodeLast, b_Node);

                                b_Node.NodeNext = mNewNode;
                                mNewNode.NodeLast = b_Node;

                                NodeEnd = mNewNode;
                            }
                            else
                            {
                                switch (b_SearchLevel)
                                {
                                    case InnerE_TwoLinkSearchLevel.IndexSearch:
                                        {
                                            var mNodeTemp = b_Node.IndexNodeNext;
                                            if (mNodeTemp == null)
                                            {
                                                this.Add(b_Node, b_NodeLinkDirection, b_FrameSyncTimer, InnerE_TwoLinkSearchLevel.ValueSearch, b_Node);
                                            }
                                            else if (mNodeTemp.Time == b_FrameSyncTimer.NextWaitTimeTick)
                                            {
                                                mNodeTemp.FrameSyncTimerlist.Add(b_FrameSyncTimer);
                                            }
                                            else if (b_FrameSyncTimer.NextWaitTimeTick < mNodeTemp.Time)
                                            {// 插入 比下一个节点时间靠前
                                                this.Add(b_Node, b_NodeLinkDirection, b_FrameSyncTimer, InnerE_TwoLinkSearchLevel.ValueSearch, b_Node);
                                            }
                                            else
                                            {
                                                this.Add(mNodeTemp, b_NodeLinkDirection, b_FrameSyncTimer, InnerE_TwoLinkSearchLevel.IndexSearch, mNodeTemp);
                                            }
                                        }
                                        break;
                                    case InnerE_TwoLinkSearchLevel.ValueSearch:
                                        {
                                            var mNodeTemp = b_Node.NodeNext;

                                            if (mNodeTemp.Time == b_FrameSyncTimer.NextWaitTimeTick)
                                            {
                                                mNodeTemp.FrameSyncTimerlist.Add(b_FrameSyncTimer);
                                            }
                                            else if (b_FrameSyncTimer.NextWaitTimeTick < mNodeTemp.Time)
                                            {// 插入 比下一个节点时间靠前
                                                var mNewNode = Root.CreateBuilder.GetInstance<InnerC_TimerLink, InnerC_TimerLinkHelper>(this);
                                                mNewNode.Time = b_FrameSyncTimer.NextWaitTimeTick;
                                                mNewNode.FrameSyncTimerlist.Add(b_FrameSyncTimer);

                                                if (mNewNode.Time - b_IndexNode.Time >= this.IndexLength)
                                                {
                                                    mNewNode.SetIndexNode();

                                                    var mIndexNodeTemp = b_IndexNode.IndexNodeNext;
                                                    if (mIndexNodeTemp != null)
                                                    {
                                                        // 续上索引链
                                                        mNewNode.IndexNodeNext = mIndexNodeTemp;
                                                        mIndexNodeTemp.IndexNodeLast = mNewNode;

                                                        if (mNewNode.IndexNodeNext.InstanceId != NodeEnd.InstanceId)
                                                        {// end 节点不能检查索引
                                                            AppendNodeIndex(mNewNode, mNewNode.IndexNodeNext);
                                                        }
                                                    }
                                                    mNewNode.IndexNodeLast = b_IndexNode;
                                                    b_IndexNode.IndexNodeNext = mNewNode;
                                                }

                                                b_Node.NodeNext = mNewNode;
                                                mNewNode.NodeLast = b_Node;

                                                mNewNode.NodeNext = mNodeTemp;
                                                mNodeTemp.NodeLast = mNewNode;
                                            }
                                            else
                                            {
                                                this.Add(mNodeTemp, b_NodeLinkDirection, b_FrameSyncTimer, InnerE_TwoLinkSearchLevel.ValueSearch, b_IndexNode);
                                            }
                                        }
                                        break;
                                    default:
                                        LogToolComponent.Error("未实现 该逻辑, 请前往实现 ! 此为必须实现逻辑块!!!!!");
                                        throw new Exception("未实现 该逻辑, 请前往实现 ! 此为必须实现逻辑块!!!!!");
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        LogToolComponent.Error("未实现 该逻辑, 请前往实现 ! 此为必须实现逻辑块!!!!!");
                        throw new Exception("未实现 该逻辑, 请前往实现 ! 此为必须实现逻辑块!!!!!");
                        break;
                }
            }
        }
    }
}
