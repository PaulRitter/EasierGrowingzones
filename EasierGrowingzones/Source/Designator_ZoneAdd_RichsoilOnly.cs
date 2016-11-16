using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class Designator_ZoneAdd_CostumSoilOnly : Designator_ZoneAdd
    {
        private TerrainDef curDef = TerrainDefOf.Soil;

        protected override string NewZoneLabel
        {
            get
            {
                return "CostumSoilOnlyZone".Translate(curDef.label.CapitalizeFirst());
            }
        }

        public Designator_ZoneAdd_CostumSoilOnly()
        {
            this.zoneTypeToPlace = typeof(Zone_Growing);
            this.defaultLabel = "CostumSoilOnlyZone".Translate(curDef.label.CapitalizeFirst());
            this.defaultDesc = "DesignatorCostumSoilZoneDesc".Translate(curDef.label.CapitalizeFirst());
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/ZoneCreate_Growing", true);
            this.tutorTag = "";
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!base.CanDesignateCell(c).Accepted)
            {
                return false;
            }
            if (Find.TerrainGrid.TerrainAt(c) != curDef)
            {
                return false;
            }
            return true;
        }

        protected override Zone MakeNewZone()
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
                string labelCap = current.label.CapitalizeFirst(); ;
                list.Add(new FloatMenuOption(labelCap, delegate
                {
                    this.ProcessInput(ev);
                    DesignatorManager.Select(this);
                    this.curDef = TerrainDef.Named(current.defName);
                }, MenuOptionPriority.Medium, null, null, 0f, null)
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
}
