"""
mossreflex.py

Author: Emma Wang (ejw38)
Date:   07-30-2024

A program built in python reflex (https://reflex.dev/) that runs the moss script.

(as of 7-30-2024) 
Mostly works, but would probably easy to break. More error catching needs to be added.
Several print statements in there are just for debugging.
Watch out for clear() and handle_dir_upload().
"""

import reflex as rx
from rxconfig import config
import subprocess
import os, shutil

languages = ["a8086", "ada", "ascii", "c", "cc", "csharp","fortran", "haskell", "java", "javascript", 
             "lisp", "matlab", "mips", "ml", "modula2", "pascal", "perl", "plsql", 
             "prolog", "python", "scheme", "spice", "vb", "verilog", "vhdl"]


class State(rx.State):
    """The app state."""

    msg: str = ""

    # Variables for options
    final: str = ""
    base: bool = False
    regex: str = ""
    lang: str = ""
    d: bool = False
    m: int = 10
    c: str = ""

    basefiles: list[str]
    sourcefiles: list[str]

    async def handle_upload(self, files: list[rx.UploadFile]):
        """Handle the upload of file(s).

        Args:
            files: The uploaded files.
        """
        for file in files:
            upload_data = await file.read()
            outfile = rx.get_upload_dir() / file.filename

            # Save the file.
            with outfile.open("wb") as file_object:
                file_object.write(upload_data)

            # Update file list
            path = rx.get_upload_dir()
            filepath = os.path.join(path, file.filename)
            if self.base:
                self.basefiles.append(filepath)
                print(f"Appended file: {self.basefiles}")
            else:
                self.sourcefiles.append(filepath)
                print(f"Appended file: {self.sourcefiles}")

    # Previously the error was
        # No such file or directory: #'uploaded_files/sample_sourcefiles/ejwMainWindow.xaml.cs'
        # because it was trying to create the new file in a subdir that didn't exist under uploaded_files
    # handle_dir_upload() should solve the problem because the subdir should now exist
    async def handle_dir_upload(self, files: list[rx.UploadFile]):
        """Handle the uploading of source directory(s) instead of files.
        
        Args:
            files: The uploaded files.
        """
        # Filter filetypes
        if self.regex:
            # Parse
            string = self.regex
            types = string.split(",")

            # Delete irrelevant file(s) from files
            for file in files[:]:
            # Look for files ending in the ext type
                parse = file.filename.split(".")
                ext = "." + parse[-1]
                # Remove files with extentions not listed in types
                if not ext in types:
                    files.remove(file)

        # Upload directories
        for file in files:
            # Parse filename to get number of directories
            layers = file.filename.split("/")
            n = len(layers)
            path = rx.get_upload_dir()

            # Loop through dirs to create the nonexistent dirs
            for i in range(n-1):
                # Add next dir to path
                layer = layers[i]
                path = os.path.join(path, layer)
                # Create dir if it doesn't exist
                if not os.path.exists(path):
                    os.mkdir(path)
            
            # Save file as before
            upload_data = await file.read()
            outfile = rx.get_upload_dir() / file.filename

            with outfile.open("wb") as file_object:
                file_object.write(upload_data)

            # Update file list
            self.sourcefiles.append(file.filename)

    def clear(self):
        """Clear uploaded files. 
        Distinguishes between source or base files by state of variable self.base.
        """
        # Make copy of file list to delete
        if self.base:
            which = self.basefiles
        else:
            which = self.sourcefiles

        # Do nothing if specified file list is empty
        if not which:
            return

        print(f"Which: {which}")                        # Print statement for debugging purposes

        dir = rx.get_upload_dir()
        for subdir in os.listdir(dir):
            filepath = os.path.join(dir, subdir)
            print(f"Checking {filepath} now")           # Print statement for debugging purposes
            try:
                if os.path.isfile(filepath):
                    if filepath in which:
                        print(f"Deleting {filepath}")   # Print for debugging
                        os.remove(filepath)
                elif os.path.isdir(filepath) and (not self.base):
                    print(f"Deleting {filepath}")       # Print for debugging
                    shutil.rmtree(filepath)
            except Exception as e:
                print(f"Couldn't delete {filepath}: {e}")

        # Update file list        
        if self.base:
            self.basefiles.clear()
        else:
            self.sourcefiles.clear()

    # There are two of these flip methods to reset the base variable
    #   and not risk a previous value carrying over.
    def flip_true(self):
        """Set variable base to True. Used to distinguish between source and base files."""
        self.base = True
    
    def flip_false(self):
        """Set variable base to False. Used to distinguish between source and base files."""
        self.base = False

    def create_string(self):
        """Build the string to run in the terminal, then call moss() to do the running.

        Structure:
            perl moss [-l language] [-d] [-b basefile1] ... [-b basefilen] [-m #] [-c "string"] file1 file2 file3 ...
        """
        string = "perl moss "

        # Append optional options
        # Language
        if self.lang:
            string += "-l {} ".format(self.lang)
        # Directory mode
        if self.d:
            string += "-d "
        # Base files
        if self.basefiles:
            for file in self.basefiles:
                string += "-b {} ".format(file)
        # m
        if self.m != 10:
            string += "-m {} ".format(self.m)
        # c
        if self.c:
            string += '-c "{}" '.format(self.c)

        # Append source files
        if not self.sourcefiles:
            self.msg = "Hey, add source files" # TODO: this message doesn't disappear even after source files get added
        else:
            for file in self.sourcefiles:
                string += "{} ".format(file)

            self.final = string  # Save final string in state

            # Call next function to run the command
            self.moss()

    def moss(self):
        """Run command in the terminal and grab the resulting link."""
        # Run
        cmd = self.final.split()
        # Get output
        link = subprocess.run(cmd, stdout=subprocess.PIPE).stdout.decode("utf-8")
        parsed = link.split()
        # Get link
        self.final = parsed[-1]

    # Used for debugging, but otherwise not really used. Might remove later
    def insert_msg(self, txt):
        """Display a message under the link.

        Args:
            txt: The message to be displayed.
        """
        self.msg = txt
    
    # TODO delete later
    def checkfiles(self):
        """Lists the sourcefiles and basefiles in the console. Used for debugging purposes.
        
        Triggered by the '?' button.
        """
        print()
        print(f"Sourcefiles: {self.sourcefiles}")
        print(f"Basefiles: {self.basefiles}")
        print()


