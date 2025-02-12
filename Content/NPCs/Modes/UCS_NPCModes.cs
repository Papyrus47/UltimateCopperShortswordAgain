using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Modes
{
    public abstract class UCS_NPCModes : NPCModes
    {
        /// <summary>
        /// 铜短剑
        /// </summary>
        public BasicSkillNPC CopperShortsword => NPC.ModNPC as CopperShortsword;
        public UCS_NPCModes(NPC npc) : base(npc)
        {
        }
    }
}
