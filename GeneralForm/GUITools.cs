using System;
using System.Reflection;
using System.Windows.Forms;

namespace GeneralForm
{
    //动态设置控件的值及反射技术应用
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
