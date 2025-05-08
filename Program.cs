using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir); 

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles); 

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

var salesSummaryDir = Path.Combine(currentDirectory, "salesSummaryDir");
Directory.CreateDirectory(salesSummaryDir);

var salesSummaryFile = Path.Combine(salesSummaryDir, "salesSummary.txt");

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }
    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {      
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void GenerateSalesSummaryReport(IEnumerable<string> salesFiles, string outputFilePath)
{
    double total = 0;
    List<string> detailLines = new();

    foreach (var file in salesFiles)
    {
        string name = Path.GetFileName(file);
        if (!name.Equals("sales.json", StringComparison.OrdinalIgnoreCase))
            continue;

        string json = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(json);
        double amount = data?.Total ?? 0;
        total += amount;

        string storeName = new DirectoryInfo(Path.GetDirectoryName(file)!).Name;
        string fileName = Path.GetFileName(file);

        detailLines.Add($" Store {storeName} - {Path.GetFileName(file)}: {amount:C2}");
    }

    var reportLines = new List<string>
    {
        "Sales Summary",
        "----------------------------",
        $" Total Sales: {total:C2}",
        "",
        " Details:"
    };

    reportLines.AddRange(detailLines);

    File.WriteAllLines(outputFilePath, reportLines);
}

GenerateSalesSummaryReport(salesFiles, salesSummaryFile);
record SalesData (double Total);