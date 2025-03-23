## Bootstrapping FFLang Using C#

## Introduction
The initial bootstrap of `FFLang` was achieved through C#, starting with the manually written `Parser_CS` beta project. Over time, this evolved into `ILGenerator_CS`, an IL code emitter capable of generating .NET IL applications.

By combining the [base IL runtime](runtime_IL/FFLang_BaseIL.il), built-in FFLang implementations, and IL-generated code from the parser, a fully functional .NET application can be created and executed, from FFLang source code.

### Why C# as the First Path?
The choice of C# was deliberate:
- **Familiarity**: The author was most proficient in C# at the time.
- **IL Integration**: C# outputs IL code, and the .NET SDK provides `ilasm` and `ildasm` tools for assembling and disassembling IL.
- **Simplicity**: IL is well-defined and easier to manipulate manually compared to other stack-based runtime languages, such as Java Bytecode.

### Writing the IL Generator in FFLang
The original `ILGenerator_CS` was a single-pass recursive descent parser designed for easy manual implementation. It was structured to avoid dependencies on C#-exclusive features, instead relying on fundamental data structures like strings and lists for easier porting.

However, porting it directly to FFLang presented challenges due to the language's constraints. The solution was a [rewrite of the generator](reference_ILGenerator_CS), adhering strictly to:
- Constructs available in FFLang.
- Using only `int` as the primary data type.
- Simulating the runtime environment, including a memory buffer for heap management.
- Explicit, unbounded memory access.

A structured [memory layout](mem_layout.txt) was developed, allocating space for variables, tables, and global data. Initially targeting 1 MB, the allocation was later increased to 2 MB to support larger IL code output (10K+ lines).

### Splitting the Project for Efficiency
To optimize code handling, the project was split into three programs:
1. **[PrepareStaticStrings.ffsrc](PrepareStaticStrings.ffsrc)**: Initializes FFLang keywords and constructs as static strings.
2. **[PrepareStaticStrings_IL.ffsrc](PrepareStaticStrings_IL.ffsrc)**: Initializes IL instructions and directives as static strings.
3. **[ILGenerator.ffsrc](ILGenerator.ffsrc)**: The core parser and IL code emitter.

Since FFLang `version 0` lacks I/O support, manual intervention is required for input injection and output extraction. A shared `RAM.bin` file holds offsets for input and output, with [helper scripts](helper_scripts) (Python-based) streamlining the injection and extraction process.

Additionally, a script was created to convert text files into static string construction code for embedding large data, such as the base IL runtime (8 KB), directly into the source.

### Achieving Self-Hosting
By injecting FFLang source code into the compiler and extracting IL output, the FFLang-based compiler produced results identical to the C# reference implementation. This verified the correctness of the self-hosted compiler, marking the successful completion of the bootstrap cycle.


## Instructions

### Referenced Applications
- **ILGenerator_CS**: The initial C# parser and IL emitter, generating .NET IL applications from FFLang source code.
- **ILGenerator_CS_Reference**: A C# rewrite of the parser with constraints mirroring FFLang, used for debugging and behavior simulation.
- **ILGenerator_fflang**: The FFLang-based parser and IL emitter, capable of self-compilation.
- **ILGenerator_fflang_self**: The self-hosted version of `ILGenerator_fflang`, compiled using itself, which is expected to be functionally identical.
- **ilasm**: The .NET SDK tool for assembling IL code into executable applications (must be in `PATH`).

### Steps

Here are all the steps needed to achieve self-hosting using the C# path

#### Step 1 - Build the reference ILGenerator
- Navigate to `reference_ILGenerator_CS` folder.
- Compile and test the project to ensure it correctly parses, generates IL code, and produces a valid .NET application.
- Verify the generated IL output of ILGenerator_CS_reference against expected results.

#### Step 2 - Generate the IL Generator Using FFLang
- Compile the three key components of the IL Generator using the C# reference compiler:
    - `ILGenerator_CS_reference --ramBinLength=2 --debug PrepareStaticStrings.ffsrc`
    - Rename the generated files as follows:
    ```
    PrepareStaticStrings_fflang.il
    PrepareStaticStrings_fflang.dll
    PrepareStaticStrings_fflang.runtimeconfig.json
    ```
    - `ILGenerator_CS_reference --ramBinLength=2 --debug PrepareStaticStrings_IL.ffsrc`    
    - Rename the generated files as follows:
    ```
    PrepareStaticStrings_IL_fflang.il
    PrepareStaticStrings_IL_fflang.dll
    PrepareStaticStrings_IL_fflang.runtimeconfig.json
    ```
    - `ILGenerator_CS_reference --ramBinLength=2 --debug ILGenerator.ffsrc`    
    - Rename the generated files as follows:
    ```
    ILGenerator_fflang.il
    ILGenerator_fflang.dll
    ILGenerator_fflang.runtimeconfig.json
    ```
- Ensure that the process completes without errors.

#### Step 3 - Prepare the RAM.bin environment
- Run the following programs to populate `RAM.bin` with static strings needed by the ILGenerator:
    - `dotnet PrepareStaticStrings_fflang.dll`
    - `dotnet PrepareStaticStrings_IL_fflang.dll`
- Verify:
    - No runtime errors.
    - `RAM.bin` is generated successfully (expected size: 2 MB).

#### Step 4 - Inject Source Code into RAM.bin and Extract IL Output
- Inject the `PrepareStaticStrings.ffsrc` into `RAM.bin` at offset 512 KB.
- Run
    - `dotnet ILGenerator_fflang.dll`
- Ensure that no errors occurred.
- Extract the generated IL output from `RAM.bin` at offset 1 MB and save as `PrepareStaticStrings_fflang_self.il`
- Repeat the process for `PrepareStaticStrings_IL.ffsrc` and `ILGenerator.ffsrc`.
- Compare the extracted IL files with previously generated IL: 
    ```
    PrepareStaticStrings_fflang.il and PrepareStaticStrings_fflang_self.il
    PrepareStaticStrings_IL_fflang.il and PrepareStaticStrings_IL_fflang_self.il
    ILGenerator_fflang.il and ILGenerator_fflang_self.il
    ```
- A perfect match confirms successful self-hosting.

#### Step 5 - Manually assemble IL code
- To manually assemble the generated IL files from ILGenerator_fflang, use `ilasm` as:
    ```
    ilasm <IL_file>.il /DLL
    ```
