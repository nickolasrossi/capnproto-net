
using System;
using System.IO;
using System.Text;
namespace CapnProto.Schema
{
    /// <summary>
    /// Command-line plugin tool for capnp
    /// </summary>
    public static class CapnpPlugin
    {
        public static int Process(Stream source, TextWriter destination, TextWriter errors, string nspace)
        {
            try
            {
                using (var msg = Message.Load(source))
                {
                    if (!msg.ReadNext()) throw new InvalidOperationException("Message on stdin not detected");
                    var req = (CodeGeneratorRequest)msg.Root;

                    var codeWriter = new CSharpStructWriter(destination, req.nodes, nspace);
                    req.GenerateCustomModel(codeWriter);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                errors.WriteLine(ex.Message);
                return -1;
            }
        }
        static int Main(string[] args)
        {
			string nspace = args.Length > 0 ? args[0] : "YourNamespace";

            try
            {
                using (var stdin = Console.OpenStandardInput())
                using (var stdout = Console.OpenStandardOutput())
                using (var encodedOut = new StreamWriter(stdout, new UTF8Encoding(true)))
                {
                    return Process(stdin, encodedOut, Console.Error, nspace);
                }
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
