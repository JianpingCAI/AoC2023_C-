string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int result = 0;

List<(int r, int c)> stars = [];

int rowExpand = 0;
for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];

    int numStars = 0;
    for (int j = 0; j < line.Length; j++)
    {
        if (line[j] == '#')
        {
            stars.Add(new(i + rowExpand, j));
            ++numStars;
        }
    }
    if (0 == numStars)
    {
        ++rowExpand;
    }
}

// check column
HashSet<int> rawColIds = [];
foreach ((int r, int c) p in stars)
{
    rawColIds.Add(p.c);
}

// empty column
List<int> emptyColIds = [];
for (int j = 0; j < lines[0].Length; j++)
{
    if (!rawColIds.Contains(j))
    {
        emptyColIds.Add(j);
    }
}

// expand increment
Dictionary<int, int> old_inc_colIds = [];
for (int j = 0; (j < lines[0].Length); j++)
{
    old_inc_colIds[j] = 0;
}

foreach (int id in emptyColIds)
{
    for (int j = 0; j < lines[0].Length; j++)
    {
        if (j > id)
        {
            old_inc_colIds[j]++;
        }
    }
}

// expand
for (int i = 0; i < stars.Count; i++)
{
    stars[i] = (stars[i].r, stars[i].c + old_inc_colIds[stars[i].c]);
}

// distances
int count = 0;
for (int i = 0; i < stars.Count - 1; i++)
{
    for (int j = i + 1; j < stars.Count; j++)
    {
        int dist = Math.Abs(stars[i].r - stars[j].r) + Math.Abs(stars[i].c - stars[j].c);
        result += dist;
        ++count;
    }
}

Console.WriteLine($"Result = {result}");
Console.WriteLine($"Count = {count}");