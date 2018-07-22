﻿namespace Sitecore.FakeDb.Tests
{
  using NSubstitute;
  using global::AutoFixture;
  using global::AutoFixture.AutoNSubstitute;
  using global::AutoFixture.Xunit2;
  using Sitecore.Data;
  using Sitecore.FakeDb.Data.Engines;
  using Sitecore.FakeDb.Data.Items;
  using Sitecore.Globalization;

  internal class DefaultAutoDataAttribute : AutoDataAttribute
  {
    public DefaultAutoDataAttribute()
      : base(new Fixture().Customize(new DefaultConventions()))
    {
    }
  }

  internal class InlineDefaultAutoDataAttribute : InlineAutoDataAttribute
  {
    public InlineDefaultAutoDataAttribute(params object[] values)
      : base(new DefaultAutoDataAttribute(), values)
    {
    }
  }

  internal class DefaultSubstituteAutoDataAttribute : DefaultAutoDataAttribute
  {
    public DefaultSubstituteAutoDataAttribute()
    {
      this.Fixture.Customize(new AutoNSubstituteCustomization());
    }
  }

  internal class DefaultConventions : ICustomization
  {
    public void Customize(IFixture fixture)
    {
      var database = Database.GetDatabase("master");
      fixture.Inject(database);
      fixture.Inject(Substitute.For<DataStorage>(database));
      fixture.Register(ItemHelper.CreateInstance);
      fixture.Register(() => Language.Parse("en"));
    }
  }
}