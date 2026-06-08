using DSPCore;

internal static class CloneModelExample
{
    public static void Register()
    {
        Models.CloneModel(
            sourceModelIndex: 230,
            modelIndex: 9554,
            ownerModGuid: "com.example.my-mod",
            configureModel: static model =>
            {
                model.Name = "Example Cloned Model";
            },
            configurePrefab: static prefab =>
            {
                prefab.modelIndex = 9554;
            });
    }
}
