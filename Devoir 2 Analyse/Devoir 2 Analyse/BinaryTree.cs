using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devoir_2_Analyse
{
    class Node
    {
        public string value;
        public Node leftChild;
        public Node rightChild;

        public Node() { }
        public Node(string value) { this.value = value; }
    }
    class BinaryTree
    {
        Node head;
        Node current;
        public BinaryTree()
        {
            head = new Node();
            current = head;
        }

        public void InsertLeft(string value)
        {
            current.leftChild = new Node(value);            
        }

        public void InsertRight(string value)
        {
            current.rightChild = new Node(value);
            current = current.rightChild;
        }
        public void setCurrentValue(string value)
        {
            current.value = value;
        }
        
        public Node getHead()
        {
            return head;
        }

        public Node getCurrent()
        {
            return current;
        }
    }
}
