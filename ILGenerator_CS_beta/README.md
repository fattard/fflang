## ILGenerator (C#) [Beta]

This project parses and generates .NET IL code for FFLang source code.

### Introduction
- Written in C#, using minimal language-specific constructs to facilitate potential porting to other languages.
- It is a single-pass recursive descent parser, meaning it reports the first error encountered and aborts execution.
- The tool emits .NET IL code based on the input source code.
- The generated IL code is assembled into a functional .NET 8 app (DLL format) using the `ilasm` tool.

### Usage

1. Pass the FFLang source code file path as a single argument to the program.
2. Address any lexing, syntactic, and semantic issues reported by the parser.
3. The generated IL code will be output to the `out` folder at the project root.
4. This IL code is automatically assembled into a .NET app using `ilasm`, and the result will also be placed in the `out` folder.
5. The `ilasm` tool should not report any issues during assembly.

### Running the Generated App
1. The generated .NET 8 app can be run using the following command:
```
dotnet.exe FFLang_Program.dll
```
2. Observe any .NET exceptions logged to the console during execution.
3. Confirm that the `Exit Code` logged at the end of the execution matches the expected result for the FFLang application.
4. Use a hex editor to inspect the memory footprints left in the `RAM.bin` file to ensure proper execution.
5. The `RAM.bin` file is reused on subsequent launches. If needed, manually clean the file before running the app again.