using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Skills
{
    /// <summary>
    /// 游走AI
    /// </summary>
    public class Move : UCS_Skills
    {
        public float speed;
        public Move(NPC npc, float speed) : base(npc)
        {
            this.speed = speed;
        }
        public override void AI()
        {
            NPC.ai[0]++;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
            float dis = NPC.Distance(Target.position);
            if (dis > 330)
                NPC.velocity = (NPC.velocity * 10 + (Target.Center - NPC.Center).SafeNormalize(default) * speed) / 11f;
            else if (dis < 100)
                NPC.velocity = (NPC.velocity * 10 + (NPC.Center - Target.Center).SafeNormalize(default) * speed) / 11f;
            else
                NPC.velocity = (NPC.velocity * 10 + (Target.Center - NPC.Center).SafeNormalize(default) * speed * 0.1f) / 11f;
        }
        public override bool ActivationCondition(NPCSkills activeSkill) => true;
        public override bool SwitchCondition(NPCSkills changeToSkill) => NPC.ai[0] > 120;
    }
}
