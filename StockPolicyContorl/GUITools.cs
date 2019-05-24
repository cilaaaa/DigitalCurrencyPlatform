using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace StockPolicyContorl
{
    public class GUITools
    {
        public static void DoubleBuffer(Control control, bool setting)
        {
            //获取类型
            Type controlType = control.GetType();
            //使用反射技术给对象的属性或控件赋值
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            //给对应属性赋值
            pi.SetValue(control, setting, null);
        }
    }
}
