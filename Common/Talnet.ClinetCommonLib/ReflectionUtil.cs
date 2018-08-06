using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Talent.ClientCommonLib
{
    public sealed class ReflectionUtil
    {

        private ReflectionUtil()
        {


        }

        public static BindingFlags bf = BindingFlags.DeclaredOnly | BindingFlags.Public |
                                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public static object InvokeMethod(object obj, string methodName, object[] args)
        {

            object objReturn = null;
            Type type = obj.GetType();
            objReturn = type.InvokeMember(methodName, bf | BindingFlags.InvokeMethod, null, obj, args);
            return objReturn;

        }

        public static void SetField(object obj, string name, object value)
        {

            FieldInfo fi = obj.GetType().GetField(name, bf);
            fi.SetValue(obj, value);

        }

        public static object GetField(object obj, string name)
        {

            FieldInfo fi = obj.GetType().GetField(name, bf);
            return fi.GetValue(obj);

        }

        public static object GetProperty(object obj, string name)
        {

            PropertyInfo fi = obj.GetType().GetProperty(name);
            return fi.GetValue(obj,null);

        }

    }
}
