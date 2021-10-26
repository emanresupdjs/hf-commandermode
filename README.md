### hf-commandermode
holdfast modification based on Bepinex

**Feel free to change and republish it**  

How to update:  
  Download latest release.  
  Replace the old *root folder*/BepInEx/plugins/DemoMod/DemoMod.dll with the new one.  
  Restart server.  
  
----------- installation guide(Windows/Linux)

1. Install Bepinex framework  
Windows:  
unzip Bepinex_x64_xxx.zip to game *root folder* (eg. "D:\Program Files\Steam\steamapps\common\Holdfast Nations At War\")  
Linux:  
* install mono
```
sudo apt update
sudo apt install dirmngr gnupg apt-transport-https ca-certificates
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
sudo sh -c 'echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" > /etc/apt/sources.list.d/mono-official-stable.list'
sudo apt update
sudo apt install mono-complete
mono --version
```
unzip Bepinex_unix_xxx.zip to game *root folder* (eg. "/home/steam/holdfastnaw-dedicated/")  

* install bepinex follow this guide  
https://docs.bepinex.dev/articles/user_guide/installation/index.html?tabs=tabid-nix  

2. Change directory to game *root folder*  
Windows:  
Run Holdfast Naw.exe   

3. Create Mod folder  
create directory "*root folder*/BepInEx/plugins/DemoMod/"  
4. Copy the mod file  
copy DemoMod.dll to the directory create last step  
5. Check whether mod is installed correctly  
open "holdfastnaw-dedicated/logs_output/outputlog_server.txt"  
if the following is present, the mod is installed  
"  
Start Loading DemoMOd  
DemoMod success loaded!  
"  

--------- Usage

**Add bots**  
In game chatbox  
send "dm addBots 0 10" to add 10 bots of template 0  
send "dm addBots 0 10 [haha] 0" to add 10 bots of template 0 spawn with nameprefix  "[haha]" and with uniform indexed with 0  
player can only add at most 20 bots  
  
In F1 Admin console  
Send "rc dm add 40 British ArmyLineInfantry [haha] 1" to add 40 British LineInfantry with nameprefix  "[haha]" and with uniform indexed with 1  
admin can spawn any number of bots  

**Admin controls**  
In F1 Admin console  
Send "rc dm enable" to enable the mod  
Send "rc dm disable" to disable the mod  
Send "rc dm set MAX_SLAVE ArmyInfantryOfficer 10" to set the upper limit of bots a player can spawn  
Admin can spawn bots even when the mod is disabled  
mod is conflict with original carbonPlayers interface!  

**Control bots**  
Press B to follow  
Fire to Trigger bots fire  
Press N to form a line on your left and right  
Press V(V menu controlls)  
"Shoulder Arms" to form one line  
After form one line "Stand Ground" to form a double line  
"Load" to reload firearm  
...  
 
**Configuration**  
In !map_rotation  
"rc dm MAX_SLAVE ArmyInfantryOfficer 10" set the max bot an Officer can spawn to 10(default 20).  
*other class's default is 0*  

 
Press Q(when spawned as officer)  
Fire  
...  
  
--------- For developers    
  
DemoMod.cs is the entrypoint where all the hooks is registered.  
Basicly I hooked all the method associated with carbonPlayers, you can checkout all the hooked methods with dnSpy to understand the game logic.  
EnhancedBots.cs is the main implementation.  
EnhancedRc.cs handle some custom rc commands and modify function of chat box.   
  
This is the first time i write c# code/unity game modification, the code might be hard to read, im sorry about that.  
