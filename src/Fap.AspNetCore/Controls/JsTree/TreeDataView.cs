using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Controls
{
    public class TreeDataView
    {
        public string Id { get; set; }
        public string Pid { get; set; }
        public string Text { get; set; }
        /// <summary>
        /// 其他数据存储Tag
        /// </summary>
        public dynamic Data { get; set; }
        //默认值
        private string _icon = "icon-folder  ace-icon fa fa-folder blue";

        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
        public NodeState State { get; set; }
        public string Li_attr { get; set; }
        public string A_attr { get; set; }
        public dynamic Owner { get; set; }

        private List<TreeDataView> _children = new List<TreeDataView>();
        public List<TreeDataView> Children
        {
            get
            {
                return _children;
            }
            set
            {
                _children = value;
            }
        }

    }
    public class TreeViewHelper
    {
        public static List<TreeDataView> MakeTree(List<TreeDataView> resultList, List<TreeDataView> orginalList, string parentid = "", int depth = 0)
        {
            List<TreeDataView> list;
            //获取所有分类
            if (parentid == "0" || string.IsNullOrWhiteSpace(parentid))
            {
                list = orginalList.FindAll(c => c.Pid == parentid || c.Pid == "" || string.IsNullOrWhiteSpace(c.Pid) || c.Pid == "#" || c.Pid == "~");
            }
            else
            {
                list = orginalList.FindAll(c => c.Pid == parentid);
            }
            //循环读取取出的所有分类
            foreach (var node in list)
            {
                //把遍历后的分类添加到目标树中
                resultList.Add(node);

                //递归操作
                MakeTree(node.Children, orginalList, node.Id, depth + 1);
            }

            //返回树供下一次递归使用
            return resultList;
        }
    }
    public class NodeState
    {
        public bool Opened { get; set; }
        public bool Disabled { get; set; }
        public bool Selected { get; set; }

    }
}
