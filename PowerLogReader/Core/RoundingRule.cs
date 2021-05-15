namespace PowerLogReader.Core
{
    public enum RoundingRule
    {
        None = 0,   //!< 丸めなし
        RoundingOff = 1,    //!< 四捨五入
        Truncate = 2,   //!< 切り捨て
        RoundingUp = 3,     //!< 切り上げ
    }
}
