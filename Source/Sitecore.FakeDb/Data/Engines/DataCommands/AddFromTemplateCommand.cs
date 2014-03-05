﻿namespace Sitecore.FakeDb.Data.Engines.DataCommands
{
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;

  public class AddFromTemplateCommand : Sitecore.Data.Engines.DataCommands.AddFromTemplateCommand, IRequireDataStorage
  {
    private DataStorage dataStorage;

    private ItemCreator itemCreator;

    public AddFromTemplateCommand()
    {
    }

    public AddFromTemplateCommand(DataStorage dataStorage)
    {
      Assert.ArgumentNotNull(dataStorage, "dataStorage");

      this.dataStorage = dataStorage;
    }

    public virtual DataStorage DataStorage
    {
      get { return this.dataStorage; }
    }

    public ItemCreator ItemCreator
    {
      get { return this.itemCreator ?? (this.itemCreator = new ItemCreator(this.DataStorage)); }
      set { this.itemCreator = value; }
    }

    protected override Sitecore.Data.Engines.DataCommands.AddFromTemplateCommand CreateInstance()
    {
      return new AddFromTemplateCommand(this.DataStorage);
    }

    protected override Item DoExecute()
    {
      return this.ItemCreator.Create(this.ItemName, this.NewId, this.TemplateId, this.Database, this.Destination);
    }

    public void SetDataStorage(DataStorage dataStorage)
    {
      Assert.IsNotNull(dataStorage, "dataStorage");

      this.dataStorage = dataStorage;
    }
  }
}