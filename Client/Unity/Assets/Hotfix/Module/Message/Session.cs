using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
using MongoDB.Bson;
using UnityEngine.Profiling;

namespace ETHotfix
{
	[ObjectSystem]
	public class SessionAwakeSystem : AwakeSystem<Session, ETModel.Session>
	{
		private static bool ShouldSuppressReconnectOnDispose()
		{
			if (GlobalDataManager.ChangeSceneIsChooseRole)
			{
				return true;
			}

			string currentSceneName = SceneComponent.Instance?.CurrentSceneName;
			if (string.Equals(currentSceneName, SceneName.ChooseRole.ToString(), StringComparison.Ordinal))
			{
				return true;
			}

			UIComponent uiComponent = Game.Scene?.GetComponent<UIComponent>();
			if (uiComponent == null)
			{
				return false;
			}

			return uiComponent.Get(UIType.UIChooseRole) != null || uiComponent.Get(UIType.UICreatRole) != null;
		}

		public override void Awake(Session self, ETModel.Session session)
		{
           // Profiler.BeginSample("SessionAwakeSystem =================JSONParse");
            self.session = session;
			SessionCallbackComponent sessionComponent = self.session.GetComponent<SessionCallbackComponent>();

			sessionComponent ??= self.session.AddComponent<SessionCallbackComponent>();
            self.opcodeTypeComponent = Game.Scene.GetComponent<OpcodeTypeComponent>();
            self.messageDispatcherComponent = Game.Scene.GetComponent<MessageDispatcherComponent>();
            long sessionId = session.Id;
            sessionComponent.MessageCallback = (s, opcode, memoryStream) => { self.Run(s, opcode, memoryStream); };
			sessionComponent.DisposeCallback = s => 
			{
				LogCollectionComponent.Instance?.Info($"#Session# Session Dispose session.Id:{sessionId}");
                self.Dispose();
				if (GlobalDataManager.IsOFFLINE)
				{
					return;
				}
				if (GlobalDataManager.IsLoginOut)
				{
                    return;
                }
				if (ShouldSuppressReconnectOnDispose())
				{
					GlobalDataManager.IsStartReConnect = false;
					LoginStageTrace.Append($"SessionDispose skip-reconnect session={sessionId} chooseRole={GlobalDataManager.ChangeSceneIsChooseRole} scene={SceneComponent.Instance?.CurrentSceneName}");
					return;
				}
				if (GlobalDataManager.IsStartReConnect==false && Game.Scene.GetComponent<SessionHelper>() is SessionHelper sessionHelper)
				{
					GlobalDataManager.IsStartReConnect = true;
					TimerComponent.Instance.RegisterTimeCallBack(1000, () => { sessionHelper.ForceTryReConnect(); });
				}

            };
          //  Profiler.EndSample();

        }
	}

	/// <summary>
	/// 用来收发热更层的消息
	/// </summary>
	public class Session: Entity
	{
		private static readonly HashSet<ushort> LoginTraceOpcodes = new HashSet<ushort>
		{
			HotfixInitOpcode.R2C_RegisterOrLoginResponse,
			HotfixInitOpcode.R2C_GetLastLoginToTheRegion,
			HotfixInitOpcode.G2C_LoginGateResponse,
		};

		private static readonly HashSet<ushort> ExtraTraceOpcodes = new HashSet<ushort>
		{
			HotfixInitOpcode.G2C_StartGameGamePlayerResponse,
			HotfixInitOpcode.G2C_ReadyResponse,
			HotfixBattleOpcode.G2C_MovePos_notice,
			HotfixOpcode.G2C_LoadingComplete,
			HotfixOpcode.G2C_ItemsIntoBackpack_notice,
			HotfixOpcode.G2C_ItemsPropChange_notice,
			HotfixOpcode.G2C_ItemsAttrEntryChange_notice,
			HotfixOpcode.G2C_OpenSkillGroup_notice,
			HotfixWarAllianceOpcode.G2C_OpenWarAllianceResponse,
			HotfixTaskOpcode.G2C_AllUpdateGameTaskNotice,
		};

