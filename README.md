AutoPD
======

Automation for Perfect Dark
Scrappy project made for some automation of the P2P program Perfect Dark. 
**I am not responsible for anything you download or *accidental* download with this**  

All automation is done in the top split section.  
Updates to the PD can/will break this.  
Setting the visual style in PD to "new style" is a requirement for functioning correctly.  
PD cannot be minimized for this to work correctly, as it automates based on PostMessage to the window handle.  
Could be improved to prevent focus pulling.  

This is a old project created in a weekend.  
may(not) get updated.  
  
Memory stuff
-----
Offsets I use in memory in the table to get the item information  
0x0 - Table Item Start  
0x8 - 32 Bytes - hash little-endian format  
0x28 - Byte - Selection,  Not Selected = 0; Selected = 1; Mouse Down = 2;  
0x34 - Byte - Status, (in search:0 = new; 1 = downloading;? > 1 =  done;); 1 = downloading; 2 = completed(?)  
0x35 - Byte - Face, 0 = Sleeping; 1 = Converting; 2 = Working; - ONLY ON DOWNLOAD  
0x38 - 4 Bytes - Filename /w tags length  
0x3C - 4 Bytes - Filename /wo tags length  
0x50 - Variable String UNICODE - Start of Filename  
0x258 - 4 Bytes - Size in bytes  
0x268 - 4 Bytes - Count  
0x2A8 - Float - Progress percentage  
