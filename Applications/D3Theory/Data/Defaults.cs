namespace D3Theory.Data
{
    using System.Collections.Generic;

    public static class Defaults
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static D3Generic GetDefaultGeneric()
        {
            return new D3Generic
                       {
                           ActionDelay = 0.2f
                       };
        }

        public static IList<D3Class> GetDefaultClasses()
        {
            IList<D3Class> classes = new List<D3Class>();

            var @class = new D3Class
            {
                Name = "Wizard",
                PrimaryAttribute = D3Attribute.Int,
                PrimaryResourceName = "Arcane Power",
                Attributes = DefaultsWizard.Attributes,
                Skills = DefaultsWizard.Skills
            };
            classes.Add(@class);

            @class = new D3Class
            {
                Name = "Witch Doctor",
                PrimaryAttribute = D3Attribute.Int,
                PrimaryResourceName = "Mana",
                Attributes = DefaultsWitchDoctor.Attributes,
                Skills = DefaultsWitchDoctor.Skills
            };
            classes.Add(@class);

            @class = new D3Class
            {
                Name = "Demon Hunter",
                PrimaryAttribute = D3Attribute.Dex,
                PrimaryResourceName = "Hatred",
                SecondaryResourceName = "Discipline",
                Attributes = DefaultsDemonHunter.Attributes,
                Skills = DefaultsDemonHunter.Skills
            };
            classes.Add(@class);

            @class = new D3Class
            {
                Name = "Monk",
                PrimaryAttribute = D3Attribute.Dex,
                PrimaryResourceName = "Spirit",
                Attributes = DefaultsMonk.Attributes,
                Skills = DefaultsMonk.Skills
            };
            classes.Add(@class);

            @class = new D3Class
            {
                Name = "Barbarian", 
                PrimaryAttribute = D3Attribute.Str,
                PrimaryResourceName = "Rage",
                Attributes = DefaultsBarbarian.Attributes,
                Skills = DefaultsBarbarian.Skills
            };
            classes.Add(@class);

            @class = new D3Class
            {
                Name = "Crusader",
                PrimaryAttribute = D3Attribute.Str,
                PrimaryResourceName = "Wrath",
                Attributes = DefaultsCrusader.Attributes,
                Skills = DefaultsCrusader.Skills
            };
            classes.Add(@class);

            return classes;
        }

        public static Simulation GetDefaultSimulation()
        {
            var sim = new Simulation
                          {
                              Attributes = new Dictionary<D3Attribute, float>
                                      {
                                          { D3Attribute.Dex, 10 },
                                          { D3Attribute.Vit, 10 },
                                      },
                              Class = "Demon Hunter",
                              TargetCountMin = 1,
                              TargetCountMax = 4,
                              Seconds = 60,
                              Gear = new List<D3Gear>
                                         {
                                             new D3Gear
                                                 {
                                                     Name = "MainHand",
                                                     Attributes = new Dictionary<D3Attribute, float>
                                                                      {
                                                                          { D3Attribute.DmgMin, 10.0f },
                                                                          { D3Attribute.DmgMax, 20.0f },
                                                                          { D3Attribute.AttackSpeed, 1.0f },
                                                                      }
                                                 }
                                         }
                          };

            return sim;
        }
    }
}
