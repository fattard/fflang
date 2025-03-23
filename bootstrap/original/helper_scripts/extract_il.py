import sys
import os

RAM_FILE = "RAM.bin"
EXTRACT_OFFSET = 1 * 1024 * 1024  # 1 MB

def extract_data(output_file):
    """Extracts text data from RAM.bin starting at 1MB until first null byte or EOF and saves to output_file."""
    # Check if RAM.bin exists and has a minimum size of 2 MB
    if not os.path.isfile(RAM_FILE) or os.path.getsize(RAM_FILE) < 2 * 1024 * 1024:
        print("Error: RAM.bin does not exist or is smaller than 2 MB.")
        sys.exit(1)

    with open(RAM_FILE, "rb") as f:
        f.seek(EXTRACT_OFFSET)
        data = f.read()

    # Extract data until first null byte or EOF
    extracted_text = data.split(b'\x00', 1)[0]

    # Save to specified output file (overwrite if exists)
    with open(output_file, "wb") as f:
        f.write(extracted_text)

    print(f"Extraction completed. Saved to {output_file}")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python extract.py <output_file>")
        sys.exit(1)

    extract_data(sys.argv[1])
