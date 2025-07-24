using Interfaces;
using States;

namespace States
{
    public class PlayerFsmInitializer
    {
        public PlayerMovementFsm CreateFsm(PlayerContext context)
        {
            var fsm = new PlayerMovementFsm();
        
            // Инициализация состояний
            fsm.AddState(new PlayerIdleState(fsm, context));
            fsm.AddState(new PlayerWalkState(fsm, context));
            fsm.AddState(new PlayerRunState(fsm, context));
            fsm.AddState(new PlayerJumpState(fsm, context));
        
            // Начальное состояние
            fsm.SetState<PlayerIdleState>();
        
            return fsm;
        }
    }
    
    public class PlayerContext
    {
        public IMovementController Movement { get; }
        public IRotationController Rotation { get; }
        public IAnimationController Animation { get; }
        public IInputProvider Input { get; }
        public ICalculator Calculator { get; }
        public ICharacterData Data { get; }

        public PlayerContext(
            IMovementController movement,
            IRotationController rotation,
            IAnimationController animation,
            IInputProvider input,
            ICalculator calculator,
            ICharacterData data)
        {
            Movement = movement;
            Rotation = rotation;
            Animation = animation;
            Input = input;
            Calculator = calculator;
            Data = data;
        }
    }
}
