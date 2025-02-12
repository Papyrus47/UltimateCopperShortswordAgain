using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Modes
{
    public class OneLevel : UCS_NPCModes
    {
        public OneLevel(NPC npc) : base(npc)
        {
        }
        public override bool ActivationCondition(NPCModes activeMode) => false;
        public override bool SwitchCondition(NPCModes changeToMode) => NPC.life < NPC.lifeMax * 0.75f;
    }
}
