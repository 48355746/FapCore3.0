using System.Collections.Generic;
using System.Collections;

namespace Fap.AspNetCore.ViewModel
{
    public class MenuViewModel : IViewModel
    {
        public List<MenuItem> Menus { get; set; }
    }
    public class MenuItem
    {
        //
        // 摘要: 
        //     获取一个值，该值指示菜单项是否是通过数据绑定创建的。
        //
        // 返回结果: 
        //     如果菜单项是通过数据绑定创建的，则为 true；否则为 false。

        public bool DataBound { get; }
        //
        // 摘要: 
        //     获取绑定到菜单项的数据项。
        //
        // 返回结果: 
        //     System.Object，表示绑定到菜单项的数据项。 默认值为 null，指示菜单项未绑定到任何数据项。

        public object DataItem { get; }
        //
        // 摘要: 
        //     获取绑定到菜单项的数据的路径。
        //
        // 返回结果: 
        //     绑定到节点的数据的路径。 此值来自 System.Web.UI.WebControls.Menu 控件绑定到的分层数据源控件。 默认值为空字符串
        //     ("")。

        public string DataPath { get; }
        //
        // 摘要: 
        //     获取菜单项的显示级别。
        //
        // 返回结果: 
        //     菜单项的显示级别。

        public int Depth { get; }
        //
        // 摘要: 
        //     获取或设置一个值，该值指示 System.Web.UI.WebControls.MenuItem 对象是否已启用，如果启用，则该项可以显示弹出图像和所有子菜单项。
        //
        // 返回结果: 
        //     如果启用菜单项，则为 true；否则为 false。

        public bool Enabled { get; set; }
        //
        // 摘要: 
        //     获取或设置显示在菜单项文本旁边的图像的 URL。
        //
        // 返回结果: 
        //     显示在菜单项文本旁边的自定义图像的 URL。 默认值为空字符串 ("")，指示尚未设置此属性。

        public string ImageUrl { get; set; }
        //
        // 摘要: 
        //     获取或设置单击菜单项时要导航到的 URL。
        //
        // 返回结果: 
        //     单击菜单项时要导航到的 URL。 默认值为空字符串 ("")，指示尚未设置此属性。

        public string NavigateUrl { get; set; }
        //
        // 摘要: 
        //     获取当前菜单项的父菜单项。
        //
        // 返回结果: 
        //     System.Web.UI.WebControls.MenuItem，表示当前菜单项的父菜单项。
   
        public MenuItem Parent { get; }
        //
        // 摘要: 
        //     获取或设置显示在菜单项中的图像的 URL，用于指示菜单项具有动态子菜单。
        //
        // 返回结果: 
        //     显示在菜单项中的图像的 URL，用于指示菜单项具有动态子菜单。 默认值为空字符串 ("")，表示尚未设置此属性。
       
        public string PopOutImageUrl { get; set; }
        //
        // 摘要: 
        //     获取或设置一个值，该值指示 System.Web.UI.WebControls.MenuItem 对象是否可选或“可单击”。
        //
        // 返回结果: 
        //     如果菜单项可选，则为 true；否则为 false。
    
        public bool Selectable { get; set; }
        //
        // 摘要: 
        //     获取或设置一个值，该值指示 System.Web.UI.WebControls.Menu 控件的当前菜单项是否已被选中。
        //
        // 返回结果: 
        //     如果 System.Web.UI.WebControls.Menu 控件的当前菜单项已选中，则为 true；否则为 false。 默认值为 false。

        public bool Selected { get; set; }
        //
        // 摘要: 
        //     获取或设置图像的 URL，该图像显示在菜单项底部，将菜单项与其他菜单项隔开。
        //
        // 返回结果: 
        //     图像的 URL，该图像用于将当前菜单项与其他菜单项隔开。

        public string SeparatorImageUrl { get; set; }
        //
        // 摘要: 
        //     获取或设置用来显示菜单项的关联网页内容的目标窗口或框架。
        //
        // 返回结果: 
        //     显示所链接的网页内容的目标窗口或框架。 默认值为空字符串 ("")，该值刷新具有焦点的窗口或框架。

        public string Target { get; set; }
        //
        // 摘要: 
        //     获取或设置 System.Web.UI.WebControls.Menu 控件中显示的菜单项文本。
        //
        // 返回结果: 
        //     System.Web.UI.WebControls.Menu 控件中的菜单项的显示文本。 默认值为空字符串 ("")。

        public string Text { get; set; }
        //
        // 摘要: 
        //     获取或设置菜单项的工具提示文本。
        //
        // 返回结果: 
        //     菜单项的工具提示文本。 默认值为空字符串 ("")。

        public string ToolTip { get; set; }
        //
        // 摘要: 
        //     获取或设置一个非显示值，该值用于存储菜单项的任何其他数据，如用于处理回发事件的数据。
        //
        // 返回结果: 
        //     菜单项的补充数据（不会显示在屏幕上）。 默认值为空字符串 ("")。

        public string Value { get; set; }
        //
        // 摘要: 
        //     获取从根菜单项到当前菜单项的路径。
        //
        // 返回结果: 
        //     由分隔符分隔的菜单项值的列表，它构成了从根菜单项到当前菜单项的路径。

        public string ValuePath { get; }
        public List<MenuItem> ChildItems { get => _childItems; set => _childItems = value; }

        private List<MenuItem> _childItems = new List<MenuItem>();
        
    }

  
}