using System;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using System.Dynamic;
using System.Threading;
using System.Collections.Generic;

namespace System.Dynamic.Test
{
    public class JSONTest
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly string _jsonString =
            $@"
            {{
                ""String"":""The quick brown fox 'jumps' over the lazy dog "",
                ""Unicode"":""\u3041 L\u00fa H\u00faang"",
                ""Integer"": 65536,
                ""Float"": 3.1415926,
                ""Bool"": true,
                ""Null"": null,
                ""Object"": {{
                    ""key1"":{{
                        ""key1a"": ""value1"", 
                        ""key1b"": true
                    }},
                    ""key2"": 256,
                    ""key3"": [1.44,true,""123""]
                }},
                ""Array"": [1.44,true,""123"", {{""x"": ""xyz"", ""y"":3.14}}],
            }}";
        dynamic _jsonObject;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputHelper"></param>
        public JSONTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _jsonObject = JSON.Parse(_jsonString);
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact(DisplayName = "Parse json string to object")]
        public void Deserialization()
        {
           Assert.NotNull(_jsonObject);
        }
        /// <summary>
        /// 
        /// </summary>
        [Fact(DisplayName = "Stringify object as string")]
        public void Serialization()
        {
            string json = JSON.Stringify(_jsonObject);
            Assert.NotNull(json);

        }
    }
}
