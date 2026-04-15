using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{

    /// <summary>
    /// 聊天数据
    /// </summary>
    public static class ChatMessageDataManager
    {
        public static List<ChatMessage> AllMessage = new List<ChatMessage>();//全部信息
        public static List<ChatMessage> WorldMessage = new List<ChatMessage>();//世界信息
        public static List<ChatMessage> FullSuitMessage = new List<ChatMessage>();//全服信息
        public static List<ChatMessage> NearMessage = new List<ChatMessage>();//附近信息
        public static List<ChatMessage> FamilyMessage = new List<ChatMessage>();//家族信息
        public static List<ChatMessage> TeamMessage = new List<ChatMessage>();//队伍信息
        public static List<ChatMessage> PrivateMessage = new List<ChatMessage>();//私聊信息
        public static List<ChatMessage> WorldData = new List<ChatMessage>();//缓存数据
        public static Dictionary<E_ChatType, long> valuePairs = new Dictionary<E_ChatType, long>()
        {
            { E_ChatType.World, -1},
            { E_ChatType.Team, -1},
            { E_ChatType.Family, -1}
        };

        /// <summary>
        /// 缓存消息
        /// </summary>
        /// <param name="chat">消息</param>
        /// <param name="type">消息类型</param>
        public static void AddChatMessage(ChatMessage chat)
        {
          //  ClearChatMeesage();
            switch ((E_ChatType)chat.curRoonID)
            {
                case E_ChatType.World:
                    WorldMessage.Add(chat);
                    if (WorldMessage.Count >= 30)
                    {
                        WorldMessage.RemoveAt(0);
                    }
                    break;
                case E_ChatType.FullSuit:
                    WorldData.Add(chat);//缓存所有的全部数据
                    FullSuitMessage.Clear();
                    //if (WorldData.Count >= 30)
                    //{
                    //    WorldData.RemoveAt(0);
                    //}
                    //if (FullSuitMessage.Count>0)
                    //{
                    //    FullSuitMessage.RemoveAt(0);//清掉
                    //}
                    for (int i = 0; i < WorldData.Count; i++)//获取所有的数据
                    {
                        if (WorldData[i].curLine == GlobalDataManager.EnterLineID)//判断是否和自己是同一线路
                        {
                            FullSuitMessage.Add(WorldData[i]);//添加
                        }
                    }
                    if (FullSuitMessage.Count >= 30)
                    {
                        FullSuitMessage.RemoveAt(0);
                    }

                    break;
                case E_ChatType.Near:
                    NearMessage.Add(chat);
                    if (NearMessage.Count >= 30)
                    {
                        NearMessage.RemoveAt(0);
                    }
                    break;
                case E_ChatType.Family:
                    FamilyMessage.Add(chat);
                    if (FamilyMessage.Count >= 30)
                    {
                        FamilyMessage.RemoveAt(0);
                    }
                    break;
                case E_ChatType.Team:
                    TeamMessage.Add(chat);
                    if (TeamMessage.Count >= 30)
                    {
                        TeamMessage.RemoveAt(0);
                    }
                    break;
                case E_ChatType.PrivateChat:
                    PrivateMessage.Add(chat);
                    if (PrivateMessage.Count >= 30)
                    {
                        PrivateMessage.RemoveAt(0);
                    }
                    break;
                default:
                    break;
            }
            if((E_ChatType)chat.curRoonID != E_ChatType.World)
                AllMessage.Add(chat);
            if (AllMessage.Count >= 100)
            {
                AllMessage.RemoveAt(0);
            }
        }
        public static void ClearChatMeesage()
        {
            AllMessage.Clear();
            WorldMessage.Clear();
            FullSuitMessage.Clear();
            NearMessage.Clear();
            FamilyMessage.Clear();
            TeamMessage.Clear();
            PrivateMessage.Clear();
        }
        public static string GetChatType(E_ChatType type) => type switch
        {
            E_ChatType.World => "<color=green>[系统]</color>",
            E_ChatType.FullSuit => "<color=green>[全服]</color>",
            E_ChatType.Near => "<color=yellow>[附近]</color>",
            E_ChatType.Family => "<color=blue>[战盟]</color>",
            E_ChatType.Team => "<color=white>[队伍]</color>",
            E_ChatType.PrivateChat => "<color=red>[私聊]</color>",
            E_ChatType.FullSuit1 => "<color=green>[世界]</color>",
            _ => string.Empty
        };
        /// <summary>
        /// 根据当前所选的聊天频道 获取对应的聊天信息集合
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<ChatMessage> GetCurMessageLis(this int self) => self switch
        {
            1 => WorldMessage,
            2 => AllMessage,
            3 => FullSuitMessage,
            4 => NearMessage,
            5 => FamilyMessage,
            6 => TeamMessage,
            7 => PrivateMessage,
            _ => AllMessage
        };

    }

    public class ChatMessage
    {
        public long chatRoomID;              // 聊天房间Id
        public long sendGameUserId;          // 发送消息的实体GameUserId
        public string sendUserName;          // 发送消息的实体昵称
        public long sendTime;                // 消息发送时间 时间戳 秒
        public string chatMessage;           // 聊天消息
        public long SendGameUserId;	         // 发送消息的实体GameUserId
        public int curRoonID;                // 当前聊天房间
        public int curLine;                  // 当前线路
        public ChatMessageType chatType = ChatMessageType.NormalChat;     // 消息类别
        public int ccolor = -1;              // 消息颜色
        public long itemId = 0;              //装备id，不是装备就为0 
    }
    public enum ChatMessageType
    {
        NormalChat,     //普通消息
        ItemChat        //装备消息
    }
}