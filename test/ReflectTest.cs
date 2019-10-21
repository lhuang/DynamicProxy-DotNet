using System;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using System.Dynamic;
using System.Threading;
using System.Collections.Generic;

namespace System.Dynamic.Test
{
    public class Data
    {
        public Data(string text, int number)
        {
            Text = text;
            Number = number;
        }
        public string Text { get; set; }
        public int Number { get; set; }

        public string GetText()
        {
            return Text;
        }

        public int GeNumber()
        {
            return Number;
        }
    }
    public class ReflectTest
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly dynamic _d;
        private readonly dynamic _e;
        private readonly Data _data;
        public ReflectTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _d = new
            {
                x = "",
                y = 123
            };
            _e = new System.Dynamic.ExpandoObject();
            _e.X = "text";
            _e.Y = 900;
            _e.Z = (Func<string>)(() =>
            {
                return _e.X;
            });
            _data = new Data("Text", 900);
        }

        [Fact(DisplayName = "GetByName")]
        public void GetPropertyByName()
        {
            var result = Reflect.GetByName(_e, "X");
            Assert.Equal("text", result);
        }

        [Fact(DisplayName = "TryGetByName")]
        public void TryGetPropertyByName()
        {
            object result;
            bool ans = Reflect.TryGetByName(_e, "Y", out result);
            Assert.Equal(900, (int)result);
        }

        [Fact(DisplayName = "SetByName")]
        public void SetPropertyByName()
        {
            Reflect.SetByName(_e, "X", "New Text");
            Assert.Equal("New Text", _e.X);
        }

        [Fact(DisplayName = "TryGetByName")]
        public void TrySetPropertyByName()
        {
            bool ans = Reflect.TrySetByName(_e, "Y", 1000);
            Assert.Equal(1000, _e.Y);
        }

        [Fact(DisplayName = "SetByPath")]
        public void SetPropertyByPath()
        {
            Reflect.SetByName(_e, "X", "New Text");
            Assert.Equal("New Text", _e.X);
        }

        [Fact(DisplayName = "TryGetByPath")]
        public void TrySetPropertyByPath() 
        {
            bool ans = Reflect.TrySetByName(_e, "Y", 1000);
            Assert.Equal(1000, _e.Y);
        }

        [Fact(DisplayName = "Method Call")]
        public void Call()
        {
            object result = Reflect.Call(_data, "GetText", null);
            Assert.Equal("Text", result.ToString());
        }

        [Fact(DisplayName = "TryMethodCall")]
        public void TryCall()
        {
            object result;
            bool ans = Reflect.TryCall(_data, "GeNumber", null, out result);
            Assert.Equal(900, (int)result);
        }

        [Fact(DisplayName = "Is dictionary")]
        public void IsDictionary()
        {
            var dict = new Dictionary<string, object>();
            Assert.True(dict.IsDictionary());
        }

        [Fact(DisplayName = "Is action")]
        public void IsAssignable()
        {
            var date = new DateTime();
            var num = 123.456;
            var str = "12eryu";
            Action action = () => { };
            Assert.True(date.IsAssignable());
            Assert.True(num.IsAssignable());
            Assert.True(str.IsAssignable());
            Assert.True(action.IsAssignable());
        }

        [Fact(DisplayName = "Is Expando")]
        public void IsExpando()
        {
            var dict = new ExpandoObject();
            Assert.True(dict.IsExpando());
        }

        [Fact(DisplayName = "Is List")]
        public void IsList()
        {
            var dict = new List<object>();
            Assert.True(dict.IsList());
        }

        [Fact(DisplayName = "Is Action")]
        public void IsAction()
        {
            Action dict = ()=> { };
            Assert.True(dict.IsAction());
        }
    }
}
