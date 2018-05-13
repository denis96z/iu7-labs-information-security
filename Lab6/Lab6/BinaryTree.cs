using System;

namespace Lab6
{
    enum SearchAction
    {
        TurnLeft,
        TurnRight,
        RollBack
    }

    class BinaryTreeNode<T> where T : IComparable<T>
    {
        public T Value { get; set; }
        public BinaryTreeNode<T> LeftChild { get; set; }
        public BinaryTreeNode<T> RightChild { get; set; }

        public BinaryTreeNode(T value,
            BinaryTreeNode<T> left = null,
            BinaryTreeNode<T> right = null)
        {
            Value = value;
            LeftChild = left;
            RightChild = right;
        }
    }

    delegate void SearchAction<T>(object currentValue,
        SearchAction searchAction);

    delegate void BypassAction<T>(object currentValue,
        object leftValue, object rightValue);

    class BinaryTree<T> where T : IComparable<T>
    {
        private BinaryTreeNode<T> rootNode = null;

        public void Insert(T value)
        {
            Insert(rootNode, value);
        }

        public void Search(SearchAction<T> searchAction)
        {
            Search(rootNode, searchAction);
        }

        public void ReplaceRoot(BinaryTreeNode<T> root)
        {
            rootNode = root;
        }

        public void InOrderBypass(BypassAction<T> bypassAction)
        {
            InOrderBypass(rootNode, bypassAction);
        }

        public void PreOrderBypass(BypassAction<T> bypassAction)
        {
            PreOrderBypass(rootNode, bypassAction);
        }

        public void PostOrderBypass(BypassAction<T> bypassAction)
        {
            PostOrderBypass(rootNode, bypassAction);
        }

        private void Insert(BinaryTreeNode<T> currentNode, T value)
        {
            if (currentNode == null)
            {
                currentNode = new BinaryTreeNode<T>(value);
            }
            else
            {
                if (currentNode.Value.CompareTo(value) > 0)
                {
                    Insert(currentNode.LeftChild, value);
                }
                else
                {
                    Insert(currentNode.RightChild, value);
                }
            }
        }

        private void Search(BinaryTreeNode<T> currentNode, SearchAction<T> searchAction)
        {
            if (currentNode != null)
            { 
                searchAction.Invoke(null, SearchAction.TurnLeft);
                Search(currentNode.LeftChild, searchAction);

                searchAction.Invoke(null, SearchAction.TurnRight);
                Search(currentNode.RightChild, searchAction);

                searchAction.Invoke(currentNode.Value, SearchAction.RollBack);
            }
            else
            {
                searchAction.Invoke(null, SearchAction.RollBack);
            }
        }

        private void InOrderBypass(BinaryTreeNode<T> currentNode, BypassAction<T> bypassAction)
        {
            if (currentNode != null)
            {
                InOrderBypass(currentNode.LeftChild, bypassAction);
                InvokeBypassAction(currentNode, bypassAction);
                InOrderBypass(currentNode.RightChild, bypassAction);
            }
        }

        private void PreOrderBypass(BinaryTreeNode<T> currentNode, BypassAction<T> bypassAction)
        {
            if (currentNode != null)
            {
                InvokeBypassAction(currentNode, bypassAction);
                PreOrderBypass(currentNode.LeftChild, bypassAction);
                PreOrderBypass(currentNode.RightChild, bypassAction);
            }
        }

        private void PostOrderBypass(BinaryTreeNode<T> currentNode, BypassAction<T> bypassAction)
        {
            if (currentNode != null)
            {
                PostOrderBypass(currentNode.LeftChild, bypassAction);
                PostOrderBypass(currentNode.RightChild, bypassAction);
                InvokeBypassAction(currentNode, bypassAction);
            }
        }

        private void InvokeBypassAction(BinaryTreeNode<T> currentNode, BypassAction<T> bypassAction)
        {
            object currentValue = currentNode.Value;

            object leftValue = null;
            if (currentNode.LeftChild != null)
            {
                leftValue = currentNode.LeftChild.Value;
            }

            object rightValue = null;
            if (currentNode.RightChild != null)
            {
                rightValue = currentNode.RightChild.Value;
            }

            bypassAction.Invoke(currentValue, leftValue, rightValue);
        }
    }
}
