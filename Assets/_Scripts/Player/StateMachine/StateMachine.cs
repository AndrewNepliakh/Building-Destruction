using System;
using System.Collections.Generic;

namespace Madkpv
{
    public class StateMachine
    {
        private IState _state;
        private Dictionary<IState, List<Transition>> _transitions = new Dictionary<IState, List<Transition>>();
        private List<Transition> _anyTransitions = new List<Transition>();
        private List<Transition> _currentTransitions = new List<Transition>();
        private List<Transition> _emptyTransitions = new List<Transition>();

        public event Action<IState> OnStateChanged;

        public void Tick()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                SetState(transition.To);
            }

            _state?.Tick();
        }

        public void SetState(IState state)
        {
            if (state == _state) return;
            _state?.OnExit();
            _state = state;

            _transitions.TryGetValue(_state, out _currentTransitions);

            if (_currentTransitions == null) _currentTransitions = _emptyTransitions;

            _state?.OnEnter();
            OnStateChanged?.Invoke(_state);
        }

        private Transition GetTransition()
        {
            foreach (var transition in _anyTransitions)
                if (transition.Condition())
                    return transition;

            foreach (var transition in _currentTransitions)
                if (transition.Condition())
                    return transition;

            return null;
        }

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            if (_transitions.TryGetValue(from, out var transitions) == false)
            {
                transitions = new List<Transition>();
                _transitions.Add(from, transitions);
            }
            transitions.Add(new Transition(to, condition));
        }

        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            _anyTransitions.Add(new Transition(to, condition));
        }

        private class Transition
        {
            public IState To { get; }
            public Func<bool> Condition { get; }
            public Transition(IState to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }

    }
}