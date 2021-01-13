using System;
using System.IO;
using System.Runtime.InteropServices;
using AlexNoddings.Protocols.Generator;
using AlexNoddings.Protocols.Parser;
using AlexNoddings.Protocols.Parser.Parsing;

namespace Host
{
    class Program
    {
        public static void Main(string[] args)
        {
            var path = args[0];
            var files = Directory.GetFiles(path, "*.protodef");
            foreach (var fp in files)
            {
                var fn = Path.GetFileNameWithoutExtension(fp);
                var def = File.ReadAllText(fp);
                var page = PageGenerator.FromParsedGame(Parser.Parse(def)).Replace("%protocolname%", fn);
                File.WriteAllText(Path.Combine(path, fn + ".html"), page);
            }
        }
    }
}

