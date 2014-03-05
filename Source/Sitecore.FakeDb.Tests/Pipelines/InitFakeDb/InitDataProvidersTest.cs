﻿namespace Sitecore.FakeDb.Tests.Pipelines.InitFakeDb
{
  using System;
  using System.Reflection;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.DataProviders;
  using Sitecore.FakeDb.Data.Engines;
  using Sitecore.FakeDb.Pipelines.InitFakeDb;
  using Sitecore.Reflection;
  using Xunit;

  public class InitDataProvidersTest
  {
    [Fact]
    public void ShouldSetDataStorageForIRequireDataStorageProviders()
    {
      // arrange
      var database = Database.GetDatabase("master");
      var dataProvider = Substitute.For<DataProvider, IRequireDataStorage>();
      typeof(Database).GetField("_dataProviders", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(database, new DataProviderCollection { dataProvider });

      var dataStorage = Substitute.For<DataStorage>();

      var args = new InitDbArgs(database, dataStorage);
      var processor = new InitDataProviders();

      // act
      processor.Process(args);

      // assert
      ((IRequireDataStorage)dataProvider).Received().SetDataStorage(dataStorage);
    }

    [Fact]
    public void ShouldNotSetDataStorageIfNoIRequireDataStorageProviderFound()
    {
      // arrange
      var database = Database.GetDatabase("master");
      var dataProvider = Substitute.For<DataProvider>();
      typeof(Database).GetField("_dataProviders", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(database, new DataProviderCollection { dataProvider });

      var args = new InitDbArgs(database, Substitute.For<DataStorage>());
      var processor = new InitDataProviders();

      // act
      Action action = () => processor.Process(args);

      // assert
      action.ShouldNotThrow();
    }
  }
}