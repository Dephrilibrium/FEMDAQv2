using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instrument.LogicalLayer.SubClasses
{
    public class ResultListsHelper
    {
        public static void DisposeArbitraryResultList(IList list)
        {
            if (list == null)
                return;

            foreach (var item in list)
            {
                if (item is IList nestedList)
                    DisposeArbitraryResultList(nestedList);
            }
            list.Clear();
        }


        public static void ClearArbitraryNestedResultList<T>(IList list)
        {
            if (list == null)
                return;

            if (list is T)
            {
                list.Clear();
                return;
            }

            foreach (var item in list)
            {
                if (item is IList nestedList) // If its more-dimensional (List<List...), call recursive
                    ClearArbitraryNestedResultList<T>(nestedList);
            }
        }




        //public static List<object> CreatePreallocatedResultList<T>(int[] capacities, IList ListToAppend)
        //{
        //    var iCurrentDimension = capacities.Length - 1;
        //    // Erstelle die aktuelle Liste mit der angegebenen Kapazität
        //    var list = new List<T>(capacities[iCurrentDimension]);

        //    // Wenn die aktuelle Tiefe die letzte Dimension ist, fülle die Liste mit Standardwerten
        //    if (iCurrentDimension == capacities.Length - 1)
        //    {
        //        for (int i = 0; i < capacities[iCurrentDimension]; i++)
        //        {
        //            list.Add(default(T));
        //        }
        //    }
        //    else
        //    {
        //        // Andernfalls, fülle die Liste rekursiv mit tieferen Listen
        //        for (int i = 0; i < capacities[iCurrentDimension]; i++)
        //        {
        //            list.Add(CreatePreallocatedResultList<T>(capacities));
        //        }
        //    }

        //    return list;
        //}
    }

}
