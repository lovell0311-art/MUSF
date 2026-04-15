
using System.Text;
using System.Threading.Tasks;
namespace CustomFrameWork.Baseic
{
    /// <summary>
    /// 工厂 组件
    /// </summary>
    public abstract class CustomFactory : ACustomComponent
    {
        /// <summary>
        /// 清理工厂 组件
        /// </summary>
        public override void Dispose()
        {
            RemoveAllCustomComponent();
        }
    }
}
