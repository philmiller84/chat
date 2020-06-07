using Need4Chat.Interfaces;
using System.Collections.Generic;

namespace Need4Chat.Interfaces.DataLookups
{
    public class TradeDataLookup : IQueryData
    {
        public List<ItemDetails> GetAvailableItems()
        {
            return new List<ItemDetails> {
                new ItemDetails {ID = "1", description = "Fruit1"},
                new ItemDetails {ID = "2", description = "Fruitt1s"},
                new ItemDetails {ID = "3", description = "Fruitt1"},
                new ItemDetails {ID = "4", description = "FRUITS"},
                new ItemDetails {ID = "5", description = "stuf"},
            };

        }
    }
}
