from tkinter import * 
import sys


if __name__ == '__main__':
	root = Tk(className=sys.argv[1])
	root.geometry("300x200")
	root.configure(bg='#f5f5f5')  

	w = Label(root, text=sys.argv[2], font = "50", bg='#f5f5f5')
	b = Button(root, text ="Ok", command = exit)

	w.pack(padx=35, pady=15)
	b.place(x=130, y=120)

	root.mainloop() 
