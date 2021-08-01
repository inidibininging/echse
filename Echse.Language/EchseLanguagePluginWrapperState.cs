using States.Core.Infrastructure.Services;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Echse.Language
{
    /// <summary>    
    /// This is used for wrapping other states of other state machines, inside the language state machine 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class EchseLanguagePluginWrapperState<TKey, TValue, TValue2> 
        : IState<TKey, TValue>, IEchseLanguageReflectionIdentifier<IState<TKey, TValue2>>        
    {
        private IState<TKey, TValue2> bag;
        private IStateMachine<TKey, TValue2> stateMachine;

        public IState<TKey, TValue2> Bag => bag;

        public EchseLanguagePluginWrapperState(IState<TKey, TValue2> state, IStateMachine<TKey, TValue2> leMachine)
        {            
            bag = state;
            this.stateMachine = leMachine;
        }

        public void Handle(IStateMachine<TKey, TValue> machine)
        {            
            bag.Handle(stateMachine);
        }
    }
    public interface IEchseLanguageReflectionIdentifier<T> {
        T Bag { get; }

    }
    // public class _CommandStateFuncDelegate<TKey, TValue> : IState<TKey, TValue>
    // {
    //     private IState<TKey, TValue> State { get; }

    //     public _CommandStateFuncDelegate(IState<TKey, TValue> state)
    //     {
    //         State = state;
    //     }

    //     public void Handle(IStateMachine<TKey, TValue> machine)
    //     {
    //         State.Handle(machine);
    //     }
    // }
}