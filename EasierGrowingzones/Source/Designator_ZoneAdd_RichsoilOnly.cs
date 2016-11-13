using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class Designator_ZoneAdd_RichsoilOnly : Designator_ZoneAdd
    {
        protected override string NewZoneLabel
        {
            get
            {
                return "RichsoilOnlyZone".Translate();
            }
        }

        public Designator_ZoneAdd_RichsoilOnly()
        {
            this.zoneTypeToPlace = typeof(Zone_Growing);
            this.defaultLabel = "RichsoilOnlyZone".Translate();
            this.defaultDesc = "DesignatorRichsoilOnlyZoneDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/ZoneCreate_Growing", true);
            this.tutorTag = "test";
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!base.CanDesignateCell(c).Accepted)
            {
                return false;
            }
            if (Find.FertilityGrid.FertilityAt(c) < 1.4f)
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
    }
}
