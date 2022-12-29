public static class BattleManager
{
   public static int JudgeTheBattle(PawnState attaker, PawnState defender)
    {
        switch (attaker)
        {
            case PawnState.Attack:
                if (defender != PawnState.Shield)
                {
                    return 1;
                }
                return 0;
            case PawnState.Shield:
                return 0;
            case PawnState.DoubleAttack:
                if (defender != PawnState.Shield)
                {
                    return 2;
                }
                return 1;
            default:
                break;
        }

        return -1; // ƒGƒ‰[
    }
}
