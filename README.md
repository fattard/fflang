### Introduction

`FFLang` is a toy programming language created as a research project on how programming languages evolve over time.  
The `version 0` of the language is the current version. It is currently in the **beta** phase.
- [Grammar](grammar.txt) for `version 0` is stable and considered feature complete.
- A reference parser and code generator (targeting .NET IL output), also in **beta**, is provided as a .NET 8 C# [project](ILGenerator_CS_beta).

### Code Example

**FFLang** uses a syntax inspired by constructs from popular languages.  
```fflang
using func sub(a: Int, b: Int) -> Int;
using func mul(a: Int, b: Int) -> Int;

func computeFactorial(n: Int) -> Int
{
    var result: Int;

    set result = 1;

    LOOP:
        if (__bool_check(n))
        {
            set result = mul(result, n);
            set n = sub(n, 1);
            goto LOOP;
        }

    return result;
}
```
See a fully working snippet [here](snippet.ffsrc).

### General
- **Type System**: Statically typed.
- **Execution Model**: Stack-based execution with explicit memory access.
- **Entry Point**: The program starts execution at a function named `main`.
- **Control Flow**: Uses `if/else`, `goto`, and labels for branching.

### Variables

- The `var` statement declares a variable that is stack-allocated and initialized with a default value.
- The `set` statement assigns a value to an existing variable.

### Functions

- Parameters are passed by value.
- All functions must return a value. A `Void` type may be introduced in future versions for functions without return values.
- To explicit discard a value, use the **discard statement** by doing `_ = someFunction(...)`.
- Recursion is allowed.

### Control Flow

- `goto` can only jump to labels **inside the same function**, either forward or backward.
- It **cannot** be used to jump into or out of functions.
- It also **cannot** jump into inner `if/else` scopes.
- `else` is optional after `if` block.

### Built-in Functions

Built-in functions are **mandatory** and the compiler must provide them using optimized low-level implementations when possible.
Any target code output implementation of FFLang **must** support the following built-ins:

- `func __builtin_int_mem_read(addr: Int) -> Int` Reads an integer of word size from memory at the given address and returns its value
- `func __builtin_int_mem_write(addr: Int, data: Int) -> Int` Writes an integer of word size value to the specified memory address. Always returns 0, so return can be discarded.
- `func __builtin_int_nand(a: Int, b: Int) -> Int` Returns the NAND result of two integers of word size
- `func __builtin_int_add(a: Int, b: Int) -> Int` Returns the ADD result of two integers of word size

**Note:** `__builtin_int_mem_read` and `__builtin_int_mem_write` may trigger processor exceptions for out-of-bounds and alignment errors.  
The compiler does not enforce any alignment and assumes those errors are handled by the hardware.

### Restrictions

The below restrictions may be relaxed on newer versions of the language.

- **Single compilation unit:** All code must be present in the same source code file.
- **Global namespace:** All code is currently part of the hidden global namespace. Formal namespace support is planned for future versions.
- **Identifiers:** Currently, only ASCII characters are allowed. If starting with `_`, it must contain at least one more character.
- **Literal numbers:** Only positive decimal numbers are accepted for now. Negative numbers are not supported yet, as they are unnecessary for writing the self-hosting compiler, which is the primary goal of this language version.
- **Forward declaration:** The `using` directive allows forward declaration. Currently, only functions can be forward declared, and all declarations must be at the top of the namespace.
- **Local variables only:** All variables are **local to the function** and must be declared at the top of the function body, before any statements.
- **Return:** All functions must include a single `return`, as the last statement.
- **Restricted `goto` usage:** Cannot bypass variable initialization scopes. Labels must be declared after all variable declarations.
- **Boolean expression:** The use of `__bool_check(expression)` is **mandatory for all `if` conditions**. It is a special construct only usable in `if` conditions and cannot be called like a regular function. It evaluates a given expression (variable, literal number, or function invocation) and returns `0` on zero and `1` on non-zero, enforcing boolean semantics.
- **Data type:** The `Int` type represents a signed integer that matches the word size of the target processor, and is the only data type available.
- **No I/O support:** Currently input data must be pre-loaded to memory, and persistent output must be written to memory and retrieved after execution ends.
- **Entry point:** `main()` must return a value, either a computed result or an exit status.
- **Runtime init:** Before calling `main()`, the runtime environment sets up the **stack pointer** to an appropriate initial address, ensuring space for function calls and local variables.
- **Line comments:** A single-line comment starts with `//`, extends to the end of the line. It is ignored during compilation.

##

### Building FFLang Apps (IL Code Target)

Use the provided `ILGenerator_CS` tool:
```
Usage:
    ILGenerator_CS <src_file_path> [--ramBinLength=<length>] [--debug]

Params:
    src_file_path:               Path to the FFLang source code file
    --debug:                     (Optional) Includes built-in: 'func __dbg_int(v: Int) -> Int'
    --ramBinLength=<length>:     (Optional) Specifies 'RAM.bin' file size in MB.
                                 Allowed range: 1 to 1024. Default 100
```
The generated IL code is automatically assembled into a .NET app using the `ilasm`, and the result is placed in the `out` folder.  
The `ilasm` tool is part of the [.NET SDK](https://github.com/dotnet/runtime/tree/main/src/coreclr/ilasm) and must be available in **path**.

**Note:** The `func __dbg_int(v: Int) -> Int` is **not** an official FFLang built-in.  
It is provided **exclusively** in the IL Code target as a convenience for simple debugging by printing values during the **beta** phase.

### Running FFLang App (IL Code Target)

1. The generated .NET 8 app can be executed using the following command:
```
dotnet FFLang_Program.dll
```
2. Observe any .NET exceptions logged to the console during execution.
3. Confirm that the `Exit Code` logged at the end of the execution matches the expected result for the FFLang application.
4. Use a hex editor to inspect the memory footprints left in the `RAM.bin` file to ensure proper execution.
5. The `RAM.bin` file is reused on subsequent launches. If needed, manually clean the file before running the app again.

##

### Goals

- Initially, the language compiles to .NET IL (Intermediate Language), with a long term goal of moving toward direct native code compilation.
- The main goal of `version 0` of the language is have enough features so that a self-hosting compiler can be written in itself.
- New features will be introduced incrementally once self-hosting is achieved.
- Future compiler versions will strive to maintain compatibility with all legacy code.

### Inspirations

- [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [C](https://en.cppreference.com/w/c) / [C++](https://en.cppreference.com/w/cpp)
- [Swift](https://www.swift.org/)
- [Java](https://dev.java/)
- ['Jack' from the book - The Elements of Computing Systems: Building a Modern Computer from First Principles, by Noam Nisan and Shimon Schocken](https://www.amazon.com/Elements-Computing-Systems-second-Principles/dp/0262539802/)

### Contributions

At this stage, the project is not open for contributions. However, bug reports and feedback are always welcome.  
Please feel free to open an issue if you encounter any bugs or have suggestions.

### License

All projects and contents within this repository, including tools, language source code, and related files, are licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.