using MelonLoader;
using ScheduleOne.UI;
using UnityEngine;
using HarmonyLib;
using static ScheduleOne.Console;
using System;
using System.Collections.Generic;
using System.Reflection;
using ScheduleOne.DevUtilities;
using ScheduleOne.ItemFramework;
using ScheduleOne.Tools;

[assembly: MelonInfo(typeof(Schedule1ConsoleUnlocker.Core), "Schedule1ConsoleUnlocker", "1.0.0", "iCallH4x", null)]
[assembly: MelonGame("TVGS", "Schedule I Free Sample")]

namespace Schedule1ConsoleUnlocker
{
    public class Core : MelonMod
    {
        private HarmonyLib.Harmony _harmony;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Schedule1ConsoleUnlocker loaded.");

            _harmony = new HarmonyLib.Harmony("com.console.unlocker");

            var originalMethod = AccessTools.PropertyGetter(typeof(ConsoleUI), "IS_CONSOLE_ENABLED");
            var postfixMethod = typeof(Core).GetMethod(nameof(ForceConsoleEnabled));

            if (originalMethod != null && postfixMethod != null)
            {
                _harmony.Patch(originalMethod, postfix: new HarmonyMethod(postfixMethod));
                MelonLogger.Msg("Successfully patched IS_CONSOLE_ENABLED");
            }
            else
            {
                MelonLogger.Warning("Failed to patch IS_CONSOLE_ENABLED - method not found.");
            }
            MelonLogger.Msg("Initializing all console commands");



            Type consoleType = typeof(ScheduleOne.Console);
            FieldInfo commandsField = consoleType.GetField("commands", BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var item in Resources.FindObjectsOfTypeAll<ItemDefinition>())
            {
                item.AvailableInDemo = true;
                MelonLogger.Msg($"Unlocked: {item.ID}");
            }

            if (commandsField != null)
            {
                if (commandsField != null)
                {
                    var commandsDict = (Dictionary<string, ConsoleCommand>)commandsField.GetValue(null) ?? new Dictionary<string, ConsoleCommand>();

                    if (commandsDict.Count == 0)
                    {
                        MelonLogger.Warning("Console commands are missing - registering manually");
                        RegisterConsoleCommands(commandsDict);
                        MelonLogger.Msg("All console commands registered");
                    }
                    else
                    {
                        MelonLogger.Msg("Console commands were already registered");
                    }
                }

                void RegisterConsoleCommands(Dictionary<string, ConsoleCommand> commandsDict)
                {
                    var commands = new Dictionary<string, ConsoleCommand>
                    {
                        ["addemployee"] = new AddEmployeeCommand(),
                        ["give"] = new AddItemToInventoryCommand(),
                        ["bind"] = new Bind(),
                        ["changecash"] = new ChangeCashCommand(),
                        ["changebalance"] = new ChangeOnlineBalanceCommand(),
                        ["clearbinds"] = new ClearBinds(),
                        ["clearinventory"] = new ClearInventoryCommand(),
                        ["clearwanted"] = new ClearWanted(),
                        ["disable"] = new Disable(),
                        ["enable"] = new Enable(),
                        ["freecam"] = new FreeCamCommand(),
                        ["addxp"] = new GiveXP(),
                        ["growplants"] = new GrowPlants(),
                        ["hideui"] = new HideUI(),
                        ["unhideui"] = new UnhideUI(),
                        ["lowerwanted"] = new LowerWanted(),
                        ["packageproduct"] = new PackageProduct(),
                        ["raisewanted"] = new RaisedWanted(),
                        ["save"] = new Save(),
                        ["setdiscovered"] = new SetDiscovered(),
                        ["setemotion"] = new SetEmotion(),
                        ["setenergy"] = new SetEnergy(),
                        ["sethealth"] = new SetHealth(),
                        ["setjumpmultiplier"] = new SetJumpMultiplier(),
                        ["setlawintensity"] = new SetLawIntensity(),
                        ["setmovespeed"] = new SetMoveSpeedCommand(),
                        ["setowned"] = new SetPropertyOwned(),
                        ["setquality"] = new SetQuality(),
                        ["setqueststate"] = new SetQuestState(),
                        ["setquestentrystate"] = new SetQuestEntryState(),
                        ["setrelationship"] = new SetRelationship(),
                        ["setstaminareserve"] = new SetStaminaReserve(),
                        ["settime"] = new SetTimeCommand(),
                        ["settimescale"] = new SetTimeScale(),
                        ["setunlocked"] = new SetUnlocked(),
                        ["setvar"] = new SetVariableValue(),
                        ["spawnvehicle"] = new SpawnVehicleCommand(),
                        ["teleport"] = new Teleport()
                    };

                    foreach (var command in commands)
                    {
                        commandsDict[command.Key] = command.Value;
                    }
                }

            }
            else
            {
                MelonLogger.Error("Failed to access the console commands dict");
            }
        }

        public class UnhideUI : ConsoleCommand
        {
            public override string CommandWord => "unhideui";
            public override string CommandDescription
            {
                get => "Unhides the UI";
            }
            public override string ExampleUsage => "unhideui";
            public override void Execute(List<string> args)
            {
                Singleton<HUD>.Instance.canvas.enabled = true;
            }
        }

        private static void ForceConsoleEnabled(ref bool __result)
        {
            MelonLogger.Msg("Forcing console to enable");
            __result = true; 
        }
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F8)) 
            {
                EnableConsole();
            }

        private void EnableConsole()
        {
            ConsoleUI console = GameObject.FindObjectOfType<ConsoleUI>();
            if (console != null)
            {
                console.SetIsOpen(true);
                MelonLogger.Msg("Dev console enabled");
            }
            else
            {
                MelonLogger.Warning("Console GameObject not found");
            }
        }
    }
}