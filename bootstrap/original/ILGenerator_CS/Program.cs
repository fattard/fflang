using System;
using System.Buffers.Binary;
using System.Text;

class Program
{
    static string s_ILBegin = @"
// FFLang Program - lang version 0.0


.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 8:0:0:0
}

.assembly extern System.Memory
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )
  .ver 8:0:0:0
}

.assembly extern System.Console
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 8:0:0:0
}

.assembly FFLang_Program
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 )
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78
                                                                                                                   63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 )

  .custom instance void [System.Runtime]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 18 2E 4E 45 54 43 6F 72 65 41 70 70 2C 56   // ....NETCoreApp,V
                                                                                                              65 72 73 69 6F 6E 3D 76 38 2E 30 01 00 54 0E 14   // ersion=v8.0..T..
                                                                                                              46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61 79   // FrameworkDisplay
                                                                                                              4E 61 6D 65 08 2E 4E 45 54 20 38 2E 30 )          // Name..NET 8.0
}
.module FFLang_Program.dll
.custom instance void [System.Runtime]System.Runtime.CompilerServices.RefSafetyRulesAttribute::.ctor(int32) = ( 01 00 0B 00 00 00 00 00 )
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


.class private abstract auto ansi sealed beforefieldinit FFLang_Runtime.'$RuntimeEnv'
       extends [System.Runtime]System.Object {

.field public static uint8[] '$_RAM'

.method private hidebysig static void  '$cs_Main'(string[] args) cil managed
{
    .entrypoint

    .maxstack  5
    .locals init (string V_0, int32 V_1, uint8[] V_2,
                valuetype [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler V_3)

    ldstr      ""RAM.bin""
    stloc.0
    ldloc.0
    call       bool [System.Runtime]System.IO.File::Exists(string)
    brfalse.s  FF_MAIN

    ldloc.0
    call       uint8[] [System.Runtime]System.IO.File::ReadAllBytes(string)
    stloc.2
    ldloc.2
    ldc.i4.0
    ldsfld     uint8[] FFLang_Runtime.'$RuntimeEnv'::'$_RAM'
    ldc.i4.0
    ldloc.2
    ldlen
    conv.i4
    call       void [System.Runtime]System.Array::Copy(class [System.Runtime]System.Array,
                                                                    int32,
                                                                    class [System.Runtime]System.Array,
                                                                    int32,
                                                                    int32)
FF_MAIN:
    call       int32 FFLang_Global.Functions::main()
    stloc.1
    ldloc.0
    ldsfld     uint8[] FFLang_Runtime.'$RuntimeEnv'::'$_RAM'
    call       void [System.Runtime]System.IO.File::WriteAllBytes(string,
                                                                            uint8[])
    ldloca.s   V_3
    ldc.i4.s   11
    ldc.i4.1
    call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::.ctor(int32,
                                                                                                                                int32)
    ldloca.s   V_3
    ldstr      ""Exit code: ""
    call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
    ldloca.s   V_3
    ldloc.1
    call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted<int32>(!!0)
    ldloca.s   V_3
    call       instance string [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::ToStringAndClear()
    call       void [System.Console]System.Console::WriteLine(string)
    ret
} // end of method '$RuntimeEnv'::'$cs_Main'

.method private hidebysig specialname rtspecialname static void  .cctor() cil managed
{
    .maxstack  8

    ldc.i4     [[[_RAM.BIN_]]]
    newarr     [System.Runtime]System.Byte
    stsfld     uint8[] FFLang_Runtime.'$RuntimeEnv'::'$_RAM'
    ret
} // end of method '$RuntimeEnv'::.cctor

} // end of class FFLang_Runtime.'$RuntimeEnv'


.class public abstract auto ansi sealed beforefieldinit FFLang_Global.Functions
       extends [System.Runtime]System.Object {

// ========== FFLang Built-ins

.method private hidebysig static int32  __builtin_int_add(int32 a, int32 b) cil managed
{
    .maxstack  2

    ldarg.0
    ldarg.1
    add
    ret
}

.method private hidebysig static int32  __builtin_int_nand(int32 a, int32 b) cil managed
{
    .maxstack  2

    ldarg.0
    ldarg.1
    and
    not
    ret
}

.method private hidebysig static int32  __builtin_int_mem_read(int32 addr) cil managed
{
    .maxstack  2

    ldsfld     uint8[] FFLang_Runtime.'$RuntimeEnv'::'$_RAM'
    ldarg.0
    call       valuetype [System.Runtime]System.Span`1<!!0> [System.Memory]System.MemoryExtensions::AsSpan<uint8>(!!0[], int32)
    call       valuetype [System.Runtime]System.ReadOnlySpan`1<!0> valuetype [System.Runtime]System.Span`1<uint8>::op_Implicit(valuetype [System.Runtime]System.Span`1<!0>)
    call       int32 [System.Memory]System.Buffers.Binary.BinaryPrimitives::ReadInt32LittleEndian(valuetype [System.Runtime]System.ReadOnlySpan`1<uint8>)
    ret
}

.method private hidebysig static int32  __builtin_int_mem_write(int32 addr, int32 data) cil managed
{
    .maxstack  2

    ldsfld     uint8[] FFLang_Runtime.'$RuntimeEnv'::'$_RAM'
    ldarg.0
    call       valuetype [System.Runtime]System.Span`1<!!0> [System.Memory]System.MemoryExtensions::AsSpan<uint8>(!!0[], int32)
    ldarg.1
    call       void [System.Memory]System.Buffers.Binary.BinaryPrimitives::WriteInt32LittleEndian(valuetype [System.Runtime]System.Span`1<uint8>, int32)
    ldc.i4.0
    ret
}

.method private hidebysig static bool  __bool_check(int32 v) cil managed
{
    .maxstack  2

    ldarg.0
    ldc.i4.0
    cgt.un
    ret
}

.method private hidebysig static int32  __dbg_int(int32 v) cil managed
{
    .maxstack  3
    .locals init (valuetype [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler V_0)

    ldloca.s   V_0
    ldc.i4.8
    ldc.i4.1
    call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::.ctor(int32, int32)
    ldloca.s   V_0
    ldstr      ""[DEBUG] ""
    call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendLiteral(string)
    ldloca.s   V_0
    ldarg.0
    call       instance void [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::AppendFormatted<int32>(!!0)
    ldloca.s   V_0
    call       instance string [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::ToStringAndClear()
    call       void [System.Console]System.Console::WriteLine(string)
    ldc.i4.0
    ret
}

// ================================


// ========== FFLang program START
";

    static string s_ILEnd = @"

// ========== FFLang program END

} // end of class FFLang_Global.Functions
";

    static string s_ILRuntimeJson = @"
{
  ""runtimeOptions"": {
    ""tfm"": ""net8.0"",
    ""framework"": {
      ""name"": ""Microsoft.NETCore.App"",
      ""version"": ""8.0.0""
    },
    ""configProperties"": {
      ""System.Reflection.Metadata.MetadataUpdater.IsSupported"": false,
      ""System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization"": false
    }
  }
}
";

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

        s_RAM = new byte[ramBinLength_MB * 1024 * 1024];
        string ramDataFile = "RAM.bin";

        BinaryPrimitives.WriteInt32LittleEndian(s_RAM.AsSpan(64), ramBinLength_MB);
        BinaryPrimitives.WriteInt32LittleEndian(s_RAM.AsSpan(68), isDebug ? 1 : 0);

        var ILBeginArr = Encoding.UTF8.GetBytes(s_ILBegin.Replace("[[[_RAM.BIN_]]]", $"{ramBinLength_MB * 1024 * 1024} // {ramBinLength_MB} MB"));
        Array.Copy(ILBeginArr, 0, s_RAM, 499712, ILBeginArr.Length);

        var ILEndArr = Encoding.UTF8.GetBytes(s_ILEnd);
        Array.Copy(ILEndArr, 0, s_RAM, 509952, ILEndArr.Length);

        var ILRuntimeJsonArr = Encoding.UTF8.GetBytes(s_ILRuntimeJson);
        Array.Copy(ILRuntimeJsonArr, 0, s_RAM, 510976, ILRuntimeJsonArr.Length);

        var srcCodeArr = System.IO.File.ReadAllBytes(srcCodePath);
        Array.Copy(srcCodeArr, 0, s_RAM, 524288, srcCodeArr.Length);

        // ------ FFLang Entry point
        int exitCode = ILGenerator.main();
        // -------------------------

        System.IO.File.WriteAllText("FFLang_Program.il", ReadString(671744, 360448));

        System.IO.File.WriteAllBytes(ramDataFile, s_RAM);

        Console.WriteLine($"Exit code: {exitCode}");
    }

    public static string ReadString(int str_ptr, int len)
    {
        var str = Encoding.UTF8.GetString(s_RAM.AsSpan(str_ptr, len));
        str = str.Substring(0, str.IndexOf('\0'));
        return str;
    }
}
