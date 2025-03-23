import sys
import os

RAM_FILE = "RAM.bin"
RAM_SIZE = 2 * 1024 * 1024  # 2 MB
INJECT_OFFSET = 512 * 1024  # 512 KB
MAX_INPUT_SIZE = 512 * 1024  # 512 KB

def create_ram_file():
    """Creates RAM.bin with a size of 2MB filled with zeroes if it does not exist."""
    with open(RAM_FILE, "wb") as f:
        f.write(b'\x00' * RAM_SIZE)

def inject_data(input_file):
    """Injects the input file's binary data into RAM.bin at offset 512KB."""
    # Check if input file exists
    if not os.path.isfile(input_file):
        print(f"Error: Input file '{input_file}' does not exist.")
        sys.exit(1)

    # Check input file size
    input_size = os.path.getsize(input_file)
    if input_size > MAX_INPUT_SIZE:
        print("Error: Input file exceeds 512 KB.")
        sys.exit(1)

    # Ensure RAM.bin exists and has a size of at least 2 MB
    if not os.path.isfile(RAM_FILE) or os.path.getsize(RAM_FILE) < RAM_SIZE:
        print("Creating RAM.bin...")
        create_ram_file()

    # Read input file data
    with open(input_file, "rb") as f:
        input_data = f.read()

    # Inject data into RAM.bin
    with open(RAM_FILE, "r+b") as f:
        f.seek(INJECT_OFFSET)
        f.write(input_data)
        f.write(b'\x00')  # Append null byte to truncate any leftover data

    print("Injection completed successfully.")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python inject.py <input_file>")
        sys.exit(1)

    inject_data(sys.argv[1])
