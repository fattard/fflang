## ILGenerator (C#) [Beta]

This project parses **FFLang** source code and generates **.NET IL** (Intermediate Language) code.

### Introduction

- Written in C#, using minimal language-specific constructs to facilitate potential porting to other languages.
- Uses a single-pass recursive descent parser that aborts on the first encountered error.
- Emits .NET IL code based on the input FFLang source.
- The generated IL code is assembled into a .NET 8 application (DLL format) using the `ilasm` tool, which needs to be available in the **path**.

### Usage

```
ILGenerator_CS <src_file_path> [--ramBinLength=<length>] [--debug]
```

1. Pass the FFLang source code file path as `src_file_path`.
2. Optionally, pass a value (in MB) between 1 and 1024 to the `--ramBinLength=` parameter to control the size of `RAM.bin` file. The default value is 100.
3. Optionally, use the flag `--debug` to enable the use of built-in: `func __dbg_int(v: Int) -> Int`
4. Address any lexing, syntactic, and semantic issues reported by the parser.
5. The generated IL code will be output to the `out` folder at the project root.
6. This IL code is automatically assembled into a .NET app using `ilasm`, and the result will also be placed in the `out` folder.
7. The `ilasm` tool should not report any issues during assembly.

### Running the Generated App

1. The generated .NET 8 app can be executed using the following command:
```
dotnet FFLang_Program.dll
```
2. Check for .NET exceptions logged to the console during execution.
3. Confirm that the `Exit Code` logged at the end of the execution matches the expected result for the FFLang application.
4. Use a hex editor to inspect the memory footprints left in the `RAM.bin` file to ensure proper execution.
5. The `RAM.bin` file is reused on subsequent launches. If needed, manually clean the file before running the app again.

### Available Macros

The parser replaces the following macros by their defined `literal number` representation.  
For a .NET application, the `Int` type in FFLang maps to .NET `int32`, so the values are:

```
__INT_MAX__              2147483647
__INT_WIDTH_BITS__       32
__INT_WIDTH_BYTES__      4
__INT_SIGN_BIT_MASK__    2147483648
```
**Note:** `__INT_MIN__` is not defined, as negative numbers are not yet supported.

### Debug Function

When `--debug` flag is specified, the following function is available:

```fflang
func __dbg_int(v: Int) -> Int
```
Prints the text representation of the specified value of `v` to the application standard output stream for. Always returns 0, so return can be discarded.