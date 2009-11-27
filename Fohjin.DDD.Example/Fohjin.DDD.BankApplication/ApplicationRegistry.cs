using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Fohjin.DDD.BankApplication.Presenters;
using Fohjin.DDD.BankApplication.Views;
using StructureMap.Configuration.DSL;

namespace Fohjin.DDD.BankApplication
{
    public class StructureMapApplicationRegistry : Registry
    {
        public StructureMapApplicationRegistry()
        {
            ForRequestedType<IClientSearchFormPresenter>().TheDefaultIsConcreteType<ClientSearchFormPresenter>();
            ForRequestedType<IClientDetailsPresenter>().TheDefaultIsConcreteType<ClientDetailsPresenter>();
            ForRequestedType<IAccountDetailsPresenter>().TheDefaultIsConcreteType<AccountDetailsPresenter>();
            ForRequestedType<IPopupPresenter>().TheDefaultIsConcreteType<PopupPresenter>();

            ForRequestedType<IClientSearchFormView>().TheDefaultIsConcreteType<ClientSearchForm>();
            ForRequestedType<IClientDetailsView>().TheDefaultIsConcreteType<ClientDetails>();
            ForRequestedType<IAccountDetailsView>().TheDefaultIsConcreteType<AccountDetails>();
            ForRequestedType<IPopupView>().TheDefaultIsConcreteType<Popup>();
        }
    }

    public class CastleApplicationRegistry
    {
        public CastleApplicationRegistry(IWindsorContainer container)
        {
            container.Register(
                Component.For<IClientSearchFormPresenter>().ImplementedBy<ClientSearchFormPresenter>(),
                Component.For<IClientDetailsPresenter>().ImplementedBy<ClientDetailsPresenter>(),
                Component.For<IAccountDetailsPresenter>().ImplementedBy<AccountDetailsPresenter>(),
                Component.For<IPopupPresenter>().ImplementedBy<PopupPresenter>(),
                Component.For<IClientSearchFormView>().ImplementedBy<ClientSearchForm>(),
                Component.For<IClientDetailsView>().ImplementedBy<ClientDetails>(),
                Component.For<IAccountDetailsView>().ImplementedBy<AccountDetails>(),
                Component.For<IPopupView>().ImplementedBy<Popup>()
                );
        }
    }
}