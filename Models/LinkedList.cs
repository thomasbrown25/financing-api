using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Models
{
    public class Node
    {
        public Node(int value)
        {
            Value = value;
            Next = null;
        }
        public int Value { get; set; }
        public Node Next { get; set; }
        public Node Prev { get; set; }
    }

    public class LinkedList
    {
        Node Head;
        public LinkedList Insert(LinkedList list, int value)
        {
            var newNode = new Node(value);

            if (list.Head == null)
            {
                list.Head = newNode;
            }
            else
            {
                Node lastNode = list.Head;
                while (lastNode.Next != null)
                {
                    lastNode = lastNode.Next;
                }
                lastNode.Next = newNode;
            }

            return list;
        }
    }
}