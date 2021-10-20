# hf-commandermode
## holdfast modification based on Bepinex

**Feel free to change and republish it**  

Known bugs/problems:  
  Sometimes uncorrectly change commander player object's forward vector.  
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
