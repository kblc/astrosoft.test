using Astrosoft.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Astrosoft.FakeSource
{
    public class FakeSource : ISource
    {
        private List<Item> items = new List<Item>();

        public FakeSource(long maxCount = 1000)
        {
            var systems = new[] { "CORE1","CORE2","CORE3" };
            var types = new[] { 'A', 'B', 'C', 'S' };

            for (int i = 0; i < maxCount; i++)
            {
                items.Add(new Item()
                {
                    Index = i + 1,
                    Message = "test message",
                    System = systems[getRandomNumber(0, systems.Length)],
                    Time = DateTime.Now,
                    Type = types[getRandomNumber(0, types.Length)],
                });
            }
        }

        public void Dispose()
        {
            items.Clear();
        }

        public SourceReadResult Read(long itemCountPerPage, long pageIndex, Func<IItem, bool> filter, string source, CancellationToken cancellationToken)
        {
            var goodItems = this.items.Where(i => !filter(i));

            var hasOther = goodItems.Count() % itemCountPerPage > 0;
            var pageCount = goodItems.Count() / itemCountPerPage + (hasOther ? 1 : 0);
            pageCount = Math.Max(1, pageCount);
            pageIndex = Math.Min(pageIndex, pageCount - 1);

            return new SourceReadResult(goodItems.Skip((int)(pageIndex * itemCountPerPage)).Take((int)(itemCountPerPage)), pageCount, pageIndex, source);
        }

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int getRandomNumber(int min, int max)
        {
            lock (syncLock) // synchronize
            { 
                return random.Next(min, max);
            }
        }
    }
}
