using Game.Singleton;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ticker
{
    public class TickerGame : Singleton<TickerGame>
    {
        private LinkedList<TickableItem> tickItems = new LinkedList<TickableItem>();
        private List<TickableItem> removeItems = new List<TickableItem>();

        private void Awake() => Inicialize(this);

        private void FixedUpdate()
        {
            if (tickItems.Count > 0)
            {
                if (removeItems.Count > 0)
                {
                    foreach (var element in removeItems)
                        tickItems.Remove(element);

                    removeItems.Clear();
                }

                foreach (var element in new List<TickableItem>(tickItems))
                    element.DoTryTick(this);
            }
        }

        public void RemoveTicker(TickableItem item) => removeItems.Add(item);

        public TickableItem AddInfinityTicker(Tickable ticker, int secondsInterval)
        {
            var element = new TickableItem(ticker, secondsInterval);
            tickItems.AddLast(element);
            return element;
        }

        public TickableItem AddDisposableTicker(Tickable ticker, int secondsInterval)
        {
            var element = new TickableItem(ticker, secondsInterval, true);
            tickItems.AddLast(element);
            return element;
        }
    }

    public delegate void Tickable(TickableItem tickableItem);

    public class TickableItem
    {
        public int GetActualTime { get => tickNow / (int)((1f / Time.fixedTime) + 1); }
        public bool IsDisposable { get; private set; } = false;

        private Tickable ticker;
        private int tickCount;
        private int tickNow;

        public TickableItem(Tickable ticker, int secondsInterval, bool isDisposable = false)
        {
            this.ticker = ticker;
            this.tickCount = secondsInterval * (int)(1f / Time.fixedTime);
            this.IsDisposable = isDisposable;
        }

        public void DoTryTick(TickerGame tickMachine)
        {
            if (tickCount <= tickNow)
            {
                tickNow = 0;
                ticker(this);

                if (IsDisposable)
                {
                    tickMachine.RemoveTicker(this);
                    return;
                }
            }

            tickNow++;
        }
    }

    public interface ITickable
    {
        void OnTick();
    }
}
