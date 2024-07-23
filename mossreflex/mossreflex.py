"""
mossreflex.py

Author: Emma Wang (ejw38)
Date:   7-17-2024

might come in handy later?: https://fastapi.tiangolo.com/tutorial/request-files/

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

    # variables
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
                self.sourcefiles.append(outfile)
            else:
                self.basefiles.append(outfile) #file.filename)
            

    def flip(self):
        self.base = True

    def thing(self):
        # build final full string
        # perl moss [-l language] [-d] [-b basefile1] ... [-b basefilen] [-m #] [-c "string"] file1 file2 file3 ...
        finalboss = "perl moss "

        # append optional options
        # language
        if self.lang:
            finalboss += "-l {} ".format(self.lang)
        # directory mode
        if self.d:
            finalboss += "-d "
        # base files
        if self.basefiles:
            for file in self.basefiles:
                finalboss += "-b {} ".format(file)
        # m
        if self.m != 10:
            finalboss += "-m {} ".format(self.m)
        # c
        if self.c:
            finalboss += "-c \"{}\" ".format(self.c)
        
        # append source files
        if not self.sourcefiles:
            self.final = "hey, add source files"
        else:
            for file in self.sourcefiles:
                finalboss += "{} ".format(file)
        
            self.final = finalboss
            self.moss()

    def moss(self):
        cmd = self.final.split()
        link = subprocess.run(cmd, stdout=subprocess.PIPE).stdout.decode('utf-8')
        #self.final = link.stdout.decode('utf-8')
        parsed = link.split()
        self.final = parsed[-1]

def index() -> rx.Component:
    # Welcome Page (Index)
    return rx.box(
        rx.heading("Mossreflex", size="9", color_scheme="grass"),
        rx.color_mode.button(position="top-right"),
        rx.box("temp placeholder", background_color="teal", border_radius="4px", width="100%", margin="4px", padding="4px"),
        rx.hstack(
            # Column 2, upload source files
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
                ),
                #rx.foreach(rx.selected_files("upload2"), rx.text),
                rx.foreach(State.sourcefiles, rx.text),
                #rx.foreach(State.img, lambda img: rx.image(src=rx.get_upload_url(img))),
                width="25%",
            ),

            # Column 1, upload base files
            rx.vstack(
                rx.button(
                    "Upload base files", color_scheme="grass",
                    on_click=lambda:[State.flip(), State.handle_upload(rx.upload_files(upload_id="upload1"))],
                    #on_click=State.handle_upload(rx.upload_files(upload_id="upload1")),
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
                ),
                rx.foreach(State.basefiles, rx.text),
                #rx.foreach(rx.selected_files("upload1"), rx.text),
                #rx.foreach(State.img, lambda img: rx.vstack(rx.image(src=rx.get_upload_url(img)),rx.text(img),),
            ),

            # Column 3, other options
            rx.vstack(
                rx.select(
                    languages,
                    placeholder="Select Language",
                    label="Languages",
                    value=State.lang,
                    on_change=State.set_lang,
                ),
                rx.checkbox(
                    "Directory mode", 
                    size="2", 
                    color_scheme="grass", 
                    default_checked=State.d,
                    on_change=State.set_d
                    ),
                rx.hstack(
                    rx.text("-m: "),
                    rx.input(
                        placeholder="Enter integer", max_length=10, 
                        value=State.m,
                        on_change=State.set_m
                        ),
                ),
                rx.hstack(
                    rx.text("-c: "),
                    rx.input(
                        placeholder="Enter string", max_length=50, 
                        value=State.c, 
                        on_change=State.set_c
                        ),
                ),
                rx.button("Do the thing", color_scheme="cyan", on_click=State.thing),
                #rx.text(State.final),
                rx.link("{}".format(State.final), href="{}".format(State.final)),
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
