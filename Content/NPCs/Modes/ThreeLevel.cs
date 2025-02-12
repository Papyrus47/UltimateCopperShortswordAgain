using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Modes
{
    public class ThreeLevel : UCS_NPCModes
    {
        public ThreeLevel(NPC npc) : base(npc)
        {
        }

        public override bool ActivationCondition(NPCModes activeMode) => NPC.life < NPC.lifeMax * 0.5f;
        public override bool SwitchCondition(NPCModes changeToMode) => false;
    }
}
