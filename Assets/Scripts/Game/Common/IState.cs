    public interface IState 
    {
        void EnterState();
        void ExitState();
        void FixedUpdateState();
        void UpdateState();
    }