### General

- **Type System**: Statically typed.
- **Execution Model**: Stack-based execution with explicit memory access.
- **Entry Point**: The program starts execution at a function named `main`.
- **Control Flow**: Uses `if/else`, `goto`, and labels for branching.

### Features of FFLang

- Simple Syntax: Focuses on minimalism and clarity, using basic constructs inspired by popular languages.
- Targeting .NET IL: Initially, the language compiles to .NET IL (Intermediate Language), with the goal of moving toward direct native code compilation.

### Variables
- The `var` statement declares a variable that is stack-allocated and initialized with a default value.
- The `set` statement assigns a value to an existing variable.

### Functions

- Parameters are passed by value.
- All functions must return a value. A `Void` type will be introduced in future versions for functions without return values.
- To explicit discard a value, use the **discard statement** by doing `_ = someFunction(...)`.
- Recursion is allowed.

### Control Flow

- `goto` can only jump to labels **inside the same function**, either forward or backward.
- It **cannot** be used to jump into or out of functions.
- It also **cannot** jump into inner `if/else` scopes.
- `else` is optional after `if` block.

### Built-in Functions
Built-in functions must be provided by the compiler, with low-level implementation:

- `func __builtin_int_mem_read(addr: Int) -> Int` Reads an integer of word size from memory at the given address and returns its value
- `func __builtin_int_mem_write(addr: Int, data: Int) -> Int` Writes an integer of word size value to the specified memory address. Always returns 0, so return can be discarded.
- `func __builtin_int_nand(a: Int, b: Int) -> Int` Returns the NAND result of two integers of word size
- `func __builtin_int_add(a: Int, b: Int) -> Int` Returns the ADD result of two integers of word size

Note: `__builtin_int_mem_read` and `__builtin_int_mem_write` may trigger processor exceptions for out-of-bounds and alignment errors.  
The compiler does not enforce any alignment and assumes those errors are handled by the hardware.

### Restrictions

The below restrictions may be relaxed on newer versions of the language.

- **Single compilation unit:** All code must be present in the same source code file.
- **Global namespace:** All code is currently part of the hidden global namespace. Formal namespace naming is planned for future versions.
- **Identifiers:** Currently, only ASCII characters are allowed. If starting with `_`, it must contains at least one more character.
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
- **Line comments:** A single-line comment starts with `//`, extends to the end of the line, and is ignored at lexer level.

### Goals

- The `version 0` of the language aims to have enough features so that a self-hosting compiler can be written in itself.
- New features will be introduced incrementally once self-hosting is achieved.
- Future compiler versions will strive to maintain compatibility with all legacy code.

### Inspirations

- [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [C](https://en.cppreference.com/w/c) / [C++](https://en.cppreference.com/w/cpp)
- [Swift](https://www.swift.org/)
- [Java](https://dev.java/)
- ['Jack' from the book - The Elements of Computing Systems: Building a Modern Computer from First Principles, by Noam Nisan and Shimon Schocken](https://www.amazon.com/Elements-Computing-Systems-second-Principles/dp/0262539802/)

### Contributions

At this stage, the project is not open for contributions. However, bug reports and feedback are always welcome. Please feel free to open an issue if you encounter any bugs or have suggestions.

### License

The project is licensed under the MIT License, which allows for maximum flexibility in usage, modification, and redistribution. See the [LICENSE](LICENSE) file for more details.