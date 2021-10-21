# hf-commandermode
## holdfast modification based on Bepinex

**Feel free to change and republish it**  

Known bugs/problems:  
  Sometimes Accidentally change commander player object's forward vector (or rotation idk) which is fatal.  
  Bots are unable to melee attack.  
  Form line algorithm is shit.  
  Commander player have to fire in order to get commander player's aiming forward vector.  
  
----------- installation guide(Windows/Linux)

1. Install Bepinex framework  
unzip Bepindexxxx.zip to game *root folder* (eg. "C:\Program Files\Steam\steamapps\common\Holdfast Nations At War\")  
2. Change directory to game *root folder*  
execute Holdfast Naw.exe or run the game(on linux)  
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

** Add bots **  
In game chatbox  
send "dm addBots 0 10" to add 10 bots of template 0  
send "dm addBots 0 10 [haha] 0" to add 10 bots of template 0 spawn with nameprefix  "[haha]" and with uniform indexed with 0  
player can only add at most 20 bots  
  
In F1 Admin console  
Send "rc dm add 40 British ArmyLineInfantry [haha] 1" to add 40 British LineInfantry with nameprefix  "[haha]" and with uniform indexed with 1  
admin can spawn any number of bots  

** Enable/Disable the mod **  
In F1 Admin console  
Send "rc dm enable" to enable the mod  
Send "rc dm disable" to disable the mod  
Admin can spawn bots even when the mod is disabled  
mod is conflict with original carbonPlayers interface!  

** Control bots **  
Press B to follow  
Fire to Trigger bots fire  
Press N to form a line on your left and right  
Press V(V menu controlls)  
"Shoulder Arms" to form one line  
After form one line "Stand Ground" to form a double line  
"Load" to reload firearm  
...  
  
Press Q(when spawned as officer)  
Fire  
...  
  
--------- For developers    
  
DemoMod.cs is the entrypoint where all the hooks is registered.  
Basicly I hooked all the method associated with carbonPlayers, you can checkout all the hooked methods with dnSpy to understand the game logic.  
EnhancedBots.cs is the main implementation.  
EnhancedRc.cs handle some custom rc commands and modify function of chat box.   
