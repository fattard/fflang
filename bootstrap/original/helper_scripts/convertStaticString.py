import sys

def escape_char(char):
    """Returns the escaped representation of special characters for comments."""
    if char == '\t':
        return r"\t"
    elif char == '\n':
        return r"\n"
    elif char == '\r':
        return r"\r"
    elif char == "'":
        return r"\'"  # Escape single quotes
    return char

def convert_file(input_filename, output_filename):
    with open(input_filename, "r", encoding="utf-8-sig") as infile, open(output_filename, "w", encoding="utf-8") as outfile:
        for char in infile.read():
            char_code = ord(char)
            escaped_char = escape_char(char)
            outfile.write(f"_ = wc({char_code}); // '{escaped_char}'\n")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print(f"Usage: {sys.argv[0]} <input_file> <output_file>")
    else:
        convert_file(sys.argv[1], sys.argv[2])
