using System;
using System.Threading.Tasks;
using CustomFrameWork;

using pbc = global::Google.Protobuf.Collections;

namespace ETModel
{
    public interface IMessage
    {
    }

    public interface IMHandler
    {
        Task Handle(Session session, object message);
        Type GetMessageType();
    }
    public abstract class AMHandler<Request> : IMHandler
        where Request : class
    {
        public async Task Handle(Session session, object msg)
        {
            Request message = msg as Request;
            if (message == null)
            {
                Log.Error($"消息类型转换错误: {msg.GetType().Name} to {typeof(Request).Name}");
                return;
            }
            if (session.IsDisposed)
            {
                Log.Error($"session disconnect {msg}");
                return;
            }

            try
            {
                await this.BeforeCodeAsync(session, message);
            }
            catch (Exception e)
            {
                string debugJson = "";
                try
                {
                    debugJson = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                }
                catch (Exception e2)
                {
                    Log.Error(e2);
                }
                Log.Error(debugJson,e);
            }
        }
        protected async virtual Task<bool> BeforeCodeAsync(Session b_Connect, Request b_Request)
        {
            return await NextCodeAsync(b_Connect, b_Request);
        }
        protected async virtual Task<bool> NextCodeAsync(Session b_Connect, Request b_Request)
        {
            return await CodeAsync(b_Connect, b_Request);
        }
        protected async virtual Task<bool> CodeAsync(Session b_Connect, Request b_Request)
        {
            return await Run(b_Request);
        }
        protected async virtual Task<bool> Run(Request b_Request) { return true; }
        public Type GetMessageType()
        {
            return typeof(Request);
        }
    }

    public interface IRequest : IMessage
    {
        int RpcId { get; set; }
    }

    public interface IResponse : IMessage
    {
        int Error { get; set; }
        string Message { get; set; }
        int RpcId { get; set; }
    }

    public class ErrorResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
    }
    public abstract class AMRpcHandler<Request, Response> : IMHandler
        where Request : class, IRequest
        where Response : class, IResponse
    {
        public async Task Handle(Session session, object message)
        {
            try
            {
                Request request = message as Request;
                if (request == null)
                {
                    Log.Error($"消息类型转换错误: {message.GetType().Name} to {typeof(Request).Name}");
                }

                Response response = Activator.CreateInstance<Response>();
                response.RpcId = request.RpcId;

                long instanceId = session.InstanceId;
                void Reply(IMessage b_Response)
                {
                    // 等回调回来,session可以已经断开了,所以需要判断session InstanceId是否一样
                    if (session.InstanceId != instanceId)
                    {
                        if (b_Response is IResponse staleResponse)
                        {
                            Log.Warning($"#RpcReply# skipped session={session.Id} remote={session.RemoteAddress} type={b_Response.GetType().Name} rpc={staleResponse.RpcId} error={staleResponse.Error} reason=InstanceChanged");
                        }
                        return;
                    }
                    if (b_Response is IResponse rpcResponse)
                    {
                        Log.Info($"#RpcReply# send session={session.Id} remote={session.RemoteAddress} type={b_Response.GetType().Name} rpc={rpcResponse.RpcId} error={rpcResponse.Error}");
                        session.Reply(rpcResponse);
                        return;
                    }

                    Log.Info($"#RpcReply# send session={session.Id} remote={session.RemoteAddress} type={b_Response.GetType().Name}");
                    session.Send(b_Response);
                }

                try
                {
                    await this.BeforeCodeAsync(session, request, response, Reply);
                }
                catch (Exception e)
                {
                    string debugJson = "";
                    try
                    {
                        debugJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    }
                    catch (Exception e2)
                    {
                        Log.Error(e2);
                    }
                    Log.Error(debugJson,e);
                    response.Error = ErrorCode.ERR_RpcFail;
                    response.Message = e.ToString();
                    Reply(response);
                }

            }
            catch (Exception e)
            {
                Log.Error($"解释消息失败: {message.GetType().FullName}\n{e}");
            }
        }
        protected async virtual Task<bool> BeforeCodeAsync(Session b_Connect, Request b_Request, Response b_Response, Action<IMessage> b_Reply)
        {
            return await NextCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
        }
        protected async virtual Task<bool> NextCodeAsync(Session b_Connect, Request b_Request, Response b_Response, Action<IMessage> b_Reply)
        {
            return await CodeAsync(b_Connect, b_Request, b_Response, b_Reply);
        }
        protected async virtual Task<bool> CodeAsync(Session b_Connect, Request b_Request, Response b_Response, Action<IMessage> b_Reply)
        {
            return await Run(b_Request, b_Response, b_Reply);
        }
        protected async virtual Task<bool> Run(Request b_Request, Response b_Response, Action<IMessage> b_Reply) { return true; }

        public Type GetMessageType()
        {
            return typeof(Request);
        }
    }


    public interface IActorMessage : IMessage
    {
        long ActorId { get; set; }
    }

    public interface IActorListMessage : IActorMessage
    {
        pbc::RepeatedField<long> ActorIdList { get; set; }
    }

    public interface IActorRequest : IRequest
    {
        long ActorId { get; set; }
        long AppendData { get; set; }
    }

    public interface IActorResponse : IResponse
    {

    }

    public abstract class AMActorRpcHandler<Request, Response> : IMHandler
        where Request : class, IActorRequest
        where Response : class, IActorResponse
    {
        public async Task Handle(Session session, object actorMessage)
        {
            try
            {
                Request request = actorMessage as Request;
                if (request == null)
                {
                    Log.Error($"消息类型转换错误: {actorMessage.GetType().FullName} to {typeof(Request).Name}");
                    return;
                }

                Response response = Activator.CreateInstance<Response>();
                response.RpcId = request.RpcId;

                long instanceId = session.InstanceId;
                void Reply(IMessage b_Response)
                {
                    // 等回调回来,session可以已经断开了,所以需要判断session InstanceId是否一样
                    if (session.InstanceId != instanceId)
                    {
                        return;
                    }
                    session.Send(b_Response);
                }

                try
                {
                    await this.BeforeCodeAsync(session, request, response, Reply);
                }
                catch (Exception exception)
                {
                    string debugJson = "";
                    try
                    {
                        debugJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    }
                    catch(Exception e2)
                    {
                        Log.Error(e2);
                    }
                    Log.Error(debugJson, exception);
                    response.Error = ErrorCode.ERR_RpcFail;
                    response.Message = exception.ToString();
                    Reply(response);
                }
            }
            catch (Exception e)
            {
                Log.Error($"解释消息失败: {actorMessage.GetType().FullName}\n{e}");
            }
        }
        protected async virtual Task<bool> BeforeCodeAsync(Session b_Connect, Request b_Request, Response b_Response, Action<IMessage> b_Reply)
        {
            return await NextCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
        }
        protected async virtual Task<bool> NextCodeAsync(Session b_Connect, Request b_Request, Response b_Response, Action<IMessage> b_Reply)
        {
            return await CodeAsync(b_Connect, b_Request, b_Response, b_Reply);
        }
        protected async virtual Task<bool> CodeAsync(Session b_Connect, Request b_Request, Response b_Response, Action<IMessage> b_Reply)
        {
            return await Run(b_Request, b_Response, b_Reply);
        }
        protected async virtual Task<bool> Run(Request b_Request, Response b_Response, Action<IMessage> b_Reply) { return true; }

        public Type GetMessageType()
        {
            return typeof(Request);
        }
    }
}
