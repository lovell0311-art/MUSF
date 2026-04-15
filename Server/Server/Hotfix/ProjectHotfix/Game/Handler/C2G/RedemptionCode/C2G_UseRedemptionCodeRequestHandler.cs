using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_UseRedemptionCodeRequestHandler : AMActorRpcHandler<C2G_UseRedemptionCodeRequest, G2C_UseRedemptionCodeResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_UseRedemptionCodeRequest b_Request, G2C_UseRedemptionCodeResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_UseRedemptionCodeRequest b_Request, G2C_UseRedemptionCodeResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            DataCacheManageComponent mDataCacheComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var mDataCache_CodeError = mDataCacheComponent.Get<DBRCodeErrorData>();
            if (mDataCache_CodeError == null)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                mDataCache_CodeError = await mDataCacheComponent.Add<DBRCodeErrorData>(dBProxy2, p => p.UserId == mPlayer.UserId
                                                                                               && p.GameUserId == mPlayer.GameUserId);
            }
            var mRCodeErrorData = mDataCache_CodeError.OnlyOne();
            if (mRCodeErrorData == null)
            {
                DBRCodeErrorData mDBRCodeErrorData = new DBRCodeErrorData()
                {
                    Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                    UserId = mPlayer.UserId,
                    GameUserId = mPlayer.GameUserId
                };
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                bool mSaveResult = await dBProxy2.Save(mDBRCodeErrorData);
                if (mSaveResult == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3205);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("删除失败!");
                    b_Reply(b_Response);
                    return true;
                }
                mDataCache_CodeError.DataAdd(mDBRCodeErrorData);

                mRCodeErrorData = mDataCache_CodeError.OnlyOne();
            }

            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);

            if (mRCodeErrorData.ErrorCount >= 5)
            {
                if (mRCodeErrorData.TimeTick < Help_TimeHelper.GetNow())
                {
                    mRCodeErrorData.ErrorCount = 0;
                    mRCodeErrorData.TimeTick = 0;
                    var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                    mWriteDataComponent.Save(mRCodeErrorData, dBProxy2).Coroutine();
                }
                else
                {
                    //b_Response.Count = 5;
                    b_Response.TimeTick = mRCodeErrorData.TimeTick;

                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3204);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("删除失败!");
                    b_Reply(b_Response);
                    return true;
                }
            }

            void AddError(int b_ErrorId)
            {
                mRCodeErrorData.ErrorCount++;

                if (mRCodeErrorData.ErrorCount >= 5)
                {
                    //b_Response.Count = mRCodeErrorData.ErrorCount;
                    mRCodeErrorData.TimeTick = Help_TimeHelper.GetNow() + /*12 * 60 **/ 60 * 1000;

                    b_Response.TimeTick = mRCodeErrorData.TimeTick;
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3204);
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(b_ErrorId);
                }
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                mWriteDataComponent.Save(mRCodeErrorData, dBProxy2).Coroutine();
            }

            // 3201 : 兑换码无效，兑换失败！
            // 3202 : 兑换码无效，兑换失败！
            // 3203 : 兑换码已被使用！
            // 3204 : 兑换码输入错误N次，连续5次输错则12小时内无法再次输入！

            Dictionary<string, (int, int, string)> InnerRedemptionCodeDic = new Dictionary<string, (int, int, string)>()
            {
                ["qzls777"] = ((int)RedemptionCodeType.WuXianZhi, 1, "[{\"ItemConfigID\":300004,\"CreateAttr\":{\"Quantity\":1,\"IsBind\":2,\"ValidTime\":10800}}]"),
                ["qzls888"] = ((int)RedemptionCodeType.WuXianZhi, 2, "[{\"ItemConfigID\":300003,\"CreateAttr\":{\"Quantity\":1,\"IsBind\":2,\"ValidTime\":10800}}]"),
                ["qzls999"] = ((int)RedemptionCodeType.WuXianZhi, 3, "[{\"ItemConfigID\":320294,\"CreateAttr\":{\"Quantity\":50000}}]"),
                ["vip666"] = ((int)RedemptionCodeType.WuXianZhi, 4, "[{\"ItemConfigID\":320294,\"CreateAttr\":{\"Quantity\":50000}}]"),
                ["vip888"] = ((int)RedemptionCodeType.WuXianZhi, 5, "[{\"ItemConfigID\":320294,\"CreateAttr\":{\"Quantity\":50000}}]"),
                //["SVIP666"] = ((int)RedemptionCodeType.WuXianZhiByPlayer, 6, "[{\"ItemConfigID\":320508,\"CreateAttr\":{\"Quantity\":1,\"IsBind\":1}}]"),
                //["SVIP777"] = ((int)RedemptionCodeType.WuXianZhiByPlayer, 7, "[{\"ItemConfigID\":310107,\"CreateAttr\":{\"Quantity\":3,\"IsBind\":2}}]"),
                //["SVIP888"] = ((int)RedemptionCodeType.WuXianZhiByPlayer, 8, "[{\"ItemConfigID\":260008,\"CreateAttr\":{\"Quantity\":1,\"IsBind\":2}}]")
            };

            if (b_Request.RedemptionCode == null || b_Request.RedemptionCode == "")
            {
                AddError(3201);
                b_Reply(b_Response);
                return true;
            }
            var mRedemptionCode = b_Request.RedemptionCode.TrimStart().TrimEnd();

            if (InnerRedemptionCodeDic.TryGetValue(mRedemptionCode, out var mRewardInfo) == false)
            {
                if (mRedemptionCode.Length <= 3)
                {
                    AddError(3201);
                    b_Reply(b_Response);
                    return true;
                }
                var mCodeStr = mRedemptionCode.Substring(3);

                var mCodeStrInfolist = mCodeStr.Split('G');
                if (mCodeStrInfolist.Length < 3)
                {
                    AddError(3201);
                    b_Reply(b_Response);
                    return true;
                }

                var mCodeTypeStr = mCodeStrInfolist[0];
                var mRewardTypeStr = mCodeStrInfolist[1];

                string GetValidityStr(string b_SourceStr)
                {
                    var mTemplist = b_SourceStr.ToCharArray().ToList();
                    for (int i = mTemplist.Count - 1; i >= 0; i--)
                    {
                        var mTemp = mTemplist[i];
                        if (mTemp >= 'G')
                        {
                            mTemplist.RemoveAt(i);
                        }
                    }

                    return new string(mTemplist.ToArray());
                }

                var mCodeTypeTemp = GetValidityStr(mCodeTypeStr);
                var mRewardTypeTemp = GetValidityStr(mRewardTypeStr);

                int mCodeType = Convert.ToInt32(mCodeTypeTemp, 16);
                int mRewardType = Convert.ToInt32(mRewardTypeTemp, 16);

                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

                switch ((RedemptionCodeType)mCodeType)
                {
                    case RedemptionCodeType.OneType:
                    case RedemptionCodeType.WuXianZhi:
                    case RedemptionCodeType.WuXianZhiByPlayer:
                    case RedemptionCodeType.LimitCount:
                    case RedemptionCodeType.LimitCountByPlayer:
                        break;
                    default:
                        {
                            AddError(3201);
                            // 兑换码错误
                            b_Reply(b_Response);
                            return true;
                        }
                        break;
                }

                var mCodelist = await dBProxy.Query<DBRCodeData>(p => p.CodeType == mCodeType && p.RewardType == mRewardType && p.CodeStr == mRedemptionCode);
                if (mCodelist == null || mCodelist.Count <= 0)
                {
                    AddError(3202);
                    // 兑换码错误
                    b_Reply(b_Response);
                    return true;
                }

                var mCodeInstance = (mCodelist[0] as DBRCodeData);
                if (mCodeInstance == null)
                {
                    AddError(3202);
                    // 兑换码错误
                    b_Reply(b_Response);
                    return true;
                }
                if (mCodeInstance.IsDispose != 0)
                {
                    AddError(3203);
                    // 兑换码 退换过了
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3203);
                    b_Reply(b_Response);
                    return true;
                }
                mCodeInstance.DeSerialize();
                switch ((RedemptionCodeType)mCodeType)
                {
                    case RedemptionCodeType.WuXianZhi:
                        {
                            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                            var mLogCodelist = await dBProxy2.Query<DBRCodeLogData>(p => p.CodeType == mCodeType
                                                                                      && p.RewardType == mRewardType
                                                                                      && p.UserId == mPlayer.UserId
                                                                                      && p.CodeStr == mRedemptionCode);
                            if (mLogCodelist != null && mLogCodelist.Count > 0)
                            {
                                AddError(3203);
                                // 兑换码 退换过了
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3203);
                                b_Reply(b_Response);
                                return true;
                            }
                            DBRCodeLogData mDBRCodeLogData = new DBRCodeLogData()
                            {
                                Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                                CodeType = mCodeType,
                                RewardType = mRewardType,
                                UserId = mPlayer.UserId,
                                CodeStr = mRedemptionCode
                            };
                            bool mSaveResult = await dBProxy2.Save(mDBRCodeLogData);
                            if (mSaveResult == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3206);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("删除失败!");
                                b_Reply(b_Response);
                                return true;
                            }
                        }
                        break;
                    case RedemptionCodeType.WuXianZhiByPlayer:
                        {
                            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                            var mLogCodelist = await dBProxy2.Query<DBRCodeLogData>(p => p.CodeType == mCodeType
                                                                                      && p.RewardType == mRewardType
                                                                                      && p.GameUserId == mPlayer.GameUserId
                                                                                      && p.CodeStr == mRedemptionCode);
                            if (mLogCodelist != null && mLogCodelist.Count > 0)
                            {
                                AddError(3203);
                                // 兑换码 退换过了
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3203);
                                b_Reply(b_Response);
                                return true;
                            }
                            DBRCodeLogData mDBRCodeLogData = new DBRCodeLogData()
                            {
                                Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                                CodeType = mCodeType,
                                RewardType = mRewardType,
                                GameUserId = mPlayer.GameUserId,
                                CodeStr = mRedemptionCode
                            };
                            bool mSaveResult = await dBProxy2.Save(mDBRCodeLogData);
                            if (mSaveResult == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3206);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("删除失败!");
                                b_Reply(b_Response);
                                return true;
                            }
                        }
                        break;
                    case RedemptionCodeType.LimitCount:
                        {
                            if (mCodeInstance.UseCount > 0)
                            {
                                mCodeInstance.DeSerialize();
                                if (mCodeInstance.UseCount == mCodeInstance.UseIdslist.Count)
                                {
                                    AddError(3203);
                                    // 兑换码 退换过了
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3203);
                                    b_Reply(b_Response);
                                    return true;
                                }
                            }
                            if (mCodeInstance.UseIdslist.Contains(mPlayer.UserId))
                            {
                                AddError(3203);
                                // 兑换码 退换过了
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3203);
                                b_Reply(b_Response);
                                return true;
                            }
                            mCodeInstance.UseIdslist.Add(mPlayer.UserId);
                            mCodeInstance.Serialize();
                            await dBProxy.Save(mCodeInstance);
                        }
                        break;
                    case RedemptionCodeType.OneType:
                    case RedemptionCodeType.LimitCountByPlayer:
                        {
                            if (mCodeInstance.UseCount > 0)
                            {
                                mCodeInstance.DeSerialize();
                                if (mCodeInstance.UseCount == mCodeInstance.UseIdslist.Count)
                                {
                                    AddError(3203);
                                    // 兑换码 退换过了
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3203);
                                    b_Reply(b_Response);
                                    return true;
                                }
                            }
                            if (mCodeInstance.UseIdslist.Contains(mPlayer.GameUserId))
                            {
                                AddError(3203);
                                // 兑换码 退换过了
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3203);
                                b_Reply(b_Response);
                                return true;
                            }
                            mCodeInstance.UseIdslist.Add(mPlayer.GameUserId);
                            mCodeInstance.Serialize();
                            await dBProxy.Save(mCodeInstance);
                        }
                        break;
                    default:
                        break;
                }

                mRewardInfo = (mCodeInstance.CodeType, mCodeInstance.RewardType, mCodeInstance.RewardStr);
            }
            else
            {
                switch ((RedemptionCodeType)mRewardInfo.Item1)
                {
                    case RedemptionCodeType.WuXianZhi:
                        {
                            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                            var mLogCodelist = await dBProxy2.Query<DBRCodeLogData>(p => p.CodeType == mRewardInfo.Item1
                                                                                      && p.RewardType == mRewardInfo.Item2
                                                                                      && p.UserId == mPlayer.UserId
                                                                                      && p.CodeStr == mRedemptionCode);
                            if (mLogCodelist != null && mLogCodelist.Count > 0)
                            {
                                AddError(3203);
                                // 兑换码 退换过了
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3203);
                                b_Reply(b_Response);
                                return true;
                            }
                            DBRCodeLogData mDBRCodeLogData = new DBRCodeLogData()
                            {
                                Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                                CodeType = mRewardInfo.Item1,
                                RewardType = mRewardInfo.Item2,
                                UserId = mPlayer.UserId,
                                CodeStr = mRedemptionCode
                            };
                            bool mSaveResult = await dBProxy2.Save(mDBRCodeLogData);
                            if (mSaveResult == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3206);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("删除失败!");
                                b_Reply(b_Response);
                                return true;
                            }
                        }
                        break;
                    case RedemptionCodeType.WuXianZhiByPlayer:
                        {
                            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                            var mLogCodelist = await dBProxy2.Query<DBRCodeLogData>(p => p.CodeType == mRewardInfo.Item1
                                                                                      && p.RewardType == mRewardInfo.Item2
                                                                                      && p.GameUserId == mPlayer.GameUserId
                                                                                      && p.CodeStr == mRedemptionCode);
                            if (mLogCodelist != null && mLogCodelist.Count > 0)
                            {
                                AddError(3203);
                                // 兑换码 退换过了
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3203);
                                b_Reply(b_Response);
                                return true;
                            }
                            DBRCodeLogData mDBRCodeLogData = new DBRCodeLogData()
                            {
                                Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                                CodeType = mRewardInfo.Item1,
                                RewardType = mRewardInfo.Item2,
                                GameUserId = mPlayer.GameUserId,
                                CodeStr = mRedemptionCode
                            };
                            bool mSaveResult = await dBProxy2.Save(mDBRCodeLogData);
                            if (mSaveResult == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3206);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("删除失败!");
                                b_Reply(b_Response);
                                return true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();

            var mRewardlist = Help_JsonSerializeHelper.DeSerialize<List<MailItem>>(mRewardInfo.Item3);
            if (mRewardlist.Count == 0)
            {
                // 无奖励
                b_Reply(b_Response);
                return true;
            }

            #region sss
            //BackpackComponent mBackpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            //if (mBackpackComponent == null)
            //{
            //    // 兑换码错误
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
            //    b_Reply(b_Response);
            //    return true;
            //}
            //async Task<bool> LingQuDaoJu()
            //{
            //    int posX = 0;
            //    int posY = 0;
            //    bool IsOk = false;
            //    List<(Item item, int x, int y)> DropItemList = new List<(Item, int, int)>();
            //    // 锁格子用的，类似 lock_guard
            //    using var backpackLockList = ItemsBoxStatus.LockList.Create();
            //    foreach (var ItemList in mRewardlist)
            //    {
            //        Item mDropItem = null;
            //        if (ItemList.ItemID != 0)
            //        {
            //            mDropItem = ItemFactory.CreateFormDB(ItemList.ItemID, mPlayer);
            //        }
            //        else
            //        {
            //            mDropItem = ItemFactory.Create(ItemList.ItemConfigID, mPlayer.GameAreaId, ItemList.CreateAttr);
            //        }

            //        if (!mBackpackComponent.mItemBox.CheckStatus(mDropItem.ConfigData.X, mDropItem.ConfigData.Y, ref posX, ref posY))
            //        {
            //            var itemList = DropItemList.ToArray();
            //            foreach (var v in itemList)
            //            {
            //                v.item.Dispose();
            //            }
            //            mDropItem.Dispose();
            //            return false;
            //        }
            //        backpackLockList.Add(mBackpackComponent.mItemBox.LockGrid(mDropItem.ConfigData.X, mDropItem.ConfigData.Y, posX, posY));
            //        DropItemList.Add((mDropItem, posX, posY));
            //    }
            //    // 手动释放锁
            //    backpackLockList.Dispose();
            //    foreach (var v in DropItemList)
            //    {
            //        {
            //            if (mBackpackComponent.AddItem(v.item, v.x, v.y, "邮件获取") == false)
            //            {
            //                // 运行到这里，说明代码出问题了
            //                Log.Error($"角色:{mPlayer.GameUserId}道具ID:{v.item.ConfigID}数量:{v.item.GetProp(EItemValue.Quantity)} 领取失败 背包不足！！！");
            //                return false;
            //            }
            //            else
            //            {
            //                IsOk = true;
            //                Log.PLog("MailItem", $"角色:{mPlayer.GameUserId}道具ID:{v.item.ConfigID}数量:{v.item.GetProp(EItemValue.Quantity)} 领取成功");
            //            }
            //        }
            //    }
            //    if (IsOk)
            //    {
            //        mCodeInstance.IsDispose = 1;
            //        await dBProxy.Save(mCodeInstance);
            //    }
            //    return IsOk;
            //}
            #endregion

            //var mLingQuDaoJuResult = await LingQuDaoJu();
            //if (mLingQuDaoJuResult == false)
            {
                //背包没有空间了 发邮件

                MailInfo mailinfo = new MailInfo();
                mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                mailinfo.MailName = "兑换码物品领取";
                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                mailinfo.MailContent = "兑换码物品通过邮件发放。";
                mailinfo.MailState = 0;
                mailinfo.ReceiveOrNot = 0;
                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                mailinfo.MailEnclosure.AddRange(mRewardlist);

                await MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mPlayer.GameAreaId);
            }

            if (mRCodeErrorData.ErrorCount != 0)
            {
                mRCodeErrorData.ErrorCount = 0;
                mRCodeErrorData.TimeTick = 0;
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                mWriteDataComponent.Save(mRCodeErrorData, dBProxy2).Coroutine();
            }

            b_Reply(b_Response);
            return true;
        }
    }
}