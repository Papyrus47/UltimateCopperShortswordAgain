using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Modes
{
    public class TwoLevel : UCS_NPCModes
    {
        public TwoLevel(NPC npc) : base(npc)
        {
        }
        public override bool ActivationCondition(NPCModes activeMode) => NPC.life < NPC.lifeMax * 0.75f;
        public override bool SwitchCondition(NPCModes changeToMode) => NPC.life < NPC.lifeMax * 0.5f;
    }
}
