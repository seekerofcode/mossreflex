#! /usr/bin/python3

import os
import sys

# Function to check if a file is executable
def is_executable(filename):
    return os.access(filename, os.X_OK)

# Function to delete executable files within a directory hierarchy
def delete_executables(dir_path):
    for root, dirs, files in os.walk(dir_path):
        for file in files:
            file_path = os.path.join(root, file)
            if is_executable(file_path):
                os.remove(file_path)
                print(f"Deleted {file_path}")

# Example usage
exe_path = sys.argv[1]
delete_executables(exe_path)
