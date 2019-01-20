using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pops
{
    public class NumericTicker : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Pops.NumericTicker"/> class.
        /// </summary>
        /// <param name="initialTick">Sets InitialTick of a new instance.</param>
        public NumericTicker(int initialTick) : this(initialTick, 0, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Pops.NumericTicker"/> class.
        /// </summary>
        /// <param name="initialTick">Sets InitialTick of new instance.</param>
        /// <param name="tickerName">Sets TickerName of new instance.</param>
        public NumericTicker(int initialTick, string tickerName) : this(initialTick, 0, tickerName) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Pops.NumericTicker"/> class.
        /// </summary>
        /// <param name="initialTick">Sets InitialTick of new instance.</param>
        /// <param name="targetTick">Sets TargetTick of new instance.</param>
        public NumericTicker(int initialTick, int targetTick) : this(initialTick, targetTick, 1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Pops.NumericTicker"/> class.
        /// </summary>
        /// <param name="initialTick">Sets InitialTick of new instance.</param>
        /// <param name="targetTick">Sets TargetTick of new instance.</param>
        /// <param name="tickerName">Sets TickerName of new instance.</param>
        public NumericTicker(int initialTick, int targetTick, string tickerName) : this(initialTick, targetTick, 1, tickerName) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Pops.NumericTicker"/> class.
        /// </summary>
        /// <param name="initialTick">Sets InitialTick of new instance.</param>
        /// <param name="targetTick">Sets TargetTick of new instance.</param>
        /// <param name="incrementBy">Sets IncrementBy value of new instance.</param>
        public NumericTicker(int initialTick, int targetTick, int incrementBy) : this(initialTick, targetTick, incrementBy, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Pops.NumericTicker"/> class.
        /// </summary>
        /// <param name="initialTick">Sets InitialTick of new instance.</param>
        /// <param name="targetTick">Sets TargetTick of new instance.</param>
        /// <param name="incrementBy">Sets IncrementBy value of new instance.</param>
        /// <param name="tickerName">Sets TickerName of new instance.</param>
        public NumericTicker(int initialTick, int targetTick, int incrementBy, string tickerName)
        {
            if (initialTick == targetTick)
                throw new Exception("InitialTick and TargetTick can not be same!");

            if (incrementBy == 0)
                throw new Exception("IncrementBy value can not be zero!");

            if (_tickers == null)
                _tickers = new List<NumericTicker>();

            InitialTick = initialTick;
            TargetTick = targetTick;

            if (InitialTick < TargetTick)
                IncrementBy = Mathf.Abs(incrementBy);
            else
                IncrementBy = Mathf.Abs(incrementBy) * -1;

            if (string.IsNullOrEmpty(tickerName))
                TickerName = TickerName = InitialTick < TargetTick ? "CountUp" : "CountDown";
            else
                TickerName = tickerName;

            Reset();
            _tickers.Add(this);
        }

        public delegate void TickerTick(NumericTicker ticker);
        public static event TickerTick OnTickerTicked;
        public static event TickerTick OnTickerCompleted;
        public event TickerTick OnCompleted;

        public int CurrentTick { private set; get; }

        public int IncrementBy { private set; get; }

        /// <summary>
        /// Gets the initial value.
        /// </summary>
        /// <value>The initial tick.</value>
        public int InitialTick { private set; get; }

        /// <summary>
        /// Gets the target value.
        /// </summary>
        /// <value>The target tick.</value>
        public int TargetTick { private set; get; }

        /// <summary>
        /// Gets the name of the ticker.
        /// </summary>
        /// <value>The name of the ticker.</value>
        public string TickerName
        {
            private set
            {
                if (!string.IsNullOrEmpty(_tickerName))
                    throw new Exception("TickerName can only be set one time!");

                int nr = Tickers
                        .Where(t => t.TickerName.StartsWith(value, StringComparison.Ordinal))
                        .Count();
                _tickerName = nr > 0 ? string.Format("{0}_{1:00}", value, nr + 1) : value;
            }
            get { return _tickerName; }
        }

        private string _tickerName;

        public TickerState State { private set; get; }

        /// <summary>
        /// Keeps all NumericTicker instances
        /// </summary>
        public static IEnumerable<NumericTicker> Tickers
        {
            get
            {
                return _tickers;
            }
        }
        private static List<NumericTicker> _tickers;


        /// <summary>
        /// Releases all resource used by the <see cref="T:Pops.NumericTicker"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:Pops.NumericTicker"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="T:Pops.NumericTicker"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="T:Pops.NumericTicker"/> so
        /// the garbage collector can reclaim the memory that the <see cref="T:Pops.NumericTicker"/> was occupying.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (_tickers.Contains(this))
                    _tickers.Remove(this);
            }

            disposed = true;
        }
        private bool disposed = false;

        public static void DisposeAllTickers()
        {
            if (_tickers == null)
                return;

            var all = Tickers.ToList();
            _tickers = new List<NumericTicker>();
            foreach (var t in all)
            {
                t.Dispose();
            }
            all = null;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            CurrentTick = InitialTick;
            State = TickerState.Active;
        }

        /// <summary>
        /// Increments or decrements (depends on InitialValue and TargetValue) <c>CurrentValue</c>
        /// </summary>
        public void Tick()
        {
            if (State != TickerState.Active)
                return;

            CurrentTick += IncrementBy;

            if ((IncrementBy < 0 && (CurrentTick <= TargetTick))
                || (IncrementBy > 0 && (CurrentTick >= TargetTick)))
            {
                State = TickerState.Completed;

                if (OnTickerCompleted != null)
                    OnTickerCompleted(this);

                if (OnCompleted != null)
                    OnCompleted(this);
            }

            if (OnTickerTicked != null)
                OnTickerTicked(this);
        }
    }
}
