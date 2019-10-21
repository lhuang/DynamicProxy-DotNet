using System;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using System.Dynamic;
using System.Threading;
using System.Collections.Generic;

namespace System.Dynamic.Test
{
    public class ProxyTest
    {
        private readonly ITestOutputHelper _outputHelper;
        dynamic _expando;
        dynamic _proxy;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputHelper"></param>
        public ProxyTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _expando = new ExpandoObject();
            _proxy = new Proxy(_expando);
            _proxy.Text = "Proxy !!!!!";
            _proxy.Number = 100;
            _proxy.Blob = new ExpandoObject();
            _proxy.Blob.PI = 3.14;
            _proxy.Blob.Name = "Blob";
            _proxy.Blob.Method = (Action)(() => { _proxy.Blob.PI = 3.145; });

            _proxy.Dict = new Dictionary<string, object>()
            {
                ["keya"] = "keya"
            };

            _proxy.Lst = new List<object>()
            {
                "item1"
            };

        }

        /// <summary>
        /// 
        /// </summary>
        [Fact(DisplayName = "Dynamically attach new property")]

        public void NewProperties()
        {

            Assert.Equal("Proxy !!!!!", _expando.Text);
            Assert.Equal(100, _expando.Number);
            Assert.True(_expando.Blob is Object);
            Assert.Equal("Blob", _expando.Blob.Name);
        }
        [Fact(DisplayName = "Edit property")]

        public void Edit()
        {
            _proxy.Text = "New Proxy !!!!!";
            _proxy.Blob.Name = "Edited Blob";

            Assert.Equal("New Proxy !!!!!", _expando.Text);
            Assert.Equal("Edited Blob", _expando.Blob.Name);
        }
        [Fact(DisplayName = "Invoke action prop")]

        public void ActionCall()
        {
            _proxy.Blob.Method();
            Assert.Equal(3.145, _expando.Blob.PI);
        }

        [Fact(DisplayName = "Invoke method")]

        public void MethodCall()
        {
            var data = new Data("Text", 900);
            Assert.Equal("Text", data.GetText());
        }
        [Fact(DisplayName = "List")]

        public void List()
        {
            Assert.Equal("item1", _expando.Lst[0]);
            _proxy.Lst[0] = "Item1";
            Assert.Equal("Item1", _expando.Lst[0]);
        }
        [Fact(DisplayName = "Dictionary")]

        public void Dictionary()
        {
            Assert.Equal("keya", _expando.Dict["keya"]);
            _proxy.Dict["keya"] = "Item1";
            Assert.Equal("Item1", _expando.Dict["keya"]);
        }
    }
}
