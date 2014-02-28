﻿namespace Sitecore.FakeDb.Tests.Configuration
{
  using System;
  using FluentAssertions;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.FakeDb.Data.DataProviders;
  using Sitecore.FakeDb.Data.Engines;
  using Xunit;
  using Xunit.Extensions;

  public class ConfigurationTest
  {
    [Fact]
    public void ShouldGetDataStorage()
    {
      Factory.CreateObject("dataStorage", true).Should().BeOfType<DataStorage>();
    }

    [Fact]
    public void ShouldGetDataStorageAsSingleton()
    {
      Factory.CreateObject("dataStorage", true).Should().BeSameAs(Factory.CreateObject("dataStorage", true));
    }

    [Fact]
    public void ShouldSetDataStorageIntoProvider()
    {
      var provider = (FakeDataProvider)Factory.CreateObject("dataProviders/main", true);
      provider.DataStorage.Should().NotBeNull();
    }

    [Theory]
    [InlineData("AddFromTemplatePrototype", typeof(FakeDb.Data.Engines.DataCommands.AddFromTemplateCommand))]
    [InlineData("AddVersionPrototype", typeof(FakeDb.Data.Engines.DataCommands.AddVersionCommand))]
    [InlineData("BlobStreamExistsPrototype", typeof(FakeDb.Data.Engines.DataCommands.BlobStreamExistsCommand))]
    [InlineData("CopyItemPrototype", typeof(FakeDb.Data.Engines.DataCommands.CopyItemCommand))]
    [InlineData("CreateItemPrototype", typeof(FakeDb.Data.Engines.DataCommands.CreateItemCommand))]
    [InlineData("DeletePrototype", typeof(FakeDb.Data.Engines.DataCommands.DeleteItemCommand))]
    [InlineData("GetBlobStreamPrototype", typeof(FakeDb.Data.Engines.DataCommands.GetBlobStreamCommand))]
    [InlineData("GetChildrenPrototype", typeof(FakeDb.Data.Engines.DataCommands.GetChildrenCommand))]
    [InlineData("GetItemPrototype", typeof(FakeDb.Data.Engines.DataCommands.GetItemCommand))]
    [InlineData("GetParentPrototype", typeof(FakeDb.Data.Engines.DataCommands.GetParentCommand))]
    [InlineData("GetRootItemPrototype", typeof(FakeDb.Data.Engines.DataCommands.GetRootItemCommand))]
    [InlineData("GetVersionsPrototype", typeof(FakeDb.Data.Engines.DataCommands.GetVersionsCommand))]
    [InlineData("HasChildrenPrototype", typeof(FakeDb.Data.Engines.DataCommands.HasChildrenCommand))]
    [InlineData("MoveItemPrototype", typeof(FakeDb.Data.Engines.DataCommands.MoveItemCommand))]
    [InlineData("RemoveDataPrototype", typeof(FakeDb.Data.Engines.DataCommands.RemoveDataCommand))]
    [InlineData("RemoveVersionPrototype", typeof(FakeDb.Data.Engines.DataCommands.RemoveVersionCommand))]
    [InlineData("ResolvePathPrototype", typeof(FakeDb.Data.Engines.DataCommands.ResolvePathCommand))]
    [InlineData("SaveItemPrototype", typeof(FakeDb.Data.Engines.DataCommands.SaveItemCommand))]
    [InlineData("SetBlobStreamPrototype", typeof(FakeDb.Data.Engines.DataCommands.SetBlobStreamCommand))]
    public void ShouldRegisterFakeCommand(string propertyName, Type propertyType)
    {
      // arrange
      foreach (var databaseName in new[] { "master", "web", "core" })
      {
        var database = Database.GetDatabase(databaseName);
        var commands = database.Engines.DataEngine.Commands;

        // act
        var propertyInfo = commands.GetType().GetProperty(propertyName);
        var command = propertyInfo.GetValue(commands);

        // assert
        command.Should().BeOfType(propertyType, "Database: \"{0}\"", databaseName);
      }
    }

    [Fact]
    public void CacheShouldBeDisabled()
    {
      Settings.Caching.Enabled.Should().BeFalse();
    }
  }
}