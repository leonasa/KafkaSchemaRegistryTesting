// See https://aka.ms/new-console-template for more information

using AvroSchemaGenerator;
using SchemaRegistry;

await ValidateSchema();

async Task  ValidateSchema()
{
    using var registry = new SchemaRegistryApi("http://localhost:8081");

    // Get first 10 subjects
    var subjects = await registry.GetAllSubjects();
    Console.WriteLine("First 10 subjects: " + String.Join(", ", subjects));

    // Get last schema by subject
    string subject = subjects.First();
    var meta = await registry.GetLatestSchemaMetadata(subject);
    Console.WriteLine($"Last version of the {subject} subject: {meta.Version}");

    // var goodAvroSchema = typeof(LogMessage).GetSchema();
    var badAvroSchema = typeof(LogMessageTest).GetSchema();
    // var isGoodCompatible = await registry.TestCompatibility(subject, goodAvroSchema);
    var isBadCompatible = await registry.TestCompatibility(subject, badAvroSchema);
    // Console.WriteLine($"Is the schema compatible to goodAvroSchema: {isGoodCompatible}");
    Console.WriteLine($"Is the schema compatible to badAvroSchema: {isBadCompatible}");

    // Register the schema (returns the same ID for identical schema)
    var newSchemaId = await registry.Register(subject, meta.Schema);
    Console.WriteLine($"Schema id: {newSchemaId}");
}

public class LogMessageTest
{
    public LogLevel Severity { get; set; }
    public string Message { get; set; }
}

public enum LogLevel
{
    None,
    Verbose,
    Info,
    Warning,
    Error,
}