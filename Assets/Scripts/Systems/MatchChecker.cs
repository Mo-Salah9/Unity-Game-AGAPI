public class MatchChecker
{
    public bool IsMatch(Card card1, Card card2)
    {
        return card1.CardId == card2.CardId;
    }
}