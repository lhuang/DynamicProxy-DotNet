using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace System.Dynamic.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            TestProxy();
            Console.WriteLine();

            //TestType();
            dynamic proxy1 = TestJsonArrayProxy();
            Console.WriteLine("\nJSON: \n" + JSON.Stringify(proxy1.Target));
            Console.WriteLine();
            dynamic proxy2 = TestJsonObjectProxy();
            Console.WriteLine("\nJSON: \n" + JSON.Stringify(proxy2.Target));
            Console.ReadLine();
        }

        static void TestProxy()
        {
            //dynamic data = new Data();
            dynamic data = new ExpandoObject();
            dynamic proxy = new Proxy(data, null, new Dictionary<string, Delegate>()
            {
                [Proxy.Operations.CALL] = (Caller)((target, prop, path, args) =>
                {
                    Console.WriteLine(new
                    {
                        action = "call",
                        path = path,
                        args = args,
                    });

                    return (null, true);
                }),
                [Proxy.Operations.GET] = (Getter)((target, prop, path) =>
                {
                    Console.WriteLine(new
                    {
                        action = "get",
                        path = path,
                        value = target[prop],
                    });
                    return (target[prop], true);
                }),
                [Proxy.Operations.SET] = (Setter)((target, prop, path, value) =>
                {
                    Console.WriteLine(new
                    {
                        action = "set",
                        path = path,
                        oldValue = target.Has(prop) ? target[prop] : null,
                        newValue = value
                    });
                    return true;
                })
            });
            proxy.Call = (Action)(() =>
            {
                Console.WriteLine("Hello Expando: " + data.Text);
            });
            proxy.Text = "Hell Proxy !!!!!";
            proxy.Number = 100;
            proxy.Blob = new
            {
                PI = 3.14
            };

            proxy.Call();
        }

        static dynamic TestJsonArrayProxy()
        {
            var jsonString =
            $@"
            [
                1.44,
                true,
                ""123"", 
                {{
                    ""x"": ""xyz"", 
                    ""y"":3.14
                }}
            ]
            ";

            object @array = JSON.Parse(jsonString);
            dynamic json = new Proxy(@array, null, new Dictionary<string, Delegate>()
            {
                [Proxy.Operations.CALL] = (Caller)((target, prop, path, args) =>
                {
                    Console.WriteLine(new
                    {
                        action = "call",
                        path = path,
                        args = args,
                    });

                    return (null, true);
                }),
                [Proxy.Operations.GET] = (Getter)((target, prop, path) =>
                {
                    Console.WriteLine(new
                    {
                        action = "get",
                        path = path,
                        value = target[prop],
                    });
                    return (target[prop], true);
                }),
                [Proxy.Operations.SET] = (Setter)((target, prop, path, value) =>
                {
                    Console.WriteLine(new
                    {
                        action = "set",
                        path = path,
                        oldValue = target.Has(prop) ? target[prop] : null,
                        newValue = value
                    });
                    return true;
                })
            });
            json[0] = 4.41;
            json[3].x = "oxygen";

            return json;
        }

        static dynamic TestJsonObjectProxy()
        {
            var jsonString =
            $@"
            {{
                ""StringProperty"":""The quick brown fox 'jumps' over the lazy dog "",
                ""UnicodeProperty"":""\u3041 L\u00fa H\u00faang"",
                ""IntegerProperty"": 65536,
                ""FloatProperty"": 3.1415926,
                ""BoolProperty"": true,
                ""NullProperty"": null,
                ""ObjectProperty"": {{
                    ""key1"":{{
                        ""key1a"": ""value1"", 
                        ""key1b"": true
                    }},
                    ""key2"": 256,
                    ""key3"": [1.44,true,""123""]
                }},
                ""ArrayProperty"": [1.44,true,""123"", {{""x"": ""xyz"", ""y"":3.14}}],
            }}";

            object @object = JSON.Parse(jsonString);
            dynamic json = new Proxy(@object, null, new Dictionary<string, Delegate>()
            {
                [Proxy.Operations.CALL] = (Caller)((target, prop, path, args) =>
                {
                    Console.WriteLine(new
                    {
                        action = "call",
                        path = path,
                        args = args,
                    });

                    return (null, true);
                }),
                [Proxy.Operations.GET] = (Getter)((target, prop, path) =>
                {
                    Console.WriteLine(new
                    {
                        action = "get",
                        path = path,
                        value = target[prop],
                    });
                    return (target[prop], true);
                }),
                [Proxy.Operations.SET] = (Setter)((target, prop, path, value) =>
                {
                    Console.WriteLine(new
                    {
                        action = "set",
                        path = path,
                        oldValue = target.Has(prop) ? target[prop] : null,
                        newValue = value
                    });
                    return true;
                })
            });
            json.ArrayProperty[0] = 4.41;
            json.ArrayProperty[3].x = "Hello";
            json.ObjectProperty.key2 = 652;
            json.ObjectProperty.key1.key1a = "Forever";
            json.ObjectProperty.key3[1] = false;

            json.ActionProperty = (Action)(() =>
            {
                Console.WriteLine("\n dynamic method call:  " + json.UnicodeProperty + " -- " + json.IntegerProperty);
            });

            json.ActionProperty();

            return json;
        }

        static void TestType()
        {
            dynamic d = new
            {
                x = "",
                y = 123,
                z = new
                {
                    m = "level 2",
                    n = 2
                }
            };
            //d.y++;

            Console.WriteLine(d.y);

            dynamic e = new System.Dynamic.ExpandoObject();
            e.x = "string";
            e.y = 900;
            e.z = new ExpandoObject();
            e.z.m = "level2";
            e.z.n = 2;

            e.y++;

            Console.WriteLine(e.y);

            object o = new object();

            Console.WriteLine(d.GetType());
            Console.WriteLine(Convert.GetTypeCode(d.GetType()));
            Console.WriteLine((d as object).IsExpando());
            Console.WriteLine((d as object).IsAssignable());

            Console.WriteLine(e.GetType());
            Console.WriteLine(Convert.GetTypeCode(e.GetType()));
            Console.WriteLine((e as object).IsExpando());
            Console.WriteLine((e as object).IsAssignable());

            Console.WriteLine(o.GetType());
            Console.WriteLine(Convert.GetTypeCode(o.GetType()));
        }

    }
}


