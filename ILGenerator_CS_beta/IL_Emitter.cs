using System.Text;

static class IL_Emitter
{
    static StringBuilder s_sb = new StringBuilder();
    static int s_ifElseScopeCount = 0;

    static string WriteIL_Begin()
    {
        return @"
// FFLang Program - lang version 0


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


.class private abstract auto ansi sealed beforefieldinit Program
       extends [System.Runtime]System.Object {

.field private static uint8[] '$_RAM'

.method private hidebysig static void  '$cs_Main'(string[] args) cil managed
{
    .entrypoint

    .maxstack  5
    .locals init (string V_0, int32 V_1, uint8[] V_2,
                valuetype [System.Runtime]System.Runtime.CompilerServices.DefaultInterpolatedStringHandler V_3)

    //ldc.i4     0x40000000 // 1GB
    ldc.i4     104857600 // 100MB
    newarr     [System.Runtime]System.Byte
    stsfld     uint8[] Program::'$_RAM'
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
    ldsfld     uint8[] Program::'$_RAM'
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
    call       int32 Program::main()
    stloc.1
    ldloc.0
    ldsfld     uint8[] Program::'$_RAM'
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
} // end of method Program::'$cs_Main'


// ========== FFLang Built-ins

.method public hidebysig static int32  __builtin_int_add(int32 a, int32 b) cil managed
{
    .maxstack  2

    ldarg.0
    ldarg.1
    add
    ret
}

.method public hidebysig static int32  __builtin_int_nand(int32 a, int32 b) cil managed
{
    .maxstack  2

    ldarg.0
    ldarg.1
    and
    not
    ret
}

.method public hidebysig static int32  __builtin_int_mem_read(int32 addr) cil managed
{
    .maxstack  2

    ldsfld     uint8[] Program::'$_RAM'
    ldarg.0
    call       valuetype [System.Runtime]System.Span`1<!!0> [System.Memory]System.MemoryExtensions::AsSpan<uint8>(!!0[], int32)
    call       valuetype [System.Runtime]System.ReadOnlySpan`1<!0> valuetype [System.Runtime]System.Span`1<uint8>::op_Implicit(valuetype [System.Runtime]System.Span`1<!0>)
    call       int32 [System.Memory]System.Buffers.Binary.BinaryPrimitives::ReadInt32LittleEndian(valuetype [System.Runtime]System.ReadOnlySpan`1<uint8>)
    ret
}

.method public hidebysig static int32  __builtin_int_mem_write(int32 addr, int32 data) cil managed
{
    .maxstack  2

    ldsfld     uint8[] Program::'$_RAM'
    ldarg.0
    call       valuetype [System.Runtime]System.Span`1<!!0> [System.Memory]System.MemoryExtensions::AsSpan<uint8>(!!0[], int32)
    ldarg.1
    call       void [System.Memory]System.Buffers.Binary.BinaryPrimitives::WriteInt32LittleEndian(valuetype [System.Runtime]System.Span`1<uint8>, int32)
    ldc.i4.0
    ret
}

.method public hidebysig static bool  __bool_check(int32 v) cil managed
{
    .maxstack  2

    ldarg.0
    ldc.i4.0
    cgt.un
    ret
}

// ================================


// ========== FFLang program START
";
    }

    static string WriteIL_End()
    {
        return @"

// ========== FFLang program END

} // end of class Program
";
    }

    static void CreateRuntimeConfigJson()
    {
        string json = @"
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

        if (!System.IO.Directory.Exists("out"))
        {
            System.IO.Directory.CreateDirectory("out");
        }

        System.IO.File.WriteAllText("out\\FFLang_Program.runtimeconfig.json", json);
    }

    public static void InitEmitter()
    {
        s_sb = new StringBuilder(15000);
        s_sb.AppendLine(WriteIL_Begin());
    }

    public static void FinishEmitter()
    {
        s_sb.AppendLine(WriteIL_End());

        if (!System.IO.Directory.Exists("out"))
        {
            System.IO.Directory.CreateDirectory("out");
        }

        System.IO.File.WriteAllText("out\\FFLang_Program.il", s_sb.ToString());

        CreateRuntimeConfigJson();
    }

    public static void Emit_Label(string label)
    {
        s_sb.AppendLine($"{label}:");
    }

    public static void Emit_Goto(string label)
    {
        s_sb.AppendLine($"    {"br.s",-11}{label}");
    }

    public static void Emit_Call(string functionName, int argsCount)
    {
        s_sb.Append($"    {"call",-11}int32 Program::'{functionName}'(");
        for (int i = 0; i < argsCount; i++)
        {
            s_sb.Append("int32");
            if (i < argsCount - 1)
            {
                s_sb.Append(", ");
            }
        }
        s_sb.AppendLine(")");
    }

    public static void Emit_BoolCheck()
    {
        s_sb.AppendLine($"    {"call",-11}bool Program::{"__bool_check"}(int32)");
    }

    public static void Emit_IfBegin(int scopeDepth)
    {
        s_sb.AppendLine($"    {"brfalse.s",-11}'IF_FALSE_${s_ifElseScopeCount}_${scopeDepth}'");
    }

    public static void Emit_Else(bool hasBlock, int scopeDepth)
    {
        if (hasBlock)
        {
            s_sb.AppendLine($"    {"br.s",-11}'IF_END_${s_ifElseScopeCount}_${scopeDepth}'");
        }
        s_sb.AppendLine($"'IF_FALSE_${s_ifElseScopeCount}_${scopeDepth}':");
    }

    public static void Emit_IfEnd(int scopeDepth)
    {
        s_sb.AppendLine($"'IF_END_${s_ifElseScopeCount}_${scopeDepth}':");
        if (scopeDepth == 0)
        {
            s_ifElseScopeCount++;
        }
    }

    public static void Emit_DiscardReturn()
    {
        s_sb.AppendLine($"    {"pop"}");
    }

    public static void Emit_Return()
    {
        s_sb.AppendLine($"    {"ret"}");
    }

    public static void Emit_LoadConst(string v)
    {
        s_sb.AppendLine($"    {"ldc.i4",-11}{v}");
    }

    public static void Emit_LoadLocalVar(int index)
    {
        s_sb.AppendLine($"    {"ldloc.s",-11}{index}");
    }

    public static void Emit_LoadArgVar(int index)
    {
        s_sb.AppendLine($"    {"ldarg.s",-11}{index}");
    }

    public static void Emit_StoreLocalVar(int index)
    {
        s_sb.AppendLine($"    {"stloc.s",-11}{index}");
    }

    public static void Emit_StoreArgVar(int index)
    {
        s_sb.AppendLine($"    {"starg.s",-11}{index}");
    }

    public static void Emit_MethodHeader(string name, int paramCount)
    {
        s_sb.Append($".method public hidebysig static int32  '{name}'(");
        for (int i = 0; i < paramCount; i++)
        {
            s_sb.Append("int32");
            if (i < paramCount - 1)
            {
                s_sb.Append(", ");
            }
        }
        s_sb.AppendLine($") cil managed");
    }

    public static void Emit_MethodBodyBegin()
    {
        s_ifElseScopeCount = 0;
        s_sb.AppendLine($"{{");
        s_sb.AppendLine($"    {".maxstack",-11}8");
        s_sb.AppendLine();
    }

    public static void Emit_MethodBodyEnd()
    {
        s_sb.AppendLine($"}}");
        s_sb.AppendLine();
    }

    public static void Emit_InitLocals(int count)
    {
        s_sb.Append($"    .locals init (");
        for (int i = 0; i < count; i++)
        {
            s_sb.Append("int32");
            if (i < count - 1)
            {
                s_sb.Append(", ");
            }
        }
        s_sb.AppendLine($")");
        s_sb.AppendLine();
    }

    public static void RunILASM()
    {
        if (!System.IO.Directory.Exists("out"))
        {
            System.IO.Directory.CreateDirectory("out");
        }

        System.Diagnostics.Process.Start("cmd.exe", "/C C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\ilasm.exe out\\FFLang_Program.il /dll");
    }
}
