import sys
import os
import shutil

# This script unpacks a directory of assignments that have been downloaded from
# moodle. The directory structure is this:
# 212 program submissions 2023
# ├── prog2 2023
# │   ├── prog2 2023 sample code
# │   │   └── prog2
# │   │       ├── BabbleSample.cs
# │   │       ├── BabbleSample.exe
# │   │       ├── BabbleSample.xaml
# │   │       ├── HashExample.cs
# │   │       ├── Sample.txt
# │   │       ├── leviticus.txt
# │   │       ├── responsible.txt
# │   │       ├── romans.txt
# │   │       ├── tale.txt
# │   │       └── tiny.txt
# │   └── prog2 2023 submissions
# │       └── 23FA CS-212-A-Program 2 Babble-1646496
# │           ├── Adham Rishmawi_3556713_assignsubmission_file
# │           │   └── Babble-prog2.zip
# │           ├── Adham Rishmawi_3556713_assignsubmission_onlinetext
# │           │   └── onlinetext.html
# │           ├── Alex Prasser_3556716_assignsubmission_file
# │           │   └── CS212_H2.zip
# │           ├── Alex Prasser_3556716_assignsubmission_onlinetext
# │           │   └── onlinetext.html
# │           ├── Alex White_3556683_assignsubmission_file
# │           │   └── cs212prog2.zip
# │           ├── Alex White_3556683_assignsubmission_onlinetext
# │           │   └── onlinetext.html
# │           ├── Alisha Start_3556664_assignsubmission_file
# │           │   └── alisha_start_babble2.zip
# etc etc etc.

# The script mostly just cleans up directory and file name, replacing spaces
# with underscores. It also unpacks zip files into the new directory structure.

# If you want to see what the script is doing, set VERBOSE to True.

VERBOSE = False

if len(sys.argv) != 3:
    print("Usage: python3 unpack.py <dirToUnpack> <newdir>")
    sys.exit(1)

dirToUnpack = sys.argv[1]
newDir = sys.argv[2]

if os.path.exists(newDir):
    print("Directory", newDir, "already exists")
    sys.exit(1)

os.mkdir(newDir)
print("Created directory", newDir)

dirs = os.scandir(dirToUnpack)
for entry in dirs:
    # the top-level directory may have a list of assignments below it:
    # e.g., prog2 2023, prog3 2023, etc.
    if entry.is_dir():
        if VERBOSE: print(entry.path, "is a directory")

        # if the name has spaces in it, replace them with underscores
        newName = entry.name.replace(" ", "_")
        newAssignmentDir = os.path.join(newDir, newName)
        os.mkdir(newAssignmentDir)
        if VERBOSE: print("Created", newAssignmentDir)

        # under the directory, which is the top directory for an assignment
        # there may be two directories: a submissions directory and a sample code
        # directory: e.g., 'prog2 2023 sample code', 'prog2 2023 submissions'
        # with spaces, of course... :-(
        oldAssignmentDir = entry.path
        subDirs = os.scandir(oldAssignmentDir)
        for subEntry in subDirs:
            if 'sample code' in subEntry.name:
                if VERBOSE: print(subEntry.path, "is a sample code directory")
                newSampleDir = os.path.join(newAssignmentDir, "sample_code")
                os.mkdir(newSampleDir)
                if VERBOSE: print("Created", newSampleDir)

                # under the sample code directory there may be more than one
                # subdirectories, each with sample code in them.
                sampleDirSubDirs = os.scandir(subEntry.path)
                for sampleDirSubDir in sampleDirSubDirs:
                    if sampleDirSubDir.is_dir():
                        if VERBOSE: print('\t', sampleDirSubDir.path, "is a sample code subdirectory")
                        newSampleSubDir = os.path.join(newSampleDir, sampleDirSubDir.name)
                        # copy the files from the old directory to the new directory
                        shutil.copytree(sampleDirSubDir.path, newSampleSubDir)
                        if VERBOSE: print('\tCopied tree from', sampleDirSubDir.path, 'to', newSampleSubDir)

            elif 'submissions' in subEntry.name:
                if VERBOSE: print(subEntry.path, "is a submissions directory")
                newSubmissionsDir = os.path.join(newAssignmentDir, "submissions")
                os.mkdir(newSubmissionsDir)
                if VERBOSE: print("Created", newSubmissionsDir)

                # under the submissions directory there will be a dumb directory
                # with a name like this: 23FA CS-212-A-Program 2 Babble-1646496
                # just go down into that directory -- don't recreate it in the new tree
                subSubDirs = os.scandir(subEntry.path)
                for subSubEntry in subSubDirs:
                    if subSubEntry.is_dir():

                        studentDirs = os.scandir(subSubEntry.path)
                        for studentDir in studentDirs:
                            if studentDir.path.endswith("_onlinetext"):
                                # print("Skipping ", studentDir.path)
                                continue
                            # Student directory containing code ends with _file. Like this:
                            # Adham Rishmawi_3556713_assignsubmission_file
                            # Make a new directory in the resulting tree called Adhim_Rishmawi.
                            studentDirName = studentDir.name.split("_")[0].replace(" ", "_")
                            # print('\t\t', studentDirName, "is a student directory")
                            targetStudentDir = os.path.join(newSubmissionsDir, studentDirName)
                            os.mkdir(targetStudentDir)

                            for studentFile in os.scandir(studentDir.path):
                                if studentFile.is_file():
                                    if studentFile.name.endswith(".zip"):
                                        shutil.unpack_archive(studentFile.path, targetStudentDir)
                                        if VERBOSE: print('\t\tUnpacked', studentFile.name, 'to', targetStudentDir)
                                    else:
                                        shutil.copy(studentFile.path, targetStudentDir)
                                        if VERBOSE: print('\t\tCopied', studentFile.name, 'to', targetStudentDir)
                                else:
                                    if VERBOSE: print("Skipping copying directory/file", studentFile.name)
            else:
                if VERBOSE: print("Skipping processing directory/file", subEntry.name)
