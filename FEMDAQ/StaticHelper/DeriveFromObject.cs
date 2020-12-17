using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMDAQ.StaticHelper
{
    public class DeriveFromObject
    {
        static string DeriveNameFromObject(object Structure)
        {
            string[] structureType = Structure.ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return structureType[structureType.Length - 1];
        }

        public static string DeriveNameFromStructure(object Structure)
        {
            var structureName = DeriveNameFromObject(Structure).Remove(0, "InfoBlock".Length);
            return structureName;
        }
    }
}
