using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmegaEngine
{
    class ProtectiveHazard
    {

        public enum materials { woodenCrate, metalCrate, safeOilDrum }
        materials material;
        string name { get; set; }
        public int protectiveHazardHP { get; set; }
        public int spaceX { get; set; }
        public int spaceY { get; set; }

        public ProtectiveHazard()
        {

        }

        public ProtectiveHazard(string name, materials material, int protectiveHazardHP)
        {
            this.name = name;
            this.material = material;
            this.protectiveHazardHP = protectiveHazardHP;
        }

        public ProtectiveHazard(ProtectiveHazard protectiveHazard)
        {
            name = protectiveHazard.name;
            material = protectiveHazard.material;
            protectiveHazardHP = protectiveHazard.protectiveHazardHP;
        }

        public bool canProtect()
        {
            if (protectiveHazardHP > 0)
                return true;
            else
                return false;
        }

        public ProtectiveHazard(Globals.protectiveHazardTypes protectiveHazardType)
        {
            switch (protectiveHazardType)
            {
                case Globals.protectiveHazardTypes.woodenCrate:

                    ProtectiveHazard woodenCrate = new ProtectiveHazard("Wooden Crate", ProtectiveHazard.materials.woodenCrate, 10);
                    this.copyProtectiveHazard(woodenCrate);
                    break;

                case Globals.protectiveHazardTypes.metalCrate:

                    ProtectiveHazard metalCrate = new ProtectiveHazard("Metal Crate", ProtectiveHazard.materials.metalCrate, 15);
                    this.copyProtectiveHazard(metalCrate);
                    break;

                case Globals.protectiveHazardTypes.safeOilDrum:

                    ProtectiveHazard safeOilDrum = new ProtectiveHazard("Safe Oil Drum", ProtectiveHazard.materials.safeOilDrum, 5);
                    this.copyProtectiveHazard(safeOilDrum);
                    break;
            }
        }

        public void copyProtectiveHazard(ProtectiveHazard original)
        {
            name = original.name;
            material = original.material;
            protectiveHazardHP = original.protectiveHazardHP;
        }
    }
}