from tkinter import *
from tkinter import ttk

# Functions



# Setup main frame
root = Tk()
root.title("Feet to Meters")
mainframe = ttk.Frame(root, padding="3 3 12 12")
mainframe.grid(column=0, row=0, sticky=(N, W, E, S))
root.columnconfigure(0, weight=1)
root.rowconfigure(0, weight=1)


# Create Widgets



# QoL stuff
for child in mainframe.winfo_children():
    child.grid_configure(padx=5, pady=5)
#root.bind('<Return>', FUNCTION)


# Totally useless code that is totally NOT used to make sure you can run code from the debugger or plugins before the GUI runs >.>
def main():
	root.mainloop()

if __name__ == '__main__':
	main()