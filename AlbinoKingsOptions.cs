using BepInEx.Logging;
using Menu.Remix.MixedUI;
using System.Collections.Generic;
using UnityEngine;

namespace AlbinoKings
{
    public class AlbinoKingsOptions : OptionInterface
    {

        //public static ScavengerVideoOptions instance = new ScavengerVideoOptions();

        /*
        public static readonly Configurable<float> dodgeSkill = instance.config.Bind<float>("dodgeSkill", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public static readonly Configurable<float> meleeSkill = instance.config.Bind<float>("meleeSkill", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public static readonly Configurable<float> midRangeSkill = instance.config.Bind<float>("midRangeSkill", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public static readonly Configurable<float> blockingSkill = instance.config.Bind<float>("blockingSkill", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public static readonly Configurable<float> reactionSkill = instance.config.Bind<float>("reactionSkill", 0f, new ConfigAcceptableRange<float>(0f, 10f));

        public static readonly Configurable<float> aggression = instance.config.Bind<float>("aggression", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public static readonly Configurable<float> bravery = instance.config.Bind<float>("bravery", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public static readonly Configurable<float> dominance = instance.config.Bind<float>("dominance", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public static readonly Configurable<float> energy = instance.config.Bind<float>("energy", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public static readonly Configurable<float> nervous = instance.config.Bind<float>("nervous", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public static readonly Configurable<float> sympathy = instance.config.Bind<float>("sympathy", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        //*/

        public Configurable<bool> allStrong;// = instance.config.Bind<float>("sympathy", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public Configurable<bool> moreAlbinos;// = instance.config.Bind<float>("sympathy", 0f, new ConfigAcceptableRange<float>(0f, 10f));
        public Configurable<float> albinoChance;// = instance.config.Bind<float>("VultureMaskStun", 1.8f, new ConfigAcceptableRange<float>(0f, 10f));


        public static Configurable<bool> AllStrong;
        public static Configurable<bool> MoreAlbinos;
        public static Configurable<float> AlbinoChance;

        public AlbinoKingsOptions()
        {
            allStrong = config.Bind<bool>("allStrong", false);
            AllStrong = allStrong;
            moreAlbinos = config.Bind<bool>("moreAlbinos", true);
            MoreAlbinos = moreAlbinos;
            albinoChance = config.Bind<float>("albinoChance", 1f, new ConfigAcceptableRange<float>(0f, 10f));
            AlbinoChance = albinoChance;
        }

        private UIelement[] UIArrPlayerOptions;

        public override void Initialize()
        {
            var opTab = new OpTab(this, "Options");
            

            UIArrPlayerOptions = new UIelement[]
            {
                new OpLabel(15f, 445f, "All kings are powerful"),
                new OpCheckBox(allStrong, 150f, 442f){
                    description = "All king vultures will be stronger, regardless of albino."
                },

                new OpLabel(15f, 405f, "More albinos"),
                new OpCheckBox(moreAlbinos, 150f, 402f){
                    description = "Albino vultures will be more common."
                },
                new OpFloatSlider(albinoChance,new Vector2(20f, 360f), 200)
                {
                    description = "Albino king spawn chance (10 = 100%)."
                },

            };

            opTab.AddItems(UIArrPlayerOptions);
            
            this.Tabs = new[]
            {
                opTab
            };
            
        }

    }
}