from tkinter import *
from tkinter import ttk

# Functions



# Setup main frame
root = Tk()
root.title("Lunar Alchemy")
mainframe = ttk.Frame(root, padding="3 3 12 12")
mainframe.grid(column=0, row=0, sticky=(N, W, E, S))
root.columnconfigure(0, weight=1)
root.rowconfigure(0, weight=1)


# Create Widgets
#Footer [5 rows, X collumns]
ROMpath = StringVar()

ROMpathentry = ttk.Entry(mainframe, width=30, textvariable=ROMpath).grid(column=1, row=1)
ttk.Label(mainframe, textvariable=ROMpath).grid(column=2, row=1, sticky=(W, E))


# QoL stuff
for child in mainframe.winfo_children():
    child.grid_configure(padx=5, pady=5)
#root.bind('<Return>', FUNCTION)



# Totally useless code that is totally NOT used to make sure you can run code from the debugger or plugins before the GUI runs >.>
def main():
	root.mainloop()

if __name__ == '__main__':
	main()