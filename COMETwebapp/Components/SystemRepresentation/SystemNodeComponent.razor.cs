﻿namespace COMETwebapp.Components.SystemRepresentation
{
    using AntDesign;

    public partial class SystemNodeComponent
    {
        Tree<GameElement> tree;

        string selectedKey;

        GameElement selectedData;

        TreeNode<GameElement> selectedNode;

        List<GameElement> games = new()
        {
        new ("0-0","0-0")
        {
            Items = new List<GameElement>()
            {
                new ("0-0-0","0-0-0")
                {
                    Items = new List<GameElement>()
                    {
                        new ("0-0-0-0","0-0-0-0"),
                        new ("0-0-0-1","0-0-0-1"),
                        new ("0-0-0-2","0-0-0-2"),
                    }
                },
                new ("0-0-1","0-0-1")
                {
                   Items = new List<GameElement>()
                   {
                       new ("0-0-1-0","0-0-1-0"),
                       new ("0-0-1-1","0-0-1-1"),
                       new ("0-0-1-2","0-0-1-2"),
                   }
                },
                new ("0-0-2","0-0-2")
                {
                   Items = new List<GameElement>()
                   {
                       new ("0-0-2-0","0-0-2-0"),
                       new ("0-0-2-1","0-0-2-1"),
                       new ("0-0-2-2","0-0-2-2"),
                   }
                 },
            }
        },
        new ("0-1","0-1")
        {
            Items = new List<GameElement>()
            {
                new ("0-1-0","0-1-0"),
                new ("0-1-1","0-1-0"),
                new ("0-1-2","0-1-2"),
            }
        },
        new ("0-2","0-2")
        {
            Items = new List<GameElement>()
            {
                new ("0-2-0","0-2-0"),
                new ("0-2-1","0-2-1"),
            }
        }
    };

        private record GameElement(string Id, string Text, string Icon = null)
        {
            public List<GameElement> Items { get; set; } = new List<GameElement>();
        }

        void onDrop(TreeEventArgs<GameElement> e)
        {

        }
    }
}
