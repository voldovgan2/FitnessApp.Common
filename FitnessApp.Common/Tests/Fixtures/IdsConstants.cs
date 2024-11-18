namespace FitnessApp.Common.Tests.Fixtures;
public static class IdsConstants
{
    public const string IdToGet = nameof(IdToGet);
    public const string IdToCreate = nameof(IdToCreate);
    public const string IdToUpdate = nameof(IdToUpdate);
    public const string IdToDelete = nameof(IdToDelete);

    public static string[] IdsToSeed { get; } = [
        IdToGet,
        IdToUpdate,
        IdToDelete
    ];
}
