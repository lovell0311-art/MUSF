

using System.Threading.Tasks;
namespace ETModel
{
	public interface IMailboxHandler
	{
		Task Handle(Session session, Entity entity, object actorMessage);
	}
}