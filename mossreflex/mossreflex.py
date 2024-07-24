"""
mossreflex.py

Author: Emma Wang (ejw38)
Date:   7-17-2024


https://reflex.dev/
might come in handy later??: https://fastapi.tiangolo.com/tutorial/request-files/

perl moss [-l language] [-d] [-b basefile1] ... [-b basefilen] [-m #] [-c "string"] file1 file2 file3 ...
"""

import reflex as rx
from rxconfig import config
import subprocess

languages = ["a8086", "ada", "ascii", "c", "cc", "csharp", "fortran", "haskell", "java", "javascript", 
                "lisp", "matlab", "mips", "ml", "modula2", "pascal", "perl", "plsql", "prolog", "python", 
                "scheme", "spice", "vb", "verilog", "vhdl"]

class State(rx.State):
    """The app state."""

    msg: str = ""

    # Variables for options
    final: str = ""
    base: bool = False
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
            if not self.base:
                self.sourcefiles.append(outfile)    # Change outfile to file.filename to not only show filename
            else:
                self.basefiles.append(outfile)      
            

    def flip(self):
        """Set variable base to True. Used to distinguish between source and base files.
        """
        self.base = True

    def create_string(self):
        """Build the string to run in the terminal.

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
            string += "-c \"{}\" ".format(self.c)
        
        # Append source files
        if not self.sourcefiles:
            self.msg = "Hey, add source files"
        else:
            for file in self.sourcefiles:
                string += "{} ".format(file)

            self.final = string     # save final string in state

            # Call next function to run the command
            self.moss()

    def moss(self):
        """Run command in the terminal and grab the resulting link."""
        # Run
        cmd = self.final.split()
        # Get output
        link = subprocess.run(cmd, stdout=subprocess.PIPE).stdout.decode('utf-8')
        parsed = link.split()
        # Get link
        self.final = parsed[-1]
    
    # Not really used. Might remove later
    def insert_msg(self, txt):
        """Display a message under the link.
        
        Args:
            txt: The message to be displayed.
        """
        self.msg = txt

def index() -> rx.Component:
    return rx.box(
        rx.heading("Mossreflex", size="9", color_scheme="grass"),
        rx.color_mode.button(position="top-right"),
        rx.box("temp placeholder", background_color="teal", border_radius="4px", width="100%", margin="4px", padding="4px"),
        rx.hstack(
            # Column 1, upload source files
            rx.vstack(
                rx.button(
                    "Upload source files", color_scheme="grass",
                    on_click=State.handle_upload(rx.upload_files(upload_id="upload2")),
                ),
                rx.button(
                    "Clear",
                    on_click=rx.clear_selected_files("upload2"),
                ),
                rx.upload(
                    rx.vstack(
                        rx.button("Select", color="black", bg="white", border="1px solid black"),
                        rx.text("Drag n drop files / click to select files"),
                    ),
                    id="upload2",
                    border="1px dotted black",
                    padding="5em",
                    on_mount=rx.call_script("""document.querySelector("div#upload2 > input").toggleAttribute("webkitdirectory")"""),
                ),
                rx.foreach(State.sourcefiles, rx.text),
                width="25%",
            ),

            # Column 2, upload base files
                # program code that also appears in the base file is not counted in matches
            rx.vstack(
                rx.button(
                    "Upload base files", color_scheme="grass",
                    on_click=lambda:[State.flip(), State.handle_upload(rx.upload_files(upload_id="upload1"))],
                ),
                rx.button(
                    "Clear",
                    on_click=rx.clear_selected_files("upload1"),
                ),
                rx.upload(
                    rx.vstack(
                        rx.button("Select", color="black", bg="white", border="1px solid black"),
                        rx.text("Drag n drop files / click to select files"),
                    ),
                    id="upload1",
                    border="1px dotted black",
                    padding="5em",
                    # TODO: add another on_mount?
                ),
                rx.foreach(State.basefiles, rx.text),
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
                # Directory mode: files in a directory are taken to be part of the same program
                rx.checkbox(
                    "Directory mode", 
                    size="2", 
                    color_scheme="grass", 
                    default_checked=State.d,
                    on_change=State.set_d
                    ),
                # m: maximum number of times a given passage may appear before it is ignored
                rx.hstack(
                    rx.text("-m: "),
                    rx.input(
                        placeholder="Enter integer", max_length=10, 
                        value=State.m,
                        on_change=State.set_m
                        ),
                ),
                # c: supplies a comment string that is attached to the generated report
                rx.hstack(
                    rx.text("-c: "),
                    rx.input(
                        placeholder="Enter string", max_length=50, 
                        value=State.c, 
                        on_change=State.set_c
                        ),
                ),

                # Create string, run command, & get link
                rx.button("Get link", color_scheme="cyan", on_click=State.create_string),
                # Display link
                rx.link("{}".format(State.final), href="{}".format(State.final)),
                # Display any other messages
                rx.text(State.msg),
                margin="50px",
            ),
        ),
        rx.logo(),
        text_align="left",
        margin="20px",
        padding="20px",
    )


app = rx.App()
app.add_page(index)
