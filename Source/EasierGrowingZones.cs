using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace EasierGrowingZones
{
    [StaticConstructorOnStartup]
    public static class TerrainZoneSelectLoader
    {
        static TerrainZoneSelectLoader()
        {
            var harmony = new Harmony("net.mseal.rimworld.mod.terrainzoneselect");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    public class Designator_EasierGrowingZoneSelect : Designator_ZoneAdd_Growing
    {
        public Designator_EasierGrowingZoneSelect()
        {
            this.zoneTypeToPlace = typeof(Zone_Growing);
            this.defaultLabel = "EasierGrowingZone".Translate(curDef.label.CapitalizeFirst());
            this.defaultDesc = "DesignatorEasierGrowingZoneDesc".Translate(curDef.label.CapitalizeFirst());
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/ZoneCreate_Growing", true);
        }

        private TerrainDef curDef = TerrainDefOf.Soil;

        public override AcceptanceReport CanDesignateCell(IntVec3 c, Map map)
        {
            if (!base.CanDesignateCell(c).Accepted)
            {
                return false;
            }
            if (map.terrainGrid.TerrainAt(c) != curDef)
            {
                return false;
            }
            return true;
        }

        public override Zone MakeNewZone()
        {
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.GrowingFood, KnowledgeAmount.Total);
            return new Zone_Growing();
        }

        public override void ProcessInput(Event ev)
        {
            if (!base.CheckCanInteract())
            {
                return;
            }
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            IEnumerable<TerrainDef> enumerable;
            enumerable = from floor in DefDatabase<TerrainDef>.AllDefs
                         where floor.fertility >= 0.4
                         select floor;
            enumerable = from x in enumerable
                         orderby x.fertility descending
                         select x;
            foreach (TerrainDef current in enumerable)
            {
                string labelCap = current.label.CapitalizeFirst();
                list.Add(new FloatMenuOption(labelCap, delegate
                {
                    ProcessInput(ev);
                    DesignatorManager.Select(this);
                    curDef = TerrainDef.Named(current.defName);
                }, MenuOptionPriority.Low, null, null, 0f, null)
                {
                    tutorTag = "SelectStuff-" + current.defName
                });
            }
            if (list.Count == 0)
            {
                Messages.Message("ERR_No_TerrainDefs".Translate(), MessageSound.RejectInput);
            }
            else
            {
                FloatMenu floatMenu = new FloatMenu(list);
                floatMenu.vanishIfMouseDistant = true;
                Find.WindowStack.Add(floatMenu);
                DesignatorManager.Select(this);
            }
        }
    }

    [HarmonyPatch(typeof(Zone_Growing), "GetZoneAddGizmos")]
    class ZoneGrowPatch
    {
        public static IEnumerable<Gizmo> GetZoneAddGizmos()
        {
            yield return DesignatorUtility.FindAllowedDesignator<Designator_EasierGrowingZoneSelect>();
        }

        static bool Prefix(ref IEnumerable<Gizmo> __result)
        {
            __result = GetZoneAddGizmos();
            return false;
        }
    }
}
