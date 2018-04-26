namespace Mock.Data
{
    using System;
    using System.Collections.Generic;
    
    internal class Trie
    {
        TrieNode root = new TrieNode((char)0);
        public void Add(string word)
        {
            TrieNode node = root;
            foreach (char c in word)
            {
                node = node.AddChild(c);
            }
            node.AddChild((char)0);
        }
        public bool Exist(string word)
        {
            TrieNode node = root;
            foreach (char c in word)
            {
                node = node.GetChild(c);
                if (node == null) return false;
            }
            return node.GetChild((char)0) != null;
        }
        public bool Exist(char c, ref object token)
        {
            TrieNode node = (token as TrieNode ?? this.root).GetChild(c);
            token = node;
            return node != null && node.Terminated;
        }

        class TrieNode
        {
            private char value;
            private SortedList<char, TrieNode> childNodes = new SortedList<char, TrieNode>();

            public TrieNode(char c)
            {
                this.value = c;
            }
            public TrieNode GetChild(char c)
            {
                TrieNode node = null;
                childNodes.TryGetValue(c, out node);
                return node;
            }
            public TrieNode AddChild(char c)
            {
                TrieNode node = GetChild(c);
                if (node == null)
                {
                    this.childNodes.Add(c, node = new TrieNode(c));
                }
                return node;
            }
            public bool Terminated { get { return this.childNodes.ContainsKey((char)0); } }
        }
    }
}
