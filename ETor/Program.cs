using ETor;
using ETor.BEncoding;
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var torrentName = "17_13_the_hobbit_the_battle_of_the_five_armies-201";

var content = await File.ReadAllBytesAsync($"C:\\Users\\Admin\\Downloads\\{torrentName}.torrent");

logger.Information("Read .torrent file {file} of size {size}", torrentName, content.Length);

var encodedContent = new BEncodeParser(content);

var dict = encodedContent.ReadDictionary();

var torrentFile = new Torrent(dict);

logger.Information("Parsed .torrent file. Name: {name}. Single-file: {mode}", torrentFile.Info.Name, torrentFile.Info.IsSingleFile);

FileManager.CreateFiles(torrentFile, torrentName);