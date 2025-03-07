## ILGenerator (C#) [Beta]

This project parses and generates .NET IL code from FFLang source code.

### Introduction
- Written in C#, using minimal language-specific constructs to facilitate potential porting to other languages.
- It is a single-pass recursive descent parser, meaning it reports the first error encountered and aborts execution.
- The tool emits .NET IL code based on the input FFLang source code.
- The generated IL code is assembled into a functional .NET 8 app (DLL format) using the `ilasm` tool.

### Usage

```
ILGenerator_CS <src_file_path> [--ramBinLength=<length>] [--debug]
```

1. Pass the FFLang source code file path as an argument to the program.
2. Optionally, pass a value (in MB) between 1 and 1024 to the `--ramBinLength=` parameter to control the size of `RAM.bin` file. The default value is 100.
3. Optionally, use the flag `--debug` to enable the use of built-in: `func __dbg_int(v: Int) -> Int`
4. Address any lexing, syntactic, and semantic issues reported by the parser.
5. The generated IL code will be output to the `out` folder at the project root.
6. This IL code is automatically assembled into a .NET app using `ilasm`, and the result will also be placed in the `out` folder.
7. The `ilasm` tool should not report any issues during assembly.

### Running the Generated App
1. The generated .NET 8 app can be executed using the following command:
```
dotnet.exe FFLang_Program.dll
```
2. Observe any .NET exceptions logged to the console during execution.
3. Confirm that the `Exit Code` logged at the end of the execution matches the expected result for the FFLang application.
4. Use a hex editor to inspect the memory footprints left in the `RAM.bin` file to ensure proper execution.
5. The `RAM.bin` file is reused on subsequent launches. If needed, manually clean the file before running the app again.