using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Skills
{
    public class StartText : UCS_Skills
    {
        public bool OnStart;
        public static LocalizedText Text1 = Language.GetOrRegister("Mods.UltimateCopperShortsword.Content.NPCs.Skills.StartText." + nameof(Text1));
        public static LocalizedText Text2 = Language.GetOrRegister("Mods.UltimateCopperShortsword.Content.NPCs.Skills.StartText." + nameof(Text2));
        public static LocalizedText Text3 = Language.GetOrRegister("Mods.UltimateCopperShortsword.Content.NPCs.Skills.StartText." + nameof(Text3));
        public static LocalizedText Text4 = Language.GetOrRegister("Mods.UltimateCopperShortsword.Content.NPCs.Skills.StartText." + nameof(Text4));
        public StartText(NPC npc) : base(npc)
        {
        }
        public override void AI()
        {
            NPC.rotation = -MathHelper.PiOver4;
            NPC.dontTakeDamage = true;
            switch ((int)NPC.ai[0]++)
            {
                case 0:
                    NPC.netUpdate = true;
                    Main.NewText(Text1, Color.OrangeRed);
                    break;
                case 60:
                    Main.NewText(Text2, Color.OrangeRed);
                    break;
                case 120:
                    Main.NewText(Text3, Color.OrangeRed);
                    break;
                case 180:
                    Main.NewText(Text4, Color.OrangeRed);
                    break;
            }
        }
        public override bool ActivationCondition(NPCSkills activeSkill) => false;
        public override bool SwitchCondition(NPCSkills changeToSkill) => NPC.ai[0] > 200;
        public override void OnSkillDeactivate(NPCSkills changeToSkill)
        {
            copperShortsword.OldSkills.Clear();
            base.OnSkillDeactivate(changeToSkill);
            NPC.dontTakeDamage = false;
        }
    }
}
