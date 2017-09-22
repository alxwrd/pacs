Instructions
===============

Open the trading window or bid window in Puzzle Pirates (either from an island or ship).
Capture the data.. then upload it.

Using the Tools Debug option to log debug information (such as text files and
screen shots) to the user AppData\PPAOCR directory.  This can be useful for troubleshooting
defects.

Change Log
===============

Ver 5.0.30
-------------------
Fix regression introduced in version 5.0.28 where the client would crash when starting
   after the Tools menu was accessed.
   
Ver 5.0.29
-------------------
Added application and AppDomain exception handlers

Ver 5.0.28
-------------------
Fix to OCR from ship after recent YPP change to screen layout broke the scanning
   this fix hopefully makes it more robust as well.
Add display of server used in upload tab.

Ver 5.0.27
-------------------
Fix to only report one error per commodity found that is unsupported by the server (avoid redundancy)
Fix to properly detect if anti_aliasfonts is set to true in Puzzle Pirates.

Ver 5.0.26
-------------------
Fix to ignore blank rows instead of reporting an invalid commodity (since the last row in a commodity
   screen can sometimes be blank).

Ver 5.0.25
-------------------
Fix to better detect if antialias fonts is on in Puzzle Pirates (if the user has never changed
   that option in Puzzle Pirates then there is no registry key for it, so changed to assume on)
Fix to detect when bitmap pixel format is not 24bpp or 32bpp.
Fix to auto convert pixel format to 32bpp if it is not 24bpp or 32bpp form the window bitmap capture.
Added DebugOutput.log for added logging when in debug mode.

Ver 5.0.24
-------------------
Change launch condition version in Setup installer to use .NET Framework 2.0 instead of 3.5

Ver 5.0.23
-------------------
Add cleanup of Debug files whenever a capture is done
Change to use .NET Framework 2.0 instead of 3.5 so that win2k will work
Fix final boundary seperator to include "--" at the end of it so that it is
   compliant and we don't get forbidden access errors when uploading to a
   server with mod_security enabled on it.

Ver 5.0.22
-------------------
Changed to point to production server for release over version 5 client.