# The layout of everything
def index() -> rx.Component:
    return rx.box(
        rx.heading("Mossreflex", size="9", color_scheme="grass"),
        rx.color_mode.button(position="top-right"),
        rx.hstack(
            rx.box(
                "",
                background_color="teal",
                border_radius="4px",
                margin="4px",
                padding="4px",
                width="95%",
            ),
            rx.button("?", color_scheme="grass", on_click=State.checkfiles),  # TODO: Temporarily used for debugging. Change/delete later
        ),
        rx.hstack(
            # Column 1, upload files

            # Source files
            rx.vstack(
                rx.upload(
                    # This stack isn't really necessary if the button is commented out;
                    #   button is commented out because it doesn't affect anything when the 'style' attribute is removed
                    rx.vstack(
                        # rx.button(
                        #     "Select",
                        #     color="black",
                        #     bg="white",
                        #     border="1px solid black",
                        # ),
                        rx.text("Drag/drop or click to select directories"),
                    ),
                    id="upload-dirs",
                    border="1px dotted black",
                    padding="2em",
                    on_mount=rx.call_script(    # doesn't work on Edge or Chrome without the removeAttribute
                        """document.querySelector("div#upload-dirs > input").setAttribute("webkitdirectory", "true"); document.querySelector("div#upload-dirs > input").removeAttribute("style")"""
                    ),
                ),
                rx.hstack(
                    rx.input(
                        placeholder="i.e. .py, .cs",
                        max_length=50,
                        value=State.regex,
                        on_change=State.set_regex,
                    ),
                    rx.button(
                    "Upload source directories",
                    color_scheme="grass",
                    on_click=lambda: [
                        State.flip_false(),
                        State.handle_dir_upload(rx.upload_files(upload_id="upload-dirs")),
                    ],
                ),
                ),
                rx.upload(
                    rx.vstack(
                        rx.button(
                            "Select",
                            color="black",
                            bg="white",
                            border="1px solid black",
                        ),
                        rx.text("Drag/drop or click to select files"),
                    ),
                    id="upload-files",
                    border="1px dotted black",
                    padding="2em",
                ),
                rx.button(
                    "Upload source files",
                    color_scheme="grass",
                    on_click=lambda: [
                        State.flip_false(),
                        State.handle_upload(rx.upload_files(upload_id="upload-files")),
                    ],
                ),

                # Base files
                rx.upload(
                    rx.vstack(
                        rx.button(
                            "Select",
                            color="black",
                            bg="white",
                            border="1px solid black",
                        ),
                        rx.text("Drag/drop or click to select files"),
                    ),
                    id="upload1",
                    border="1px dotted black",
                    padding="2em",
                ),
                rx.button(
                    "Upload base files",
                    color_scheme="grass",
                    on_click=lambda: [
                        State.flip_true(),
                        State.handle_upload(rx.upload_files(upload_id="upload1")),
                    ],
                ),
                width="30%",
                height="100%"
            ),

            # Column 2, list uploaded files
            rx.flex(
                rx.hstack(
                    rx.button(
                        "Clear", 
                        size="1",
                        color_scheme="grass",
                        on_click=lambda: [
                            State.flip_false(),
                            State.clear() #, rx.clear_selected_files("upload-dirs"))
                        ], # TODO: previously on_click just cleared selected and not uploaded files
                    ), 
                    rx.text("Source files:", font_weight="bold", size="5"),
                ),
                rx.foreach(State.sourcefiles, rx.text),
                rx.hstack(
                    rx.button(
                        "Clear",
                        size="1",
                        color_scheme="grass",
                        on_click=lambda: [
                            State.flip_true(),
                            State.clear() #, rx.clear_selected_files("upload1")
                        ], # TODO: same as other clear button
                    ),
                    rx.text("Base files:", font_weight="bold", size="5"),
                ),
                rx.foreach(State.basefiles, rx.text),
                width="25%",
                direction="column",
                spacing="1",
            ),

            # Column 3, other options
            rx.vstack(
                # Language
                rx.select(
                    languages,
                    placeholder="Select Language",
                    label="Languages",
                    value=State.lang,
                    on_change=State.set_lang,
                ),
                # Directory mode
                rx.checkbox(
                    "Directory mode: files in a directory are taken to be part of the same program",
                    size="2",
                    color_scheme="grass",
                    default_checked=State.d,
                    on_change=State.set_d,
                ),
                # m
                rx.text(
                    "Maximum number of times a given passage may appear before it is ignored ",
                    size="2",
                ),
                rx.input(
                    placeholder="Enter integer",
                    max_length=10,
                    value=State.m,
                    on_change=State.set_m,
                ),
                # c
                rx.text(
                    "Comment string that is attached to the generated report ", size="2"
                ),
                rx.input(
                    placeholder="Enter string",
                    max_length=50,
                    value=State.c,
                    on_change=State.set_c,
                ),
                # Create string, run command, & get link
                rx.button("Get link", color_scheme="cyan", on_click=State.create_string),
                # Display link
                rx.link("{}".format(State.final), href="{}".format(State.final)),
                # Display other messages
                rx.text(State.msg),     # Currently it's just the msg to upload source files
                margin="50px",
            ),
        ),
        text_align="left",
        margin="20px",
        padding="20px",
    )


app = rx.App()
app.add_page(index)
