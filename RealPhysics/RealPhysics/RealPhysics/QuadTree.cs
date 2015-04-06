using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RealPhysics
{
    public class QuadTree
    {
        private QuadNode rootNode;
        private RectangleF wholeArea;
        private List<InPlay> misfits = new List<InPlay>();

        public QuadTree(RectangleF screen, int minimumDepth)
        {
            wholeArea = screen;
            rootNode = new QuadNode(null, wholeArea, minimumDepth, 1, "root");
        }

        public List<RectangleF> quadQueue()
        {
            List<RectangleF> queue = new List<RectangleF>();
            rootNode.queueUp(queue);
            return queue;
        }

        public List<InPlay> queueUp()
        {
            List<InPlay> list = new List<InPlay>();
            rootNode.queueUpInPlays(list);
            return list;
        }

        public void add(InPlay obj)
        {
            rootNode.add(obj);
        }

        public void traverse(NodeDelegate del, bool movementChange)
        {
            List<QuadNode> queue = new List<QuadNode>();
            queue.Add(rootNode);
            for(int a = 0; a < queue.Count; a++)
            {
                QuadNode node = queue.ElementAt(a);
                node.traverse(del, queue, movementChange);
            }
        }

        public void traverse(ObjectDelegate del, bool movementChange)
        {
            List<QuadNode> queue = new List<QuadNode>();
            queue.Add(rootNode);
            for (int a = 0; a < queue.Count; a++)
            {
                QuadNode node = queue.ElementAt(a);
                node.traverse(del, queue, movementChange);
            }
        }
    }
}
