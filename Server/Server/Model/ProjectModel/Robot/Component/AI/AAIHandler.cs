using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETModel.Robot
{
    public class AIHandlerAttribute : BaseAttribute
    {
    }

    public abstract class AAIHandler
    {
        // 检查是否满足条件
        public abstract int Check(AIComponent aiComponent, AI_Config aiConfig);

        // 协程编写必须可以取消
        public abstract Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken);
    }
}
