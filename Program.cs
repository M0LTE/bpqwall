using System.Text.Json;

const string datafile = "/tmp/wall.json";

var call = Console.ReadLine();

if (!File.Exists(datafile))
{
    WriteWall(new Wall());
}

var wall = ReadWall();

if (wall.Items.Count == 0)
{
    WriteLine("There are no wall posts yet");
}

foreach (var item in wall.Items.OrderByDescending(item => item.Timestamp).Take(10))
{
    WriteLine(item);
}

while(true)
{
    WriteLine("WRITE or QUIT? >");
    var cmd = Console.ReadLine();
    if (string.Equals(cmd, "quit", StringComparison.OrdinalIgnoreCase)) return;
    if (string.Equals(cmd, "write", StringComparison.OrdinalIgnoreCase)) break;
}

WriteLine("Enter a line of text, or nothing to abort >");

var wallText = Console.ReadLine();

if (string.IsNullOrWhiteSpace(wallText))
{
    Console.WriteLine("Nothing received, bye");
    return;
}

wall = ReadWall();
wall.Items.Add(new WallItem { From = call, Timestamp = DateTime.UtcNow, Message = wallText.Trim() });
WriteWall(wall);

WriteLine("Saved, thanks.");

void WriteLine(object line) => Console.Write(line + "\n");
Wall ReadWall() => Wall.FromJson(File.ReadAllText(datafile)) ?? throw new Exception();
void WriteWall(Wall wall) => File.WriteAllText(datafile, wall.ToJson());

class Wall
{
    public List<WallItem> Items { get; set; } = [];

    internal string? ToJson() => JsonSerializer.Serialize(this);

    internal static Wall? FromJson(string json) => JsonSerializer.Deserialize<Wall>(json);
}

class WallItem
{
    public string? From { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Message { get; set; }

    public override string ToString()
    {
        return $"{From} at {Timestamp:d MMM HH:mm}:\n  > {Message}\n";
    }
}