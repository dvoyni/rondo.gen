using System;
using System.IO;
using System.Linq;

namespace Rondo.Gen.Core.Lib {
    public static class CGen {
        const int PMax = 4;
        const int AMax = 4;

        public static void Generate(string dir = "") {
            var text = $"\n" +
                    $"//DO NOT MODIFY. FILE IS GENERATED\n" +
                    $"using System.Runtime.InteropServices;\n" +
                    $"using Rondo.Core.Extras;\n" +
                    $"using Rondo.Core.Memory;\n\n" +
                    $"namespace Rondo.Core.Lib {{\n" +
                    $"{J("\n", 0, PMax + 1, T)}\n}}";

            File.WriteAllText(Path.Combine(dir, "Lib/C.cs"), text);
        }

        static string T(int t) {
            return S("\n", Ca(t), Cf(t), SCa(t), SCf(t));
        }

        static string Ca(int t) {
            if (t == 0) {
                return "";
            }
            return
                    $"    [StructLayout(LayoutKind.Sequential, Pack=1)]\n" +
                    $"    public readonly unsafe struct Ca<{J(", ", 0, t, tx => $"T{tx}")}> {{\n" +
                    $"       {J("\n        ", 0, AMax, a => $"private readonly void* _arg{a};")}\n" +
                    $"        private readonly int _arity;\n" +
                    $"        private readonly void* _fn;\n" +
                    $"\n" +
                    $"       {J("\n       ", 0, AMax + 1, pa => ACtor(pa, t))}" +
                    $"\n" +
                    $"        public void Invoke({J(", ", 0, t, pt => $"T{pt} p{pt}")}) {{\n" +
                    $"            switch (_arity) {{\n" +
                    $"            {J("\n            ", 0, AMax + 1, pa => $"case {pa}:\n                ((delegate*<{S(", ", J(",", 0, t, pt => $"T{pt}"), J(", ", 0, pa, _ => "void*"))}, void>)_fn)({S(", ", J(",", 0, t, pt => $"p{pt}"), J(", ", 0, pa, pa2 => $"_arg{pa2}"))});\n                break;")}" +
                    $"\n" +
                    $"            default:\n" +
                    $"                Assert.Fail(\"Unsupported closure arity\");\n" +
                    $"                return;\n" +
                    $"            }}\n" +
                    $"        }}\n" +
                    $"    }}\n";
        }

        static string ACtor(int a, int t) {
            return
                    $"public Ca({S(", ", $"delegate*<{S(", ", J(",", 0, t, pt => $"T{pt}"), J(", ", 0, a, pa => "void*"))}, void> fn", J(", ", 0, a, pa => $"void* a{pa}"))}) {{\n" +
                    $"           _fn = fn;\n" +
                    $"           {J("\n           ", 0, AMax, pa => $"_arg{pa} = {(pa < a ? $"a{pa}" : "null")};")}\n" +
                    $"           _arity = {a};\n" +
                    $"       }}\n";
        }

        static string SCa(int t) {
            if (t == 0) {
                return "";
            }
            return
                    $"    public static unsafe partial class Ca {{\n" +
                    $"        {J("\n", 0, AMax + 1, a => SCac(t, a + 1))}" +
                    $"    }}\n";
        }

        static string SCac(int t, int a) {
            if (a == 0) {
                return "";
            }
            return
                    $"        public static Ca<{J(", ", 0, t, pt => $"T{pt}")}> New<{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => $"A{pa - 1}"))}>(delegate*<{S(",", $"{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => $"A{pa - 1}*"), "void")}> fn", J(", ", 1, a, pa => $"A{pa - 1} a{pa - 1}"))}){J("", 1, a, pa => $"\n                where A{pa - 1}: unmanaged")} {{\n" +
                    J("", 1, a, pa => $"            var pa{pa - 1} = Mem.C.Copy(a{pa - 1});\n") +
                    $"            return new Ca<{J(", ", 0, t, pt => $"T{pt}")}>({S(", ", $"(delegate*<{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => "void*"), "void>)fn")}", J(", ", 1, a, pa => $"pa{pa - 1}"))});\n" +
                    $"        }}\n";
        }

        static string Cf(int t) {
            return
                    $"    [StructLayout(LayoutKind.Sequential, Pack=1)]\n" +
                    $"    public readonly unsafe struct Cf<{S(", ", J(", ", 0, t, tx => $"T{tx}"), "TR")}> {{\n" +
                    $"       {J("\n        ", 0, AMax, a => $"private readonly void* _arg{a};")}\n" +
                    $"        private readonly int _arity;\n" +
                    $"        private readonly void* _fn;\n" +
                    $"\n" +
                    $"       {J("\n       ", 0, AMax + 1, pa => FCtor(pa, t))}" +
                    $"\n" +
                    $"        public TR Invoke({J(", ", 0, t, pt => $"T{pt} p{pt}")}) {{\n" +
                    $"            switch (_arity) {{\n" +
                    $"            {J("\n            ", 0, AMax + 1, pa => $"case {pa}:\n                return ((delegate*<{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 0, pa, _ => "void*"), "TR")}>)_fn)({S(", ", J(", ", 0, t, pt => $"p{pt}"), J(", ", 0, pa, pa2 => $"_arg{pa2}"))});")}" +
                    $"\n" +
                    $"            default:\n" +
                    $"                Assert.Fail(\"Unsupported closure arity\");\n" +
                    $"                return default;\n" +
                    $"            }}\n" +
                    $"        }}\n" +
                    $"    }}\n";
        }

        static string SCf(int t) {
            return
                    $"    public static unsafe partial class Cf {{\n" +
                    $"        {J("\n", 0, AMax + 1, a => SCfc(t, a + 1))}" +
                    $"    }}\n";
        }

        static string SCfc(int t, int a) {
            return
                    $"        public static Cf<{S(",", J(", ", 0, t, pt => $"T{pt}"), "TR")}> New<{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => $"A{pa - 1}"), "TR")}>(delegate*<{S(",", $"{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => $"A{pa - 1}*"), "TR")}> fn", J(", ", 1, a, pa => $"A{pa - 1} a{pa - 1}"))}){J("", 1, a, pa => $"\n                where A{pa - 1}: unmanaged")} {{\n" +
                    J("", 1, a, pa => $"            var pa{pa - 1} = Mem.C.Copy(a{pa - 1});\n") +
                    $"            return new Cf<{S(", ", J(", ", 0, t, pt => $"T{pt}"), "TR")}>({S(", ", $"(delegate*<{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => "void*"), "TR>)fn")}", J(", ", 1, a, pa => $"pa{pa - 1}"))});\n" +
                    $"        }}\n";
        }

        static string FCtor(int a, int t) {
            return
                    $"public Cf({S(", ", $"delegate*<{S(", ", J(",", 0, t, pt => $"T{pt}"), J(", ", 0, a, pa => "void*"), "TR")}> fn", J(", ", 0, a, pa => $"void* a{pa}"))}) {{\n" +
                    $"           _fn = fn;\n" +
                    $"           {J("\n           ", 0, AMax, pa => $"_arg{pa} = {(pa < a ? $"a{pa}" : "null")};")}\n" +
                    $"           _arity = {a};\n" +
                    $"       }}\n";
        }

        static string S(string sep, params string[] s) {
            return string.Join(sep, s.Where(x => !string.IsNullOrEmpty(x)));
        }

        static string J(string sep, int min, int max, Func<int, string> x) {
            return string.Join(sep, Enumerable.Range(min, max - min).Select(x));
        }
    }
}