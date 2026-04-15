
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETModel
{
    [ObjectSystem]
    public class AstarPathfinderAwake : AwakeSystem<AstarPathfinder, AstarNode, AstarNode, AstarFindCallback>
    {
        public override void Awake(AstarPathfinder self, AstarNode a, AstarNode b, AstarFindCallback c)
        {
            self.Awake(a,b,c);
        }
    }
    public class AstarPathfinder : Entity
    {
        public AstarNode startPosition;
        public AstarNode endPosition;

        public  bool IsDone = false;

        public List<AstarNode> foundPath;

        private AstarFindCallback callback;

        public void Awake(AstarNode start, AstarNode target, AstarFindCallback callback)
        {
            this.IsDone = false;
            this.startPosition = start;
            this.endPosition = target;
            this.callback = callback;
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            base.Dispose();

            this.startPosition = null;
            this.endPosition = null;
            this.callback = null;
        }

        public void FindPah()
        {
            foundPath = FindPathActual(startPosition, endPosition);
            IsDone = true;
        }

        public void NotifyComplete()
        {
            if (this.callback != null)
            {
                this.callback(foundPath);
                this.callback = null;
            }
        }

        List<AstarNode> nullPath = new List<AstarNode>();
        private List<AstarNode> FindPathActual(AstarNode start, AstarNode target)
        {
            if (target==null||!target.isWalkable)
            {
                return nullPath;
            }

            //Typical A* algorythm from here and on

            List<AstarNode> foundPath = new List<AstarNode>();

            //We need two lists, one for the nodes we need to check and one for the nodes we've already checked
            List<AstarNode> openSet = new List<AstarNode>();
            HashSet<AstarNode> closedSet = new HashSet<AstarNode>();

            //We start adding to the open set
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                AstarNode currentNode = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    //We check the costs for the current node
                    //You can have more opt. here but that's not important now
                    if (openSet[i].FCost < currentNode.FCost ||
                        (openSet[i].FCost == currentNode.FCost &&
                        openSet[i].hCost < currentNode.hCost))
                    {
                        //and then we assign a new current node
                        if (!currentNode.Equals(openSet[i]))
                        {
                            currentNode = openSet[i];
                        }
                    }
                }

                //we remove the current node from the open set and add to the closed set
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                //if the current node is the target node
                if (currentNode.Equals(target))
                {
                    //that means we reached our destination, so we are ready to retrace our path
                    foundPath = RetracePath(start, currentNode);
                    break;
                }

                //if we haven't reached our target, then we need to start looking the neighbours
                foreach (AstarNode neighbour in GetNeighbours(currentNode, true))
                {
                    if (!closedSet.Contains(neighbour))
                    {
                        //we create a new movement cost for our neighbours
                        float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                        //and if it's lower than the neighbour's cost
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            //we calculate the new costs
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, target);
                            //Assign the parent node
                            neighbour.parentNode = currentNode;
                            //And add the neighbour node to the open set
                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
            }

            //we return the path at the end
            return foundPath;
        }

        private List<AstarNode> RetracePath(AstarNode startNode, AstarNode endNode)
        {
            //Retrace the path, is basically going from the endNode to the startNode
            List<AstarNode> path = new List<AstarNode>();
            AstarNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                //by taking the parentNodes we assigned
                currentNode = currentNode.parentNode;
            }

            //then we simply reverse the list
            path.Reverse();

            return path;
        }

        private List<AstarNode> GetNeighbours(AstarNode node, bool getVerticalneighbours = false)
        {
            //This is were we start taking our neighbours
            List<AstarNode> retList = new List<AstarNode>();

            for (int x = -1; x <= 1; x++)
            {
                for (int yIndex = -1; yIndex <= 1; yIndex++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        int y = yIndex;

                        //If we don't want a 3d A*, then we don't search the y
                        if (!getVerticalneighbours)
                        {
                            y = 0;
                        }

                        if (x == 0 && y == 0 && z == 0)
                        {
                            //000 is the current node
                        }
                        else
                        {
                            AstarNode searchPos = new AstarNode();

                            //the nodes we want are what's forward/backwars,left/righ,up/down from us
                            searchPos.x = node.x + x;
                            //searchPos.y = node.y + y;
                            //searchPos.y = 0;
                            searchPos.z = node.z + z;

                            AstarNode newNode = GetNeighbourNode(searchPos, true, node);

                            if (newNode != null)
                            {
                                retList.Add(newNode);
                            }
                        }
                    }
                }
            }
            return retList;
        }
        private AstarNode GetNeighbourNode(AstarNode adjPos, bool searchTopDown, AstarNode currentNodePos)
        {
            //this is where the meat of it is
            //We can add all the checks we need here to tweak the algorythm to our heart's content
            //but first let's start from the the usual stuff you'll see in A*

            AstarNode retVal = null;

            //let's take the node from the adjacent positions we passed
            AstarNode node = GetNode(adjPos.x, 0, adjPos.z);

            //if it's not null and we can walk on it
            if (node != null && node.isWalkable)
            {
                //we can use that node
                retVal = node;
            }//if not
            else if (searchTopDown)//and we want to have 3d A* 
            {
                //then look what the adjacent node have under him
                //adjPos.y -= 1;
                AstarNode bottomBlock = GetNode(adjPos.x, 0, adjPos.z);

                //if there is a bottom block and we can walk on it
                if (bottomBlock != null && bottomBlock.isWalkable)
                {
                    retVal = bottomBlock;// we can return that
                }
                else
                {
                    //otherwise, we look what it has on top of it
                    //adjPos.y += 2;
                    AstarNode topBlock = GetNode(adjPos.x, 0, adjPos.z);
                    if (topBlock != null && topBlock.isWalkable)
                    {
                        retVal = topBlock;
                    }
                }
            }

            //if the node is diagonal to the current node then check the neighbouring nodes
            //so to move diagonally, we need to have 4 nodes walkable
            int originalX = adjPos.x - currentNodePos.x;
            int originalZ = adjPos.z - currentNodePos.z;

            if (Mathf.Abs(originalX) == 1 && Mathf.Abs(originalZ) == 1)
            {
                // the first block is originalX, 0 and the second to check is 0, originalZ
                //They need to be pathfinding walkable
                AstarNode neighbour1 = GetNode(currentNodePos.x + originalX, 0, currentNodePos.z);
                if (neighbour1 == null || !neighbour1.isWalkable)
                {
                    retVal = null;
                }

                AstarNode neighbour2 = GetNode(currentNodePos.x, 0, currentNodePos.z + originalZ);
                if (neighbour2 == null || !neighbour2.isWalkable)
                {
                    retVal = null;
                }
            }

            //and here's where we can add even more additional checks
            if (retVal != null)
            {
                //Example, do not approach a node from the left
                /*if(node.x > currentNodePos.x) {
                    node = null;
                }*/
            }

            return retVal;
        }

        private AstarNode GetNode(int x, int y, int z)
        {
            AstarNode n = null;

            lock (AstarComponent.Instance)
            {
                n = AstarComponent.Instance.GetNode(x, z);
            }
            return n;
        }

        private int GetDistance(AstarNode posA, AstarNode posB)
        {
            //We find the distance between each node
            //not much to explain here

            int distX = Mathf.Abs(posA.x - posB.x);
            int distZ = Mathf.Abs(posA.z - posB.z);
            //int distY = Mathf.Abs(posA.y - posB.y);

            if (distX > distZ)
            {
                return 14 * distZ + 10 * (distX - distZ);//+ 10 * distY;
            }

            return 14 * distX + 10 * (distZ - distX);//+ 10 * distY;
        }
        
      
    }
}
