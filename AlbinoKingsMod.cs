using System;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using IL.MoreSlugcats;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;


#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]


namespace AlbinoKings
{
    [BepInPlugin("ShinyKelp.AlbinoKings", "Albino Kings", "1.0.0")]
    public class AlbinoKingsMod : BaseUnityPlugin
    {
        private void OnEnable()
        {
            On.RainWorld.PreModsInit += RainWorld_PreModsInit;
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
        }

        #region Hailstorm hooks

        private void RainWorld_PreModsInit(On.RainWorld.orig_PreModsInit orig, RainWorld self)
        {
            orig(self);
            if (IsInit) return;

            On.Vulture.ctor += Vulture_ctor_Pre;
            On.Vulture.Update += Vulture_Update_Pre;
        }

        private void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig(self);
            if (IsInit) return;
            if (hasHailstorm)
            {
                On.Vulture.ctor += Vulture_ctor_Post;
                On.Vulture.Update += Vulture_Update_Post;
            }
            else
            {
                On.Vulture.Update -= Vulture_Update_Pre;
                On.Vulture.Update -= Vulture_Update_Pre;
            }
            IsInit = true;
        }
        private void Vulture_ctor_Pre(On.Vulture.orig_ctor orig, Vulture self, AbstractCreature abstractCreature, World world)
        {
            if (hasHailstorm && abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.MirosVulture && abstractCreature.superSizeMe)
                SetHailstormVariables();

            orig(self, abstractCreature, world);
        }

        private void Vulture_ctor_Post(On.Vulture.orig_ctor orig, Vulture self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if (hasHailstorm && abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.MirosVulture && abstractCreature.superSizeMe)
                ReturnHailstormVariables();
        }

        private void Vulture_Update_Pre(On.Vulture.orig_Update orig, Vulture self, bool eu)
        {
            if (hasHailstorm && self != null && self.IsMiros && (self.graphicsModule as VultureGraphics).albino)
                SetHailstormVariables();

            orig(self, eu);
        }

        private void Vulture_Update_Post(On.Vulture.orig_Update orig, Vulture self, bool eu)
        {
            orig(self, eu);
            if (hasHailstorm && self != null && self.IsMiros && (self.graphicsModule as VultureGraphics).albino)
                ReturnHailstormVariables();
        }

        void SetHailstormVariables()
        {
            Hailstorm.HSRemix.ScissorhawkEagerBirds.Value = true;
            Hailstorm.HSRemix.ScissorhawkNoNormalLasers.Value = true;
            Hailstorm.HSRemix.AuroricMirosEverywhere.Value = true;
        }
        void ReturnHailstormVariables()
        {
            Hailstorm.HSRemix.ScissorhawkEagerBirds.Value = hailstormAI;
            Hailstorm.HSRemix.ScissorhawkNoNormalLasers.Value = hailstormLasers;
            Hailstorm.HSRemix.AuroricMirosEverywhere.Value = hailstormAll;
        }
        void CopyHailstormVariables()
        {
            hailstormLasers = Hailstorm.HSRemix.ScissorhawkNoNormalLasers.Value;
            hailstormAI = Hailstorm.HSRemix.ScissorhawkEagerBirds.Value;
            hailstormAll = Hailstorm.HSRemix.AuroricMirosEverywhere.Value;
        }

        #endregion

        private bool hailstormLasers, hailstormAI, hailstormAll;

        private bool IsInit;
        AlbinoKingsOptions options;
        bool hasApexUp, hasHailstorm;



        private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (IsInit) return;

                hasApexUp = hasHailstorm = false;
                foreach(ModManager.Mod mod in ModManager.ActiveMods)
                {
                    if (mod.id == "theincandescent")
                        hasHailstorm = true;
                    if (mod.id == "ShinyKelp.ApexUpYourSpawns")
                        hasApexUp = true;
                }

                //Your hooks go here
                IL.KingTusks.Tusk.Update += Tusk_Update;
                On.AbstractCreature.ctor += AbstractCreature_ctor;
                On.Vulture.DropMask += Vulture_DropMask;
                On.Vulture.Violence += Vulture_Violence;
                On.Vulture.Snap += Vulture_Snap;
                On.VultureGraphics.ctor += VultureGraphics_ctor;
                On.VultureAI.OnlyHurtDontGrab += VultureAI_OnlyHurtDontGrab;
                On.GameSession.ctor += GameSession_ctor;

                options = new AlbinoKingsOptions();

                MachineConnector.SetRegisteredOI("ShinyKelp.AlbinoKings", options);
                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        private void AbstractCreature_ctor(On.AbstractCreature.orig_ctor orig, AbstractCreature self, World world, CreatureTemplate creatureTemplate, Creature realizedCreature, WorldCoordinate pos, EntityID ID)
        {
            orig(self, world, creatureTemplate, realizedCreature, pos, ID);
            if(world != null && world.game != null)
            {
                if(self.creatureTemplate.IsVulture ||
                    self.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.MirosVulture)
                {
                    if (!self.superSizeMe && !hasApexUp && AlbinoKingsOptions.MoreAlbinos.Value)
                    {
                        if(world.game.IsArenaSession)
                            self.superSizeMe = UnityEngine.Random.value > 0.9f;
                        else
                            self.superSizeMe = UnityEngine.Random.value > 0.95f;
                    }
                }
            }
        }

