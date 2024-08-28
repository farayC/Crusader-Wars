![MAIN BANNER](https://i.imgur.com/6Z9IfPm.png)
# What is Crusader Wars?
Crusader Wars is a complete overhaul of Crusader Kings 3 warfare system making it possible to fight your battles in a 1:1 scale in Total War: Attila.
# Current features:
## v0.9.11.3 "Quality Of Life"
### Accurate Armies Composition
The armies in the Total War battle will be 1:1 scale of the Crusader Kings 3 battle, their unit's culture is based on the commander's culture or heritage.
### Levies
Levies are not just simply converted as one type of unit made of pitchfork peasants, each culture has its type of levy composition trying to be the most historically accurate possible.
### Automatic Battle Results Transfer
When the Total War battle ends, CK3 opens up automatically with the battle results with a 98% accuracy, counting each regiment's soldiers dead and killed.
### Commander & Knights
The army commander will be the Total War general unit with his strength and XP level being based on his noble rank, bodyguards, and prowess.
His martial skills represent a major role on the battlefield, increasing the whole army's XP level and the amount and quality of defensive deployable when defending.
Each knight is also present in a unit and each one of their prowess and martial increases the strength of it individually.

If your characters fall in battle can get injured, imprisoned, or even killed!
Injured characters also get less strength in soldiers when fighting according to the severity of the injury.
### Terrain & Weather
The Total War battle map will be based on the CK3 battle terrain and defending armies in mountains, hills, etc... will always get the high ground.
The month the battle takes place is also accounted to make the battle atmosphere resemble the season they are in!
Winter severity also increases the amount of winter effects there is on the Total War battle.
### Unique Battle Maps
If you're fighting in Gizeh, Stonehenge, or Hadrian's Wall in CK3, you will get a battle map there!
### Battle Modifiers
Ruler leading the soldiers will increase the army XP level, and Realm in Debt, Army Gathering, Low Supplies, and No Supplies reduce the army XP.
### Unit Cards Names
The Total War unit cards' names will be their CK3 regiment name equivalent.
### Period Accurate
We have mapped each CK3 culture, heritage, and Men-At-Arms with a collection of Attila mods to be the most historically accurate possible. We have two unit mappers for Anno Domini and three for Medieval Kingdoms 1212 covering the range from 8th-Century to 15th-Century.
### CK3 "LOTR: Realms of Exile" & "The Fallen Eagle" Compatible!
We also have 1 unit mapper for the Attila Dawnless Days and some sub-mods to be used with CK3 "LOTR: Realms of Exile".
Then we got a unit mapper for Attila's "Fall of the Eagle", one for "Fireforged Empires" and one for "Age of Justinian" to be used with CK3 "The Fallen Eagle".
# Roadmap
Here you can see all our concepts, ideas, and plans for the mod.
[<img src="https://i.imgur.com/BKru2vr.png">](https://trello.com/b/abR09huC/crusader-wars)
# How does it work?
If you want to code a mod like this, these are the major steps to achieve it:
## Crusader Kings 3 Data Output:
There are two ways Crusader Wars outputs data for it to interpret. One is by Console Commands, the other is by a Save File.
I created a mod for CK3 that consists of a button on the ```window_combat.gui```, that when pressed, will create a save file and then execute some console commands that output some important data, which the Crusader Wars app, will read from the ```console_history.txt``` located in ```Documents\Paradox Interactive\Crusader Kings III\``` folder.

## Crusader Kings 3 Data Reading:
80% of data Crusader Wars reads from is from the save file directly, the rest on the console_history.txt file.
On the Save File, it reads data from these sections (in order):
- combats
- traits_lookup
- armies
- units
- army_regiments
- regiments
- landed_titles
- counties
- mercenary_company_manager
- living
- accolades
- court_positions
- culture_manager

On the console_history.txt it gets this data, which is: 
- Year and Month
- Battle Name
- Left & Right Realm Names
- User Character ID and Culture ID
- Left & Right Main Participants Character ID and Culture ID
- Left & Right Commanders Character ID, Culture ID, Martial Value, Prowess Value, and Feudal Rank
- Left & Right Knights Character ID, Names and Prowess Value
- Knights Effectiveness
- Terrain Type
- Province ID
- Winter Severity
- Special Building in the Holding
- Left & Right Advantages Modifiers
- KEYWORD, for the app to detect a battle has started
```
Log.ClearAll
DateYear:936
DateMonth:12
BattleName:Battle of Zürich
T Your E TOOLTIP:GAME_CONCEPTrealm Realm: ONCLICK:TITLE982 TOOLTIP:LANDED_TITLE982 L Kingdom of Germany
T Enemy E TOOLTIP:GAME_CONCEPTrealm Realm: ONCLICK:TITLE7186 TOOLTIP:LANDED_TITLE7186 L Kingdom of Burgundy
PlayerCharacter_ID:12185
PlayerCharacter_Culture:36
LeftSide_Owner_ID:12185
LeftSide_Owner_Culture:36
RightSide_Owner_ID:10874
RightSide_Owner_Culture:37
---------Player Army---------
LeftSide_Commander_Culture:41
T Commander E TOOLTIP:GAME_CONCEPTadvantage Advantage: 23
Martial skill: positive_value 23
PlayerProwess:3
PlayerRank:3
ONCLICK:CHARACTER11796 TOOLTIP:CHARACTER11796 L high Margrave high Gero: 18 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER12148 TOOLTIP:CHARACTER12148 L high Lord-Mayor high Adam: 18 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER9936 TOOLTIP:CHARACTER9936 L high Duke high Dirk: 12 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER11292 TOOLTIP:CHARACTER11292 L high Count high Amadeus: 12 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER12452 TOOLTIP:CHARACTER12452 L high Prince high Heinrich: 9 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER19917 TOOLTIP:CHARACTER19917 L high Mayor high Eggerd: 9 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER12175 TOOLTIP:CHARACTER12175 L high Duke high Hermann: 8 E TOOLTIP:GAME_CONCEPTprowess Prowess
x
---------Enemy Army---------
RightSide_Commander_Culture:209
T Commander E TOOLTIP:GAME_CONCEPTadvantage Advantage: 17
Martial skill: positive_value 17
EnemyProwess:7
EnemyRank:3
ONCLICK:CHARACTER11822 TOOLTIP:CHARACTER11822 L high Count high Charles-Constantin: 19 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER12618 TOOLTIP:CHARACTER12618 L high Count high Heinrich: 13 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER11529 TOOLTIP:CHARACTER11529 L high Count high Rotbold: 9 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER12484 TOOLTIP:CHARACTER12484 L high Count high Conrad: 8 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER12485 TOOLTIP:CHARACTER12485 L high Count high Artôd: 7 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER20546 TOOLTIP:CHARACTER20546 L high Mayor high Dietmar: 7 E TOOLTIP:GAME_CONCEPTprowess Prowess
ONCLICK:CHARACTER25894 TOOLTIP:CHARACTER25894 L high Acfred: 7 E TOOLTIP:GAME_CONCEPTprowess Prowess
From Train Commanders: TOOLTIP:GAME_CONCEPTknight_i knight_iconP 3%
---------Completed---------
TOOLTIP:TERRAINplains L Plains
Region:Europe
ProvinceID:2051
Mild
Continental European
PlayerID:11230
EnemyID:12485
PlayerName:Count Meginhard II of Zutphen
EnemyName:Count Artôd of Lyon
SpecialBuilding:
---------Modifiers---------
T E TOOLTIP:GAME_CONCEPTadvantage Advantage: P 11 in your favor
Your damage is increased by V 22%

Our Advantage: positive_value 33indent_newline:3 
Commander E TOOLTIP:GAME_CONCEPTmartial_skill Martial Skill: positive_value 23indent_newline:3 
E TOOLTIP:GAME_CONCEPTcombat_roll Battle Roll: positive_value 10

Their Advantage: positive_value 22indent_newline:3 
Commander E TOOLTIP:GAME_CONCEPTmartial_skill Martial Skill: positive_value 17indent_newline:3 
E TOOLTIP:GAME_CONCEPTcombat_roll Battle Roll: positive_value 5
Keyword:CRUSADERWARS3
```

## Total War: Attila Data Input
With all this data, Crusader Wars creates a historical battle .modpack with the help of the RPFM app.
By following this tutorial as I did in the early development of this mod, you will know which data to insert on the .modpack https://www.twcenter.net/forums/showthread.php?815366-Scripted-Historical-Battles
...
...
(to be continued...)
