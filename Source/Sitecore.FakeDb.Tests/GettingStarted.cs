﻿namespace Examples
{
  using System;
  using System.Linq;
  using NSubstitute;
  using Xunit;

  public class GettingStarted
  {
    #region Content

    [Fact]
    public void HowDoICreateASimpleItem()
    {
      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
        {
          new Sitecore.FakeDb.DbItem("Home") { { "Title", "Welcome!" } }
        })
      {
        Sitecore.Data.Items.Item homeItem = db.GetItem("/sitecore/content/home");
        Assert.Equal("Welcome!", homeItem["Title"]);
      }
    }

    [Fact]
    public void HowDoICreateAnItemHierarchy()
    {
      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
        {
          new Sitecore.FakeDb.DbItem("Articles")
            {
              new Sitecore.FakeDb.DbItem("Getting Started"),
              new Sitecore.FakeDb.DbItem("Troubleshooting")
            }
        })
      {
        Sitecore.Data.Items.Item articles = db.GetItem("/sitecore/content/Articles");
        Assert.NotNull(articles["Getting Started"]);
        Assert.NotNull(articles["Troubleshooting"]);
      }
    }

    [Fact]
    public void HowDoICreateAMultilingualItem()
    {
      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
        {
          new Sitecore.FakeDb.DbItem("home")
            {
              new Sitecore.FakeDb.DbField("Title") { { "en", "Hello!" }, { "da", "Hej!" } }
            }
        })
      {
        Sitecore.Data.Items.Item homeEn = db.GetItem("/sitecore/content/home", "en");
        Assert.Equal("Hello!", homeEn["Title"]);

        Sitecore.Data.Items.Item homeDa = db.GetItem("/sitecore/content/home", "da");
        Assert.Equal("Hej!", homeDa["Title"]);
      }
    }

    [Fact]
    public void HowDoICreateAnItemOfSpecificTemplate()
    {
      Sitecore.Data.ID templateId = Sitecore.Data.ID.NewID;

      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
        {
          new Sitecore.FakeDb.DbTemplate("products", templateId) { "Name" },
          new Sitecore.FakeDb.DbItem("Apple") { TemplateID = templateId }
        })
      {
        Sitecore.Data.Items.Item item = db.GetItem("/sitecore/content/apple");
        Assert.Equal(templateId, item.TemplateID);
        Assert.NotNull(item.Fields["Name"]);
      }
    }

    [Fact]
    public void HowDoICreateAVersionedItem()
    {
      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
        {
          new Sitecore.FakeDb.DbItem("home")
            {
              new Sitecore.FakeDb.DbField("Title") { { "en", 1, "Hello!" }, { "en", 2, "Welcome!" } }
            }
        })
      {
        Sitecore.Data.Items.Item homeV1 = db.GetItem("/sitecore/content/home", "en", 1);
        Assert.Equal("Hello!", homeV1["Title"]);

        Sitecore.Data.Items.Item homeV2 = db.GetItem("/sitecore/content/home", "en", 2);
        Assert.Equal("Welcome!", homeV2["Title"]);
      }
    }

    [Fact]
    public void HowDoICreateATemplateWithStandardValues()
    {
      var templateId = Sitecore.Data.ID.NewID;

      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
        {
          new Sitecore.FakeDb.DbTemplate("sample", templateId) { { "Title", "$name"} }
        })
      {
        var root = db.GetItem(Sitecore.ItemIDs.ContentRoot);
        var item = Sitecore.Data.Managers.ItemManager.CreateItem("Home", root, templateId);
        Assert.Equal("Home", item["Title"]);
      }
    }

    [Fact]
    public void HowDoICreateATemplateHierarchy()
    {
      var baseTemplateIdOne = Sitecore.Data.ID.NewID;
      var baseTemplateIdTwo = Sitecore.Data.ID.NewID;
      var templateId = Sitecore.Data.ID.NewID;

      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
      {
        new Sitecore.FakeDb.DbTemplate("base one", baseTemplateIdOne),
        new Sitecore.FakeDb.DbTemplate("base two", baseTemplateIdTwo),
        new Sitecore.FakeDb.DbTemplate("Main", templateId)
        {
          BaseIDs = new Sitecore.Data.ID[] {baseTemplateIdOne, baseTemplateIdTwo}
        }
      })
      {
        var template = Sitecore.Data.Managers.TemplateManager.GetTemplate(templateId, db.Database);

        Assert.Contains(baseTemplateIdOne, template.BaseIDs);
        Assert.Contains(baseTemplateIdTwo, template.BaseIDs);

        Assert.True(template.InheritsFrom(baseTemplateIdOne));
        Assert.True(template.InheritsFrom(baseTemplateIdTwo));
      }
    }

    #endregion

    #region Links

    [Fact]
    public void HowDoIWorkWithLinkDatabase()
    {
      // arrange your database and items

      Sitecore.Data.ID sourceId = Sitecore.Data.ID.NewID;

      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
      {
        new Sitecore.FakeDb.DbItem("source", sourceId),
        new Sitecore.FakeDb.DbItem("clone"),
        new Sitecore.FakeDb.DbItem("alias", Sitecore.Data.ID.NewID, Sitecore.TemplateIDs.Alias)
        {
          // not really needed but this is how aliases look in the real world
          new Sitecore.FakeDb.DbField("Linked item")
          {
            Type = "Link",
            Value = string.Format("<link linktype='internal' id='{0}' />", sourceId)
          }
        }
      })
      {
        // arrange desired LinkDatabase behavior
        var behavior = Substitute.For<Sitecore.Links.LinkDatabase>();

        Sitecore.Data.Items.Item source = db.GetItem("/sitecore/content/source");
        Sitecore.Data.Items.Item alias = db.GetItem("/sitecore/content/alias");
        Sitecore.Data.Items.Item clone = db.GetItem("/sitecore/content/clone");

        behavior.GetReferrers(source).Returns(new[]
        {
          new Sitecore.Links.ItemLink(alias, alias.Fields["Linked item"].ID, source, source.Paths.FullPath),
          new Sitecore.Links.ItemLink(clone, Sitecore.FieldIDs.Source, source, source.Paths.FullPath)
        });

        // act & assert

        // link database is clean
        Assert.Equal(Sitecore.Globals.LinkDatabase.GetReferrers(source).Count(), 0);
 
        using (new Sitecore.FakeDb.Links.LinkDatabaseSwitcher(behavior))
        {
          Sitecore.Links.ItemLink[] referrers = Sitecore.Globals.LinkDatabase.GetReferrers(source);

          Assert.Equal(referrers.Count(), 2);
          Assert.Equal(referrers.Count(r => r.SourceItemID == clone.ID && r.TargetItemID == source.ID), 1);
          Assert.Equal(referrers.Count(r => r.SourceItemID == alias.ID && r.TargetItemID == source.ID), 1);
        }
 
         // link database is clean again
        Assert.Equal(Sitecore.Globals.LinkDatabase.GetReferrers(source).Count(), 0);
      }
    }

    #endregion

    #region Security

    [Fact]
    public void HowDoIConfigureItemAccess()
    {
      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
        {
          new Sitecore.FakeDb.DbItem("home") { Access = { CanRead = false } }
        })
      {
        Sitecore.Data.Items.Item item = db.GetItem("/sitecore/content/home");

        // item is null because read is denied
        Assert.Null(item);
      }
    }

    #endregion

    #region Pipelines

    /// <summary>
    /// How do I ensure the pipeline is called.
    /// Imagine you have a product repository. The repository should be able to get a product by id.
    /// The implementation of the repository is 'thin' and does nothing than calling a corresponding pipeline with proper arguments.
    /// The next example shows how to unit test the pipeline call (please note that the pipeline is not defined in the tests assembly config file):
    /// </summary>
    [Fact]
    public void HowDoIEnsureThePipelineIsCalled()
    {
      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db())
      {
        // configure a pipeline watcher to expect the "createProduct" pipeline call with
        // product name passed to the arguments custom data
        db.PipelineWatcher
          .Expects("createProduct", a => a.CustomData["ProductName"].Equals("MyProduct"));

        // create a repository and call the create product method
        var repository = new ProductRepository();
        repository.CreateProduct("MyProduct");

        // assert the expected pipeline is called and the product name is passed
        db.PipelineWatcher.EnsureExpectations();
      }
    }

    private partial class ProductRepository
    {
      public void CreateProduct(string name)
      {
        var args = new Sitecore.Pipelines.PipelineArgs();
        args.CustomData["ProductName"] = name;

        Sitecore.Pipelines.CorePipeline.Run("createProduct", args);
      }
    }

    /// <summary>
    /// How do I configure the pipeline behaviour.
    /// The code sample above checks that the pipeline is called with proper arguments. 
    /// The next scenario would be to validate the pipeline call results. 
    /// In the code below we configure pipeline proressor behaviour to return an expected product only
    /// if the product id id set to "1".
    /// </summary>
    [Fact]
    public void HowDoIConfigureThePipelineBehaviour()
    {
      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db())
      {
        // create a product to get from the repository
        object expectedProduct = new object();

        // configure processing of the pipeline arguments. Will set the 'expectedProduct' instance 
        // to CustomData["Product"] property only when the CustomData["ProductId"] is "1"
        string productId = "1";

        // configure a pipeline watcher to expect a pipeline call where the args custom data contains
        // ProductId. Once the args received the pipeline result is set into Product custom data property
        db.PipelineWatcher
          .WhenCall("findProductById")
          .WithArgs(a => a.CustomData["ProductId"].Equals(productId))
          .Then(a => a.CustomData["Product"] = expectedProduct);

        // create a repository and call get product method
        ProductRepository repository = new ProductRepository();
        var actualProduct = repository.GetProductById(productId);

        // assert the received product is the same as the expected one
        Assert.Equal(expectedProduct, actualProduct);
      }
    }

    private partial class ProductRepository
    {
      public object GetProductById(string id)
      {
        var args = new Sitecore.Pipelines.PipelineArgs();
        args.CustomData["ProductId"] = id;

        Sitecore.Pipelines.CorePipeline.Run("findProductById", args);

        return args.CustomData["Product"];
      }
    }

    #endregion

    #region Configuration

    [Fact]
    public void HowDoIConfigureSettings()
    {
      using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db())
      {
        // set the setting value in unit test using db instance
        db.Configuration.Settings["MySetting"] = "1234";

        // get the setting value in your code using regular Sitecore API
        var value = Sitecore.Configuration.Settings.GetSetting("MySetting");
        Assert.Equal("1234", value);
      }
    }

    [Fact]
    public void HowDoIGetItemFromSitecoreDatabase()
    {
      using (new Sitecore.FakeDb.Db
        {
          new Sitecore.FakeDb.DbItem("Home")
        })
      {
        Sitecore.Data.Database database = Sitecore.Configuration.Factory.GetDatabase("master");
        Assert.NotNull(database.GetItem("/sitecore/content/home"));
      }
    }

    #endregion

    #region Miscellaneous

    [Fact]
    public void HowDoIMockContentSearchLogic()
    {
      try
      {
        var index = Substitute.For<Sitecore.ContentSearch.ISearchIndex>();
        Sitecore.ContentSearch.ContentSearchManager.SearchConfiguration.Indexes.Add("my_index", index);

        using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db { new Sitecore.FakeDb.DbItem("home") })
        {
          var searchResultItem = Substitute.For<Sitecore.ContentSearch.SearchTypes.SearchResultItem>();
          searchResultItem.GetItem().Returns(db.GetItem("/sitecore/content/home"));
          index.CreateSearchContext().GetQueryable<Sitecore.ContentSearch.SearchTypes.SearchResultItem>().Returns((new[] { searchResultItem }).AsQueryable());

          Sitecore.Data.Items.Item result = index.CreateSearchContext().GetQueryable<Sitecore.ContentSearch.SearchTypes.SearchResultItem>().Single().GetItem();

          Assert.Equal("/sitecore/content/home", result.Paths.FullPath);
        }
      }
      finally
      {
        Sitecore.ContentSearch.ContentSearchManager.SearchConfiguration.Indexes.Remove("my_index");
      }
    }

    [Fact]
    public void HowDoIMockTrackerVisitor()
    {
      // create a visitor mock
      var visitorMock = Substitute.For<Sitecore.Analytics.Data.DataAccess.Visitor>(Guid.NewGuid());

      // inject the visitor mock into Analytics Tracker
      using (new Sitecore.Common.Switcher<Sitecore.Analytics.Data.DataAccess.Visitor>(visitorMock))
      {
        // the mocked visitor instance is now available via Analytics.Tracker.Visitor property
        var currentVisitor = Sitecore.Analytics.Tracker.Visitor;
        Assert.Equal(visitorMock, currentVisitor);
      }
    }

    #endregion
  }
}