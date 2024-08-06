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
            foreach (var item in list)
            {
                if (item is IList nestedList)
                    DisposeArbitraryResultList(nestedList);
            }
            list.Clear();
        }


        public static void ClearArbitraryNestedResultList(IList list)
        {
            foreach (var item in list)
            {
                if (item is IList nestedList)
                            ClearArbitraryNestedResultList(nestedList);
            }
        }




        public static IList CreatePreinitializedResultList<T>(int[] capacities, int dimensions)
        {
            if (dimensions >= capacities.Length - 1)
                return new List<T>(capacities[dimensions]);

            var aktuelleListe = new List<IList>(capacities[dimensions]);

            for (int i = 0; i < capacities[dimensions]; i++)
            {
                aktuelleListe.Add(CreatePreinitializedResultList<T>(capacities, dimensions + 1));
            }

            return aktuelleListe;
        }
    }

}
