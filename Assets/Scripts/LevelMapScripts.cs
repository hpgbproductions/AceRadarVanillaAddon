/*
 * Some simple scripts for sandbox and levels
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class LevelMapScripts : MonoBehaviour
{
    [SerializeField] private AceRadarBackend backend;

    private string LevelName;

    private bool IsCombatLevel = false;
    private string[] CombatLevels = new string[]
    {
        "Gun Training",
        "Rocket Training",
        "Bomb Training",
        "Missile Training",
        "WW2 Dogfight",
        "WW2 Torpedo Bomber",
        "Bomber Escort",
        "Bridge Demolition",
        "Convoy Assault",
        "Dogfight"
    };
    private string[] CombatLevelGroundTargets = new string[]
    {
        "AntiAircraftTankScript",
        "SimpleGroundVehicleScript",
        "SinkableShipScript",
        "BombTargetScript"
    };

    private bool InSandboxCurrent = false;
    private bool InSandboxPrevious = true;
    private bool InLevel = false;
    private bool InDesigner = false;
    private bool InDesignerPrevious = false;

    private int IntervalRegularAction = 60;
    private int NextRegularAction = 1;

    private void Update()
    {
        LevelName = ServiceProvider.Instance.GameState.CurrentLevelName;
        InLevel = ServiceProvider.Instance.GameState.IsInLevel;
        InDesigner = ServiceProvider.Instance.GameState.IsInDesigner;
        InSandboxCurrent = InLevel && !InDesigner;

        // Exited sandbox
        if (!InSandboxCurrent && InSandboxPrevious)
        {
            // Reset flags
            IsCombatLevel = false;

            // Reset blip color
            backend.SetDefaultBlipColor(AceRadarBackend.AceRadarColors.White);
        }
        // Entered sandbox
        else if (InSandboxCurrent && !InSandboxPrevious)
        {
            // Set combat level blip color
            if (CombatLevels.Contains(LevelName))
            {
                IsCombatLevel = true;
            }
        }


        if (InSandboxCurrent)
        {
            // (IntervalRegularAction) frames pass between each Regular Action
            if (NextRegularAction <= 0)
            {
                if (IsCombatLevel)
                {
                    Component[] allComponents = FindObjectsOfType<Component>();
                    foreach (Component c in allComponents)
                    {
                        Type cType = c.GetType();

                        if (cType.Name == "AiControlledAircraftScript")
                        {
                            if (LevelName == "Bomber Escort")
                            {
                                PropertyInfo isEnemyInfo = cType.GetProperty("IsFlightTargetDestructible");
                                bool isEnemy = (bool)isEnemyInfo.GetValue(c);
                                if (!isEnemy)    // Bomber
                                {
                                    backend.ModifyTargetBlip(c, AceRadarBackend.AceRadarSprites.Aircraft, AceRadarBackend.AceRadarColors.Blue, true);
                                }
                                else    // Enemy (Bomber Escort)
                                {
                                    backend.ModifyTargetBlip(c, AceRadarBackend.AceRadarSprites.AircraftCircled, AceRadarBackend.AceRadarColors.Red, true);
                                }
                            }
                            else
                            {
                                backend.ModifyTargetBlip(c, AceRadarBackend.AceRadarSprites.AircraftCircled, AceRadarBackend.AceRadarColors.Red, true);
                            }
                        }
                        else if (CombatLevelGroundTargets.Contains(cType.Name) || (LevelName == "Bridge Demolition" && cType.Name == "FracturedObject"))
                        {
                            backend.ModifyTargetBlip(c, AceRadarBackend.AceRadarSprites.GroundCircled, AceRadarBackend.AceRadarColors.Red);
                        }
                    }
                }

                NextRegularAction = IntervalRegularAction;
            }
            else
            {
                NextRegularAction--;
            }
        }

        InSandboxPrevious = InSandboxCurrent;
        InDesignerPrevious = InDesigner;
    }
}
