using Umbraco.Cms.Core.Packaging;

namespace Phases.Umbraco.DummyDataGen.Migrations
{
    public class CustomPackageMigrationPlan : PackageMigrationPlan
    {
        public CustomPackageMigrationPlan() : base("Phases.Umbraco.DummyDataGen")
        {
        }

        protected override void DefinePlan()
        {
            To<CustomPackageMigration>(new Guid("4FD681BE-E27E-4688-922B-29EDCDCB8A49"));
        }
    }
}
