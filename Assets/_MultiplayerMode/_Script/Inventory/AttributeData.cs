[System.Serializable]
public class AttributeData
{
    public int HP_Level;
    public int Attack_Level;
    public int MoveSpeed_Level;

    public int originHP;
    public int originAttack;
    public int originMoveSpeed;

    public AttributeData(int hpLevel, int attackLevel, int moveSpeedLevel, int originHP, int originAttack, int originMoveSpeed)
    {
        this.HP_Level = hpLevel;
        this.Attack_Level = attackLevel;
        this.MoveSpeed_Level = moveSpeedLevel;
        this.originHP = originHP;
        this.originAttack = originAttack;
        this.originMoveSpeed = originMoveSpeed;
    }
}
