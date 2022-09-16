using System;
using IARA.DataAccess;
using IARA.Interfaces.FSM;

namespace IARA.Infrastructure.FSM.Models
{
    internal class TransitionContext
    {
        public IARADbContext Db { get; set; }
        public string ToState { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public IApplicationStateMachine StateMachine { get; set; }
    }
}
