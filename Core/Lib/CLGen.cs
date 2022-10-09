using System;
using System.IO;
using System.Linq;

namespace Rondo.Gen.Core.Lib {
    public static class CLGen {
        const int PMax = 4;
        const int AMax = 4;

        public static void Generate(string dir = "") {
            var text = $"\n" +
                    $"//DO NOT MODIFY. FILE IS GENERATED\n" +
                    $"using System;\n" +
                    $"using System.Runtime.InteropServices;\n" +
                    $"using Rondo.Core.Extras;\n" +
                    $"using Rondo.Core.Memory;\n\n" +
                    $"namespace Rondo.Core.Lib {{\n" +
                    $"{J("\n", 0, PMax + 1, T)}\n}}";

            File.WriteAllText(Path.Combine(dir, "Lib/CL.cs"), text);
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
                    $"    public readonly unsafe struct CLa<{J(", ", 0, t, tx => $"T{tx}")}> : IDisposable, IEquatable<CLa<{J(", ", 0, t, tx => $"T{tx}")}>> {{\n" +
                    $"       {J("\n       ", 0, AMax, a => $"private readonly IntPtr _arg{a};")}\n" +
                    $"       private readonly int _arity;\n" +
                    $"       private readonly void* _fn;\n" +
                    $"\n" +
                    $"       {J("\n       ", 0, AMax + 1, pa => ACtor(pa, t))}" +
                    $"\n" +
                    $"       public void Invoke({J(", ", 0, t, pt => $"T{pt} p{pt}")}) {{\n" +
                    $"            switch (_arity) {{\n" +
                    $"            {J("\n            ", 0, AMax + 1, pa => $"case {pa}:\n                ((delegate*<{S(", ", J(",", 0, t, pt => $"T{pt}"), J(", ", 0, pa, _ => "void*"))}, void>)_fn)({S(", ", J(",", 0, t, pt => $"p{pt}"), J(", ", 0, pa, pa2 => $"_arg{pa2}.ToPointer()"))});\n                break;")}" +
                    $"\n" +
                    $"            default:\n" +
                    $"                Assert.Fail(\"Unsupported closure arity\");\n" +
                    $"                return;\n" +
                    $"            }}\n" +
                    $"        }}\n\n" +
                    $"        public void Dispose() {{\n" +
                    $"            {J("\n            ", 0, AMax, pa => $"if (_arg{pa} != IntPtr.Zero) {{\n                Mem.FreeOuterMemory(_arg{pa});\n            }}")}\n" +
                    $"        }}\n\n" +
                    $"        public bool Equals(CLa<{J(", ", 0, t, tx => $"T{tx}")}> other) {{\n" +
                    $"#pragma warning disable CS8909\n" +
                    $"            return {S(" && ", "_fn == other._fn", "_arity == other._arity", J(" && ", 0, AMax, pa => $"_arg{pa} == other._arg{pa}"))};\n" +
                    $"#pragma warning restore CS8909\n" +
                    $"        }}\n" +
                    $"    }}\n";
        }

        static string ACtor(int a, int t) {
            return
                    $"public CLa({S(", ", $"delegate*<{S(", ", J(",", 0, t, pt => $"T{pt}"), J(", ", 0, a, pa => "IntPtr"))}, void> fn", J(", ", 0, a, pa => $"IntPtr a{pa}"))}) {{\n" +
                    $"           _fn = fn;\n" +
                    $"           {J("\n           ", 0, AMax, pa => $"_arg{pa} = {(pa < a ? $"a{pa}" : "IntPtr.Zero")};")}\n" +
                    $"           _arity = {a};\n" +
                    $"       }}\n";
        }

        static string SCa(int t) {
            if (t == 0) {
                return "";
            }
            return
                    $"    public static unsafe partial class CLa {{\n" +
                    $"        {J("\n", 0, AMax + 1, a => SCac(t, a + 1))}" +
                    $"    }}\n";
        }

