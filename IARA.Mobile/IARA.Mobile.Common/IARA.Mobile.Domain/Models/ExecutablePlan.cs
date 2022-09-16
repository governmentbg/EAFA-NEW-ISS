using System;
using System.Timers;

namespace IARA.Mobile.Domain.Models
{
    public sealed class ExecutablePlan : IDisposable
    {
        private Timer planTimer;
        private readonly Action planAction;
        private readonly bool isRepeatedPlan;

        private ExecutablePlan(int millisecondsDelay, Action planAction, bool isRepeatedPlan)
        {
            planTimer = new Timer(millisecondsDelay)
            {
                Enabled = true
            };
            planTimer.Elapsed += GenericTimerCallback;

            this.planAction = planAction;
            this.isRepeatedPlan = isRepeatedPlan;
        }

        public static ExecutablePlan Delay(int millisecondsDelay, Action planAction)
        {
            return new ExecutablePlan(millisecondsDelay, planAction, false);
        }

        public static ExecutablePlan Repeat(int millisecondsInterval, Action planAction)
        {
            return new ExecutablePlan(millisecondsInterval, planAction, true);
        }

        private void GenericTimerCallback(object sender, ElapsedEventArgs e)
        {
            planAction();
            if (!isRepeatedPlan)
            {
                Abort();
            }
        }

        public void Abort()
        {
            if (planTimer != null)
            {
                planTimer.Enabled = false;
                planTimer.Elapsed -= GenericTimerCallback;
            }
        }

        public void Dispose()
        {
            if (planTimer != null)
            {
                Abort();
                planTimer.Dispose();
                planTimer = null;
            }
        }
    }
}
