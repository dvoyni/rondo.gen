using Rondo.Gen.Core.Lib;

namespace Rondo.Gen {
    internal static class Program {
        private static void Main(string[] _) {
            const string root = "../../../../../rondo.core/Runtime";
            CGen.Generate(root);
            CLGen.Generate(root);
        }
    }
}