        private void GameSession_ctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            orig(self, game);
            if (hasHailstorm)
                CopyHailstormVariables();
        }

        private bool VultureAI_OnlyHurtDontGrab(On.VultureAI.orig_OnlyHurtDontGrab orig, VultureAI self, PhysicalObject testObj)
        {
            if(self != null && self.vulture != null && self.vulture.graphicsModule != null && 
                (self.vulture.graphicsModule as VultureGraphics).albino && (self.vulture.State as HealthState).health < 0.7f)
            {
                if(testObj != null && testObj is Creature creature && !creature.dead)
                    return true;
            }
            return orig(self, testObj);
        }

        bool ApplicableBuffs(Vulture vulture)
        {
            if (vulture is null || vulture.graphicsModule is null)
                return false;
            return vulture.IsKing && ((vulture.graphicsModule as VultureGraphics).albino || AlbinoKingsOptions.AllStrong.Value);
        }

        private void Vulture_Snap(On.Vulture.orig_Snap orig, Vulture self, BodyChunk snapAt)
        {
            orig(self, snapAt);
            if(self != null && ApplicableBuffs(self))
            {
                self.snapFrames = 33;
            }
        }

        private void Vulture_Violence(On.Vulture.orig_Violence orig, Vulture self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos onAppendagePos, Creature.DamageType type, float damage, float stunBonus)
        {
            bool isAlbinoKing = false;
            if(self != null && ApplicableBuffs(self))
            {
                isAlbinoKing = true;
                if (source != null && source.owner is ExplosiveSpear)
                    damage *= 0.6f;
                else damage *= 0.8f;
                stunBonus *= 0.8f;
                if ((self.State as Vulture.VultureState).health > 0.65f)
                    damage *= 0.4f;
                else damage *= 2.5f;
            }
            orig(self, source, directionAndMomentum, hitChunk, onAppendagePos, type, damage, stunBonus);
            if (isAlbinoKing)
            {
                if ((self.State as Vulture.VultureState).health > 0.65f)
                    self.AI.disencouraged -= damage * 0.1f;
                else
                    self.AI.disencouraged += damage *= 0.35f;
            }
        }

        private void Vulture_DropMask(On.Vulture.orig_DropMask orig, Vulture self, UnityEngine.Vector2 violenceDir)
        {
            if (!self.dead && ApplicableBuffs(self))
                return;
            else orig(self, violenceDir);
        }

        private void VultureGraphics_ctor(On.VultureGraphics.orig_ctor orig, VultureGraphics self, Vulture ow)
        {
            orig(self, ow);
            if(self != null && self.vulture != null && self.vulture.abstractCreature != null && self.vulture.abstractCreature.world != null && self.vulture.abstractCreature.world.game != null)
            {
                if (!self.albino && self.vulture.abstractCreature.superSizeMe)
                    self.albino = true;

                if ((self.albino || AlbinoKingsOptions.AllStrong.Value) && self.vulture.IsKing)
                    self.ColorB = new HSLColor(Mathf.Lerp(0.73f, 0.85f, UnityEngine.Random.value),
                        Mathf.Lerp(0.8f, 1f, 1f - UnityEngine.Random.value * UnityEngine.Random.value),
                        Mathf.Lerp(0.45f, 1f, UnityEngine.Random.value * UnityEngine.Random.value));   
            }
            
        }

        private void Tusk_Update(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdcI4(0x19));
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<KingTusks.Tusk, int>>((self) =>
            {
                if(AlbinoKingsOptions.AllStrong.Value)
                    return 0xF;
                if (self is null || self.vulture is null || self.vulture.abstractCreature is null)
                    return 0x19;
                if (self.vulture.abstractCreature.superSizeMe)
                    return 0xF;
                else return 0x19;
            });

            c.Index = 0;
            c.GotoNext(MoveType.After,
                x => x.MatchLdcI4(0x50));
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<KingTusks.Tusk, int>>((self) =>
            {
                if (AlbinoKingsOptions.AllStrong.Value)
                    return 0x14;
                if (self is null || self.vulture is null || self.vulture.abstractCreature is null)
                    return 0x50;

                if (self.vulture.abstractCreature.superSizeMe)
                    return 0x14;
                else return 0x50;
            });

            c.Index = 0;
            c.GotoNext(MoveType.After,
                x => x.MatchLdfld<KingTusks.Tusk>("currWireLength"),
                x => x.MatchLdsfld<KingTusks.Tusk>("maxWireLength"),
                x => x.MatchLdcR4(90f));
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<KingTusks.Tusk, float>>((self) =>
            {
                if (AlbinoKingsOptions.AllStrong.Value)
                    return 40f;
                if (self is null || self.vulture is null || self.vulture.abstractCreature is null)
                    return 90f;
                if (self.vulture.abstractCreature.superSizeMe)
                    return 40f;
                else return 90f;
            });
            c.Index = 0;
            c.GotoNext(MoveType.After,
                x => x.MatchLdcR4(120f));
            c.GotoNext(MoveType.After,
                x => x.MatchDiv(),
                x => x.MatchSub(),
                x => x.MatchStfld<KingTusks.Tusk>("stuck"));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Action<KingTusks.Tusk>>((self) =>
            {
                if (AlbinoKingsOptions.AllStrong.Value)
                    self.stuck -= 0.025f;
                else if (self is null || self.vulture is null || self.vulture.abstractCreature is null)
                    return;
                else if (self.vulture.abstractCreature.superSizeMe)
                    self.stuck -= 0.025f;
            });

            c.Index = 0;
            c.GotoNext(MoveType.After,
                x => x.MatchLdcI4(0xf0));
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<KingTusks.Tusk, int>>((self) =>
            {
                if (AlbinoKingsOptions.AllStrong.Value)
                    return 0x50;
                if (self is null || self.vulture is null || self.vulture.abstractCreature is null)
                    return 0xF0;
                if (self.vulture.abstractCreature.superSizeMe)
                    return 0x50;
                else return 0xF0;
            });

        }

    }
}
