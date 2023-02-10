public interface IDataPersistence
{
      void LoadFromJson(GameData data);

      void SaveToJson(ref GameData data);
}
