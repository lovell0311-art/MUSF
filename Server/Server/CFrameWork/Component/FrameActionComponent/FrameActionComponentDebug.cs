
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomFrameWork.Baseic;

namespace CustomFrameWork.Component
{
    public partial class FrameActionComponent
    {
        public void debu(string tag)
        {
            int id = 0;
            var sdsdsd = _NodeLink.NodeFirst;
            while (sdsdsd != null)
            {
                id++;

                //if (sdsdsd.NodeNext != null)
                //    System.Console.WriteLine($"{tag}  id:{sdsdsd.Time} next:{sdsdsd.NodeNext.Time}  差距:{sdsdsd.NodeNext.Time - sdsdsd.Time}");
                //else
                //    System.Console.WriteLine($"{tag}  id:{sdsdsd.Time}");

                if (sdsdsd.NodeNext != null)
                    System.Console.WriteLine($"{tag}  id:{sdsdsd.InstanceId} next:{sdsdsd.NodeNext.InstanceId}  差距:{sdsdsd.NodeNext.Time - sdsdsd.Time}");
                else
                    System.Console.WriteLine($"{tag}  id:{sdsdsd.InstanceId}");

                sdsdsd = sdsdsd.NodeNext;
            }
        }
        public void debugindex(string tag)
        {
            int id = 0;
            var sdsdsd = _NodeLink.NodeFirst;
            while (sdsdsd != null)
            {
                id++;

                //if (sdsdsd.NodeNext != null)
                //    System.Console.WriteLine($"{tag}  id:{sdsdsd.Time} next:{sdsdsd.NodeNext.Time}  差距:{sdsdsd.NodeNext.Time - sdsdsd.Time}");
                //else
                //    System.Console.WriteLine($"{tag}  id:{sdsdsd.Time}");

                if (sdsdsd.IndexNodeNext != null)
                    System.Console.WriteLine($"索引 {tag}  id:{sdsdsd.InstanceId} next:{sdsdsd.IndexNodeNext.InstanceId}  差距:{sdsdsd.IndexNodeNext.Time - sdsdsd.Time}");
                else
                    System.Console.WriteLine($"索引 {tag}  id:{sdsdsd.InstanceId}");

                sdsdsd = sdsdsd.IndexNodeNext;
            }
        }
    }
}
