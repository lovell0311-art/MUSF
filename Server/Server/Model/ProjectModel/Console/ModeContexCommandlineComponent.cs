using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using System.Threading.Tasks;


namespace ETModel
{
    public abstract class C_ConsoleCommandLine : ADataContext
    {
        public virtual async Task Run(ModeContexCommandlineComponent b_Component, string b_Contex, string b_ConsoleCommandLine)
        {
            if (b_Component == null)
            {
                await Run(b_Contex);
            }
            else
            {
                await Run(b_Component, b_ConsoleCommandLine);
            }
        }
        public virtual async Task Run(string b_Contex)
        {

        }
        public virtual async Task Run(ModeContexCommandlineComponent b_Component, string b_Contex)
        {

        }
    }
    public class ModeContexCommandlineComponent : TCustomComponent<ConsoleComponent>
    {
        public string Contex { get; set; }
    }
}
