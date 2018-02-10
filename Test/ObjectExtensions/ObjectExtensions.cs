

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectExtensions
{
    public static class ObjectExtensions
    {

        public static void SetField(this object obj, string fieldName, object value)
        {
            Type type = obj.GetType();
            PropertyInfo fieldInfo = type.GetProperty(fieldName, BindingFlags.GetField | BindingFlags.NonPublic);
            fieldInfo.SetValue(obj, value);
        }

        public static void SetField(this object obj, Type fieldType, object value)
        {
            Type type = obj.GetType();
            IEnumerable<PropertyInfo> properties = type.GetProperties(BindingFlags.GetField | BindingFlags.NonPublic);
            foreach(PropertyInfo propertyInfo in properties)
            {
                if(propertyInfo.PropertyType == fieldType)
                {
                    propertyInfo.SetValue(obj, value);
                }
            }
        }
    }
}
