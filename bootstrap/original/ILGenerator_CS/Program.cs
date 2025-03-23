using System;
using System.Buffers.Binary;
using System.Text;

static class Program
{
    public static byte[] s_RAM;

    static void Main(string[] args)
    {
        string srcCodePath = "";
        int ramBinLength_MB = 100;
        bool isDebug = false;

        if (args.Length == 0)
        {
            Console.WriteLine("Nothing to parse.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("    ILGenerator_CS <src_file_path> [--ramBinLength=<length>] [--debug]");
            Console.WriteLine();
            Console.WriteLine("Params:");
            Console.WriteLine("    src_file_path:               Path to the FFLang source code file");
            Console.WriteLine("    --debug:                     (Optional) Includes built-in: 'func __dbg_int(v: Int) -> Int'");
            Console.WriteLine("    --ramBinLength=<length>:     (Optional) Specifies 'RAM.bin' file size in MB.");
            Console.WriteLine("                                 Allowed range: 1 to 1024. Default 100");
            return;
        }

        foreach (var arg in args)
        {
            if (arg.StartsWith("--ramBinLength="))
            {
                if (int.TryParse(arg.Replace("--ramBinLength=", ""), out ramBinLength_MB))
                {
                    if (ramBinLength_MB <= 0 || ramBinLength_MB > 1024)
                    {
                        Console.WriteLine($"Invalid 'RAM.bin' size. Allowed range (in MB): 1 to 1024. Default 100");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid 'RAM.bin' size. Allowed range (in MB): 1 to 1024. Default 100");
                    return;
                }
            }
            else if (arg == "--debug")
            {
                isDebug = true;
            }
            else
            {
                srcCodePath = arg;
            }
        }

        var ramLen = ramBinLength_MB > 1 ? ramBinLength_MB : 2;
        s_RAM = new byte[ramLen * 1024 * 1024];
        string ramDataFile = "RAM.bin";

        BinaryPrimitives.WriteInt32LittleEndian(s_RAM.AsSpan(64), ramBinLength_MB);
        BinaryPrimitives.WriteInt32LittleEndian(s_RAM.AsSpan(68), isDebug ? 1 : 0);

        var srcCodeArr = System.IO.File.ReadAllBytes(srcCodePath);
        Array.Copy(srcCodeArr, 0, s_RAM, 524288, srcCodeArr.Length);

        // ------ FFLang Entry point
        PrepareStaticStrings.main();
        PrepareStaticStrings_IL.main();
        int exitCode = ILGenerator.main();
        // -------------------------

        if (!System.IO.Directory.Exists("out"))
        {
            System.IO.Directory.CreateDirectory("out");
        }

        System.IO.File.WriteAllText("out\\FFLang_Program.il", ReadString(1048576, 1048576));
        System.IO.File.WriteAllText("out\\FFLang_Program.runtimeconfig.json", ReadString(510976, 1024));

        System.IO.File.WriteAllBytes(ramDataFile, s_RAM);

        Console.WriteLine($"Exit code: {exitCode}");

        if (exitCode == 0)
        {
            RunILASM();
        }
    }

    public static string ReadString(int str_ptr, int len)
    {
        var str = Encoding.UTF8.GetString(s_RAM.AsSpan(str_ptr, len));
        str = str.Substring(0, str.IndexOf('\0'));
        return str;
    }

    private static void RunILASM()
    {
        if (!System.IO.Directory.Exists("out"))
        {
            System.IO.Directory.CreateDirectory("out");
        }

        var process = System.Diagnostics.Process.Start("ilasm", "out\\FFLang_Program.il /dll");
        process?.WaitForExit();
    }
}
