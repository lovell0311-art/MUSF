namespace ETModel
{
	public interface IActorLocationMessage : IActorRequest
	{
	}

	public interface IActorLocationRequest : IActorRequest
	{
	}
	
	public interface IActorLocationResponse : IActorResponse
	{
	}

	public interface IActorLocation2Message : IActorRequest
	{
	}
	public interface IActorLocation2Request : IActorLocationRequest
	{
	}

	public interface IActorLocation2Response : IActorLocationResponse
	{
	}
}