		private static readonly HashSet<string> ExtraTraceRequests = new HashSet<string>
		{
			nameof(C2G_StartGameGamePlayerRequest),
			nameof(C2G_ReadyRequest),
			nameof(C2G_EnterChatRoom),
			nameof(C2G_OpenWarAllianceRequest),
			nameof(C2G_RepairEquipItemRequest),
		};

		public ETModel.Session session;

		private static int RpcId { get; set; }
		private readonly Dictionary<int, Action<IResponse>> requestCallback = new Dictionary<int, Action<IResponse>>();
		private readonly Dictionary<int, string> requestNames = new Dictionary<int, string>();
		public OpcodeTypeComponent opcodeTypeComponent;
		public MessageDispatcherComponent messageDispatcherComponent;

        private readonly int mainThreadId = Thread.CurrentThread.ManagedThreadId;

		private static bool IsLoginTraceRequest(IRequest request)
		{
			return request is C2R_RegisterOrLoginRequest
				|| request is C2R_GetLastLoginToTheRegion
				|| request is C2G_LoginGateRequest;
		}

		private static bool IsLoginTraceOpcode(ushort opcode)
		{
			return LoginTraceOpcodes.Contains(opcode);
		}

		private static bool ShouldTraceRequest(IRequest request)
		{
			return IsLoginTraceRequest(request) || ExtraTraceRequests.Contains(request.GetType().Name);
		}

		private static bool ShouldTraceOpcode(ushort opcode)
		{
			return IsLoginTraceOpcode(opcode) || ExtraTraceOpcodes.Contains(opcode);
		}

		private string PendingRpcSummary()
		{
			try
			{
				int callbackCount = this.requestCallback?.Count ?? 0;
				int nameCount = this.requestNames?.Count ?? 0;
				if (callbackCount == 0 && nameCount == 0)
				{
					return "none";
				}

				return $"callbacks={callbackCount},names={nameCount}";
			}
			catch (Exception e)
			{
				return $"summaryError={e.GetType().Name}";
			}
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
            
           

            base.Dispose();

			if (this.requestCallback.Count > 0)
			{
				Log.Info($"#ClientHotfixCall# stage=Dispose session={this.session?.Id} remote={this.session?.RemoteAddress} error={this.session?.Error} pending=[{this.PendingRpcSummary()}]");
				LoginStageTrace.Append($"SessionDispose session={this.session?.Id} remote={this.session?.RemoteAddress} error={this.session?.Error} pending=[{this.PendingRpcSummary()}]");
			}

			Action<IResponse>[] callbacks = new Action<IResponse>[this.requestCallback.Count];
			this.requestCallback.Values.CopyTo(callbacks, 0);
			foreach (Action<IResponse> action in callbacks)
			{
				action.Invoke(new ResponseMessage { Error = this.session.Error });
			}

			this.requestCallback.Clear();
			this.requestNames.Clear();
			this.session.Dispose();
           
			
		}