Ver 5.0.21
-------------------
Fix blank cell validation to only show one error message instead of one for each blank cell
Add detection of new font aliasing option in Puzzle Pirates (can't scan island name from ship when it is on)

Ver 5.0.20
-------------------
Add more validation of commodity and bid data and logging of tif files if invalid
Changed to show blank cells instead of -1 in commodity data tab


Ver 5.0.19
-------------------
Fix bid data captures to support scroll bar for when there is more than 4 commodities spawned by an island
Change so that font smoothing does not need to be turned off when using Java 5

Ver 5.0.18
-------------------
Fix SendInput to support Vista x64

Ver 5.0.17
-------------------
Add error checking to ensure there is an island selector ("island=") value when submitting island selector page

Ver 5.0.16
-------------------
Add support for dual display
Fix bid data capture to work with localized settings where thousands separator is "." instead of ","
Changed to remember previously selected PP window when refreshing PP window list

Ver 5.0.15
-------------------
Fix bug where island name is not always recognized when scanning from island
Removed web address label and text box from Upload tab (right click menu properties can be used instead)
Fix to allow uploading of commodities even when missing commodity error message is shown
Fix to ignore "Navy Dye" commodity since it is not a real commodity
Changed error message for screen boundary check to indicate not within the "primary" screen

Ver 5.0.14
-------------------
Added support for uploading of bid data.
Changed "Home" button to be more visible and add "Home" text.
Fixed to upload even when missing commodities in the commodity map (just ignores
   those commodities) but still displays an error message.
   
Ver 5.0.13
-------------------
Added scanning of bid data (no uploading yet of bid data)
Added ability to select Puzzle Pirates window to use when there are multiple windows
Added "Home" button in upload tab in case user needs to go back to home page of server
Added display of web address for server in the upload tab
Added version number to product name in installer

Ver 5.0.12
-------------------
Added Options dialog to persist options settings (along with help text and help button)
Added option for user to turn on/off auto disabling/restoring of the
   font smoothing state when the client starts/exits
Added font smoothing warning dialog the first time the application is used
Change debug mode to be in new Options dialog instead of menu
Fix "Hamattan" and "Typhoon" island names to be OCR'd correctly (Font2 Kernning issue)


Ver 5.0.11
-------------------
Fix About dialog to be fixed size and not have minimize/maximize buttons
Fix AssemblyFileVersion to match AssemblyVersion (build part of version auto-increments)
   Note:  AssemblyVersion should technically be fixed and not increment with each
          build unless it breaks backwards compatibility.  But, this assembly has
          no public API (i.e. clients) and this was easiest to implement.
Change product name from PPAOCR to "Puzzle Pirates Automated OCR", kept install and appdata dir as PPAOCR
Change "Count" to "Rows" in GUI.
Change to remember font smoothing state when starting PPAOCR and restore system to that state when exiting PPAOCR
Change installer to remove previous versions before installing newer version

          
Ver 5.0.10
-------------------
Fix Capture and Quit links in Upload tab to work.
Fix font2 Kerning so "Swampfen" is recognized.
Fix font2 Kerning so "Spectre" is recognized.
Speed up scanning/ocr by factor of around 3x (now typically around 550 rows/sec on test PC)
Lengthen timeout for ocr of island name (waiting for /w to take effect) from 1 sec to 2 sec

Ver 5.0.9
-------------------
Add option for desktop shortcut to installer
Added "Y" to Font2 bitmap (alphabet should be complete now for Font2 bitmap)
Added logging of commodity data to Commodities.txt when in debug mode
Fix to check for "..." even for non-stall names
Fix to try to match stall type for stall type abbreviation when truncated names are encountered (i.e. "...")

Ver 5.0.8
-------------------
Fix bug where client would not upload when in debug mode
Fix to avoid mistaking puzzle pirates forum open in a browser for puzzle pirates window when "javaw" process not found
Fix to move chat window scroll bar back down after capturing market data (side effect of moving 
   commodity area scroll bar to the top with CTRL+HOME also moves chat window scroll bar to top)
   
Ver 5.0.7
-------------------
Moved url for host website to App.Config file so that it can be changed by users.
Add "UseLocalCommodMapFile" variable to App.Config to allow user to use local file instead of 
   getting it from the server
Change process finding routine to be more robust by searching for process with main window title
   containing "Puzzle Pirates" if it fails to find "javaw" process.
Add display of ocean name in addition to island name.

Ver 5.0.6
--------------------
Moved commodity/index mapping for data uploads to XML file.
Change client to get commodity mapping from commodmap.php web page (XML from web page).
Added instruction comments to readme file.
Changed client version reported when uploading marketdata to be 005 instead of 003.
Credits added to about dialog.

Ver 5.0.5
--------------------
Add "U","Q", and "z" to Font2 (only "Y" is still missing for alphabet).
Add logging of shop names ("ShopNames.txt") when in debug mode.
Changed to save debug and upload data to user application directory instead of install dir so it works on Vista.
Add "-" character to Font1 for Shop/Stall names.

Ver 5.0.4
--------------------
Add more missing characters to Font2 for island names.
Added "&" character to Font1 (for shop names).

Ver 5.0.3
--------------------
Fix commodity names to avoid lower case "L" being recognized as "I" (upper case "i").

Ver 5.0.2
--------------------
Fix to find commodity area properly when there is no commodity area vertical scroll bar
Fix to support island names with "'" by adding it to Font2
Fix "H" bitmap for island names
Fix (hack) for delayed garbage collection memory usage by forcing garbage collection and waiting for finalizers.
Improve island name ocr by adjusting Kerning values for Font2 (for island names)
Added '.' character to Font1 for ocr of stall/shop names
Added check to avoid uploading commodities where stall/shop name is incomplete (has '...') due to narrow cell width on ships
Added regression testing based upon screen shots
Added support for ">" in quantities.  >1000 is represented as 1001.
Refactor OCR code into two classes (one with PP specific and the other for just OCR)

Ver 5.0.1 (20081222)
--------------------
Added use of Kerning & Tracking to detect spaces better with fonts
Changed fonts to get characters from new FontInfo xml file
Fixed bug in "X" character bitmap for Font1
Fixed bug in "M" character bitmap for Font2
Changed to always log island TIF files when Debug mode is on
Renamed setup and app to be "Puzzle Pirates Automated OCR" with PPAOCR abbreviation

Ver 5.0.0 (20081221)
--------------------
Added support for variable puzzle pirate window size
Fixed memory usage issue by forcing Garbage collection after screen captures
Added Tools | Debug menu
Added Help | About menu
Changed to use colors from known locations instead of fixed colors since they can vary slightly between computers
