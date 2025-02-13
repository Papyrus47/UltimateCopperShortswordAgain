using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Skills
{
    /// <summary>
    /// 改变激活条件的SwingShoot,变为远离情况
    /// </summary>
    public class MoveSwingShoot : SwingShoot
    {
        public MoveSwingShoot(NPC npc, Vector2 startVel, float rot, float scaleY, float scaleAll, float maxTime) : base(npc, startVel, rot, scaleY, scaleAll, maxTime)
        {
        }

        public override void AI()
        {
            base.AI();
        }
        public override bool ActivationCondition(NPCSkills activeSkill)
        {
            float dis = NPC.Distance(Target.position);
            if (dis > 1000) // 太远
                return true;
            return Main.rand.Next((int)MathF.Pow(dis, 0.5f) + 1) < rand;
        }
    }
}