        static string SCac(int t, int a) {
            if (a == 0) {
                return "";
            }
            return
                    $"        public static CLa<{J(", ", 0, t, pt => $"T{pt}")}> New<{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => $"A{pa - 1}"))}>(delegate*<{S(",", $"{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => $"A{pa - 1}*"), "void")}> fn", J(", ", 1, a, pa => $"A{pa - 1} a{pa - 1}"))}){J("", 1, a, pa => $"\n                where A{pa - 1}: unmanaged")} {{\n" +
                    J("", 1, a, pa => $"            var sz{pa - 1} = Mem.SizeOf<A{pa - 1}>();\n            var pa{pa - 1} = Mem.AllocOuterMemoryAndCopy(&a{pa - 1}, sz{pa - 1});\n") +
                    $"            return new CLa<{J(", ", 0, t, pt => $"T{pt}")}>({S(", ", $"(delegate*<{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => "IntPtr"), "void>)fn")}", J(", ", 1, a, pa => $"pa{pa - 1}"))});\n" +
                    $"        }}\n";
        }

        static string Cf(int t) {
            return
                    $"    [StructLayout(LayoutKind.Sequential, Pack=1)]\n" +
                    $"    public readonly unsafe struct CLf<{S(", ", J(", ", 0, t, tx => $"T{tx}"), "TR")}> : IDisposable, IEquatable<CLf<{S(", ", J(", ", 0, t, tx => $"T{tx}"), "TR")}>> {{\n" +
                    $"       {J("\n       ", 0, AMax, a => $"private readonly IntPtr _arg{a};")}\n" +
                    $"       private readonly int _arity;\n" +
                    $"       private readonly void* _fn;\n" +
                    $"\n" +
                    $"       {J("\n       ", 0, AMax + 1, pa => FCtor(pa, t))}" +
                    $"\n" +
                    $"       public TR Invoke({J(", ", 0, t, pt => $"T{pt} p{pt}")}) {{\n" +
                    $"            switch (_arity) {{\n" +
                    $"            {J("\n            ", 0, AMax + 1, pa => $"case {pa}:\n                return ((delegate*<{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 0, pa, _ => "void*"), "TR")}>)_fn)({S(", ", J(", ", 0, t, pt => $"p{pt}"), J(", ", 0, pa, pa2 => $"_arg{pa2}.ToPointer()"))});")}" +
                    $"\n" +
                    $"            default:\n" +
                    $"                Assert.Fail(\"Unsupported closure arity\");\n" +
                    $"                return default;\n" +
                    $"            }}\n" +
                    $"        }}\n\n" +
                    $"        public void Dispose() {{\n" +
                    $"            {J("\n            ", 0, AMax, pa => $"if (_arg{pa} != IntPtr.Zero) {{\n                Mem.FreeOuterMemory(_arg{pa});\n            }}")}\n" +
                    $"        }}\n\n" +
                    $"        public bool Equals(CLf<{S(", ", J(", ", 0, t, tx => $"T{tx}"), "TR")}> other) {{\n" +
                    $"#pragma warning disable CS8909\n" +
                    $"            return {S(" && ", "_fn == other._fn", "_arity == other._arity", J(" && ", 0, AMax, pa => $"_arg{pa} == other._arg{pa}"))};\n" +
                    $"#pragma warning restore CS8909\n" +
                    $"        }}\n" +
                    $"    }}\n";
        }

        static string SCf(int t) {
            return
                    $"    public static unsafe partial class CLf {{\n" +
                    $"        {J("\n", 0, AMax + 1, a => SCfc(t, a + 1))}" +
                    $"    }}\n";
        }

        static string SCfc(int t, int a) {
            return
                    $"        public static CLf<{S(",", J(", ", 0, t, pt => $"T{pt}"), "TR")}> New<{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => $"A{pa - 1}"), "TR")}>(delegate*<{S(",", $"{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => $"A{pa - 1}*"), "TR")}> fn", J(", ", 1, a, pa => $"A{pa - 1} a{pa - 1}"))}){J("", 1, a, pa => $"\n                where A{pa - 1}: unmanaged")} {{\n" +
                    J("", 1, a, pa => $"            var sz{pa - 1} = Mem.SizeOf<A{pa - 1}>();\n            var pa{pa - 1} = Mem.AllocOuterMemoryAndCopy(&a{pa - 1}, sz{pa - 1});\n") +
                    $"            return new CLf<{S(", ", J(", ", 0, t, pt => $"T{pt}"), "TR")}>({S(", ", $"(delegate*<{S(", ", J(", ", 0, t, pt => $"T{pt}"), J(", ", 1, a, pa => "void*"), "TR>)fn")}", J(", ", 1, a, pa => $"pa{pa - 1}"))});\n" +
                    $"        }}\n";
        }

        static string FCtor(int a, int t) {
            return
                    $"public CLf({S(", ", $"delegate*<{S(", ", J(",", 0, t, pt => $"T{pt}"), J(", ", 0, a, pa => "void*"), "TR")}> fn", J(", ", 0, a, pa => $"IntPtr a{pa}"))}) {{\n" +
                    $"           _fn = fn;\n" +
                    $"           {J("\n           ", 0, AMax, pa => $"_arg{pa} = {(pa < a ? $"a{pa}" : "IntPtr.Zero")};")}\n" +
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