		public void Run(ETModel.Session s, ushort opcode, MemoryStream memoryStream)
		{
			this.opcodeTypeComponent ??= Game.Scene.GetComponent<OpcodeTypeComponent>();
			this.messageDispatcherComponent ??= Game.Scene.GetComponent<MessageDispatcherComponent>();
		//	OpcodeTypeComponent opcodeTypeComponent = Game.Scene.GetComponent<OpcodeTypeComponent>();
			object instance = opcodeTypeComponent.GetInstance(opcode);
			if (ShouldTraceOpcode(opcode))
			{
				Log.Info($"#ClientHotfixRecv# stage=Enter opcode={opcode} session={this.session.Id} remote={this.session.RemoteAddress} hasInstance={instance != null} pending=[{this.PendingRpcSummary()}]");
				LoginStageTrace.Append($"RecvEnter opcode={opcode} session={this.session.Id} remote={this.session.RemoteAddress} hasInstance={instance != null} pending=[{this.PendingRpcSummary()}]");
			}

			if (instance == null)
			{
				if (ShouldTraceOpcode(opcode))
				{
					Log.Error($"#ClientHotfixRecv# stage=MissingInstance opcode={opcode} session={this.session.Id} remote={this.session.RemoteAddress}");
					LoginStageTrace.Append($"RecvMissingInstance opcode={opcode} session={this.session.Id} remote={this.session.RemoteAddress}");
				}
				return;
			}
			object message = this.session.Network.MessagePacker.DeserializeFrom(instance, memoryStream);
			//Log.DebugBrown($"派发消息：opcode：{opcode} message：{message == null} ");
			
            IResponse response = message as IResponse;
            //Log.DebugWhtie($"message as IResponse：{message==null}");
            if (response == null)
            {
				if (ShouldTraceOpcode(opcode))
				{
					Log.Info($"#ClientHotfixRecv# stage=DecodedNonResponse opcode={opcode} type={message?.GetType().Name ?? "null"} session={this.session.Id}");
					LoginStageTrace.Append($"RecvNonResponse opcode={opcode} type={message?.GetType().Name ?? "null"} session={this.session.Id}");
				}
               // Game.Scene.GetComponent<MessageDispatcherComponent>().Handle(session, new MessageInfo(opcode, message));
                messageDispatcherComponent.Handle(session, new MessageInfo(opcode, message));
                return;
            }

			if (ShouldTraceOpcode(opcode))
			{
				Log.Info($"#ClientHotfixRecv# stage=DecodedResponse opcode={opcode} type={message.GetType().Name} rpc={response.RpcId} error={response.Error} session={this.session.Id} hasCallback={this.requestCallback.ContainsKey(response.RpcId)} pending=[{this.PendingRpcSummary()}]");
				LoginStageTrace.Append($"RecvResponse opcode={opcode} type={message.GetType().Name} rpc={response.RpcId} error={response.Error} session={this.session.Id} hasCallback={this.requestCallback.ContainsKey(response.RpcId)} pending=[{this.PendingRpcSummary()}]");
			}

            Action<IResponse> action;
            if (!this.requestCallback.TryGetValue(response.RpcId, out action))
            {
				if (ShouldTraceOpcode(opcode))
				{
					Log.Error($"#ClientHotfixRecv# stage=CallbackMissing opcode={opcode} type={message.GetType().Name} rpc={response.RpcId} error={response.Error} session={this.session.Id} pending=[{this.PendingRpcSummary()}]");
					LoginStageTrace.Append($"RecvCallbackMissing opcode={opcode} type={message.GetType().Name} rpc={response.RpcId} error={response.Error} session={this.session.Id} pending=[{this.PendingRpcSummary()}]");
				}
                throw new Exception($"not found rpc, response message: {StringHelper.MessageToStr(response)}");
            }
            this.requestCallback.Remove(response.RpcId);
			this.requestNames.Remove(response.RpcId);
			if (ShouldTraceOpcode(opcode))
			{
				Log.Info($"#ClientHotfixRecv# stage=CallbackInvoke opcode={opcode} rpc={response.RpcId} error={response.Error} session={this.session.Id} pendingAfterRemove=[{this.PendingRpcSummary()}]");
				LoginStageTrace.Append($"RecvCallbackInvoke opcode={opcode} rpc={response.RpcId} error={response.Error} session={this.session.Id} pendingAfterRemove=[{this.PendingRpcSummary()}]");
			}
            action(response);
        }

		public void Send(IMessage message)
		{
			//ushort opcode = Game.Scene.GetComponent<OpcodeTypeComponent>().GetOpcode(message.GetType());
			ushort opcode = opcodeTypeComponent.GetOpcode(message.GetType());
			this.Send(opcode, message);
        }

		public void Send(ushort opcode, IMessage message)
		{
			if (OpcodeHelper.IsNeedDebugLogMessage(opcode))
			{
				//Log.Msg(message);
			}
			//Log.DebugGreen($"是否已经掉线：{session.IsDisposed}");
			if(Thread.CurrentThread.ManagedThreadId != mainThreadId)
			{
                LogCollectionComponent.Instance.Error($"非主线程({mainThreadId})调用: {message.GetType().FullName} ThreadId:{Thread.CurrentThread.ManagedThreadId}");
            }
            session.Send(opcode, message);
            
		}

