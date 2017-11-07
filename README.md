# WinFolderList

1. Allow the user to select a folder from the local C: drive
2. Walk the tree comprising that folder and the tree beneath it
3. For every file found during this walk:
a. Load the filename, size and last-touched date into memory
b. Push this object into a MSMQ queue
4. In a separate thread, de-queue items from this queue and use them to build a grid in the GUI
5. The Grid should comprise a scrollable list of [filename] [path] [size] [date]
6. The user should be able to see the grid while it builds. i.e. the GUI should not ‘freeze’
7. Format each item to make it easy to understand
8. Show a progress bar that updates in real time, showing how far through the files we are
9. Show separately a small log control (on screen) which indicates when processing has completed and how many files were processed.


Above features should all be implemented. Threading is implemented as Tasks. Progress bar is visible once there is an indication as to how many more files there are to process. (not imediately, and could take some time for deep trees)

Scroll viewer of grid and log viewer will lock to bottom in the same style as the Visual Studio output window. 

Items in the grid and log viewer are laid up in a virtualising stack panel, so reduce the memory foot print for large scans. (only the model, and not the UI element of each file willl be stored in memory)

There is no 'folder select' dialog. This doesn't actually exist in the wpf framework so needs to be imported from winforms. Also, from an mvvm 'purist' point of view, the implementaion of this is currently to time consuming for this project.


What i'm not happy about, and would like to re-address
======================================================

Having worked with UI applications extensively over the past years, I have a tendency to prefer mvvm priciples over SOLID. Eg. Even though the ‘ViewModel’ pattern implements the dependency inversion well, it tends to forfeit single responsibility.

The last message I put onto the message queue is a dummy file to signal the end. I’m unhappy about this and need to come up with a way to re-factor.

I started this project as ‘Test Driven’ but it quickly resorted to creating unit tests after code. There was a lot of change in strategy at the beginning, and I’ve only had limited pockets of time to work on this.

My commit pattern for this project has been relatively 'chaotic' so far. I'm fairly new to GIT and have missed some of the tfs features. I have also been working from a few different machines, as and when i've had the chance.
