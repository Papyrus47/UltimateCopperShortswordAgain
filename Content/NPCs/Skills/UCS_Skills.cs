using ReLogic.Content;
using Terraria.GameContent;
using UltimateCopperShortsword.Content.NPCs.Modes;
using UltimateCopperShortsword.Content.Projs.Bosses.UltimateCopperShortswordProj;
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
            SyncNPC();
        }
        public override void OnSkillDeactivate(NPCSkills changeToSkill)
        {
            NPC.ai[0] = NPC.ai[1] = NPC.ai[2] = NPC.ai[3] = 0;
            SkillTimeOut = false;
            SyncNPC();
        }
        public void SyncNPC()
        {
            if (Target != null && Main.netMode != NetmodeID.MultiplayerClient)
                NPC.netUpdate = true;
                //NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }
        public void Shoot(Vector2 center = default,Vector2 vel = default, float speed = 10f)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if(center == default)
                center = NPC.Center;
            if(vel == default)
                vel = NPC.velocity.SafeNormalize(Vector2.UnitY);
            int shootType = ModContent.ProjectileType<UCSProj1>();
            if (copperShortsword.CurrentMode is not OneLevel)
                shootType = ModContent.ProjectileType<UCSProj2>();
            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), center, vel * speed, shootType, NPC.damage, 0f, Target.whoAmI).netUpdate = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Item[ItemID.CopperShortsword].Value;
            if(copperShortsword.CurrentMode is TwoLevel)
                tex = CopperShortsword2.Value;
            else if(copperShortsword.CurrentMode is ThreeLevel)
                tex = CopperShortsword3.Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.White with { A = 0 }, NPC.rotation, new Vector2(tex.Width * 0.41f, tex.Height * 0.41f), NPC.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.White with { A = 0 }, NPC.rotation, new Vector2(tex.Width * 0.59f, tex.Height * 0.59f), NPC.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.White with { A = 0 }, NPC.rotation, new Vector2(tex.Width * 0.41f, tex.Height * 0.59f), NPC.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.White with { A = 0 }, NPC.rotation, new Vector2(tex.Width * 0.59f, tex.Height * 0.41f), NPC.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, drawColor, NPC.rotation, tex.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
