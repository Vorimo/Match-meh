using System;
using Enum;

namespace Managers {
    [Serializable]
    public class EndGameRequirementState {
            public LevelLimitationType levelLimitationType;
            public int remainingValue;
    }
}