		public ETTask<IResponse> Call(IRequest request)
		{
			int rpcId = ++RpcId;
			var tcs = new ETTaskCompletionSource<IResponse>();
			string requestName = request.GetType().Name;
			bool traceCall = ShouldTraceRequest(request);
			if (traceCall)
			{
				Log.Info($"#ClientHotfixCall# stage=Register request={requestName} rpc={rpcId} session={this.session.Id} remote={this.session.RemoteAddress} pendingBefore=[{this.PendingRpcSummary()}]");
				LoginStageTrace.Append($"CallRegister request={requestName} rpc={rpcId} session={this.session.Id} remote={this.session.RemoteAddress} pendingBefore=[{this.PendingRpcSummary()}]");
			}
			this.requestNames[rpcId] = requestName;
			//向字典压入响应请求的处理方法
			//方法要当做参数保存 肯定是使用委托
			//而以后要调用方法 实际就是执行委托
			this.requestCallback[rpcId] = (response) =>
			{
				if (traceCall)
				{
					Log.Info($"#ClientHotfixCall# stage=Callback request={requestName} rpc={rpcId} responseType={response?.GetType().Name ?? "null"} error={response?.Error ?? -1} session={this.session.Id}");
					LoginStageTrace.Append($"CallCallback request={requestName} rpc={rpcId} responseType={response?.GetType().Name ?? "null"} error={response?.Error ?? -1} session={this.session.Id}");
				}
				try
				{
					if (ErrorCode.IsRpcNeedThrowException(response.Error))
					{
                     //   Log.DebugWhtie($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}  == {rpcId}  {response.Error} response.Message:{response.Message}");
						if (!tcs.TrySetException(new Exception($"Rpc Error: {request.GetType().FullName}", new RpcException(response.Error, response.Message))) && traceCall)
						{
							LoginStageTrace.Append($"CallLateException request={requestName} rpc={rpcId} error={response.Error} session={this.session.Id}");
						}
						return;
					}
                    //返回await等待的地方 继续执行下半部分的代码
                    //或者说 执行缓存了await下半部分的代码,并传递返回的网络数据反序列化后得到的proto对象

					if (!tcs.TrySetResult(response) && traceCall)
					{
						LoginStageTrace.Append($"CallLateResponse request={requestName} rpc={rpcId} error={response.Error} session={this.session.Id}");
					}
				}
				catch (Exception e)
				{
					if (!tcs.TrySetException(new Exception($"Rpc Error: {request.GetType().FullName}", e)) && traceCall)
					{
						LoginStageTrace.Append($"CallLateFault request={requestName} rpc={rpcId} type={e.GetType().Name} session={this.session.Id}");
					}
				}
			};

			request.RpcId = rpcId;
			if (traceCall)
			{
				Log.Info($"#ClientHotfixCall# stage=Send request={requestName} rpc={rpcId} session={this.session.Id} remote={this.session.RemoteAddress}");
				LoginStageTrace.Append($"CallSend request={requestName} rpc={rpcId} session={this.session.Id} remote={this.session.RemoteAddress}");
			}
            this.Send(request);
            //返回task
            return tcs.Task;
		}

		public ETTask<IResponse> Call(IRequest request, CancellationToken cancellationToken)
		{
			int rpcId = ++RpcId;
			var tcs = new ETTaskCompletionSource<IResponse>();

			this.requestCallback[rpcId] = (response) =>
			{
				try
				{
					if (ErrorCode.IsRpcNeedThrowException(response.Error))
					{
						throw new RpcException(response.Error, response.Message);
					}

					tcs.SetResult(response);
				}
				catch (Exception e)
				{
					tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
				}
			};

			cancellationToken.Register(() =>
			{
				this.requestCallback.Remove(rpcId);
				this.requestNames.Remove(rpcId);
			});

			request.RpcId = rpcId;

			this.Send(request);
			return tcs.Task;
		}
	}
}
