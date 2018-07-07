using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceLib
{
    /// <summary>
    /// 接口导出类的注释
    /// </summary>
    [AttributeUsage(validOn:AttributeTargets.Class)]
    public class ExportAttribute : Attribute
    {

    }
}
