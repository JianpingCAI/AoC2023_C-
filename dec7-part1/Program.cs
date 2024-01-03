string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

long result = 0;

Dictionary<string, long> card_bid_pairs = [];
Dictionary<string, long> card_type_pairs = [];

Dictionary<char, int> c_v_pairs = new()
{
    ['B'] = 10,
    ['C'] = 11,
    ['D'] = 12,
    ['E'] = 13,
    ['F'] = 14
};

for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    List<string> pair = line.Split(' ').ToList();
    string replace = pair[0].Replace('A', 'F').Replace('K', 'E').Replace('Q', 'D').Replace('J', 'C').Replace('T', 'B');

    card_bid_pairs[replace] = long.Parse(pair[1]);
}

foreach (string card in card_bid_pairs.Keys)
{
    int cardType = getCardType(card);
    card_type_pairs[card] = cardType;
}

int getCardType(string card)
{
    HashSet<char> charSet = [.. card];

    if (charSet.Count == 1)
    {
        return 7;
    }
    if (charSet.Count == 2)
    {
        int occurs = card.Count(x => x == card[0]);
        if (occurs == 1 || occurs == 4)
        {
            return 6;
        }

        return 5;
    }
    if (charSet.Count == 3)
    {
        foreach (char c in charSet)
        {
            int occurs = card.Count(x => x == c);
            if (3 == occurs)
            {
                return 4;
            }
        }
        return 3;
    }
    if (charSet.Count == 4)
    {
        return 2;
    }
    if (charSet.Count == 5)
    {
        return 1;
    }

    return 0;
}

int compareCards(string card1, string card2)
{
    if (card_type_pairs[card1] < card_type_pairs[card2])
    {
        return -1;
    }
    if (card_type_pairs[card1] > card_type_pairs[card2])
    {
        return 1;
    }

    for (int i = 0; i < card1.Length; i++)
    {
        int v1 = getValue(card1[i]);
        int v2 = getValue(card2[i]);

        if (v1 == v2)
        {
            continue;
        }

        return v1 - v2;
    }

    return 0;
}

int getValue(char v)
{
    if (char.IsDigit(v))
    {
        return v - '0';
    }

    return c_v_pairs[v];
}

// sort
List<string> cards = card_bid_pairs.Keys.ToList();
cards.Sort(compareCards);

for (int i = 0; i < cards.Count; i++)
{
    long rank = i + 1;
    long bid = card_bid_pairs[cards[i]];

    result += rank * bid;
}


//int count = getCardType("B55C5");
Console.WriteLine($"Result = {result}");