using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;


namespace Crusader_Wars
{
    public static class BattleScript
    {
        static string filePath = Directory.GetFiles("Battle Files\\script", "tut_start.lua", SearchOption.AllDirectories)[0];



        public static void CreateScript()
        {
            string script_base = "\n\nfunction remaining_soldiers()\r\n    dev.log(\"-----REMAINING SOLDIERS-----!!\")";
            File.AppendAllText(filePath, script_base);
        }

        public static void SetLocals(string unitName, string declarationName)
        {
            string local = $"\n\tdev.log(\"{unitName}-\".. {declarationName}.unit:number_of_men_alive())";
            File.AppendAllText(filePath, local);
        }

        public static void SetLocalsKills(List<(string unitName, string declarationName)> units_scripts_list)
        {
            //Units Script Start
            string start = $"\n\nfunction kills()\r\n    dev.log(\"-----NUMBERS OF KILLS-----!!\")";
            File.AppendAllText(filePath, start);

            //Units Locals Kills
            foreach (var unit in units_scripts_list)
            {
                string locals = $"\n\tdev.log(\"kills_{unit.unitName}-\".. {unit.declarationName}.unit:number_of_enemies_killed())";
                File.AppendAllText(filePath, locals);
            }

        }

        public static void CloseScript()
        {
            string close = "\nend;";
            File.AppendAllText(filePath, close);
        }

        //Add
        public static void EraseScript()
        {
            string original = "\r\n-----------------------------------------------------------------------------------\r\n-----------------------------------------------------------------------------------\r\n--\r\n--\tINITIAL SCRIPT SETUP\r\n--\r\n-----------------------------------------------------------------------------------\r\n-----------------------------------------------------------------------------------\r\n\r\n-- clear out loaded files\r\nsystem.ClearRequiredFiles();\r\n\r\nlocal logging_enabled = true;\r\n\r\n-- load in battle script library\r\nrequire \"lua_scripts.Battle_Script_Header\";\r\n\r\n-- declare battlemanager object\r\nbm = battle_manager:new(empire_battle:new());\r\n\r\n-- get battle name from folder, and print header\r\nbattle_name, battle_shortform = get_folder_name_and_shortform();\r\n\r\n-- load in other script files associated with this battle\r\npackage.path = package.path .. \";data/Script/\" .. battle_name .. \"/?.lua\";\r\n\r\nrequire (battle_shortform .. \"_Declarations\");\r\n\r\n----------------------------------------------------------------------------------------------------------------------------\r\n----------------------------------------------------------------------------------------------------------------------------\r\n--\r\n--\tHISTORICAL BATTLE CUTSCENE AND UNIT POSITION SCRIPT\r\n--\r\n----------------------------------------------------------------------------------------------------------------------------\r\n----------------------------------------------------------------------------------------------------------------------------\r\n\r\ndev = require(\"lua_scripts.dev\");\r\n\r\nrequire(\"lua_scripts.logging_callbacks\");\r\n\r\nlocal date = os.date(\"%A, %c\");\r\n\r\ndev.log(\"\\n\" .. date);\r\ndev.log\"\\n Script Loaded\";\r\n\r\n\r\nbm:setup_victory_callback(function() file_debug() end);\r\nbm:register_phase_change_callback(\"Complete\", function() file_debug() end);\t\r\n\r\n \r\nfunction Deployment_Phase()\r\n\tbm:out(\"Battle is in deployment phase\");\r\nend;\r\n\r\nfunction Start_Battle()\r\n\tbm:out(\"Battle is Starting\");\r\n\t\r\nend;\r\n\r\nlocal scripting = require \"lua_scripts.episodicscripting\"\r\n-- Callbacks\r\nfunction EndBattle(context)\r\n   \r\n    if context.string == \"button_end_battle\" then\r\n        dev.log(\"Battle has finished\")\r\n    end;\r\n    \r\n    if context.string == \"button_dismiss_results\" then\r\n        dev.log(\"Battle has finished\")\r\n    end;\r\n\r\nend;\r\nscripting.AddEventCallBack(\"ComponentLClickUp\", EndBattle);\r\n\r\n\r\n\r\n--Crusader Wars Get Winner\r\nfunction file_debug()\r\n\r\n\tbm:callback(function() bm:end_battle() end, 1000);\r\n\n\tif is_routing_or_dead(Alliance_Stark) then\t\r\n\t\tbm:out(\"Player has lost, army is routing\");\r\n        dev.log(\"Defeat\")\r\n\telseif is_routing_or_dead(Alliance_Bolton) then\r\n\t\tbm:out(\"Player has won !\");\r\n\t\tdev.log(\"Victory\")\r\n\tend;\r\n\r\n    commander_system();\r\n\tremaining_soldiers();\r\n\tkills();\r\n\t\r\n\r\nend;\r\n\r\nfunction commander_system()\r\n    \r\n    if(not Stark:is_commander_alive()) then\r\n        dev.log(\"Player general has fallen\")\r\n    end;\r\n    \r\n    if(not Bolton:is_commander_alive()) then\r\n        dev.log(\"Enemy general has fallen\")\r\n    end;\r\nend;";
            File.WriteAllText(filePath, original) ;
        }
    }
}
