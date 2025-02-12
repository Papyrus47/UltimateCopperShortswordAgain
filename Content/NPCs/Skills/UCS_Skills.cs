using ReLogic.Content;
using Terraria.GameContent;
using UltimateCopperShortsword.Content.NPCs.Modes;
using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Skills
{
    public abstract class UCS_Skills : NPCSkills
    {
        public static Asset<Texture2D> CopperShortsword2 = ModContent.Request<Texture2D>("UltimateCopperShortsword/Content/NPCs/CopperShortsword2");
        public static Asset<Texture2D> CopperShortsword3 = ModContent.Request<Texture2D>("UltimateCopperShortsword/Content/NPCs/CopperShortsword3");
        public CopperShortsword copperShortsword => ModNPC as CopperShortsword;
        public Player Target => copperShortsword.TargetPlayer;
        public UCS_Skills(NPC npc) : base(npc)
        {
        }
        public override void OnSkillActive(NPCSkills activeSkill)
        {
            NPC.ai[0] = NPC.ai[1] = NPC.ai[2] = NPC.ai[3] = 0;
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate(NPCSkills changeToSkill)
        {
            NPC.ai[0] = NPC.ai[1] = NPC.ai[2] = NPC.ai[3] = 0;
            SkillTimeOut = false;
        }
        public void SyncNPC()
        {
            if(Target != null && Target.whoAmI == Main.myPlayer)
                NPC.netUpdate = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Item[ItemID.CopperShortsword].Value;
            if(copperShortsword.CurrentMode is TwoLevel)
                tex = CopperShortsword2.Value;
            else if(copperShortsword.CurrentMode is ThreeLevel)
                tex = CopperShortsword3.Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, drawColor, NPC.rotation, tex.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
