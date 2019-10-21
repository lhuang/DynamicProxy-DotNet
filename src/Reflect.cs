using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Dynamic
{
    public static class Reflect
    {
        public static object GetByName(this object target, string prop)
        {
            if (target.IsDictionary())
            {
                return (target as Dictionary<string, object>)[prop];
            }
            else if (target.IsList())
            {
                return (target as List<object>)[int.Parse(prop)];
            }
            else if (target.IsExpando())
            {
                var dict = target as IDictionary<string, Object>;
                return dict[prop];
            }
            else
            {
                foreach (var info in target?.GetType().GetProperties())
                {
                    if (info.Name == prop)
                    {
                        return info.GetValue(target);
                    }
                }
            }

            return null;
        }
        public static void SetByName(this object target, string prop, object value)
        {
            if (target.IsDictionary())
            {
                (target as Dictionary<string, object>)[prop] = value;
            }
            else if (target.IsList())
            {
                (target as List<object>)[int.Parse(prop)] = value;
            }
            else if (target.IsExpando())
            {
                var dict = target as IDictionary<string, Object>;
                if (dict.ContainsKey(prop))
                    dict[prop] = value;
                else
                    dict.Add(prop, value);
            }
            else
            {
                foreach (var info in target?.GetType().GetProperties())
                {
                    if (info.Name == prop)
                    {
                        info.SetValue(target, value);
                    }
                }
            }
        }
        public static object GetByPath(this object target, string path)
        {
            var paths = path.Split('.');
            Type type = target.GetType();
            foreach (var prop in paths)
            {
                if (String.IsNullOrEmpty(prop)) continue;
                if (target.IsList())
                {
                    int index = int.Parse(prop);
                    target = (target as List<object>)[index];
                    type = target.GetType();
                }
                else if (target.IsDictionary())
                {
                    target = (target as Dictionary<string, object>)[prop];
                    type = target.GetType();
                }
                else
                {
                    PropertyInfo info = type.GetProperty(prop);
                    if (info != null)
                    {
                        target = info.GetValue(target, null);
                        type = info.PropertyType;
                    }
                    else throw new ArgumentException("Properties path is not correct");
                }
            }
            return target;
        }
        public static bool SetByPath(this object target, string path, object value)
        {
            var prop = path.Substring(path.LastIndexOf(".") + 1);
            try
            {
                var found = target.GetByPath(path.Substring(0, path.LastIndexOf(".")));
                found.SetByName(prop, value);
                return true;
            }
            catch
            {
                throw;
            }
        }
        public static object Call(this object target, string method, params object[] args)
        {
            MethodInfo found = null;
            foreach (var info in target?.GetType().GetMethods())
            {
                if (info.Name == method)
                {
                    found = info;
                    break;
                }
            }

            return found?.Invoke(target, args);
        }
        public static bool Has(this object target, string prop)
        {
            foreach (var info in target?.GetType().GetProperties())
            {
                if (info.Name == prop)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool TryGetByName(this object target, string prop, out object result)
        {
            try
            {
                result = target.GetByName(prop);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool TrySetByName(this object target, string prop, object value)
        {
            try
            {
                target.SetByName(prop, value);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static bool TryGetByPath(this object target, string path, out object result)
        {
            try
            {
                result = target.GetByPath(path);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool TrySetByPath(this object target, string path, object value)
        {
            try
            {
                target.SetByPath(path, value);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool TryCall(this object target, string method, object[] args, out object result)
        {
            try
            {
                result = target.Call(method, args);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool IsDictionary(this object target)
        {
            return target != null && target.GetType().FullName.StartsWith("System.Collections.Generic.Dictionary");
        }
        public static bool IsList(this object target)
        {
            return target != null && target.GetType().FullName.StartsWith("System.Collections.Generic.List");
        }
        public static bool IsExpando(this object target)
        {
            return target != null && target is ExpandoObject;
        }
        public static bool IsAssignable(this object target)
        {
            if (target == null) return true;
            Type type = target.GetType();
            return
                type.IsPrimitive ||
                new Type[] {
                    typeof(Enum),
                    typeof(String),
                    typeof(Decimal),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid),
                    typeof(Action),
                    typeof(DynamicObject)
                }.Contains(type);
        }
        public static bool IsAction(this object target)
        {
            return target != null && target.GetType().FullName.StartsWith("System.Action");
        }
    }
}
