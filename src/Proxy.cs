using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace System.Dynamic
{
    /*
    public delegate object Convert(object target, ConvertBinder binder);
    public delegate object BinaryOp(object target, object right,  BinaryOperationBinder binder);
    public delegate object UnaryOp(object target, UnaryOperationBinder binder);
    */
    public delegate (object, bool) Caller(Dictionary<string, object> target, string prop, string path, params object[] args);
    public delegate (object, bool) Getter(Dictionary<string, object> target, string prop, string path);
    public delegate bool Setter(Dictionary<string, object> target, string prop, string path, object value);

    public class Proxy : DynamicObject
    {
        public class Operations
        {

            public const string CALL = "call";
            public const string GET = "get";
            public const string SET = "set";
        }

        protected readonly object _target;
        protected readonly string _path;
        protected readonly Dictionary<string, object> _map;
        protected readonly Dictionary<string, Delegate> _handler;

        private Proxy()
        {
            _map = new Dictionary<string, object>();
        }

        public Proxy(object target, string path = null, Dictionary<string, Delegate> handler = null) : this()
        {
            if (target == null || target is DynamicObject) throw new ArgumentException("target");

            _target = target;
            _path = path ?? "";
            _handler = handler ?? new Dictionary<string, Delegate>();

            if (target.IsDictionary())
            {
                foreach (var item in target as Dictionary<string, object>)
                {
                    _map.Add(item.Key, item.Value.IsAssignable() ? item.Value : new Proxy(item.Value, $"{_path}.{item.Key}", handler));
                }
            }
            else if (target.IsList())
            {
                int index = 0;
                foreach (var item in target as List<object>)
                {
                    _map.Add(index.ToString(), item.IsAssignable() ? item : new Proxy(item, $"{_path}.{index}", handler));
                    index++;
                }
            }
            else
            {

                foreach (var info in target.GetType().GetProperties())
                {
                    var val = info.GetValue(target, null);
                    _map.Add(info.Name, val.IsAssignable() ? val : new Proxy(val, $"{_path}.{info.Name}", handler));

                }
            }
        }

        public object Target {
            get { return _target; }
        }

        #region Dynamic Object Overrides

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var prop = indexes[0].ToString();

            bool @default = true;
            result = null;

            if (_handler.ContainsKey(Operations.GET))
            {
                var handler = (Getter)_handler[Operations.GET];
                var turple = handler(_map, prop, _path);
                result = turple.Item1;
                @default = turple.Item2;
            }

            if (@default)
            {
                result = _map[prop];
            }

            return result != null;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var prop = binder.Name;
            bool @default = true;
            result = null;

            if (_handler.ContainsKey(Operations.GET))
            {
                var handler = (Getter)_handler[Operations.GET];
                var turple = handler(_map, prop, _path);
                result = turple.Item1;
                @default = turple.Item2;
            }

            if (@default)
            {
                result = _map[prop];
            }


            return result != null;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (value is DynamicObject) return false;
            var prop = indexes[0].ToString();

            if (!_target.TrySetByName(prop, value)) return false;

            //_map[prop] = _map.ContainsKey(prop) ? value : new Proxy(value, $"{_path}.{prop}", _handler);
            bool @default = true;

            if (_handler.ContainsKey(Operations.SET))
            {
                var handler = (Setter)_handler[Operations.SET];
                @default = handler(_map, prop, _path, value);
            }
            if (@default)
            {
                _map[prop] = (_map.ContainsKey(prop) || value.IsAssignable()) ? value : new Proxy(value, $"{_path}.{prop}", _handler);
            }

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (value is DynamicObject) return false;

            var prop = binder.Name;

            if (!_target.TrySetByName(prop, value)) return false;

            bool @default = true;
            if (_handler.ContainsKey(Operations.SET))
            {
                var handler = (Setter)_handler[Operations.SET];
                @default = handler(_map, prop, _path, value);
            }

            if (@default)
            {
                _map[prop] = (_map.ContainsKey(prop) || Reflect.IsAssignable(value)) ? value : new Proxy(value, $"{_path}.{prop}", _handler);
            }
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var prop = binder.Name;

            if (!_target.TryCall(prop, args, out result)) return false;

            bool @default = true;
            if (_handler.ContainsKey(Operations.CALL))
            {
                var handler = (Caller)_handler[Operations.CALL];
                var turple = handler(_map, prop, _path, args);
                result = turple.Item1;
                @default = turple.Item2;
            }
            if (@default)
            {

            }
            return result != null;
        }

        #endregion
    }
}